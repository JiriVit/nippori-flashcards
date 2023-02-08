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
            if (status.Equals(string.Empty))
            {
                Status = VocableStatus.Active;
            }
            else
            {
                Status = (VocableStatus)int.Parse(status);
            }
            Enabled = Status != VocableStatus.Inactive;
        }

        #endregion
    }
}
