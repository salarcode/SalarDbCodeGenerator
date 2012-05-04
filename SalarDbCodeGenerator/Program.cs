using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using SalarDbCodeGenerator.CodeGen.Generator;
using SalarDbCodeGenerator.DbProject;

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
#if DEBUG
			//frmTest frm = new frmTest();
			//frm.ShowDialog();
			//////RenamingTest();
			//return;
#else
			Application.ThreadException += Application_ThreadException;
#endif
			PleaseWait.ShowPleaseWait("Initializing Systems", true, false);
			Application.Run(new frmCodeGen());
		}

#if DEBUG
		private static void RenamingTest()
		{
			ProjectRenaming ren = new ProjectRenaming();
			ren.RemoveUnderline.Enabled = false ;
			ren.UnderlineWordDelimiter = true;
			ren.CaseChange.Enabled = true;
			ren.CaseChangeMode = ProjectRenaming.CaseChangeType.Capitalize;

			var test = "This_WAS__a_testMan!";
			var result = GeneratorEngine.NaturalizeNames_RenamingOptions(test, ren,true,true);

			MessageBox.Show("Source is: " + test + "\nResult is:   " + result, "Renaming");
		}
#endif

		static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			PleaseWait.Abort();
			MessageBox.Show("Error, something went wrong!\n" + e.Exception.Message, "Unhandled error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
		}
	}
}
