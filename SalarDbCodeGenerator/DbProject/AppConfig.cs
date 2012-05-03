using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Programmer: Salar Khalilzadeh <salar2k@gmail.com>
// Copytight(c) 2012, All rights reserved
// 2009-9-30
// ====================================
namespace SalarSoft.DbCodeGenerator.DbProject
{
	public static class AppConfig
	{
		public const string AppVersionMain = "1.0";
		public const string AppVersionFull = "1.0.2012.0503";
		public const string AppGeneratorSign = "Salar dotNET DbCodeGenerator";
		public readonly static string NewLine = Environment.NewLine;

		public const string AppVarApplicationPath = "%APP%";
		public const string AppVarProjectPath = "%PROJECT%";

		public const string PatternProjectExtension = ".dbpat";

		public static string DefaultOutputPath
		{
			get
			{
				return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CodeGenProjects");
			}
		}


		public static string DefaultPatternFileName
		{
			get
			{
				return AppConfig.AppVarApplicationPath +
				       @"\Patterns\3Tier C#\3Tier C# Pattern.dbpat";
				//return Path.Combine(Path.GetDirectoryName(Application.ExecutablePath),
				//    @"\Patterns\3Tier C#\3Tier C# Pattern.dbpat");
			}
		}
		public static string RecentProjectsConfig
		{
			get
			{
				return Path.Combine(Path.GetDirectoryName(Application.ExecutablePath),
					@"RecentProjects.dat");
			}
		}
		public static string PatternProjectsDirectory
		{
			get
			{
				return Path.Combine(Path.GetDirectoryName(Application.ExecutablePath),
					"Patterns");
			}
		}


		public static bool IsFilesAssociated()
		{
			const string CodeGenProjectFiles = ".dbgen";
			const string CodeGenProjectFiles_EntryName = "Salar.CodeGen.DBProject";

			string entryValue;
			try
			{
				entryValue = Registry.GetValue(@"HKEY_CLASSES_ROOT\" + CodeGenProjectFiles, "", "").ToString();
			}
			catch
			{
				entryValue = null;
			}

			if (entryValue != CodeGenProjectFiles_EntryName)
				return false;
			else
				return true;
		}


		public static void AssociateApplicationFiles()
		{
			const string CodeGenProjectFiles = ".dbgen";
			const string CodeGenProjectFiles_EntryName = "Salar.CodeGen.DBProject";
			const string PatternProjectFiles = ".dbpat";
			const string PatternContentFiles = ".patml";
			int iconIndex;
			try
			{
				// Program project files
				iconIndex = 1;
				Registry.SetValue(@"HKEY_CLASSES_ROOT\" + CodeGenProjectFiles, "", CodeGenProjectFiles_EntryName, RegistryValueKind.String);
				Registry.SetValue(@"HKEY_CLASSES_ROOT\Salar.CodeGen.DBProject", "", "SalarCodeGen DbProject Project", RegistryValueKind.String);
				Registry.SetValue(@"HKEY_CLASSES_ROOT\Salar.CodeGen.DBProject\DefaultIcon", "", string.Format("{0},{1}", Application.ExecutablePath, iconIndex), RegistryValueKind.String);
				Registry.SetValue(@"HKEY_CLASSES_ROOT\Salar.CodeGen.DBProject\shell\Open\command", "", string.Format("\"{0}\" \"%1\"", Application.ExecutablePath), RegistryValueKind.String);

				// Program pattern project files
				iconIndex = 2;
				Registry.SetValue(@"HKEY_CLASSES_ROOT\" + PatternProjectFiles, "", "Salar.CodeGen.DBPattern", RegistryValueKind.String);
				Registry.SetValue(@"HKEY_CLASSES_ROOT\Salar.CodeGen.DBPattern", "", "SalarCodeGen DbPattern Project", RegistryValueKind.String);
				Registry.SetValue(@"HKEY_CLASSES_ROOT\Salar.CodeGen.DBPattern\DefaultIcon", "", string.Format("{0},{1}", Application.ExecutablePath, iconIndex), RegistryValueKind.String);

				// Program pattern files
				iconIndex = 3;
				Registry.SetValue(@"HKEY_CLASSES_ROOT\" + PatternContentFiles, "", "Salar.CodeGen.DBPatternML", RegistryValueKind.String);
				Registry.SetValue(@"HKEY_CLASSES_ROOT\Salar.CodeGen.DBPatternML", "", "SalarCodeGen DbPattern File", RegistryValueKind.String);
				Registry.SetValue(@"HKEY_CLASSES_ROOT\Salar.CodeGen.DBPatternML\DefaultIcon", "", string.Format("{0},{1}", Application.ExecutablePath, iconIndex), RegistryValueKind.String);
			}
			catch (Exception ex)
			{
				PleaseWait.Abort();
				throw new Exception("Failed to associate application files. \nTo associate application files run the application with administrator rights.", ex);
			}
		}
	}
}
