using System;
using System.IO;
using System.Text;

namespace VisualCrypt.BuildTools
{
    public class Program
    {
        // Feature 1
        const string GenerateResourceWrapperSwitch = "-generateresourcewrapper";
        const string InFilename = "Resources.designer.cs";
        const string OutFilename = "ResourceWrapper.generated.cs";
        const string TemplateFilename = "ResourceWrapper.generated.template.cs";
        const string MergeMark = "// generated";
        const string SuggestedCommandLine = @"$(SolutionDir)\misc\VisualCrypt.BuildTools\bin\Debug\VisualCrypt.BuildTools.exe -generateresourcewrapper $(ProjectDir)Strings";

        public static string ResXFilesPath;
        public static string CodeGenerationPath;

        // Feature 2
        const string GenerateResourceClassesSwitch = "-generateresourceclasses";

        public static int Main(string[] args)
        {
            if (args.Length < 2 || (args[0] != GenerateResourceWrapperSwitch && args[0] != GenerateResourceClassesSwitch))
                return HelpAndExit();

            CodeGenerationPath = args[1]; // Project root Dir of VisualCrypt.Language
            ResXFilesPath = CodeGenerationPath.Replace("VisualCrypt.Language", "VisualCrypt.Language.Editing");

            // The working dir is needed for feature 1 and 2
            if (!Directory.Exists(CodeGenerationPath)) 
            {
                Console.WriteLine("Can't find directory {0}", CodeGenerationPath);
                return HelpAndExit();
            }
            if (!Directory.Exists(ResXFilesPath)) 
            {
                Console.WriteLine("Can't find directory {0}", ResXFilesPath);
                return HelpAndExit();
            }

            // Feature 2
            if (args[0] == GenerateResourceClassesSwitch)
            {
                int result = ResourceClassGen.Run();
                if (result == 0)
                    return result;
                return HelpAndExit();
            }

            // Feature 1
            try
            {

                var fullInfileName = Path.GetFullPath(Path.Combine(ResXFilesPath, InFilename));
                var fullTemplatefileName = Path.GetFullPath(Path.Combine(CodeGenerationPath, TemplateFilename));
                var fullOutFileName = Path.GetFullPath(Path.Combine(CodeGenerationPath, OutFilename));

                if (!File.Exists(fullInfileName))
                {
                    Console.WriteLine("Can't find file {0}", fullInfileName);
                    return HelpAndExit();
                }
                if (!File.Exists(fullTemplatefileName))
                {
                    Console.WriteLine("Can't find file {0}", fullTemplatefileName);
                    return HelpAndExit();
                }
                if (File.Exists(fullOutFileName))
                {
                    Console.WriteLine("Deleting previous version of {0}", fullOutFileName);
                    File.Delete(fullOutFileName);
                    Console.WriteLine("Previous version deleted.");
                }
                var success = GenerateCode(fullInfileName, fullTemplatefileName, fullOutFileName);
                if (success == 0)
                    return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
            return HelpAndExit();
        }


        static int HelpAndExit()
        {
            // Feature 1
            Console.WriteLine("Generates {0} from {1}", OutFilename, InFilename);
            Console.WriteLine("To generate code for {0} use this command line as pre-build step in VisualCrypt.Languange: {1}", OutFilename, SuggestedCommandLine);

            // Feature 2
            Console.WriteLine("OR");
            Console.WriteLine("Use the switch {0} to generate 'Resource Classes' from .resx XML files.", GenerateResourceClassesSwitch);
            return -1;
        }

        static int GenerateCode(string fullInfileName, string fullTemplateFilename, string fullOutFileName)
        {
            // 0. Contents of Resources.designer.cs
            var resources = File.ReadAllText(fullInfileName);

            // 1. Extract the relevant code area
            string pos1 = "resourceCulture = value;";
            int pos1Index = resources.IndexOf(pos1, StringComparison.Ordinal);

            string pos2 = "/// <summary>";
            int pos2Index = resources.IndexOf(pos2, pos1Index, StringComparison.Ordinal);  // find pos2 start looking at pos1Index

            string requiredCode = resources.Substring(pos2Index); // that's what we need, but the closing brackets } } for class and namespace are unwanted

            string netRequiredCode = requiredCode.Remove(requiredCode.LastIndexOf("}", StringComparison.Ordinal));
            netRequiredCode = requiredCode.Remove(netRequiredCode.LastIndexOf("}", StringComparison.Ordinal));

            // 2. Get the template
            var template = File.ReadAllText(fullTemplateFilename);
            if (template.IndexOf(MergeMark, StringComparison.Ordinal) < 0)
            {
                Console.WriteLine("Can't find merge mark '{0}' in {1}", MergeMark, fullTemplateFilename);
            }

            // 3. Merge
            var merged = template.Replace(MergeMark, netRequiredCode);

            const string a1 = "ResourceManager.GetString(\"";
            const string b1 = "_generatedResource.";
            merged = merged.Replace(a1, b1);

            const string a2 = "\", resourceCulture);";
            const string b2 = ";";
            merged = merged.Replace(a2, b2);

            const string a3 = "internal static";
            const string b3 = "public";
            merged = merged.Replace(a3, b3);

            // 4. Save result
            File.WriteAllText(fullOutFileName, merged, Encoding.Unicode);

            // 5. Done
            Console.WriteLine("Successfully generated {0} from {1} using {2}", OutFilename, InFilename, TemplateFilename);
            return 0;
        }
    }
}
