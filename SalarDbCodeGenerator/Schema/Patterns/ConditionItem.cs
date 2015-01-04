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
// © 2014, All rights reserved
 // ====================================
namespace SalarDbCodeGenerator.Schema.Patterns
{
	/// <summary>
	/// The "Content" key with in "PatternContent" tag
	/// </summary>
	public class ConditionItem
	{
		/// <summary>
		/// Condition on if this item should be applied or not
		/// </summary>
		[Serializable]
		public enum ApplyConditionMode
		{
			NotSet,
			Equals,
			NotEquals
		}

		public ConditionItem()
		{
			Key = string.Empty;
			ContentText = string.Empty;
		}

		/// <summary>
		/// Replacement mode
		/// </summary>
		[XmlAttribute]
		public string Key { get; set; }

		/// <summary>
		/// Replacement Content
		/// </summary>
		[XmlText]
		public string ContentText { get; set; }

		public override string ToString()
		{
			return Key;
		}


		/// <summary>
		/// Condition on if this item should be applied or not
		/// </summary>
		[XmlAttribute]
		public ApplyConditionMode ApplyCondition { get; set; }
	}
	 
}
