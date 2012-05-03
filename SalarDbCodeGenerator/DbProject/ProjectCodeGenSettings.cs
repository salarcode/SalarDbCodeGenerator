using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;
using System;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Programmer: Salar Khalilzadeh <salar2k@gmail.com>
// Copytight(c) 2012, All rights reserved
// 2009-9-30
// ====================================
namespace SalarSoft.DbCodeGenerator.DbProject
{
	[Serializable]
	public class ProjectCodeGenSettings
	{
		#region properties
		public string DefaultNamespace { get; set; }
		public bool GenerateCustomizablePartial { get; set; }
		public string CodeGenPatternFile { get; set; }
		public StringCollection SelectedPatterns { get; set; }

		/// <summary>
		/// Determines if the engine should generate columns description, if is supported.
		/// </summary>
		public bool GenerateColumnsDescription { get; set; }

		/// <summary>
		/// Determines if the engine should generate tables foreign keys, if is supported.
		/// </summary>
		public bool GenerateTablesForeignKeys { get; set; }

		/// <summary>
		/// Determines if the engine should generate tables contraints keys, if supported.
		/// </summary>
		public bool GenerateConstraintKeys { get; set; }
		#endregion

		#region public methods
		public ProjectCodeGenSettings()
		{
			DefaultNamespace = "SalarSoft.CodeGen";
			GenerateCustomizablePartial = true;
			GenerateColumnsDescription = true;
			GenerateTablesForeignKeys = true;
			GenerateConstraintKeys = true;
			CodeGenPatternFile = "";
			SelectedPatterns = new StringCollection();
		}
		public static ProjectCodeGenSettings LoadDefaultSettings()
		{
			ProjectCodeGenSettings settings = new ProjectCodeGenSettings();

			settings.CodeGenPatternFile = AppConfig.AppVarApplicationPath + @"\DbProject\3Tier C#\3TierPattern.dbpat";
			settings.DefaultNamespace = "SalarSoft.CodeGen";

			return settings;
		}
		#endregion

	}
}
