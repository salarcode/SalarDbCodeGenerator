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
// Salar Khalilzadeh <salar2k@gmail.com>
// © 2012, All rights reserved
// 2012/07/06
// ====================================
namespace SalarDbCodeGenerator.Schema.Patterns
{
	/// <summary>
	/// Patterns project
	/// </summary>
	[Serializable]
	public class PatternProject : SchemaPatternBase<PatternProject>
	{
		public struct PatternFile
		{
			[XmlText]
			public string Path { get; set; }
			[XmlAttribute]
			public PatternsListItemAction Action { get; set; }
			[XmlAttribute]
			public string ActionCopyPath { get; set; }
		}
		
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
		public bool SeperateReferenceColumns { get; set; }
		public PatternLngSettings LanguageSettings { get; set; }

		[XmlElement("PatternFile")]
		public List<PatternFile> PatternFiles { get; set; }

		/// <summary>
		/// Pattern project file name
		/// </summary>
		[NonSerialized]
		public string PatternFileName;

		public PatternProject()
		{
			PatternFiles = new List<PatternFile>();
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

		public static new PatternProject ReadFromFile(string projectFilename)
		{
			PatternProject project;
			var loader = new XmlSerializer(typeof(PatternProject));
			using (var reader = new StreamReader(projectFilename))
				project = (PatternProject)loader.Deserialize(reader);
		
			project.PatternFileName = projectFilename;
			return project;
		}

 	}


	public enum PatternsListItemAction
	{
		Generate,
		Copy
	}

}
