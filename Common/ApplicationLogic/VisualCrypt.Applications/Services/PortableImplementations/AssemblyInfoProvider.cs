using System;
using System.Linq;
using System.Reflection;
using VisualCrypt.Applications.Services.Interfaces;

namespace VisualCrypt.Applications.Services.PortableImplementations
{
	public class AssemblyInfoProvider : IAssemblyInfoProvider
	{
		Assembly _assembly;
		string _product;
		string _version;
		string _company;
		string _copyright;

	
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

        public string AssemblyDescription
        {
            get
            {
                if (_product != null)
                    return _product;
                _product = GetAttribute<AssemblyDescriptionAttribute>().Description;
                return _product;
            }
        }

        public string AssemblyVersion
        {
            get
            {
                if (_version != null)
                    return _version;
                var ver = Assembly.GetName().Version;

                _version = string.Format("{0}.{1}.{2}", ver.Major, ver.Minor, ver.Build);
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

        public Assembly Assembly
        {
            get
            {
                if(_assembly == null)
                    _assembly = typeof(AssemblyInfoProvider).GetTypeInfo().Assembly;
                return _assembly;
            }
            set
            {
                _assembly = value;
            }
        }

        T GetAttribute<T>() where T : Attribute
		{
			return (T)(Assembly.GetCustomAttributes(typeof(T))).Single();
		}
	}
}
