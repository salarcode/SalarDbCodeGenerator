using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Programmer: Salar Khalilzadeh <salar2k@gmail.com>
// Copytight(c) 2012, All rights reserved
// 2010-06-09
// ====================================
namespace SalarSoft.DbCodeGenerator.CodeGen.PatternsSchema
{
	/// <summary>
	/// Patterns project
	/// </summary>
	[Serializable]
	public class PatternProject
	{
		/// <summary>
		/// Pattern name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Pattern description
		/// </summary>
		public string Description { get; set; }
		public string Author { get; set; }
		public string LastUpdate { get; set; }
		public string Language { get; set; }
		public string FileExtension { get; set; }
		public string SupportedDatabases { get; set; }
		/// <summary>
		/// Generate referenced columns seperated
		/// </summary>
		public bool SeperateRefOtherColumns { get; set; }
		public PatternLngSettings LanguageSettings { get; set; }
		public List<PatternsListType> PatternFiles { get; set; }

		/// <summary>
		/// Pattern project file name
		/// </summary>
		[NonSerialized]
		public string PatternFileName;

		public PatternProject()
		{
			PatternFiles = new List<PatternsListType>();
			LanguageSettings = new PatternLngSettings();
		}

		public bool Equals(PatternProject patProject)
		{
			return patProject.Name == Name &&
				patProject.Description == Description &&
				patProject.Author == Author &&
				patProject.Language == Language &&
				patProject.LastUpdate == LastUpdate &&
				patProject.FileExtension == FileExtension;
		}

		public static PatternProject ReadFromFile(string projectFilename)
		{
			PatternProject project;
			XmlSerializer loader = new XmlSerializer(typeof(PatternProject));
			using (StreamReader reader = new StreamReader(projectFilename))
				project = (PatternProject)loader.Deserialize(reader);

			project.PatternFileName = projectFilename;
			return project;
		}

		public static void SaveToFile(PatternProject patternProject, string projectFilename)
		{
			XmlSerializer saver = new XmlSerializer(typeof(PatternProject));
			using (StreamWriter writer = new StreamWriter(projectFilename))
				saver.Serialize(writer, patternProject);
		}
	}

	public struct PatternsListType
	{
		public string Path { get; set; }
	}
}
