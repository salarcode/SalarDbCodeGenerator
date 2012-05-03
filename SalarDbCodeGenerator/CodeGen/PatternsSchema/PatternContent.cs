using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Programmer: Salar Khalilzadeh <salar2k@gmail.com>
// Copytight(c) 2012, All rights reserved
// 2009-9-30
// ====================================
namespace SalarSoft.DbCodeGenerator.CodeGen.PatternsSchema
{
	/// <summary>
	/// The pattern content in pattern file
	/// </summary>
	public class PatternContent
	{
		private List<PatternContent> _patternContents;

		/// <summary>
		/// Pattern name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Base content used for all items
		/// </summary>
		public string BaseContent { get; set; }

		/// <summary>
		/// Where to apply
		/// </summary>
		public PatternContentAppliesTo AppliesTo { get; set; }

		/// <summary>
		/// Where to apply
		/// </summary>
		public PatternContentKeyMode ContentKeyMode { get; set; }

		/// <summary>
		/// The seperator between items
		/// </summary>
		public string ItemsSeperator { get; set; }

		/// <summary>
		/// Pertial contents
		/// </summary>
		public List<PatternReplacement> Replacements { get; set; }

		/// <summary>
		/// List if internal pattern contents
		/// </summary>
		public List<PatternContent> PatternContents
		{
			get
			{
				return _patternContents ?? (_patternContents = new List<PatternContent>());
			}
		}

		public PatternContent()
		{
			Replacements = new List<PatternReplacement>();
		}

		public PatternReplacement GetReplacement(string mode)
		{
			foreach (var repItem in Replacements)
			{
				if (repItem.KeyMode == mode)
					return repItem;
			}
			return null;
		}

		/// <summary>
		/// Reads pattern content from xml file
		/// </summary>
		internal static PatternContent ReadFromXElement(XElement element)
		{
			PatternContent result = new PatternContent
			{
				Name = element.Attribute("Name").Value
			};

			// BaseContent
			var baseContent = element.Element("BaseContent");
			result.BaseContent = baseContent == null ? "" : baseContent.Value;

			// AppliesTo
			string appliesTo = element.Attribute("AppliesTo").Value;
			result.AppliesTo = (PatternContentAppliesTo)Enum.Parse(typeof(PatternContentAppliesTo), appliesTo, true);

			// ContentKeyMode
			string contentKeyMode = element.Attribute("ContentKeyMode").Value;
			result.ContentKeyMode = (PatternContentKeyMode)Enum.Parse(typeof(PatternContentKeyMode), contentKeyMode, true);

			XElement xSeperator = element.Element("ItemsSeperator");
			if (xSeperator != null)
				result.ItemsSeperator = xSeperator.Value;

			// Reads the relacement tags
			foreach (var e in element.Elements("Content"))
			{
				result.Replacements.Add(PatternReplacement.ReadFromXElement(e));
			}

			// Looking for embeded pattern contents!
			foreach (var e in element.Elements("PatternContent"))
			{
				result.PatternContents.Add(PatternContent.ReadFromXElement(e));
			}

			// and the result
			return result;
		}
	}
}
