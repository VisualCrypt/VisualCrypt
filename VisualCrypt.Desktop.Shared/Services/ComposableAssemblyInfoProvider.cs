using System.ComponentModel.Composition;
using VisualCrypt.Cryptography.Portable;
using VisualCrypt.Cryptography.Portable.Apps.Services;

namespace VisualCrypt.Desktop.Shared.Services
{
	[Export(typeof(IAssemblyInfoProvider))]
	public class ComposableAssemblyInfoProvider : AssemblyInfoProvider
	{
	}
}
