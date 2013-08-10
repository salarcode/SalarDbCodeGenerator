// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Salar Khalilzadeh <salar2k@gmail.com>
// © 2012, All rights reserved
// 2012/07/06
// ====================================
namespace SalarDbCodeGenerator.Schema.DbSchemaReaders
{
	public enum DatabaseProvider
	{
		SQLServer,
		SqlCe4,
		Oracle,
		SQLite,
		Npgsql,
		MySql
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
