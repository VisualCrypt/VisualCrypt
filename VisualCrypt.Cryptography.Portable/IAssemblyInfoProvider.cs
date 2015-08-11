namespace VisualCrypt.Cryptography.Portable
{
	public interface IAssemblyInfoProvider
	{
		string AssemblyProduct { get; }
		string AssemblyVersion { get; }
		string AssemblyCompany { get; }
		string AssemblyCopyright { get; }
	}
}
