using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SalarDbCodeGenerator.DbProject;
using SalarDbCodeGenerator.Schema.Database;
using SalarDbCodeGenerator.Schema.DbSchemaReaders;
using SalarDbCodeGenerator.Schema.Patterns;

namespace SalarDbCodeGenerator.Tests
{
	[TestClass]
	public class GenerationTestBase
	{
		public ProjectDefinaton Project { get; set; }
		public PatternProject Pattern { get; set; }
		public DbDatabase Database { get; set; }

		[TestInitialize]
		public void InitializeSchemaProject()
		{
			Project = new ProjectDefinaton();
			Pattern = new PatternProject();
			Database = new DbDatabase();

			Project.GenerationPath = Path.Combine(Path.GetTempPath(), @"SalarDbCodeGenerator.Tests\" + DateTime.Now.Ticks);
			Project.ProjectName = "SalarDbCodeGenerator.Tests";

			Pattern.LanguageSettings.KeywordsCaseSensitive = true;
			Pattern.LanguageSettings.LanguageInvalidChars = ", ./<>?;'\\:\"|[]{}`-=~!@#$%^&*()+";
			Pattern.LanguageSettings.LanguageInvalidCharsSuppress = '_';
			Pattern.LanguageSettings.LanguageKeywords.AddRange(new string[]
			                                                   	{
			                                                   		"readonly", "base", "break", "byte", "case", "catch", "checked",
			                                                   		"class", "const", "continue", "default", "delegate", "do",
			                                                   		"value", "else", "enum", "event", "explicit", "extern", "false",
			                                                   		"finally", "fixed", "for", "foreach", "goto", "if", "implicit",
			                                                   		"in", "interface", "internal", "is", "namespace", "new", "null",
			                                                   		"object", "operator", "out", "override", "private", "protected",
			                                                   		"public", "readonly", "ref", "return", "sealed", "sizeof",
			                                                   		"static", "struct", "switch", "this", "throw", "true", "try",
			                                                   		"typeof", "typeof", "unchecked", "unsafe", "using", "virtual",
			                                                   		"while"
			                                                   	});
			Pattern.LanguageSettings.NullableDataTypes.AddRange(new[] { "String", "Object" });
			Pattern.LanguageSettings.ExplicitCastDataTypes.AddRange(new[] { "Object", "Guid", "DateTimeOffset", "TimeSpan" });
			Pattern.LanguageSettings.VoidDataType = "void";
			Pattern.LanguageSettings.TextFieldIdenticator = "text";
			Pattern.LanguageSettings.ArrayIdenticator = "[]";
			Pattern.LanguageSettings.DbDecimalName = "decimal";
			Pattern.LanguageSettings.DbNumericName = "numeric";
			Pattern.LanguageSettings.DbDecimalType = "decimal([:Precision:],[:Scale:])";
			Pattern.LanguageSettings.DbNumericType = "numeric([:Precision:],[:Scale:])";
			Pattern.LanguageSettings.LanguageKeywordsSuppress = "{0}_{1}";


			Database.DatabaseName = "SalarDbCodeGeneratorTests";
			Database.Provider = DatabaseProvider.SQLServer;
		}

	}
}
