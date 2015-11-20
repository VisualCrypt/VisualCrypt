using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using VisualCrypt.Applications.Constants;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.Services.PortableImplementations;
using VisualCrypt.Cryptography.VisualCrypt2.Infrastructure;
using VisualCrypt.Language.Strings;
using VisualCrypt.UWP.Pages;

namespace VisualCrypt.UWP.Services
{
    class FileService : IFileService
    {
        readonly ILog _log;
        readonly SettingsManager _settingsManager;
        readonly ResourceWrapper _resourceWrapper;
        readonly IMessageBoxService _messageBoxService;
        public readonly Dictionary<string, string> AccessTokens;


        public FileService()
        {
            _settingsManager = (SettingsManager)Service.Get<AbstractSettingsManager>();
            _log = Service.Get<ILog>();
            _resourceWrapper = Service.Get<ResourceWrapper>();
            _messageBoxService = Service.Get<IMessageBoxService>();
            AccessTokens = new Dictionary<string, string>();
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

        public bool Exists(string pathAndFilename)
        {
            var task = Task.Run(async () =>
            {
                var token = GetTokenByPathAndFilename(pathAndFilename);
                if (token != null)
                {
                    try
                    {
                        StorageFile tokenFile = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(token);
                        return tokenFile != null;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }

                var storageFolder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(pathAndFilename));
                var file = storageFolder.TryGetItemAsync(Path.GetFileName(pathAndFilename));
                return file != null;
            });
            task.Wait();
            return task.Result;

        }

        public async Task<byte[]> ReadAllBytes(string pathAndFilename, LongRunningOperationContext context)
        {
            var result = await Task.Run(async () =>
            {
                IStorageFile storagefile;
                var token = GetTokenByPathAndFilename(pathAndFilename);
                if (token != null)
                {
                    storagefile = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(token);
                }
                else
                {
                    storagefile = await StorageFile.GetFileFromPathAsync(pathAndFilename);
                }

                using (IRandomAccessStream readStream = await storagefile.OpenAsync(FileAccessMode.Read))
                {
                    using (DataReader dataReader = new DataReader(readStream))
                    {
                        dataReader.InputStreamOptions = InputStreamOptions.Partial;
                        ulong inputSize = readStream.Size;
                        if (inputSize > int.MaxValue)
                            throw new IOException("File is too big ");

                        byte[] outputBytes = new byte[inputSize];
                        uint readBufferSize = 8192u;
                        ulong writePos = 0;

                        // START encryptionProgress / Cancellation
                        context.CancellationToken.ThrowIfCancellationRequested();
                        context.EncryptionProgress.Percent = 0;
                        context.EncryptionProgress.Message = 0.ToString();
                        context.EncryptionProgress.Report(context.EncryptionProgress);

                        // END encryptionProgress
                        while (writePos<inputSize)
                        {
                            uint loaded = await dataReader.LoadAsync(readBufferSize);
                            IBuffer buffer = dataReader.ReadBuffer(loaded);
                            buffer.CopyTo(0u, outputBytes, (int)writePos, (int)buffer.Length);
                            writePos += loaded;

                            // START encryptionProgress / Cancellation
                            context.CancellationToken.ThrowIfCancellationRequested();
                            var progressValue = writePos / (decimal)(inputSize - 1) * 100m;
                            context.EncryptionProgress.Percent = (int)progressValue;
                            context.EncryptionProgress.Message = writePos.ToString();
                            context.EncryptionProgress.Report(context.EncryptionProgress);
                            // END encryptionProgress
                        }
                        return outputBytes;
                        //while (dataReader.UnconsumedBufferLength > 0)
                        //{
                        //    // START encryptionProgress / Cancellation
                        //    context.CancellationToken.ThrowIfCancellationRequested();
                        //    var progressValue = writePos / (decimal)(inputSize - 1) * 100m;
                        //    context.EncryptionProgress.Percent = (int)progressValue;
                        //    context.EncryptionProgress.Report(context.EncryptionProgress);

                        //    // END encryptionProgress
                        //    //IBuffer buffer = dataReader.ReadBuffer(readBufferSize);
                        //    //buffer.CopyTo(0u, outputBytes, writePos, (int)buffer.Length);
                        //    //writePos += (int)buffer.Length;


                        //}
                        //// START encryptionProgress / Cancellation
                        //context.CancellationToken.ThrowIfCancellationRequested();
                        //context.EncryptionProgress.Percent = (int)(writePos / (decimal)(inputSize - 1) * 100m); 
                        //context.EncryptionProgress.Report(context.EncryptionProgress);
                        // END encryptionProgress
                        return outputBytes;
                    }
                }
            });

            return result;
        }

        public async void WriteAllBytes(string pathAndFilename, byte[] encodedTextBytes)
        {
            IStorageFile storagefile;
            var token = GetTokenByPathAndFilename(pathAndFilename);
            if (token != null)
            {
                storagefile = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(token);
            }
            else
            {
                var storageFolder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(pathAndFilename));
                storagefile = await storageFolder.CreateFileAsync(Path.GetFileName(pathAndFilename), CreationCollisionOption.ReplaceExisting);
            }

            await FileIO.WriteBytesAsync(storagefile, encodedTextBytes).AsTask();
        }

        string GetTokenByPathAndFilename(string pathAndFilename)
        {
            if (AccessTokens.ContainsKey(pathAndFilename))
                return AccessTokens[pathAndFilename];
            return null;
        }




