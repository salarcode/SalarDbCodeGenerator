using SalarDbCodeGenerator.GeneratorEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SalarDbCodeGenerator.DbProject;
using SalarDbCodeGenerator.Schema.Patterns;
using SalarDbCodeGenerator.Schema.Database;

namespace SalarDbCodeGenerator.Tests
{


	/// <summary>
	///This is a test class for SchemaAnalyzerTest and is intended
	///to contain all SchemaAnalyzerTest Unit Tests
	///</summary>
	[TestClass()]
	public class SchemaAnalyzerTest : GenerationTestBase
	{


		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion



		/// <summary>
		///A test for Determine_DataTypeNullable
		///</summary>
		[TestMethod()]
		[DeploymentItem("SalarDbCodeGenerator.exe")]
		public void Determine_DataTypeNullableTest()
		{
			var analyzer = new SchemaAnalyzer_Accessor(Project, Pattern, Database);
			var dbColumn = new DbColumn("testField");

			dbColumn.DataTypeDotNet = "String";
			Assert.IsTrue(analyzer.Determine_DataTypeNullable(dbColumn));

			dbColumn.DataTypeDotNet = "Object";
			Assert.IsTrue(analyzer.Determine_DataTypeNullable(dbColumn));

			dbColumn.DataTypeDotNet = "Object[]";
			Assert.IsTrue(analyzer.Determine_DataTypeNullable(dbColumn));

			dbColumn.DataTypeDotNet = "Int32";
			Assert.IsFalse(analyzer.Determine_DataTypeNullable(dbColumn));

			dbColumn.DataTypeDotNet = "System.Guid";
			Assert.IsFalse(analyzer.Determine_DataTypeNullable(dbColumn));

			dbColumn.DataTypeDotNet = "Microsoft.SqlServer.Types.FooType";
			Assert.IsFalse(analyzer.Determine_DataTypeNullable(dbColumn));
		}

		/// <summary>
		///A test for Determine_ExplicitCastDataType
		///</summary>
		[TestMethod()]
		[DeploymentItem("SalarDbCodeGenerator.exe")]
		public void Determine_ExplicitCastDataTypeTest()
		{
			var analyzer = new SchemaAnalyzer_Accessor(Project, Pattern, Database);
			var dbColumn = new DbColumn("testField");

			dbColumn.DataTypeDotNet = "String";
			Assert.IsFalse(analyzer.Determine_ExplicitCastDataType(dbColumn));

			dbColumn.DataTypeDotNet = "Int64";
			Assert.IsFalse(analyzer.Determine_ExplicitCastDataType(dbColumn));

			dbColumn.DataTypeDotNet = "Object";
			Assert.IsTrue(analyzer.Determine_ExplicitCastDataType(dbColumn));

			dbColumn.DataTypeDotNet = "System.Guid";
			Assert.IsTrue(analyzer.Determine_ExplicitCastDataType(dbColumn));

			dbColumn.DataTypeDotNet = "System.Object";
			Assert.IsTrue(analyzer.Determine_ExplicitCastDataType(dbColumn));

			dbColumn.DataTypeDotNet = "Microsoft.SqlServer.Types.FooType";
			Assert.IsTrue(analyzer.Determine_ExplicitCastDataType(dbColumn));

		}

