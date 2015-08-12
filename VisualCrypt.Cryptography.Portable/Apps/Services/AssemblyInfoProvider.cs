using System;
using System.Composition;
using System.Linq;
using System.Reflection;

namespace VisualCrypt.Cryptography.Portable.Apps.Services
{
	[Export(typeof(IAssemblyInfoProvider))]
	public class AssemblyInfoProvider : IAssemblyInfoProvider
	{
		readonly Assembly _assembly;
		string _product;
		string _version;
		string _company;
		string _copyright;

		public AssemblyInfoProvider()
		{
			_assembly = typeof(AssemblyInfoProvider).GetTypeInfo().Assembly;
		}
	
		public string AssemblyProduct
		{
			get
			{
				if (_product != null)
					return _product;
				_product = GetAttribute<AssemblyProductAttribute>().Product;
				return _product;
			}
		}

		public string AssemblyVersion
		{
			get
			{
				if (_version != null)
					return _version;
				_version = GetAttribute<AssemblyVersionAttribute>().Version;
				return _version;

			}
		}

		public string AssemblyCompany
		{
			get
			{
				if (_company != null)
					return _company;
				_company = GetAttribute<AssemblyCompanyAttribute>().Company;
				return _company;
			}
		}

		public string AssemblyCopyright
		{
			get
			{
				if (_copyright != null)
					return _copyright;
				_copyright = GetAttribute<AssemblyCopyrightAttribute>().Copyright;
				return _copyright;
			}
		}

		T GetAttribute<T>() where T : Attribute
		{
			return (T)(_assembly.GetCustomAttributes(typeof(T))).Single();
		}
	}
}
