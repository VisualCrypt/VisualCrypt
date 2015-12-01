using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Cryptography.VisualCrypt2.Infrastructure;
using VisualCrypt.Applications.Constants;
using VisualCrypt.Language.Strings;
using Service = VisualCrypt.Applications.Services.Interfaces.Service;
using VisualCrypt.Applications.Services.PortableImplementations;
using System.IO;
using VisualCrypt.Views;

namespace VisualCrypt.Droid.Services
{
    class FileService : IFileService
    {

        readonly ILog _log;
        readonly SettingsManager _settingsManager;
        readonly ResourceWrapper _resourceWrapper;
        readonly IMessageBoxService _messageBoxService;
        public readonly Dictionary<string, string> AccessTokens;

        string LocalDriectory
        {
            get { return "lala1"; }
        }


        public FileService()
        {
            _settingsManager = (SettingsManager)Service.Get<AbstractSettingsManager>();
            _log = Service.Get<ILog>();
            _resourceWrapper = Service.Get<ResourceWrapper>();
            _messageBoxService = Service.Get<IMessageBoxService>();
            AccessTokens = new Dictionary<string, string>();
        }
        /// <summary>
        /// ATM this code is the same in WPF, Android and UWP, consider sharing!
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool CheckFilenameForQuickSave(string filename)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filename) || filename == _resourceWrapper.constUntitledDotVisualCrypt)
                    return false;
                if (!filename.EndsWith(PortableConstants.DotVisualCrypt, StringComparison.Ordinal))
                    return false;
                if (Exists(filename))
                    return true;
                return false;
            }
            catch (Exception e)
            {
                _log.Exception(e);
                return false;
            }
        }

        public Task DeleteAsync(string pathAndFilename)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string filename)
        {
            return File.Exists(filename);
        }

        public string GetEncodingDisplayString(Encoding saveEncoding)
        {
            return saveEncoding.EncodingName;
        }

      

        public string PathGetFileName(string filename)
        {
            return Path.GetFileName(filename);
        }

        public async Task<Tuple<bool, string>> PickFileAsync(string filenames, DialogFilter diaglogFilter, FileDialogMode fileDialogMode, string title)
        {
            switch (fileDialogMode)
            {
                case FileDialogMode.SaveAs: // Built-in filename prompt
                    return await SaveAsAsync();
                case FileDialogMode.ExplicitSaveAs: // System SaveFilePicker
                    return await ExplicitSaveAsAsync(filenames, diaglogFilter, title);
                case FileDialogMode.Rename:
                    return await RenameAsync(filenames);
                case FileDialogMode.Delete:
                    return await DeleteAsyncWithCustomDialog(new string[] { filenames }, title);
                case FileDialogMode.DeleteMany:
                    return await DeleteAsyncWithCustomDialog(filenames.Split(';'), title);
                case FileDialogMode.Open:
                    return await OpenAsyncWithSystemPicker(diaglogFilter, title);
            }
            throw new NotSupportedException(fileDialogMode.ToString());
           
        }

        private Task<Tuple<bool, string>> OpenAsyncWithSystemPicker(DialogFilter diaglogFilter, string title)
        {
            throw new NotImplementedException();
        }

        private Task<Tuple<bool, string>> DeleteAsyncWithCustomDialog(string[] v, string title)
        {
            throw new NotImplementedException();
        }

        private Task<Tuple<bool, string>> RenameAsync(object filenames)
        {
            throw new NotImplementedException();
        }

        private Task<Tuple<bool, string>> ExplicitSaveAsAsync(object filenames, DialogFilter diaglogFilter, string title)
        {
            throw new NotImplementedException();
        }

        async Task<Tuple<bool, string>> SaveAsAsync()
        {
            var canceledOrFailed = new Tuple<bool, string>(false, null);
          
            var tcs = new TaskCompletionSource<Tuple<bool, string>>();
            MainActivity.MainActivityInstance.DisplayFilenameDialog(FileDialogMode.SaveAs, new string[] { }, tcs.SetResult);
           
            var result = await tcs.Task;
            if (!result.Item1)
            {
                MainActivity.MainActivityInstance.HideFilenameDialog();
                return canceledOrFailed;
            }

            var shortFilename = result.Item2 + PortableConstants.DotVisualCrypt;
            var pathAndFilename = Path.Combine(_settingsManager.CurrentDirectoryName, shortFilename);

            MainActivity.MainActivityInstance.HideFilenameDialog();

            return new Tuple<bool, string>(true, pathAndFilename);
        }

        public async Task<byte[]> ReadAllBytes(string filename, LongRunningOperationContext context)
        {
            var tcs = new TaskCompletionSource<byte[]>();
            var fileBytes = File.ReadAllBytes(filename);
            tcs.SetResult(fileBytes);
            return await tcs.Task;
        }

        public string ReadAllText(string filename, Encoding selectedEncoding)
        {
            return File.ReadAllText(filename, selectedEncoding);
        }

        public void WriteAllBytes(string filename, byte[] encodedTextBytes)
        {
            File.WriteAllBytes(filename, encodedTextBytes);
        }

        public async Task<ObservableCollection<FileReference>> GetFileReferences(string directoryPath)
        {
           
            var items = new DirectoryInfo(_settingsManager.CurrentDirectoryName).EnumerateFiles();

            
            var files = new ObservableCollection<FileReference>();
            foreach (var item in items)
            {
               
                if (item.Extension.ToLowerInvariant() != ".visualcrypt")
                    continue;
               
                files.Add(new FileReference
                {
                    ShortFilename = item.Name,
                    PathAndFileName = item.FullName,
                    ModifiedDate = item.LastWriteTimeUtc.ToString(),
                    FileSystemObject = item
                });
            }
            return files;
        }
    }
}