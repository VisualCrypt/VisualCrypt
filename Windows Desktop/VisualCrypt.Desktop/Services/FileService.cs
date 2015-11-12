using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using VisualCrypt.Applications.Constants;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.Services.PortableImplementations;
using VisualCrypt.Language.Strings;

namespace VisualCrypt.Desktop.Services
{
    public class FileService : IFileService
    {
        readonly ILog _log;
        readonly SettingsManager _settingsManager;
        readonly ResourceWrapper _resourceWrapper;

        public FileService()
        {
            _settingsManager = (SettingsManager)Service.Get<AbstractSettingsManager>();
            _log = Service.Get<ILog>();
            _resourceWrapper = Service.Get<ResourceWrapper>();
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

        public void WriteAllBytes(string filename, byte[] encodedTextBytes)
        {
            File.WriteAllBytes(filename, encodedTextBytes);
        }

        public bool Exists(string filename)
        {
            return File.Exists(filename);
        }

        public string ReadAllText(string filename, Encoding selectedEncoding)
        {
            return File.ReadAllText(filename, selectedEncoding);
        }

        public async Task<Tuple<bool, string>> PickFileAsync(string suggestedFilename, DialogFilter diaglogFilter,
            FileDialogMode dialogDirection, string title = null)
        {
            FileDialog fileDialog;
            if (dialogDirection == FileDialogMode.Open)
                fileDialog = new OpenFileDialog();
            else
                fileDialog = new SaveFileDialog();

            if (title != null)
                fileDialog.Title = title;


            if (!string.IsNullOrEmpty(suggestedFilename))
            {
                fileDialog.FileName = Path.GetFileName(suggestedFilename);
                if (fileDialog.FileName == suggestedFilename)  //equal if suggestedfilename did not include a path
                {
                    fileDialog.InitialDirectory = _settingsManager.CurrentDirectoryName;
                }
                else
                {
                    fileDialog.InitialDirectory = Path.GetDirectoryName(suggestedFilename);
                }
                
                
            }

            if (diaglogFilter == DialogFilter.VisualCrypt)
            {
                fileDialog.DefaultExt = Constants.VisualCryptDialogFilter_DefaultExt;
                fileDialog.Filter = Constants.VisualCryptDialogFilter;
            }
            else if (diaglogFilter == DialogFilter.VisualCryptAndText)
            {
                fileDialog.DefaultExt = Constants.VisualCryptAndTextDialogFilter_DefaultExt;
                fileDialog.Filter = Constants.VisualCryptAndTextDialogFilter;
            }
            else
            {
                fileDialog.DefaultExt = Constants.TextDialogFilter_DefaultExt;
                fileDialog.Filter = Constants.TextDialogFilter;
            }

            var tcs = new TaskCompletionSource<Tuple<bool, string>>();

            var okClicked = fileDialog.ShowDialog() == true;
            var selectedFilename = fileDialog.FileName;

            tcs.SetResult(new Tuple<bool, string>(okClicked, selectedFilename));
            return await tcs.Task;
        }

        public byte[] ReadAllBytes(string filename)
        {
            return File.ReadAllBytes(filename);
        }

        public string PathGetFileName(string filename)
        {
            return Path.GetFileName(filename);
        }

        public string GetEncodingDisplayString(Encoding saveEncoding)
        {

            return saveEncoding != null ?
                saveEncoding.EncodingName
                : "HEX";
        }
    }
}