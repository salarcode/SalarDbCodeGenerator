using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using SalarSoft.DbCodeGenerator.CodeGen.PatternsSchema;
using SalarSoft.DbCodeGenerator.CodeGen.SchemaEngines;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Programmer: Salar Khalilzadeh <salar2k@gmail.com>
// Copytight(c) 2012, All rights reserved
// 2011-10-10
// ====================================
namespace SalarSoft.DbCodeGenerator.DbProject
{
	[Serializable]
	public class ProjectDefinaton : ICloneable
	{
		#region local variables
		#endregion

		#region field variables
		#endregion

		#region properties
		public float GeneratorVersion { get; set; }
		public string ProjectName { get; set; }
		public DateTime LastGeneration { get; set; }
		public string GenerationPath { get; set; }
		public ProjectCodeGenSettings CodeGenSettings { get; set; }
		public ProjectDbSettions DbSettions { get; set; }
		public ProjectRenaming RenamingOptions { get; set; }

		[NonSerialized]
		public string ProjectFileName;
		#endregion

		#region public methods
		public ProjectDefinaton()
		{
			ProjectName = "Project1";
			LastGeneration = DateTime.MinValue;
			GenerationPath = "";
			DbSettions = new ProjectDbSettions();
			CodeGenSettings = new ProjectCodeGenSettings();
			RenamingOptions = new ProjectRenaming();
			GeneratorVersion = 1.0f;
		}

		public object Clone()
		{
			return this.MemberwiseClone();
		}

		public static void SaveToFile(ProjectDefinaton definaton, string fileName)
		{
			var genPath = definaton.GenerationPath;
			var patPath = definaton.CodeGenSettings.CodeGenPatternFile;
			try
			{
				// make relative
				definaton.CodeGenSettings.CodeGenPatternFile =
					Common.AppVarPathMakeRelative(definaton.CodeGenSettings.CodeGenPatternFile);
				definaton.GenerationPath = Common.ProjectPathMakeRelative(definaton.GenerationPath, fileName);

				// save
				XmlSerializer saver = new XmlSerializer(typeof(ProjectDefinaton));
				using (StreamWriter writer = new StreamWriter(fileName))
					saver.Serialize(writer, definaton);

			}
			finally
			{
				// restore
				definaton.CodeGenSettings.CodeGenPatternFile = patPath;
				definaton.GenerationPath = genPath;
			}
		}
		public static ProjectDefinaton LoadFromFile(string fileName)
		{
			ProjectDefinaton definaton;
			XmlSerializer loader = new XmlSerializer(typeof(ProjectDefinaton));
			using (StreamReader reader = new StreamReader(fileName))
				definaton = (ProjectDefinaton)loader.Deserialize(reader);

			definaton.ProjectFileName = fileName;
			return definaton;
		}
		public static ProjectDefinaton LoadDefaultProject()
		{
			ProjectDefinaton result = new ProjectDefinaton();

			result.ProjectName = "Project1";
			result.LastGeneration = DateTime.MinValue;
			result.GenerationPath = AppConfig.DefaultOutputPath;
			result.ProjectFileName = "";

			result.DbSettions = ProjectDbSettions.LoadDefaultSettings();
			result.CodeGenSettings = ProjectCodeGenSettings.LoadDefaultSettings();
			result.CodeGenSettings.CodeGenPatternFile = AppConfig.DefaultPatternFileName;

			return result;
		}

		#endregion
	}
}
