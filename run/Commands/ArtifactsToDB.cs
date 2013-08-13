using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Model;
using NLog;
using run.Configuration;
using run.Misc;

namespace run.Commands
{
    class ArtifactsToDB
    {
        readonly static Logger logger = LogManager.GetCurrentClassLogger();

        public static bool Run()
        {
            logger.Info("Getting config section [artifacts].");
            var userConfig = (ArtifactsSection)ConfigurationManager.GetSection("artifacts");
            if (userConfig == null)
            {
                logger.Info("Failed to Find config section [artifacts].");
                return false;
            }

            string dirPath = userConfig.Directory;
            logger.Info("Searching artifacts path {0}", dirPath);

            logger.Info("Extracting folders from {1}.", dirPath);
            var paths = Directory.GetDirectories(dirPath).ToList();
            logger.Info("[{0}] folders in {1}.", paths.Count, dirPath);


            logger.Info("Extracting Builds and categories from database.");
            HashSet<string> categories;
            List<Build> existingBuilds;
            using (var context = new whatsgoingonEntities())
            {
                existingBuilds = context.Builds.ToList();
                categories = new HashSet<string>(context.Categories.Select(c => c.Name).ToList());
            }

            logger.Info("[{0} existing builds.", existingBuilds.Count);
            logger.Info("[{0} existing categories.", categories.Count);

            foreach (string artifactPath in paths)
            {
                bool incomplete = false;

                string folderName =
                    artifactPath.Substring(
                        artifactPath.LastIndexOf('\\') + 1);

                logger.Info("Processing [{0}]", folderName);

                Build newBuild = Helper.GetNewBuild(folderName, existingBuilds, artifactPath,
                    userConfig.ZipFile, userConfig.DllInZipFile);
                if (newBuild == null)
                {
                    continue;
                }

                foreach (ZipFileElement zipFile in userConfig.ZipFiles)
                {
                    string servicePath = Path.Combine(artifactPath, zipFile.File);
                    logger.Info("Processing [{0}]", servicePath);

                    var zip = new MyZipFile(servicePath);

                    foreach (ZipFolderElement zipFolder in zipFile.Folders)
                    {

                        foreach (AssemblyElement a in zipFolder.Assemblies)
                        {
                            var dll = a.Dll;

                            string fullPath = zipFolder.Path + dll;
                            logger.Info("Processing [{0}]", fullPath);

                            var assembly = zip.GetAssembly(fullPath);
                            if (assembly == null)
                            {
                                incomplete = true;
                                continue;
                            }

                            string assemblyNamespace =
                                a.Interface.Substring(0, a.Interface.LastIndexOf('.'));

                            var apiFromAssembly = assembly.GetType(a.Interface);
                            MethodInfo[] methods = apiFromAssembly.GetMethods();

                            string category = a.Category;
                            var api = new Api(category);
                            if (!categories.Contains(category))
                            {
                                logger.Info("Adding category [{0}]", category);
                                categories.Add(category);
                                using (var context = new whatsgoingonEntities())
                                {
                                    context.Categories.Add(new Category {Name = category});
                                    context.SaveChanges();
                                }
                            }

                            var nsTypes = new HashSet<Type>();

                            var signature = new StringBuilder(200);
                            foreach (var method in methods)
                            {
                                BuildMethodSignature(signature, method, assemblyNamespace, nsTypes);

                                api.AddMethod(method.Name, signature);
                            }


                            var typesCopy = new HashSet<Type>(nsTypes);

                            while (typesCopy.Count > 0)
                            {
                                var type = typesCopy.First();

                                if (type.IsClass)
                                {
                                    var classBuilder = BuildClassSignature(type, assemblyNamespace, nsTypes, typesCopy);

                                    api.AddClass(type.Name, classBuilder);
                                }
                                else if (type.IsEnum)
                                {
                                    var enumBuilder = BuildEnumSignature(type);

                                    api.AddEnum(type.Name, enumBuilder);
                                }

                                typesCopy.Remove(type);
                            }

                            api.ToBuild(newBuild);

                        }

                    }
                }

                var now = DateTime.Now;
                newBuild.Modified = now;
                newBuild.Created = now;
                newBuild.Incomplete = incomplete;

                logger.Info("Adding build to DB.");
                using (var context = new whatsgoingonEntities())
                {
                    context.Builds.Add(newBuild);
                    context.SaveChanges();
                }
            }

            return true;
        }

        
        private static StringBuilder BuildEnumSignature(Type type)
        {
            var enumBuilder = new StringBuilder();

            enumBuilder.Append("enum ")
                       .Append(type.Name)
                       .Append(" {")
                       .Append("\n");

            foreach (var name in Enum.GetNames(type))
            {
                enumBuilder.Append("  ")
                           .Append(name)
                           .Append(",")
                           .Append("\n");
            }
            enumBuilder.Append("}");
            return enumBuilder;
        }

