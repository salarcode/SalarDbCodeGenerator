using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalarDbCodeGenerator.DbProject
{
	public class ProjectRenaming
	{
		public class RenamingTaget
		{
			public bool Enabled;
			public bool Properties;
			public bool Tables;
		}
		public enum CaseChangeType
		{
			Capitalize,
			CamelCase,
			PascalCase,
			Lower,
			Upper
		}

		public ProjectRenaming()
		{
			RemoveUnderline = new RenamingTaget() { Properties = true, Tables = true, Enabled = false };
			CaseChange = new RenamingTaget() {Properties = true, Tables = true, Enabled = false};
		}

		public bool UnderlineWordDelimiter { get; set; }
		public RenamingTaget RemoveUnderline { get; set; }
		public RenamingTaget CaseChange { get; set; }
		public CaseChangeType CaseChangeMode { get; set; }
	}
}
