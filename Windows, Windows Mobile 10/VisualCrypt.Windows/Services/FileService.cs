using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.ViewManagement;

namespace VisualCrypt.Windows.Services
{
    class FileService : IFileService
    {
        public void WriteAllBytes(string filename, byte[] encodedTextBytes)
        {
            System.IO.File.WriteAllBytes(filename,encodedTextBytes);
        }

        public bool CheckFilenameForQuickSave(string filename)
        {
            return true;
        }

        public bool Exists(string filename)
        {
            return System.IO.File.Exists(filename);
        }

        public string ReadAllText(string filename, Encoding selectedEncoding)
        {
            return System.IO.File.ReadAllText(filename,selectedEncoding);
        }

        public async Task<Tuple<bool, string>> PickFileAsync(string suggestedFilename, DialogFilter diaglogFilter, DialogDirection dialogDirection, string title = null)
        {
            if (dialogDirection == DialogDirection.Save)
            {
                await ShowSaveFileDialog();
                
            }
               
            throw new NotImplementedException();
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

        async Task ShowSaveFileDialog()
        {
            string saveStatus = string.Empty;
            if (EnsureUnsnapped())
            {
                FileSavePicker savePicker = new FileSavePicker();
                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                // Dropdown of file types the user can save the file as
                savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
                // Default file name if the user does not type one in or select a file to replace
                savePicker.SuggestedFileName = "New Document";

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

        bool EnsureUnsnapped()
        {
            // FilePicker APIs will not work if the application is in a snapped state.
            // If an app wants to show a FilePicker while snapped, it must attempt to unsnap first
            bool unsnapped = ((ApplicationView.Value != ApplicationViewState.Snapped) || ApplicationView.TryUnsnap());
            if (!unsnapped)
            {
               // NotifyUser("Cannot unsnap the sample.", NotifyType.StatusMessage);
            }

            return unsnapped;
        }
    }
}
