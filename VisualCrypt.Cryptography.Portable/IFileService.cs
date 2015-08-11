﻿using System;
using System.Text;
using System.Threading.Tasks;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.AppLogic;

namespace VisualCrypt.Cryptography.Portable
{
    public interface IFileService
    {
        void WriteAllBytes(string filename, byte[] encodedTextBytes);

        bool CheckFilenameForQuickSave(string filename);

	    bool Exists(string filename);
	    string ReadAllText(string filename, Encoding selectedEncoding);

	    Task<Tuple<bool, string>> PickFileAsync(string suggestedFilename, DialogFilter diaglogFilter,
		    DialogDirection dialogDirection, string title = null);
    }
}