		/// <summary>
		///A test for NaturalizeNames_ForeignTableFieldName
		///</summary>
		[TestMethod()]
		[DeploymentItem("SalarDbCodeGenerator.exe")]
		public void NaturalizeNames_ForeignTableFieldNameTest()
		{
			Pattern.LanguageSettings.KeywordsCaseSensitive = true;
			var analyzer = new SchemaAnalyzer_Accessor(Project, Pattern, Database);
			var table = new DbTable("tbl_Test");
			var foreignKey0 = new DbForeignKey() { ForeignTableName = "tblOthers" };
			var foreignKey1 = new DbForeignKey() { ForeignTableName = "tblTest" };
			var foreignKey2 = new DbForeignKey() { ForeignTableName = "tblTest" };
			var foreignKey3 = new DbForeignKey() { ForeignTableName = "tblTest" };
			var foreignKey4 = new DbForeignKey() { ForeignTableName = "tblTest" };
			var foreignKey5 = new DbForeignKey() { ForeignTableName = "tblTest" };
			var foreignKey6 = new DbForeignKey() { ForeignTableName = "tblTest" };
			table.ForeignKeys.AddRange(new[]
                            	{
									foreignKey0,
									foreignKey1,
                            		foreignKey2,
                            		foreignKey3,
                            		foreignKey4,
                            		foreignKey5,
                            		foreignKey6,
                            	});
			foreach (var key in table.ForeignKeys)
			{
				key.ForeignTableNameInLocalTable = analyzer.NaturalizeNames_ForeignTableFieldName(table, key);
			}
			Assert.AreEqual("tblTest", foreignKey1.ForeignTableNameInLocalTable);

			Assert.AreEqual("tblTest_", foreignKey2.ForeignTableNameInLocalTable);
		}

		/// <summary>
		///A test for NaturalizeNames_FieldName
		///</summary>
		[TestMethod()]
		[DeploymentItem("SalarDbCodeGenerator.exe")]
		public void NaturalizeNames_FieldNameTest()
		{
			Pattern.LanguageSettings.KeywordsCaseSensitive = true;
			var analyzer = new SchemaAnalyzer_Accessor(Project, Pattern, Database);
			var table = new DbTable("tbl_Test");
			var col0 = new DbColumn("Test_Other");
			var col1 = new DbColumn("Test_Col");
			var col2 = new DbColumn("Test_Col");
			var col3 = new DbColumn("Test_Col");
			var col4 = new DbColumn("Test_Col");
			var col5 = new DbColumn("Test_Col");
			table.SchemaColumns.AddRange(new[]
			                             	{
												col0,
												col1,
												col2,
												col3,
												col4,
												col5
			                             	});
			foreach (var col in table.SchemaColumns)
			{
				col.FieldNameSchema = analyzer.NaturalizeNames_FieldName(table, col, col.FieldNameSchema, true);
			}
			Assert.AreEqual("Test_Col", col1.FieldNameSchema);
			Assert.AreEqual("Test_Col_", col2.FieldNameSchema);
			Assert.AreEqual("Test_Col_3", col5.FieldNameSchema);
		}

		[TestMethod()]
		[DeploymentItem("SalarDbCodeGenerator.exe")]
		public void NaturalizeNames_FieldNameTest_SameAsParent()
		{
			Pattern.LanguageSettings.KeywordsCaseSensitive = true;
			var analyzer = new SchemaAnalyzer_Accessor(Project, Pattern, Database);
			var table = new DbTable("SameName");
 			var col1 = new DbColumn("SameName");
			var col2 = new DbColumn("SameName");
			var col3 = new DbColumn("SameName");
			table.SchemaColumns.AddRange(new[]
			                             	{
 												col1,
												col2,
												col3,
			                             	});
			foreach (var col in table.SchemaColumns)
			{
				col.FieldNameSchema = analyzer.NaturalizeNames_FieldName(table, col, col.FieldNameSchema, true);
			}
			Assert.AreEqual("SameName_", col1.FieldNameSchema);
			Assert.AreEqual("SameName_1", col2.FieldNameSchema);
		}


