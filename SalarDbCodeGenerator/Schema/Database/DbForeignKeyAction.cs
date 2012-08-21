using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalarDbCodeGenerator.Schema.Database
{
	public enum DbForeignKeyAction
	{
		/// <summary>
		/// Not specified in database
		/// </summary>
		NotSet = -1,
		NoAction = 0,
		Cascade,
		SetNull,
		SetDefault,
		Restrict
	}
}