        public string ReadAllText(string filename, Encoding selectedEncoding)
        {
            return System.IO.File.ReadAllText(filename, selectedEncoding);
        }

        public async Task<Tuple<bool, string>> PickFileAsync(string filenames, DialogFilter diaglogFilter, FileDialogMode fileDialogMode, string title = null)
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

        async Task<Tuple<bool, string>> ExplicitSaveAsAsync(string defaultFileNameOrPathAndFilename, DialogFilter diaglogFilter, string title)
        {
            var canceledOrFailed = new Tuple<bool, string>(false, null);

            if (string.IsNullOrEmpty(defaultFileNameOrPathAndFilename))
                defaultFileNameOrPathAndFilename = _resourceWrapper.constUntitledDotVisualCrypt;

            bool hasPath = defaultFileNameOrPathAndFilename != Path.GetFileName(defaultFileNameOrPathAndFilename);

            var picker = new FileSavePicker
            {
                DefaultFileExtension = diaglogFilter == DialogFilter.Text ? ".txt" : ".visualcrypt"

            };
            if (diaglogFilter == DialogFilter.Text)
            {
                picker.FileTypeChoices.Add("Text", new List<string> { ".txt" });
                picker.FileTypeChoices.Add("All Files", new List<string> { "." });

            }
            else
            {
                picker.FileTypeChoices.Add("VisualCrypt", new List<string> { ".visualcrypt" });
                picker.FileTypeChoices.Add("All Files", new List<string> { "." });
            }


            if (!hasPath)
            {
                picker.SuggestedStartLocation = PickerLocationId.Desktop;
                picker.SuggestedFileName = defaultFileNameOrPathAndFilename;
            }
            else
            {
                var shortFilename = Path.GetFileName(defaultFileNameOrPathAndFilename);
                var directoryName = Path.GetDirectoryName(defaultFileNameOrPathAndFilename);
                if (directoryName.Contains("AppData"))
                {
                    picker.SuggestedStartLocation = PickerLocationId.Desktop;
                    picker.SuggestedFileName = shortFilename;
                }
                else
                {
                    picker.SuggestedSaveFile = await StorageFile.GetFileFromPathAsync(defaultFileNameOrPathAndFilename);
                }
            }

            var fileToSave = await picker.PickSaveFileAsync();

            if (fileToSave == null)
                return canceledOrFailed;

            if (!Path.GetDirectoryName(fileToSave.Path).Equals(ApplicationData.Current.LocalFolder.Path))
            {
                string fileToken = StorageApplicationPermissions.FutureAccessList.Add(fileToSave, fileToSave.Path);
                AccessTokens[fileToSave.Path] = fileToken; // add or replace
            }
            return new Tuple<bool, string>(true, fileToSave.Path);
        }

        async Task<Tuple<bool, string>> OpenAsyncWithSystemPicker(DialogFilter diaglogFilter, string title)
        {
            var canceledOrFailed = new Tuple<bool, string>(false, null);

            FileOpenPicker picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.Desktop };
            picker.FileTypeFilter.Add("*");
            StorageFile fileToOpen = await picker.PickSingleFileAsync();
            if (fileToOpen == null)
                return canceledOrFailed;

            string fileToken = StorageApplicationPermissions.FutureAccessList.Add(fileToOpen, fileToOpen.Path);
            AccessTokens[fileToOpen.Path] = fileToken; // add or replace
            return new Tuple<bool, string>(true, fileToOpen.Path);
        }

        private Task<Tuple<bool, string>> DeleteAsyncWithCustomDialog(string[] v, string title)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Effectively called from PortableMainViewModel. Must return full valid path.
        /// </summary>
        async Task<Tuple<bool, string>> SaveAsAsync()
        {
            var canceledOrFailed = new Tuple<bool, string>(false, null);

            var mainPage = MainPagePhone.PageReference;
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

            var filesPage = FilesPage.PageReference;
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

        public async Task DeleteAsync(string pathAndFilename)
        {
            var storageFile = await StorageFile.GetFileFromPathAsync(pathAndFilename);
            await storageFile.DeleteAsync();
        }





        public string PathGetFileName(string filename)
        {
            return System.IO.Path.GetFileName(filename);
        }

        public string GetEncodingDisplayString(Encoding saveEncoding)
        {
            return saveEncoding != null ?
             saveEncoding.EncodingName
             : "HEX";
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

        public async Task<ObservableCollection<FileReference>> GetFileReferences(string directoryPath)
        {
            var fold = await StorageFolder.GetFolderFromPathAsync(directoryPath);

            var items = await fold.GetItemsAsync();
            var files = new ObservableCollection<FileReference>();
            foreach (var item in items)
            {
                var storageFile = item as StorageFile;
                if (storageFile == null)
                    continue;
                if (storageFile.FileType.ToLowerInvariant() != ".visualcrypt")
                    continue;
                var basicProperties = await storageFile.GetBasicPropertiesAsync();
                var modifiedDate = basicProperties.DateModified.ToLocalTime().ToString();
                files.Add(new FileReference
                {
                    ShortFilename = storageFile.Name,
                    PathAndFileName = storageFile.Path,
                    ModifiedDate = modifiedDate,
                    FileSystemObject = storageFile
                });
            }
            return files;
        }

    }
}
