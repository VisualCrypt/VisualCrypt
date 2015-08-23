namespace VisualCrypt.Applications.Portable.Apps.Services
{
	public interface IAssemblyInfoProvider
	{
		string AssemblyProduct { get; }
		string AssemblyVersion { get; }
		string AssemblyCompany { get; }
		string AssemblyCopyright { get; }
	}
}
