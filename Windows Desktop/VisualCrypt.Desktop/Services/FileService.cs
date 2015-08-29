﻿using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using VisualCrypt.Applications.Constants;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Language;

namespace VisualCrypt.Desktop.Services
{
    public class FileService : IFileService
    {
        readonly ILog _log;
        readonly ISettingsManager _settingsManager;

        public FileService()
        {
            _settingsManager = Service.Get<ISettingsManager>();
            _log = Service.Get<ILog>();
        }
        public bool CheckFilenameForQuickSave(string filename)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filename) || filename == Loc.Strings.constUntitledDotVisualCrypt)
                    return false;
                if (!filename.EndsWith(PortableConstants.DotVisualCrypt, StringComparison.Ordinal))
                    return false;
                if (File.Exists(filename))
                    return true;
                return false;
            }
            catch (Exception)
            {
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
            DialogDirection dialogDirection, string title = null)
        {
            FileDialog fileDialog;
            if (dialogDirection == DialogDirection.Open)
                fileDialog = new OpenFileDialog();
            else
                fileDialog = new SaveFileDialog();

            if (title != null)
                fileDialog.Title = title;

            if (!string.IsNullOrEmpty(suggestedFilename))
                fileDialog.FileName = suggestedFilename;

            fileDialog.InitialDirectory = _settingsManager.CurrentDirectoryName;
            if (diaglogFilter == DialogFilter.VisualCrypt)
            {
                fileDialog.DefaultExt = Constants.VisualCryptDialogFilter_DefaultExt;
                fileDialog.Filter = Constants.VisualCryptDialogFilter;
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


    }
}