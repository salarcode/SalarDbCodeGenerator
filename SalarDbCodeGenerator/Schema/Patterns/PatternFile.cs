using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
 
// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Salar Khalilzadeh <salar2k@gmail.com>
// © 2012, All rights reserved
// 2012/07/06
// ====================================
namespace SalarDbCodeGenerator.Schema.Patterns
{
	[Serializable]
	public class PatternFile : SchemaPatternBase<PatternFile>
	{
		public class PatternFileOptions
		{
			/// <summary>
			/// Pattern group
			/// </summary>
			[XmlAttribute]
			public string Group { get; set; }

			/// <summary>
			/// Overwrite existing file
			/// </summary>
			[XmlAttribute]
			public bool Overwrite { get; set; }

			/// <summary>
			/// Generated file path
			/// </summary>
			[XmlAttribute]
			public string FilePath { get; set; }

			/// <summary>
			/// Pattern language
			/// </summary>
			[XmlAttribute]
			public string Language { get; set; }

			/// <summary>
			/// Where this pattern applies
			/// </summary>
			[XmlAttribute]
			public PatternFileAppliesTo AppliesTo { get; set; }
		}

		#region properties
		/// <summary>
		/// Pattern name
		/// </summary>
		[XmlElement]
		public string Name { get; set; }

		/// <summary>
		/// Pattern description
		/// </summary>
		[XmlElement]
		public string Description { get; set; }

		/// <summary>
		/// Pattern description
		/// </summary>
		[XmlElement]
		public PatternFileOptions Options { get; set; }

		/// <summary>
		/// All tables/views should apply to one file?
		/// </summary>
		[XmlIgnore]
		public bool IsAllToOne
		{
			get
			{
				if (Options.AppliesTo == PatternFileAppliesTo.TablesAll ||
					Options.AppliesTo == PatternFileAppliesTo.TablesAndViewsAll ||
					Options.AppliesTo == PatternFileAppliesTo.ViewsAll)
				{
					return true;
				}
				return false;
			}
		}

		/// <summary>
		/// Pattern content
		/// </summary>
		public string BaseContent { get; set; }

		/// <summary>
		/// Pertial contents
		/// </summary>
		[XmlElement("PatternContent")]
		public List<PatternContent> PatternContents { get; set; }
		#endregion

		#region public methods
		public PatternFile()
		{
			PatternContents = new List<PatternContent>();
			Options = new PatternFileOptions();
			//var o = new PatternContent();
			//o.ConditionContents.Add(new PatternContent());
			//o.ConditionContents.Add(new PatternContent());
			//var cond = new ConditionItem();
			//cond.ContentText = "qqqq";
			//cond.Key = "aaa";
			//o.Condition.Add(cond);
			//o.Condition.Add(cond);
			//PatternContents.Add(o);
			//PatternContents.Add(o);
		}

		public PatternFile Clone()
		{
			return (PatternFile)this.MemberwiseClone();
		}

		public string ToSummaryString()
		{
			var sb = new StringBuilder();

			sb.AppendFormat("Name= {0}\n", Name);
			sb.AppendFormat("AppliesTo= {0}\n", AppliesToString(Options.AppliesTo));
			sb.AppendFormat("Overwrite= {0}\n", Options.Overwrite);
			sb.AppendFormat("Language= {0}", Options.Language);

			return sb.ToString();
		}
		#endregion

		#region private methods
		private string AppliesToString(PatternFileAppliesTo appliesTo)
		{
			switch (appliesTo)
			{
				case PatternFileAppliesTo.ProjectFile:
					return "Once per project";

				case PatternFileAppliesTo.General:
					return "Once per project";

				case PatternFileAppliesTo.TablesAndViewsEach:
					return "Each table/view one file";

				case PatternFileAppliesTo.TablesAndViewsAll:
					return "One file for all tables/views";

				case PatternFileAppliesTo.TablesEach:
					return "Each table one file";

				case PatternFileAppliesTo.TablesAll:
					return "One file for all tables";

				case PatternFileAppliesTo.ViewsEach:
					return "Each view one file";

				case PatternFileAppliesTo.ViewsAll:
					return "One file for all views";

			}
			return "[Undefined]";
		}
		#endregion

	}


}