		/// <summary>
		///A test for NaturalizeNames_TableSchemaName_Duplicate
		///</summary>
		[TestMethod()]
		[DeploymentItem("SalarDbCodeGenerator.exe")]
		public void NaturalizeNames_TableSchemaName_DuplicateTest_ForTable()
		{
			Pattern.LanguageSettings.KeywordsCaseSensitive = true;
			var analyzer = new SchemaAnalyzer_Accessor(Project, Pattern, Database);
			var table0 = new DbTable("tbl_Other");
			var table1 = new DbTable("tbl_Test");
			var table2 = new DbTable("tbl_Test");
			var table3 = new DbTable("tbl_Test");
			var table4 = new DbTable("tbl_Test");
			var table5 = new DbTable("tbl_Test");
			var table6 = new DbTable("tbl_Test");
			Database.SchemaTables.AddRange(new[]
			                               	{
												table0,
			                               		table1,
												table2,
												table3,
												table4,
												table5,
												table6,
			                               	});
			try
			{
				foreach (var dbTable in Database.SchemaTables)
				{
					dbTable.TableNameSchema = analyzer.NaturalizeNames_TableSchemaName_Duplicate(dbTable, false);
				}
			}
			finally
			{
				Database.SchemaTables.Clear();
			}
			Assert.AreEqual("tbl_Test", table1.TableNameSchema);
			Assert.AreEqual("tbl_Test_", table2.TableNameSchema);
			Assert.AreEqual("tbl_Test_1", table3.TableNameSchema);
			Assert.AreEqual("tbl_Test_2", table4.TableNameSchema);
		}

		/// <summary>
		///A test for NaturalizeNames_TableSchemaName_Duplicate
		///</summary>
		[TestMethod()]
		[DeploymentItem("SalarDbCodeGenerator.exe")]
		public void NaturalizeNames_TableSchemaName_DuplicateTest_ForView()
		{
			Pattern.LanguageSettings.KeywordsCaseSensitive = true;
			var analyzer = new SchemaAnalyzer_Accessor(Project, Pattern, Database);
			var table0 = new DbView("tbl_Other");
			var table1 = new DbView("tbl_Test");
			var table2 = new DbView("tbl_Test");
			var table3 = new DbView("tbl_Test");
			var table4 = new DbView("tbl_Test");
			var table5 = new DbView("tbl_Test");
			var table6 = new DbView("tbl_Test");
			Database.SchemaViews.AddRange(new[]
			                               	{
												table0,
			                               		table1,
												table2,
												table3,
												table4,
												table5,
												table6,
			                               	});
			try
			{
				foreach (var dbTable in Database.SchemaViews)
				{
					dbTable.TableNameSchema = analyzer.NaturalizeNames_TableSchemaName_Duplicate(dbTable, true);
				}
			}
			finally
			{
				Database.SchemaViews.Clear();
			}
			Assert.AreEqual("tbl_Test", table1.TableNameSchema);
			Assert.AreEqual("tbl_Test_", table2.TableNameSchema);
			Assert.AreEqual("tbl_Test_1", table3.TableNameSchema);
			Assert.AreEqual("tbl_Test_2", table4.TableNameSchema);
		}

		///// <summary>
		/////A test for NaturalizeNames_DotNetTypeClean
		/////</summary>
		//[TestMethod()]
		//[DeploymentItem("SalarDbCodeGenerator.exe")]
		//public void NaturalizeNames_DotNetTypeCleanTest()
		//{
		//    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
		//    SchemaAnalyzer_Accessor target = new SchemaAnalyzer_Accessor(param0); // TODO: Initialize to an appropriate value
		//    string dataTypeDotNet = string.Empty; // TODO: Initialize to an appropriate value
		//    string expected = string.Empty; // TODO: Initialize to an appropriate value
		//    string actual;
		//    actual = target.NaturalizeNames_DotNetTypeClean(dataTypeDotNet);
		//    Assert.AreEqual(expected, actual);
		//    Assert.Inconclusive("Verify the correctness of this test method.");
		//}
		///// <summary>
		/////A test for NaturalizeNames_Name_RemoveInvalidChars
		/////</summary>
		//[TestMethod()]
		//[DeploymentItem("SalarDbCodeGenerator.exe")]
		//public void NaturalizeNames_Name_RemoveInvalidCharsTest()
		//{
		//    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
		//    SchemaAnalyzer_Accessor target = new SchemaAnalyzer_Accessor(param0); // TODO: Initialize to an appropriate value
		//    string name = string.Empty; // TODO: Initialize to an appropriate value
		//    string expected = string.Empty; // TODO: Initialize to an appropriate value
		//    string actual;
		//    actual = target.NaturalizeNames_Name_RemoveInvalidChars(name);
		//    Assert.AreEqual(expected, actual);
		//    Assert.Inconclusive("Verify the correctness of this test method.");
		//}