        private static StringBuilder BuildClassSignature(Type type, string assemblyNamespace, HashSet<Type> nsTypes, HashSet<Type> typesCopy)
        {
            var classBuilder = new StringBuilder();
            classBuilder.Append("struct ")
                        .Append(type.Name)
                        .Append(" { ")
                        .Append("\n");

            int propNum = 1;
            foreach (var prop in type.GetProperties())
            {
                classBuilder.Append("  ")
                            .Append(propNum++)
                            .Append(":")
                            .Append(" ");

                var more = new HashSet<Type>();

                string propTypeName = GetThriftName(prop.PropertyType, assemblyNamespace, more);

                Type moreOne = more.FirstOrDefault();
                if (moreOne != null)
                {
                    if (!nsTypes.Contains(moreOne))
                    {
                        nsTypes.Add(moreOne);
                        typesCopy.Add(moreOne);
                    }
                }

                classBuilder.Append(propTypeName)
                            .Append(" ")
                            .Append(LowercaseFirst(prop.Name))
                            .Append(",")
                            .Append("\n");
            }

            classBuilder.Append("}");
            return classBuilder;
        }

        private static void BuildMethodSignature(StringBuilder signature, MethodInfo method, string assemblyNamespace,
                                                 HashSet<Type> nsTypes)
        {
            signature.Clear();

            signature.Append(GetThriftName(method.ReturnType, assemblyNamespace, nsTypes))
                     .Append(" ")
                     .Append(method.Name)
                     .Append("(");

            ParameterInfo[] ps = method.GetParameters();

            bool comma = false;

            int paramNum = 1;
            foreach (var p in ps)
            {
                if (comma)
                    signature.Append(", ");

                signature.Append(paramNum++)
                         .Append(":")
                         .Append(GetThriftName(p.ParameterType, assemblyNamespace, nsTypes))
                         .Append(" ")
                         .Append(p.Name);

                comma = true;
            }
            signature.Append(")");
        }


        private static string LowercaseFirst(string name)
        {
            return Char.ToLowerInvariant(name[0]) + name.Substring(1);
        }

        private static string GetThriftName(Type t, string ns, HashSet<Type> nsTypes)
        {
            if (t == typeof(string))
                return "string";
            if (t == typeof(byte))
                return "byte";
            if (t == typeof(Int16))
                return "i16";
            if (t == typeof(Int32))
                return "i32";
            if (t == typeof(Int64))
                return "i64";
            if (t == typeof(double))
                return "double";
            if (t == typeof(void))
                return "void";
            if (t == typeof(bool))
                return "bool";

            if (t.IsGenericType)
            {
                Type genericTypeDefinition = t.GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof (List<>))
                {
                    Type itemType = t.GetGenericArguments()[0];

                    string nestedName = GetThriftName(itemType, ns, nsTypes);
                    return string.Format("list<{0}>", nestedName);
                }
                
                if (genericTypeDefinition == typeof (Dictionary<,>))
                {
                    Type itemType0 = t.GetGenericArguments()[0];
                    string nestedName0 = GetThriftName(itemType0, ns, nsTypes);

                    Type itemType1 = t.GetGenericArguments()[1];
                    string nestedName1 = GetThriftName(itemType1, ns, nsTypes);

                    return string.Format("map<{0},{1}>", nestedName0, nestedName1);
                }
            }

            if (t.Namespace != null && t.Namespace.Equals(ns))
            {
                if (!nsTypes.Contains(t))
                    nsTypes.Add(t);

                return t.Name;
            }

            throw new Exception(string.Format("Type not found exception [{0}]",
                                              t.Name));
        }
    
    }
}
