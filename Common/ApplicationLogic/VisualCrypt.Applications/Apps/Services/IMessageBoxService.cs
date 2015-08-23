using System;
using System.Runtime.CompilerServices;

namespace VisualCrypt.Applications.Portable.Apps.Services
{
    public interface IMessageBoxService
    {
        RequestResult Show(string messageBoxText, string title, RequestButton buttons,
            RequestImage image);

        void ShowError(Exception e, [CallerMemberName] string callerMemberName = "");

        void ShowError(string error);
    }

	public enum RequestResult
	{

		None = 0,

	
		OK = 1,

		
		Cancel = 2,

	
		Yes = 6,

		No = 7,

	}

	
	public enum RequestImage
	{
		
		None = 0,

		
		Hand = 0x00000010,

		
		Question = 0x00000020,

		
		Exclamation = 0x00000030,

		
		Asterisk = 0x00000040,

		
		Stop = Hand,

		
		Error = Hand,

		
		Warning = Exclamation,

	
		Information = Asterisk,

		 
	}
	
	public enum RequestButton
	{
		
		OK = 0x00000000,

		OKCancel = 0x00000001,

		YesNoCancel = 0x00000003,

		
		YesNo = 0x00000004,

	}
}
