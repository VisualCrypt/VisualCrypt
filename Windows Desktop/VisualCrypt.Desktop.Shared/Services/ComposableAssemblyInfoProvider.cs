﻿using System.ComponentModel.Composition;
using VisualCrypt.Applications.Apps.Services;

namespace VisualCrypt.Desktop.Shared.Services
{
	[Export(typeof(IAssemblyInfoProvider))]
	public class ComposableAssemblyInfoProvider : AssemblyInfoProvider
	{
	}
}
