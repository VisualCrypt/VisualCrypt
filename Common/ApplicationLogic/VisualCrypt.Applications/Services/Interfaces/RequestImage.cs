namespace VisualCrypt.Applications.Services.Interfaces
{
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
}