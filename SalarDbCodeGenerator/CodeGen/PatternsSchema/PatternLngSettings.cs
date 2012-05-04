using System;
using System.Collections.Generic;
using System.Xml.Linq;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Programmer: Salar Khalilzadeh <salar2k@gmail.com>
// Copytight(c) 2012, All rights reserved
// 2009-9-30
// ====================================
namespace SalarDbCodeGenerator.CodeGen.PatternsSchema
{
	[Serializable]
	public class PatternLngSettings
	{
		public string VoidDataType { get; set; }
		//public StringCollection NativeNullableTypes { get; set; }

		public string TextFieldIdenticator { get; set; }
		public string ArrayIdenticator { get; set; }
		public string DbDecimalName { get; set; }
		public string DbNumericName { get; set; }
		public string DbDecimalType { get; set; }
		public string DbNumericType { get; set; }
		public string LanguageKeywordsSuppress { get; set; }
		public bool KeywordsCaseSensitive { get; set; }
		public List<string> LanguageKeywords { get; set; }

		public PatternLngSettings()
		{
			LanguageKeywords = new List<string>();
			LanguageKeywordsSuppress = "{0}_{1}";
			VoidDataType = "void";
			ArrayIdenticator = "[]"; //c#
			TextFieldIdenticator = "text";
			DbDecimalName = "decimal";
			DbNumericName = "numeric";
			DbDecimalType = "decimal({Precision})";
			DbNumericType = "numeric({Precision},{:Scale:})";

			//NativeNullableTypes = new StringCollection();
			//NativeNullableTypes.Add("string");
			//NativeNullableTypes.Add("object");
		}

		public void ReadFromXml(XElement element)
		{
			this.VoidDataType = element.Element("VoidDataType").Value;
			this.TextFieldIdenticator = element.Element("TextFieldIdenticator").Value;
			this.ArrayIdenticator = element.Element("ArrayIdenticator").Value;
			this.DbDecimalName = element.Element("DbDecimalName").Value;
			this.DbNumericName = element.Element("DbNumericName").Value;
			this.DbDecimalType = element.Element("DbDecimalType").Value;
			this.DbNumericType = element.Element("DbNumericType").Value;
			this.LanguageKeywordsSuppress = element.Element("LanguageKeywordsSuppress").Value;
			this.KeywordsCaseSensitive = Convert.ToBoolean(element.Element("KeywordsCaseSensitive").Value);
		}
	}
}
