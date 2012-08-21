using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Salar Khalilzadeh <salar2k@gmail.com>
// © 2012, All rights reserved
// 2012/07/06
// ====================================
namespace SalarDbCodeGenerator.DbProject
{
	public static class Common
	{
		public static string AppVarPathMakeRelative(string path)
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				return string.Empty;
			}
			var appPath = Path.GetDirectoryName(Application.ExecutablePath)
				.ToLower();
			if (path.ToLower().IndexOf(appPath) == 0)
			{
				return AppConfig.AppVarApplicationPath + path.Remove(0, appPath.Length);
			}
			return path;
		}

		public static string AppVarPathMakeAbsolute(string path)
		{
			var appPath = Path.GetDirectoryName(Application.ExecutablePath);
			if (string.IsNullOrWhiteSpace(path))
			{
				return appPath;
			}
			return path.Replace(AppConfig.AppVarApplicationPath, appPath);
		}


		public static string ProjectPathMakeRelative(string path, string projectPath)
		{
			if (File.Exists(projectPath))
				return PathMakeRelativeTo(path, Path.GetDirectoryName(projectPath), AppConfig.AppVarProjectPath);
			else
				return PathMakeRelativeTo(path, projectPath, AppConfig.AppVarProjectPath);
		}

		public static string ProjectPathMakeAbsolute(string path, string projectPath)
		{
			if (File.Exists(projectPath))
				return PathMakeAbsoluteTo(path, Path.GetDirectoryName(projectPath), AppConfig.AppVarProjectPath);
			else
				return PathMakeAbsoluteTo(path, projectPath, AppConfig.AppVarProjectPath);
		}


		public static string PathMakeRelativeTo(string path, string relativeTo, string relativeMarkReplacement)
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				return string.Empty;
			}
			var appPath = relativeTo.ToLower();
			if (path.ToLower().IndexOf(appPath) == 0)
			{
				return relativeMarkReplacement + path.Remove(0, appPath.Length);
			}
			return path;
		}

		public static string PathMakeAbsoluteTo(string path, string relativeTo, string relativeMarkReplacement)
		{
			var appPath = relativeTo;
			if (string.IsNullOrWhiteSpace(path))
			{
				return appPath;
			}
			return path.Replace(relativeMarkReplacement, appPath);
		}

		public static bool TryConvertBoolean(object b, bool defValue)
		{
			try
			{
				return Convert.ToBoolean(b);
			}
			catch (Exception)
			{
				try
				{
					return Convert.ToBoolean(Convert.ToInt16(b));
				}
				catch (Exception)
				{
					return defValue;
				}
			}
		}

		public static int TryConvertInt32(string number, int defValue)
		{
			int r;
			if (int.TryParse(number, out r))
			{
				return r;
			}
			return defValue;
		}

		public static long TryConvertInt64(string number, long defValue)
		{
			long r;
			if (long.TryParse(number, out r))
			{
				return r;
			}
			return defValue;
		}

		/// <summary>
		/// Implements fast string replacing algorithm for CS
		/// </summary>
		public static string ReplaceExIgnoreCase(string original, string pattern, string replacement)
		{
			return Common.ReplaceEx(original, pattern, replacement, StringComparison.CurrentCultureIgnoreCase);
		}

		/// <summary>
		/// Implements fast string replacing algorithm for CS
		/// </summary>
		public static string ReplaceEx(string original, string pattern, string replacement, StringComparison comparisonType)
		{
			if (original == null)
			{
				return null;
			}

			if (String.IsNullOrEmpty(pattern))
			{
				return original;
			}

			int lenPattern = pattern.Length;
			int idxPattern = -1;
			int idxLast = 0;

			StringBuilder result = new StringBuilder();

			while (true)
			{
				idxPattern = original.IndexOf(pattern, idxPattern + 1, comparisonType);

				if (idxPattern < 0)
				{
					result.Append(original, idxLast, original.Length - idxLast);

					break;
				}

				result.Append(original, idxLast, idxPattern - idxLast);
				result.Append(replacement);

				idxLast = idxPattern + lenPattern;
			}

			return result.ToString();
		}

		public static string GetExceptionTechMessage(Exception ex, string seperator = null)
		{
			if (string.IsNullOrEmpty(seperator))
				seperator = "\r\n";
			string result = ex.GetType().Name + " -> " + ex.Message;
			Exception inner = ex.InnerException;

			while (inner != null)
			{
				result += seperator + inner.GetType().Name + " -> " + inner.Message;
				inner = inner.InnerException;
			}
			return result;
		}

	}
}
