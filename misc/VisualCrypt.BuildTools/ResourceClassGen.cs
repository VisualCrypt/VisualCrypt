using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace VisualCrypt.BuildTools
{
    static class ResourceClassGen
    {
        const string csnamespace = "namespace VisualCrypt.Language.Strings {";



        public static int Run(string directory)
        {
            try
            {
                var languages = new string[] { "en", "de", "ru", "fr", "it" };

                foreach (var l in languages)
                {
                    var resourceDictionary = LoadResourceDictionary(l, directory);
                    if (l == "en")
                        GenerateInterface(directory, resourceDictionary);
                    GenerateInterfaceImplementation(directory, l, resourceDictionary);
                }
                Console.WriteLine("VisualCrypt.BuildTools: Resources code generation complete!");
            }
            catch (Exception e)
            {
                Console.WriteLine("VisualCrypt.BuildTools: Error in Resources code generation: " + e.Message);
                return -23;
            }
            return 0;
        }

        private static void GenerateInterfaceImplementation(string directory, string lang, Dictionary<string, string> resourceDictionary)
        {
            var sb = new StringBuilder();
            sb.AppendLine("namespace VisualCrypt.Language.Strings {");
            sb.AppendFormat("class Generated_{0} : IGeneratedResource {{", lang);
            foreach (var entry in resourceDictionary)
                AppendPropertyImplementation(sb, entry.Key, entry.Value);
            sb.AppendLine("}");
            sb.AppendLine("}");
            var generatedClass = sb.ToString();
            var filename = Path.Combine(directory, string.Format("Generated_{0}.cs", lang));
            File.WriteAllText(filename, generatedClass, Encoding.Unicode);
            Console.WriteLine("Generated {0}", filename);


        }

        static void GenerateInterface(string directory, Dictionary<string, string> resourceDictionary)
        {
            var sb = new StringBuilder();
            sb.AppendLine("namespace VisualCrypt.Language.Strings {");
            sb.AppendLine("interface IGeneratedResource {");
            foreach (var entry in resourceDictionary)
                AppendPropSignature(sb, entry.Key, entry.Value);
            sb.AppendLine("}");
            sb.AppendLine("}");
            var generatedInterface = sb.ToString();
            File.WriteAllText(Path.Combine(directory, "IGeneratedResource.cs"), generatedInterface, Encoding.Unicode);

        }

        static Dictionary<string, string> LoadResourceDictionary(string language, string directory)
        {
            var resxFilename = language == "en" ? "resources.resx" : string.Format("resources.{0}.resx", language);
            var doc = XDocument.Load(Path.Combine(directory, resxFilename));
            var dict = new Dictionary<string, string>();

            // get all of these:
            // <data name="constUntitledDotVisualCrypt" xml:space = "preserve">
            // <value> Untitled.visualcrypt </value>
            // </data>
            var dataElements = doc.Root.Elements("data")
                        .Where(e => e.Attribute("name") != null)
                        .Where(e => e.HasElements);

            foreach (var de in dataElements)
            {
                var propname = de.Attribute("name").Value;

                var contents = de.Element("value").Value;
                if (contents == null)
                    contents = string.Empty;
                contents = contents.Replace("\r", "").Replace("\n", "");

                dict.Add(propname, contents);
            }
            return dict;
        }

        static void AppendPropertyImplementation(StringBuilder sb, string propname, string value)
        {
            sb.AppendFormat("public string {0} {{ get {{ return \"{1}\"; }} }}\r\n", propname, value);  // double '{' means escape {
            sb.AppendLine();
        }

        /// <summary>
        /// Text like {0}
        /// </summary>
        static void AppendPropSignature(StringBuilder sb, string propname, string englishValue)
        {
            sb.AppendLine("/// <summary>");
            sb.AppendFormat("/// Text like '{0}'.\r\n", englishValue);
            sb.AppendLine("/// </summary>");
            sb.AppendFormat("string {0} {{ get; }}\r\n", propname);  // double '{' means escape {
            sb.AppendLine();
        }
    }
}
