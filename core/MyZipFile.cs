using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Ionic.Zip;
using NLog;

namespace Model
{
    public class MyZipFile
    {
        private readonly string _path;

        readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public MyZipFile(string path)
        {
            if (path == null) throw new ArgumentNullException("path");

            _path = path;

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

        }

        ZipFile _file;

        public Assembly GetAssembly(string filePath, bool exactMatch = true)
        {
            if (_file == null)
            {
                try
                {
                    _file = ZipFile.Read(_path);
                }
                catch (FileNotFoundException e)
                {
                    // This happens. It is okay.
                    _logger.InfoException(string.Format("Skipping directory {0}", _path), e);
                    return null;
                }
                catch (DirectoryNotFoundException)
                {
                    // This happens. It is okay.
                    _logger.Info("Skipping directory {0}", _path);

                    return null;
                }
            }


            ZipEntry found = _file.FirstOrDefault(entry => entry.FileName.Equals(filePath));
            if(found==null && !exactMatch)
                found = _file.FirstOrDefault(entry => entry.FileName.EndsWith(filePath));

            if (found == null)
            {
                _logger.Info("[{0}] does not contain [{1}].",
                             _path, filePath);
                return null;
            }

            var memoryStream = new MemoryStream(2000000);
            found.Extract(memoryStream);

            Assembly assembly = Assembly.Load(memoryStream.ToArray());

            return assembly;
        }

        // Very dodgy
        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            int firstComma = args.Name.IndexOf(',');

            string dll = args.Name.Substring(0, firstComma) + ".dll";

            return GetAssembly(dll, exactMatch:false);
        }
    }
}