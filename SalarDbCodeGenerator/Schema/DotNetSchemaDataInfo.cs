using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalarDbCodeGenerator.Schema.Patterns;

namespace SalarDbCodeGenerator.Schema
{
	public class DotNetSchemaDataInfo
	{
		public const string DotNetArrayIdenticator = "[]";

		public static bool IsStringType(string dotNetDataType)
		{
			if (dotNetDataType == "System.String")
				return true;
			return false;
		}
		public static string DetermineNumericType(string dotNetDataType)
		{
			switch (dotNetDataType)
			{
				case "System.Int16":
					return ConditionKeyModeConsts.FieldNumericType.Integer;
				case "System.Int32":
					return ConditionKeyModeConsts.FieldNumericType.Integer;
				case "System.Int64":
					return ConditionKeyModeConsts.FieldNumericType.Integer;
				case "System.Byte":
					return ConditionKeyModeConsts.FieldNumericType.Integer;
				case "System.Decimal":
					return ConditionKeyModeConsts.FieldNumericType.Decimal;
				case "System.Single":
					return ConditionKeyModeConsts.FieldNumericType.Decimal;
				case "System.Double":
					return ConditionKeyModeConsts.FieldNumericType.Decimal;
 			}
			return "";
		}

	}
}
