using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using NLog;
using run.Misc;

namespace Model
{
    public class Helper
    {

        private static int[] DiffCharCodes(string aText)
        {
            var Codes = new int[aText.Length];

            for (int n = 0; n < aText.Length; n++)
                Codes[n] = aText[n];

            return (Codes);
        }

        public static List<BuildDiff> GetDiffs(Build prevBuild, Build currBuild, out List<int> buildDataIdsProcessed, bool removeProcessed = true)
        {
            buildDataIdsProcessed = new List<int>();

            var diffs = new List<BuildDiff>();

            var names = new HashSet<string>();
            var before = new Dictionary<string, BuildData>();
            var after = new Dictionary<string, BuildData>();

            if (removeProcessed)
            {
                Add(prevBuild.BuildDatas, names, before);
                
                // It's unlikely, but possible that all methods were removed.
                // This if statement handles that scenario.
                int totalCount = currBuild.BuildDatas.Count;
                Add(currBuild.BuildDatas.Where(bd => !bd.Processed), names, after);

                // If we remove all of the processed entries and there are 0 in the after
                // array. This probably means that we don't need to do anything. We double 
                // check to see if the total count is zero. If the total count is zero, that
                // would mean everything is removed and we need to generate a diff where 
                // everything is highlighted red.
                if (after.Count == 0 && totalCount != 0)
                    return diffs;
            }
            else
            {
                Add(prevBuild.BuildDatas, names, before);
                Add(currBuild.BuildDatas, names, after);
            }

            foreach (var name in names)
            {
                BuildData previousBuildData, currentBuildData;

                before.TryGetValue(name, out previousBuildData);
                after.TryGetValue(name, out currentBuildData);

                string type = (previousBuildData != null ? previousBuildData.Type : null) ??
                              (currentBuildData != null ? currentBuildData.Type : null);
                if (type == null)
                    continue;

                string prevText = previousBuildData != null ? previousBuildData.Data : string.Empty;
                string currText = currentBuildData != null ? currentBuildData.Data : string.Empty;

                Diff.Item[] item;
                if (type == "Method")
                {
                    int[] prev = DiffCharCodes(prevText);
                    int[] curr = DiffCharCodes(currText);

                    item = Diff.DiffInt(prev, curr);
                }
                else
                {
                    item = Diff.DiffText(prevText, currText, false, false, false);
                }

                if (previousBuildData!=null)
                    buildDataIdsProcessed.Add(previousBuildData.Id);
                if (currentBuildData != null)
                    buildDataIdsProcessed.Add(currentBuildData.Id);

                if (item.Length > 0)
                {
                    string text = SerializeObject(item);

                    var buildDiff = new BuildDiff
                                        {
                                            Diff = text,
                                            BuildId = currBuild.Id,
                                            PreviousBuildData = previousBuildData,
                                            CurrentBuildData = currentBuildData
                                        };

                    if (previousBuildData != null)
                        buildDiff.PreviousBuildDataId = previousBuildData.Id;
                    
                    if (currentBuildData != null)
                        buildDiff.CurrentBuildDataId = currentBuildData.Id;

                    diffs.Add(buildDiff);
                }
            }

            return diffs;
        }

        private static void Add(IEnumerable<BuildData> buildDatas, HashSet<string> names, Dictionary<string, BuildData> dict)
        {
            foreach (var s in buildDatas)
            {
                string b = s.Category + s.Name;

                if (!names.Contains(b))
                {
                    names.Add(b);
                }

                dict.Add(b, s);
            }
        }

        public static string SerializeObject(Diff.Item[] toSerialize)
        {
            var xmlSerializer = new XmlSerializer(typeof(Diff.Item[]));
            var textWriter = new StringWriter();

            xmlSerializer.Serialize(textWriter, toSerialize);
            return textWriter.ToString();
        }

        readonly static Logger logger = LogManager.GetCurrentClassLogger();

        public static Build GetNewBuild(string folderName)
        {
            return GetNewBuild(folderName, new List<Build>());
        }

        ///
        /// This expects folders in one of two formats:
        /// a.b.c.d-g[sha]
        /// a.b.c.d
        ///
        /// If the format is a.b.c.d this attempts to get the version from a 
        /// a pre-determined assembly in the build folder.
        public static Build GetNewBuild(string folderName, ICollection<Build> existingBuilds, string artifactPath = null, string zipFile = null, string dllInZipFile = null)
        {
            var b = new Build();

            string[] parts = folderName.Split(new[] { "-g" }, StringSplitOptions.None);
            if (parts.Length != 2 || parts[1].Length != 7)
            {
                if (artifactPath != null && zipFile != null && dllInZipFile != null)
                {
                    try
                    {
                        string servicePath = Path.Combine(artifactPath, zipFile);
                        var zip = new MyZipFile(servicePath);

                        var ass = zip.GetAssembly(
                            dllInZipFile);

                        var result =
                            (AssemblyInformationalVersionAttribute)ass.GetCustomAttributes(typeof (AssemblyInformationalVersionAttribute), true)[0];

                        folderName = result.InformationalVersion;
                    }
                    catch
                    {
                        
                    }
                    
                }
            }

            parts = folderName.Split(new[] { "-g" }, StringSplitOptions.None);
            if (parts.Length != 2 || parts[1].Length != 7)
            {

                OutputInvalidPath(folderName);
                return null;
            }

            var versionParts = parts[0].Split('.');
            if (versionParts.Length != 4)
            {
                OutputInvalidPath(folderName);
                return null;
            }

            string major = versionParts[0].Replace("v", string.Empty);

            int ver;
            if (!int.TryParse(major, out ver))
            {
                OutputInvalidPath(folderName);
                return null;
            }
            b.VMajor = ver;

            if (!int.TryParse(versionParts[1], out ver))
            {
                OutputInvalidPath(folderName);
                return null;
            }
            b.VMinor = ver;

            if (!int.TryParse(versionParts[2], out ver))
            {
                OutputInvalidPath(folderName);
                return null;
            }
            b.VPatch = ver;

            if (!int.TryParse(versionParts[3], out ver))
            {
                OutputInvalidPath(folderName);
                return null;
            }
            b.VBuild = ver;

            b.GitSha = parts[1];

            if (existingBuilds.Contains(b))
                return null;

            return b;
        }

        private static void OutputInvalidPath(string artifactPath)
        {
            logger.Info(
                "Skipping invalid artifact folder[{0}]. The correct format is [v0.9.0.12-gc450ec5].",
                artifactPath);
        }

    }
}