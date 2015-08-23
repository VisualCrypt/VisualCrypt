using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.PubSubEvents;
using VisualCrypt.Applications.Portable.Apps.Services;
using VisualCrypt.Applications.Portable.Apps.Settings;
using VisualCrypt.Applications.Portable.Apps.ViewModels;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.Implementations;
using VisualCrypt.Desktop.ModuleEditor.Views;

namespace VisualCrypt.Desktop.Views
{
	[Export]
	public class ShellViewModel : PortableMainViewModel
	{
		[ImportingConstructor]
		public ShellViewModel(IEventAggregator eventAggregator, ILog log, IMessageBoxService messageBoxService, IEncryptionService encryptionService, INavigationService navigationService, IPasswordDialogDispatcher passwordDialogDispatcher, ISettingsManager settingsManager, IFileService fileService, IBrowserService browserService, IAssemblyInfoProvider assemblyInfoProvider, ILifeTimeService lifeTimeService, IClipBoardService clipBoardService, IWindowManager windowManager) 
			: base(eventAggregator, log, messageBoxService, encryptionService, navigationService, passwordDialogDispatcher, settingsManager, fileService, browserService, assemblyInfoProvider, lifeTimeService, clipBoardService, windowManager)
		{
			EditorViewModel.Context = this;
		}
	}
}
