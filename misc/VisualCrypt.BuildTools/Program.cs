using System;
using System.IO;
using System.Text;

namespace VisualCrypt.BuildTools
{
    public class Program
    {
        public const string GenerateResourceWrapperSwitch = "-generateresourcewrapper";
        public const string InFilename = "Resources.designer.cs";
        public const string OutFilename = "ResourceWrapper.generated.cs";
        public const string TemplateFilename = "ResourceWrapper.generated.template.cs";
        public const string MergeMark = "// generated";
        public const string SuggestedCommandLine = @"$(SolutionDir)\misc\VisualCrypt.BuildTools\bin\Debug\VisualCrypt.BuildTools.exe -generateresourcewrapper $(ProjectDir)Strings";

        public static int Main(string[] args)
        {
            if (args.Length < 2 || args[0] != GenerateResourceWrapperSwitch)
                return HelpAndExit();
            try
            {
                if (!Directory.Exists(args[1]))
                {
                    Console.WriteLine(string.Format("Can't find directory {0}", args[1]));
                    return HelpAndExit();
                }
                var dir = args[1];
                var FullInfileName = Path.GetFullPath(Path.Combine(dir, InFilename));
                var FullTemplatefileName = Path.GetFullPath(Path.Combine(dir, TemplateFilename));
                var FullOutFileName = Path.GetFullPath(Path.Combine(dir, OutFilename));

                if (!File.Exists(FullInfileName))
                {
                    Console.WriteLine(string.Format("Can't find file {0}", FullInfileName));
                    return HelpAndExit();
                }
                if (!File.Exists(FullTemplatefileName))
                {
                    Console.WriteLine(string.Format("Can't find file {0}", FullTemplatefileName));
                    return HelpAndExit();
                }
                if (File.Exists(FullOutFileName))
                {
                    Console.WriteLine(string.Format("Deleting previous version of {0}", FullOutFileName));
                    File.Delete(FullOutFileName);
                    Console.WriteLine("Previous version deleted.");
                }
                var success = GenerateCode(FullInfileName, FullTemplatefileName, FullOutFileName);
                if (success == 0)
                    return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("Error: {0}", e.Message));
            }
            return HelpAndExit();
        }


        static int HelpAndExit()
        {
            Console.WriteLine(string.Format("Generates {0} from {1}", OutFilename, InFilename));
            Console.WriteLine(string.Format("To generate code for {0} use this command line as pre-build step in VisualCrypt.Languange: {1}", OutFilename, SuggestedCommandLine));
            return -1;
        }

        static int GenerateCode(string fullInfileName, string fullTemplateFilename, string fullOutFileName)
        {
            // 0. Contents of Resources.designer.cs
            var resources = File.ReadAllText(fullInfileName);

            // 1. Extract the relevant code area
            string pos1 = "resourceCulture = value;";
            int pos1Index = resources.IndexOf(pos1);

            string pos2 = "/// <summary>";
            int pos2Index = resources.IndexOf(pos2, pos1Index);  // find pos2 start looking at pos1Index

            string requiredCode = resources.Substring(pos2Index); // that's what we need, but the closing brackets } } for class and namespace are unwanted

            string netRequiredCode = requiredCode.Remove(requiredCode.LastIndexOf("}"));
            netRequiredCode = requiredCode.Remove(netRequiredCode.LastIndexOf("}"));

            // 2. Get the template
            var template = File.ReadAllText(fullTemplateFilename);
            if (template.IndexOf(MergeMark) < 0)
            {
                Console.WriteLine(string.Format("Can't find merge mark '{0}' in {1}", MergeMark, fullTemplateFilename));
            }

            // 3. Merge
            var merged = template.Replace(MergeMark, netRequiredCode);

            var a1 = "ResourceManager.GetString(\"";
            var b1 = "Resources.";
            merged = merged.Replace(a1, b1);

            var a2 = "\", resourceCulture);";
            var b2 = ";";
            merged = merged.Replace(a2, b2);

            var a3 = "internal static";
            var b3 = "public";
            merged = merged.Replace(a3, b3);

            // 4. Save result
            File.WriteAllText(fullOutFileName, merged, Encoding.Unicode);

            // 5. Done
            Console.WriteLine(string.Format("Successfully generated {0} from {1} using {2}", OutFilename, InFilename, TemplateFilename));
            return 0;
        }
    }
}
