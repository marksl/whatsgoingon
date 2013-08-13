using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Model;

namespace view
{
    public class Data
    {
        public string Txt { get; set; }
        public string Val { get; set; }
    }

    public class BuildData
    {
        public static Data GetNameData(Build build)
        {
            return new Data
                       {
                           Val = GetName(build),
                           Txt = string.Format("{0}.{1}.{2}.{3}",
                                               build.VMajor, build.VMinor, build.VPatch, build.VBuild)
                       };

        }
        public static string GetName(Build build)
        {
            return string.Format("{0}.{1}.{2}.{3}-g{4}",
                                    build.VMajor, build.VMinor, build.VPatch, build.VBuild, build.GitSha);
        }

        public BuildData(string version, IEnumerable<Build> builds)
            : this()
        {
            if (version == null) throw new ArgumentNullException("version");
            if (builds == null) throw new ArgumentNullException("builds");

            Version = version;

            foreach (var build in builds)
            {
                AppendHtml(build);
            }
        }

        public BuildData(Build build) 
            :this()
        {
            if (build == null) throw new ArgumentNullException("build");

            Version = 
                 string.Format("{0}.{1}.{2}.{3}",
                                    build.VMajor, build.VMinor, build.VPatch, build.VBuild);
                
            AppendHtml(build);
        }

        public BuildData()
        {
            _categoryToHtml = new Dictionary<string, StringBuilder>();
        }

        private void AppendHtml(Build build)
        {
            string jiraUrl = ConfigurationManager.AppSettings["JiraUrl"];

            var jiras = new StringBuilder();
            GetJira(build, jiraUrl, jiras);

            if(jiras.Length!=0)
                _categoryToHtml.Add("JIRA", jiras);

            foreach (var diff in build.BuildDiffs.ToList())
            {
                string curr = diff.CurrentData;

                String category = diff.Category;
                string type = diff.Type;

                Model.BuildData prevBuildData = diff.PreviousBuildData;

                string prev = prevBuildData == null ? string.Empty : prevBuildData.Data;

                StringBuilder html = HtmlBuilder.GetHtml(diff.Diff, type, prev, curr);

                

                if (!_categoryToHtml.ContainsKey(category))
                {
                    _categoryToHtml.Add(category, new StringBuilder());
                }

                _categoryToHtml[category].Append(html);
            }
        }

        public static void GetJira(Build build, string jiraUrl, StringBuilder jiras)
        {
            foreach (var jira in build.BuildJiras.Select(b => b.Jira.ToUpper()))
            {
                if (!string.IsNullOrWhiteSpace(jiraUrl))
                {
                    // HREF
                    jiras.Append(string.Format("<a href=\"{0}{1}\">{1}</><br/>",
                                               jiraUrl, jira));
                }
                else
                {
                    // no HREFs
                    jiras.Append(jira).Append("<br/>");
                }
            }
        }

        private Dictionary<string, StringBuilder> _categoryToHtml;

        public string Version { get; private set; }

        public string GetHtml(string category)
        {
            StringBuilder val;
            return _categoryToHtml.TryGetValue(category, out val) ? val.ToString() : null;
        }

        public bool IsEmpty
        {
            get { return _categoryToHtml.Values.All(c => c == null); }
        }
        


    }
}