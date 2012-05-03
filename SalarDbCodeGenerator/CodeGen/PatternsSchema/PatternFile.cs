using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Programmer: Salar Khalilzadeh <salar2k@gmail.com>
// Copytight(c) 2012, All rights reserved
// 2010-06-09
// ====================================
namespace SalarSoft.DbCodeGenerator.CodeGen.PatternsSchema
{
    [Serializable]
    public class PatternFile
    {
        #region local variables
        #endregion

        #region field variables
        #endregion


        #region properties
        /// <summary>
        /// Pattern name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Pattern group
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Pattern description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Overwrite existing file
        /// </summary>
        public bool Overwrite { get; set; }

        /// <summary>
        /// Generated file path
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Pattern language
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Where this pattern applies
        /// </summary>
        public PatternFileAppliesTo AppliesTo { get; set; }

        /// <summary>
        /// All tables/views should apply to one file?
        /// </summary>
        public bool AppliesToAllToOne
        {
            get
            {
                if (AppliesTo == PatternFileAppliesTo.Tables_All ||
                    AppliesTo == PatternFileAppliesTo.TablesAndViews_All ||
                    AppliesTo == PatternFileAppliesTo.Views_All)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Pattern content
        /// </summary>
        public string BaseContent { get; set; }


        /// <summary>
        /// Pertial contents
        /// </summary>
        public List<PatternContent> PatternContents { get; set; }
        #endregion

        #region public methods
        public PatternFile()
        {
            PatternContents = new List<PatternContent>();
        }

        public PatternFile Clone()
        {
            return (PatternFile)this.MemberwiseClone();
        }

        public void LoadFromFile(string patternXmlFile)
        {
            LoadFromFile(patternXmlFile, false);
        }

        public void LoadFromFile(string patternXmlFile, bool onlyGeneralInfo)
        {
            XDocument xDoc = XDocument.Load(patternXmlFile);

            Name = xDoc.Root.Element("Name").Value;
            Description = xDoc.Root.Element("Description").Value;

            XElement xOptions = xDoc.Root.Element("Options");
            Group = xOptions.Attribute("Group").Value;
            Overwrite = Convert.ToBoolean(xOptions.Attribute("Overwrite").Value);
            FilePath = xOptions.Attribute("FilePath").Value;
            Language = xOptions.Attribute("Language").Value;

            // Pattern files where to apply
            AppliesTo = (PatternFileAppliesTo)Enum.Parse(typeof(PatternFileAppliesTo), xOptions.Attribute("AppliesTo").Value, true);

            PatternContents.Clear();
            if (onlyGeneralInfo == false)
            {
                // Base contents
                BaseContent = xDoc.Root.Element("BaseContent").Value;

                // Read pattern contents list
                IEnumerable<PatternContent> contentsList = from e in xDoc.Root.Elements("PatternContent")
                                                           orderby e.Attribute("Name").Value
                                                           select PatternContent.ReadFromXElement(e);
                PatternContents.Clear();
                PatternContents.AddRange(contentsList);
            }
        }

        public string ToSumuaryString()
        {
            var sb = new StringBuilder();

            sb.AppendFormat("Name= {0}\n", Name);
            sb.AppendFormat("AppliesTo= {0}\n", AppliesToString(AppliesTo));
            sb.AppendFormat("Overwrite= {0}\n", Overwrite);
            sb.AppendFormat("Language= {0}", Language);

            return sb.ToString();
        }


        #endregion

        #region protected methods
        #endregion

        #region private methods
        private string AppliesToString(PatternFileAppliesTo appliesTo)
        {
            switch (appliesTo)
            {
                case PatternFileAppliesTo.ProjectFile:
                    return "Once per project";
                
                case PatternFileAppliesTo.GeneralOnce:
                    return "Once per project";

                case PatternFileAppliesTo.TablesAndViews_Each:
                    return "Each table/view one file";

                case PatternFileAppliesTo.TablesAndViews_All:
                    return "One file for all tables/views";

                case PatternFileAppliesTo.Tables_Each:
                    return "Each table one file";

                case PatternFileAppliesTo.Tables_All:
                    return "One file for all tables";

                case PatternFileAppliesTo.Views_Each:
                    return "Each view one file";

                case PatternFileAppliesTo.Views_All:
                    return "One file for all views";

            }
            return "[Undefined]";
        }
        #endregion

    }


}
