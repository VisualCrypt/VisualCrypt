using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisualCrypt.Portable.APIV2.DataTypes;

namespace VisualCrypt.Desktop.Features
{
    class SaveLogic
    {
        private bool isSaveButtonEnabled;

        // in view
         public void TextChanged() 
         {
            if(!isSaveButtonEnabled)
                isSaveButtonEnabled = true;
        }

     
      

        // in viewController
        // Model
        private FileModel file;

       



        // in ViewController
        public void SaveCommand() {
        if (file.IsEncrypted)
        {
            //saveCipherText();
        } else
        {
           // savePlaintextAfterEncrypting();
        }
    }
    
    //public void EncryptCommand()
    //{
    //    if !file.IsPasswordPresent
    //    {
    //        passwordAlert.queryForPassword(self, callback: {
    //            (password: String?) in
    //            if password != nil {
    //                self.file.setPassword(password!)
    //                self.encryptCommand() // loop!
    //            }
    //        })
    //    } else {
    //        encryptCore()
    //    }
    //}
    
    public void decryptCommand(string text)
    {
        if (!file.IsPasswordPresent)
        {
            passwordAlert_queryForPassword(this, (delegate(byte[] password)
            {

                if (password != null)
                {
                    file.SetPassword(password);
                    decryptCommand(text); // loop!
                }
            }));
        } else
        {
            decryptCore(text);
        }
    }

        private void passwordAlert_queryForPassword(SaveLogic saveLogic, Action<byte[]>  callback)
        {
            callback(new byte[0]);
            
        }

    //    private func encryptCore() {
    //    editorView.textBox1.text = file.encrypt(editorView.textBox1.text)
    //    editorView.switchToCipherTextAccessoryView(self)
    //    copyVisualCryptToClipboard()
    //    info("Encrypted, copied to clipboard!")
    //}
    
    void  decryptCore(string text)
    {
       // file.Decrypt(new VisualCryptText(text));
        //    editorView.switchToPlainTextAccessoryView(self)
    }
    
    //private func saveCore() {
    //    file.saveEncrypted()
    //    setFilenameTitle(file.Filename)
    //    editorView.saveButton.enabled = false
    //    info("Saved encrypted!")
    //}
    

    //    private func saveCipherText()
    //{
    //    println("*** File is already encrypted, saving immediately!")
        
    //    if !file.IsFilenamePresent {
    //        println("*** Filename is not valid, querying for filename.")
    //        filenameAlert.queryForfilename(self, callback: {
    //            (filename: String?) in
    //            if filename != nil {
    //                self.file.setFilename(filename!)
    //                self.saveCommand() // loop!
    //            }
    //        })
    //    } else {
    //        saveCore() // core action now
    //    }
    //}
    
    //private func savePlaintextAfterEncrypting() {
        
    //    println("*** File text is not yet encrypted, starting extensive save workflow.")
        
    //    if !file.IsFilenamePresent
    //    {
    //        println("*** Filename is not valid, querying for filename.")
    //        filenameAlert.queryForfilename(self, callback: {
    //            (filename: String?) in
    //            if filename != nil {
    //                self.file.setFilename(filename!)
    //                self.saveCommand()  // loop!
    //            }
    //        })
    //        return
    //    }
        
    //    println("*** Filename is valid, checking for password.")
        
    //    if !file.IsPasswordPresent
    //    {
    //        println("*** Password is not set, querying for password.")
    //        passwordAlert.queryForPassword(self, callback: {
    //            (password: String?) in
    //            if password != nil {
    //                self.file.setPassword(password!)
    //                self.saveCommand() // loop!
    //            } else {
    //                println("*** No password was entered, stopping.")
    //            }
    //        })
    //        return
    //    }
        
    //    info("Encrypting & saving...")
        
    //    println("*** Filename AND Password is set!")
        
    //    println("*** Encrypting File... ")
    //    encryptCore()
        
    //    println("*** Saving File...")
    //    saveCore() // core action now
        
    //    Delay.delay(0.5, closure: {
    //        self.decryptCore()
    //    })
    //}
    }
}
