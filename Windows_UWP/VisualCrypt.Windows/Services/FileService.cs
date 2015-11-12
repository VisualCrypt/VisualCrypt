using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using VisualCrypt.Applications.Constants;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.Services.PortableImplementations;
using VisualCrypt.Language.Strings;
using VisualCrypt.Windows.Controls;
using VisualCrypt.Windows.Pages;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;

namespace VisualCrypt.Windows.Services
{
    class FileService : IFileService
    {
        readonly ILog _log;
        readonly SettingsManager _settingsManager;
        readonly ResourceWrapper _resourceWrapper;
        readonly IMessageBoxService _messageBoxService;

        public FileService()
        {
            _settingsManager = (SettingsManager)Service.Get<AbstractSettingsManager>();
            _log = Service.Get<ILog>();
            _resourceWrapper = Service.Get<ResourceWrapper>();
            _messageBoxService = Service.Get<IMessageBoxService>();
        }

        public async void WriteAllBytes(string pathAndFilename, byte[] encodedTextBytes)
        {
            var storageFolder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(pathAndFilename));
            var storageFile = await storageFolder.CreateFileAsync(Path.GetFileName(pathAndFilename), CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBytesAsync(storageFile, encodedTextBytes).AsTask();
        }

        /// <summary>
        /// ATM this code is the same in WPF and UWP, consider sharing!
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
                if (File.Exists(filename))
                    return true;
                return false;
            }
            catch (Exception e)
            {
                _log.Exception(e);
                return false;
            }
        }

        public bool Exists(string filename)
        {
            return System.IO.File.Exists(filename);
        }

        public string ReadAllText(string filename, Encoding selectedEncoding)
        {
            return System.IO.File.ReadAllText(filename, selectedEncoding);
        }

        public async Task<Tuple<bool, string>> PickFileAsync(string filenames, DialogFilter diaglogFilter, FileDialogMode fileDialogMode, string title = null)
        {
            switch (fileDialogMode)
            {
                case FileDialogMode.SaveAs:
                    return await SaveAsAsync();
                case FileDialogMode.Rename:
                    return await RenameAsync(filenames);
                case FileDialogMode.Delete:
                    return await DeleteAsync(new string[] { filenames }, title);
                case FileDialogMode.DeleteMany:
                    return await DeleteAsync(filenames.Split(';'), title);
            }
            throw new NotSupportedException(fileDialogMode.ToString());
        }

        /// <summary>
        /// Effectively called from PortableMainViewModel. Must return full valid path.
        /// </summary>
        async Task<Tuple<bool, string>> SaveAsAsync()
        {
            var canceledOrFailed = new Tuple<bool, string>(false, null);

            var mainPage = MainPagePhone.GetPageReference();
            var tcs = new TaskCompletionSource<Tuple<bool, string>>();
            mainPage.FilenameUserControl.InitViewModel(FileDialogMode.SaveAs, null, tcs.SetResult);

            mainPage.DisplayFilenameDialog();

            var result = await tcs.Task;
            if (!result.Item1)
            {
                mainPage.HideFilenameDialog();
                return canceledOrFailed;
            }

            var shortFilename = result.Item2 + PortableConstants.DotVisualCrypt;
            var pathAndFilename = Path.Combine(_settingsManager.CurrentDirectoryName, shortFilename);

            mainPage.HideFilenameDialog();

            return new Tuple<bool, string>(true, pathAndFilename);
        }

        async Task<Tuple<bool, string>> RenameAsync(string oldPathAndFilename)
        {
            var canceledOrFailed = new Tuple<bool, string>(false, null);

            var filesPage = FilesPage.GetPageReference();
            var tcs = new TaskCompletionSource<Tuple<bool, string>>();
            var oldShortFilename = Path.GetFileNameWithoutExtension(oldPathAndFilename);
            filesPage.FilenameUserControl.InitViewModel(FileDialogMode.Rename, new string[] { oldShortFilename }, tcs.SetResult);

            filesPage.DisplayFilenameDialog();

            var result = await tcs.Task;
            if (!result.Item1)
            {
                filesPage.HideFilenameDialog();
                return canceledOrFailed;
            }

            var newShortFilenamee = result.Item2 + PortableConstants.DotVisualCrypt;
            var newPathAndFilename = Path.Combine(Path.GetDirectoryName(oldPathAndFilename), newShortFilenamee);
            if (newPathAndFilename == oldPathAndFilename)
            {
                filesPage.HideFilenameDialog();
                return canceledOrFailed;
            }
            var fi = new FileInfo(oldPathAndFilename);
            fi.MoveTo(newPathAndFilename);

            filesPage.HideFilenameDialog();

            return new Tuple<bool, string>(true, newPathAndFilename);
        }

        async Task<Tuple<bool, string>> DeleteAsync(string[] suggestedFilename, string title)
        {
            var canceledOrFailed = new Tuple<bool, string>(false, null);


            return canceledOrFailed;
        }



        public byte[] ReadAllBytes(string filename)
        {
            return System.IO.File.ReadAllBytes(filename);
        }

        public string PathGetFileName(string filename)
        {
            return System.IO.Path.GetFileName(filename);
        }

        public string GetEncodingDisplayString(Encoding saveEncoding)
        {
            return saveEncoding.EncodingName;
        }

        async Task ShowSaveFileDialog(string suggestedFilename, DialogFilter diaglogFilter)
        {
            string saveStatus = string.Empty;

            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = suggestedFilename;

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);
                // write to file
                await FileIO.WriteTextAsync(file, file.Name);
                // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == FileUpdateStatus.Complete)
                {
                    saveStatus = "File " + file.Name + " was saved.";
                }
                else
                {
                    saveStatus = "File " + file.Name + " couldn't be saved.";
                }
            }
            else
            {
                saveStatus = "Operation cancelled.";
            }

        }


    }
}
