namespace VisualCrypt.Applications.Services.Interfaces
{
	public interface IAssemblyInfoProvider
	{
		string AssemblyProduct { get; }
		string AssemblyVersion { get; }
		string AssemblyCompany { get; }
		string AssemblyCopyright { get; }
	}
}
