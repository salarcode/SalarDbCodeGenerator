using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalarDbCodeGenerator
{
	public enum DatabaseProvider
	{
		SQLServer,
		SqlCe4,
		Oracle,
		SQLite
	}

	public enum DataProviderClassNames
	{
		ClassPrefix,
		ClassCommand,
		ClassConnection,
		ClassDataAdapter,
		ClassDataReader,
		ClassParameter,
		ClassTransaction,
		ClassNamespace,
		AssemblyReference,
		StoredProcParamPrefix
	}

}
