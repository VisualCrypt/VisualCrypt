using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace VisualCrypt.Windows.Temp
{
    class SampleFiles
    {
        const string SecretText = @"VisualCrypt/AgoAYmsPKTVUZuqcLlpkX38rwYoryXOm+V10ZHyFEo0QFHmCkM29kl37faR76l
                                    wt0u$y6C+Jtqh37EridGFHLwu4t9OMRjRy232qfRuFR$T5C3M=";
        public static async Task CreateSampleFiles()
        {
            StorageFolder folder1 = ApplicationData.Current.LocalFolder;
            StorageFolder folder2 = ApplicationData.Current.LocalFolder;

            var cleartextFile = await folder1.CreateFileAsync("Cleartext.txt", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(cleartextFile, "Hello Developer!").AsTask();
            var sampleSecret = await folder2.CreateFileAsync("Secret.visualcrypt", CreationCollisionOption.ReplaceExisting).AsTask();
            await FileIO.WriteTextAsync(sampleSecret, SecretText).AsTask();

        }
    }
}
