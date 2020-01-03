using System.Reflection;

#if __MACOS__ || __TVOS__ || __WATCHOS__ || __IOS__

using Foundation;
[assembly: LinkerSafe]
#endif

// Information about this assembly is defined by the following attributes. 
// Change them to the values specific to your project.

[assembly: AssemblyTitle ("Buerkert.KeraLua (.NET Framework 4.5)")]

[assembly: AssemblyDescription ("Binding library for native Lua")]
[assembly: AssemblyCompany ("Buerkert")]
[assembly: AssemblyProduct ("Buerkert.KeraLua")]
[assembly: AssemblyCopyright ("Copyright © Buerkert 2020")]
[assembly: AssemblyCulture ("")]

// The assembly version has the format "{Major}.{Minor}.{Build}.{Revision}".
// The form "{Major}.{Minor}.*" will automatically update the build and revision,
// and "{Major}.{Minor}.{Build}.*" will update just the revision.

[assembly: AssemblyVersion("1.0.1.0")]
[assembly: AssemblyFileVersion("1.0.1.0")]

