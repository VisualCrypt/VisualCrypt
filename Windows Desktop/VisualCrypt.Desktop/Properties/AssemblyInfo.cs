﻿// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle("VisualCrypt")]
[assembly: AssemblyDescription("This Release: Fix a bug that led to garbled data with compiler optimizations enabled.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("VisualCrypt AG")]
[assembly: AssemblyProduct("VisualCrypt Pro")]
[assembly: AssemblyCopyright("Copyright © 2014 - 2015 VisualCrypt AG")]
[assembly: AssemblyTrademark("VisualCrypt")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible(false)]

// In order to begin building localizable applications, set 
// <UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
// inside a <PropertyGroup>.  For example, if you are using US english
// in your source files, set the <UICulture> to en-US.  Then uncomment
// the NeutralResourceLanguage attribute below.  Update the "en-US" in
// the line below to match the UICulture setting in the project file.
// [assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]

[assembly: ThemeInfo(
    // where theme specific resource dictionaries are located
    // (used if a resource is not found in the page, 
    // or application resource dictionaries)
    ResourceDictionaryLocation.None,

    // where the generic resource dictionary is located
    // (used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly)]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]

[assembly: AssemblyVersion("2.0.6.*")]
[assembly: AssemblyFileVersion("2.0.6.0")]
[assembly: CLSCompliant(false)]