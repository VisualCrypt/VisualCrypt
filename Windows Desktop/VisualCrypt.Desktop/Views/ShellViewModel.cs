using System.ComponentModel.Composition;
using Prism.Events;
using VisualCrypt.Applications.Apps.Services;
using VisualCrypt.Applications.Apps.Settings;
using VisualCrypt.Applications.Apps.ViewModels;
using VisualCrypt.Cryptography.VisualCrypt2.Interfaces;
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
