using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using SalarDbCodeGenerator.DbProject;
using SalarDbCodeGenerator.Schema.Patterns;

namespace SalarDbCodeGenerator
{
	static class Program
	{
		public static string[] ProgramArgs = null;
#if DEBUG
		//public static frmTest frm;
#endif

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			ProgramArgs = args;
			//SaveTempPatternSchema();
#if DEBUG
			Application.ThreadException += Application_ThreadException_Debug;
#else
			Application.ThreadException += Application_ThreadException_Release;
#endif
			PleaseWait.ShowPleaseWait("Initializing Systems", true, false);
			Application.Run(new frmCodeGen());
		}

#if DEBUG

		private static void SaveTempPatternSchema()
		{
			var patternProject = new PatternProject();
			patternProject.Author = "aa";
			patternProject.Description = "Description";
			patternProject.FileExtension = "FileExtension";
			patternProject.Language = "Language";
			patternProject.LastUpdate = "LastUpdate";
			patternProject.SeperateReferenceColumns = true;
			patternProject.SupportedDatabases = "SupportedDatabases";
			patternProject.PatternFiles.Add(new PatternProject.PatternFile()
			{
				Action = PatternsListItemAction.Generate,
				ActionCopyPath = "/",
				Path = "gooo.pml"
			});
			patternProject.PatternFiles.Add(new PatternProject.PatternFile()
			{
				Action = PatternsListItemAction.Copy,
				ActionCopyPath = "/",
				Path = "goood.pml"
			});
			patternProject.LanguageSettings.LanguageKeywords.Add("get");
			patternProject.LanguageSettings.LanguageKeywords.Add("set");
			patternProject.LastUpdate = "LastUpdate";
			patternProject.SaveToFile(@"E:\Programming\C#.NET\SalarDbCodeGenerator_new\temp\PatternProject.dbpat");

			var file = new PatternFile();
			file.BaseContent = "BaseContent";
			file.Description = "Description";
			file.Name = "Name";
			file.Options.AppliesTo = PatternFileAppliesTo.TablesAndViewsAll;
			file.Options.FilePath = "ok/go.cs";
			file.Options.Group = "Group";
			file.Options.Language = "Language";
			file.Options.Overwrite = true;

			var pc = new PatternContent();
			pc.BaseContent = "BaseContent";
			pc.ConditionKeyMode = PatternConditionKeyMode.FieldKeyType;
			pc.ItemsSeperator = ", ";
			pc.Name = "Name";
			var ci = new ConditionItem();
			ci.ContentText = "ContentText ContentText ContentText";
			ci.Key = "KEY1";
			pc.Conditions.Add(ci);
			ci = new ConditionItem();
			ci.ContentText = "Content2Text ContentText2 ContentText";
			ci.Key = "KEY2";
			pc.Conditions.Add(ci);
			file.PatternContents.Add(pc);

			var cpc = new PatternContent();
			cpc.BaseContent = "BaseContent";
			pc.ConditionKeyMode = PatternConditionKeyMode.FieldKeyType;
			cpc.ItemsSeperator = ", ";
			cpc.Name = "Name";
			  ci = new ConditionItem();
			ci.ContentText = "ContentText ContentText ContentText";
			ci.Key = "KEY1";
			cpc.Conditions.Add(ci);
			ci = new ConditionItem();
			ci.ContentText = "Content2Text ContentText2 ContentText";
			ci.Key = "KEY2";
			cpc.Conditions.Add(ci);
			pc.ConditionContents.Add(cpc);
			pc.ConditionContents.Add(cpc);

			pc = new PatternContent();
			pc.BaseContent = "BaseContent2";
			pc.ConditionKeyMode = PatternConditionKeyMode.FieldPrimaryKey;
			pc.ItemsSeperator = ", ";
			pc.Name = "NameKey2";
			ci = new ConditionItem();
			ci.ContentText = "ContentText _ ContentText _ ContentText";
			ci.Key = "KEY15";
			pc.Conditions.Add(ci);
			ci = new ConditionItem();
			ci.ContentText = "Content2Text6 ContentText6 ContentText";
			ci.Key = "KEY6";
			pc.Conditions.Add(ci);
			file.PatternContents.Add(pc);

 			file.SaveToFile(@"E:\Programming\C#.NET\SalarDbCodeGenerator_new\temp\PatternFile.patml");
		}

		private static void RenamingTest()
		{
			//ProjectRenaming ren = new ProjectRenaming();
			//ren.RemoveUnderline.Enabled = false;
			//ren.UnderlineWordDelimiter = true;
			//ren.CaseChange.Enabled = true;
			//ren.CaseChangeMode = ProjectRenaming.CaseChangeType.Capitalize;

			//var test = "This_WAS__a_testMan!";
			//var result = Generator.NaturalizeNames_RenamingOptions(test, ren, true, true);

			//MessageBox.Show("Source is: " + test + "\nResult is:   " + result, "Renaming");
		}
#endif

		static void Application_ThreadException_Debug(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			PleaseWait.Abort();
			MessageBox.Show("Error, something went wrong!\n" + e.Exception.ToString(), "Unhandled error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
		}
		static void Application_ThreadException_Release(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			PleaseWait.Abort();
			MessageBox.Show("Error, something went wrong!\n" + Common.GetExceptionTechMessage(e.Exception), "Unhandled error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
		}
	}
}
