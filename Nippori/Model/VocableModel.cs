using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using Nippori.Enums;

namespace Nippori.Model
{
    /// <summary>
    /// Represents a vocable.
    /// </summary>
    public class VocableModel
    {
        #region .: Properties :.

        /// <summary>
        /// Vocable fields (like Czech/English meaning, infinitive, past tense etc.).
        /// </summary>
        public string[] Fields { get; private set; }

        /// <summary>
        /// Gets list of types assigned to this vocable.
        /// </summary>
        public List<TypeModel> Types { get; set; }
        /// <summary>
        /// Gets list of groups assigned to this vocable.
        /// </summary>
        public List<GroupModel> Groups { get; set; }
        public VocableStatus Status { get; set; }
        /// <summary>
        /// Indicates that the vocable is enabled for examination.
        /// </summary>
        public bool Enabled { get; set; }

        #endregion

        #region .: Constructors :.

        public VocableModel(XmlNode xmlNode, int fieldsCount)
        {
            int i;

            // load translation fields
            Fields = new string[fieldsCount];
            for (i = 1; i <= fieldsCount; i++)
            {
                Fields[i - 1] = xmlNode.Attributes[$"field{i}"].Value;
            }

            // read 'active' flag
            string status = xmlNode.Attributes[$"field{fieldsCount + 3}"].Value;
            Status = (VocableStatus)int.Parse(status);
            Enabled = Status != VocableStatus.Inactive;
        }

        #endregion

        #region .: Public Methods :.

        /// <summary>
        /// Finds out if this vocable is of given vocable type.
        /// It is evaluated if the type is listed among allowed types in the vocabulary and also if
        /// the first input field defined by the type is not empty.
        /// </summary>
        /// <param name="vocableType">Vocable type to be checked against.</param>
        /// <returns>Boolean result.</returns>
        public bool IsType(TypeModel vocableType)
        {
            return 
                (Types.Any(type => type == vocableType)) &&
                (Fields[vocableType.InputColumns[0] - 1].Length > 0);
        }

        #endregion
    }
}
