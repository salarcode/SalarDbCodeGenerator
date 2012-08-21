using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SalarDbCodeGenerator.Schema.Patterns
{
	public class SchemaPatternBase<T> where T : class
	{

		public static T ReadFromFile(string projectFilename)
		{
			T project;
			var loader = new XmlSerializer(typeof(T));
			using (var reader = new StreamReader(projectFilename))
				project = (T)loader.Deserialize(reader);
			return project;
		}

		public void SaveToFile(string projectFilename)
		{
			var saver = new XmlSerializer(typeof(T));
			using (var writer = new StreamWriter(projectFilename))
				saver.Serialize(writer, this);
		}
	}
}