		///// <summary>
		/////A test for NaturalizeNames_RenamingOptions
		/////</summary>
		//[TestMethod()]
		//[DeploymentItem("SalarDbCodeGenerator.exe")]
		//public void NaturalizeNames_RenamingOptionsTest()
		//{
		//    string name = string.Empty; // TODO: Initialize to an appropriate value
		//    ProjectRenaming opt = null; // TODO: Initialize to an appropriate value
		//    bool isTable = false; // TODO: Initialize to an appropriate value
		//    bool isProp = false; // TODO: Initialize to an appropriate value
		//    string expected = string.Empty; // TODO: Initialize to an appropriate value
		//    string actual;
		//    actual = SchemaAnalyzer_Accessor.NaturalizeNames_RenamingOptions(name, opt, isTable, isProp);
		//    Assert.AreEqual(expected, actual);
		//    Assert.Inconclusive("Verify the correctness of this test method.");
		//}

		///// <summary>
		/////A test for NaturalizeNames_TableName_Rename
		/////</summary>
		//[TestMethod()]
		//[DeploymentItem("SalarDbCodeGenerator.exe")]
		//public void NaturalizeNames_TableName_RenameTest()
		//{
		//    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
		//    SchemaAnalyzer_Accessor target = new SchemaAnalyzer_Accessor(param0); // TODO: Initialize to an appropriate value
		//    string name = string.Empty; // TODO: Initialize to an appropriate value
		//    string expected = string.Empty; // TODO: Initialize to an appropriate value
		//    string actual;
		//    actual = target.NaturalizeNames_TableName_Rename(name);
		//    Assert.AreEqual(expected, actual);
		//    Assert.Inconclusive("Verify the correctness of this test method.");
		//}

		///// <summary>
		/////A test for NaturalizeNames_TableSchemaNameCS_Duplicate
		/////</summary>
		//[TestMethod()]
		//[DeploymentItem("SalarDbCodeGenerator.exe")]
		//public void NaturalizeNames_TableSchemaNameCS_DuplicateTest()
		//{
		//    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
		//    SchemaAnalyzer_Accessor target = new SchemaAnalyzer_Accessor(param0); // TODO: Initialize to an appropriate value
		//    string name = string.Empty; // TODO: Initialize to an appropriate value
		//    bool checkViews = false; // TODO: Initialize to an appropriate value
		//    string expected = string.Empty; // TODO: Initialize to an appropriate value
		//    string actual;
		//    actual = target.NaturalizeNames_TableSchemaNameCS_Duplicate(name, checkViews);
		//    Assert.AreEqual(expected, actual);
		//    Assert.Inconclusive("Verify the correctness of this test method.");
		//}



		///// <summary>
		/////A test for NaturalizeNames_ViewName_Rename
		/////</summary>
		//[TestMethod()]
		//[DeploymentItem("SalarDbCodeGenerator.exe")]
		//public void NaturalizeNames_ViewName_RenameTest()
		//{
		//    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
		//    SchemaAnalyzer_Accessor target = new SchemaAnalyzer_Accessor(param0); // TODO: Initialize to an appropriate value
		//    string name = string.Empty; // TODO: Initialize to an appropriate value
		//    string expected = string.Empty; // TODO: Initialize to an appropriate value
		//    string actual;
		//    actual = target.NaturalizeNames_ViewName_Rename(name);
		//    Assert.AreEqual(expected, actual);
		//    Assert.Inconclusive("Verify the correctness of this test method.");
		//}
	}
}
