using System.Collections.Generic;
using System.Text;
using Model;

namespace run.Misc
{
    class Api
    {
        public Api(string category)
        {
            Category = category;
        }

        public string Category { get; private set; }

        private readonly Dictionary<string, string> methods = new Dictionary<string, string>();
        private readonly Dictionary<string, string> classes = new Dictionary<string, string>();
        private readonly Dictionary<string, string> enums = new Dictionary<string, string>(); 

        public void AddMethod(string name, StringBuilder signature)
        {
            string method = signature.ToString();
            methods.Add(name, method);
        }

        public void AddClass(string name, StringBuilder classBuilder)
        {
            string newClass = classBuilder.ToString();
            classes.Add(name, newClass);
        }

        public void AddEnum(string name, StringBuilder enumBuilder)
        {
            string newEnum = enumBuilder.ToString();
            enums.Add(name, newEnum);
        }

        public void ToBuild(Build build)
        {
            AddData(build, "Class", classes);
            AddData(build, "Enum", enums);
            AddData(build, "Method", methods);
        }

        private void AddData(Build build, string type, Dictionary<string, string> data)
        {
            foreach (KeyValuePair<string, string> c in data)
            {
                var d = new BuildData
                {
                    Category = Category,
                    Name = c.Key,
                    Data = c.Value,
                    Type = type,
                };

                build.BuildDatas.Add(d);
            }
        }
    }
}