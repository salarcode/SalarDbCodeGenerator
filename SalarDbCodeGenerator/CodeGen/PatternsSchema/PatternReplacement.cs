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
	/// The "Content" key with in "PatternContent" tag
	/// </summary>
	public class PatternReplacement
	{
		/// <summary>
		/// Replacement mode
		/// </summary>
		public string KeyMode { get; set; }

		/// <summary>
		/// Replacement Content
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// Read data from xml
		/// </summary>
		internal static PatternReplacement ReadFromXElement(XElement element)
		{
			PatternReplacement result = new PatternReplacement()
			{
				KeyMode = element.Attribute("KeyMode").Value,
				Content = element.Value
			};
			return result;
		}
	}
}
