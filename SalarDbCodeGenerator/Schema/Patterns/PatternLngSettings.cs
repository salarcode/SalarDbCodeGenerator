using System;
using System.Collections.Generic;
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
	public class PatternLngSettings : SchemaPatternBase<PatternLngSettings>
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

		/// <summary>
		/// Types that doesn't support implitic converts
		/// </summary>
		public List<string> ExplicitCastDataTypes { get; set; }
		public List<string> NullableDataTypes { get; set; }
		
		private string _languageInvalidChars;
		public char LanguageInvalidCharsSuppress { get; set; }
		public string LanguageInvalidChars
		{
			get { return _languageInvalidChars; }
			set
			{
				_languageInvalidChars = value;
				if (_languageInvalidChars != null)
					LanguageInvalidCharsArray = _languageInvalidChars.ToCharArray();
				else
					LanguageInvalidCharsArray = new char[0];
			}
		}
		[XmlIgnore]
		public char[] LanguageInvalidCharsArray { get; private set; }

		public PatternLngSettings()
		{
			ExplicitCastDataTypes = new List<string>();
			NullableDataTypes = new List<string>();
			LanguageKeywords = new List<string>();
			//NullableDataTypes.Add("String");
			//NullableDataTypes.Add("Object");
			//ExplicitCastDataTypes.Add("Object");
			//ExplicitCastDataTypes.Add("Guid");

			LanguageKeywordsSuppress = "{0}_{1}";
			LanguageInvalidChars = ", ./<>?;'\\:\"|[]{}`-=~!@#$%^&*()+";
			LanguageInvalidCharsSuppress = '_';

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
	}
}
