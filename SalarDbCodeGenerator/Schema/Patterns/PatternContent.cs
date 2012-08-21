using System;
using System.Collections.Generic;
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
	/// <summary>
	/// The pattern content in pattern file
	/// </summary>
	public class PatternContent : SchemaPatternBase<PatternContent>
	{
		private List<PatternContent> _conditionContents;

		/// <summary>
		/// Pattern name
		/// </summary>
		[XmlAttribute]
		public string Name { get; set; }

		/// <summary>
		/// Base content used for all items
		/// </summary>
		[XmlElement]
		public string BaseContent { get; set; }

		/// <summary>
		/// Where to apply
		/// </summary>
		[XmlAttribute]
		public PatternConditionKeyMode ConditionKeyMode { get; set; }

		/// <summary>
		/// The seperator between items
		/// </summary>
		[XmlElement]
		public string ItemsSeperator { get; set; }

		/// <summary>
		/// Pertial contents
		/// </summary>
		[XmlElement("Condition")]
		public List<ConditionItem> Conditions { get; set; }

		/// <summary>
		/// List if internal pattern contents
		/// </summary>
		public List<PatternContent> ConditionContents
		{
			get
			{
				return _conditionContents ?? (_conditionContents = new List<PatternContent>());
			}
		}

		public PatternContent()
		{
			Conditions = new List<ConditionItem>();
		}

		public ConditionItem GetReplacement(string key)
		{
			foreach (var repItem in Conditions)
			{
				if (repItem.Key == key)
					return repItem;
			}
			return null;
		}
		public ConditionItem GetFirst()
		{
			if (Conditions.Count > 0)
				return Conditions[0];
			return null;
		}
	}
}
