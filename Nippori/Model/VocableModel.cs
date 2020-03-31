using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Nippori.Model
{
    /// <summary>
    /// Represents a vocable.
    /// </summary>
    public class VocableModel
    {
        #region .: Properties :.

        public string[] Fields { get; private set; }

        /// <summary>
        /// Lists types allowed for this vocable.
        /// </summary>
        public List<TypeModel> AllowedTypes { get; set; }
        /// <summary>
        /// Lists groups which this vocable belongs to.
        /// </summary>
        public List<GroupModel> Groups { get; set; }
        /// <summary>
        /// Indicates if the vocable is active for exercising or not.
        /// </summary>
        public bool Active { get; set; }

        #endregion

        #region .: Private Fields :.

        /// <summary>
        /// Číslo řádku v Excelu, z kterého bylo slovíčko načteno.
        /// </summary>
        private int id;

        #endregion

        #region .: Constructors :.

        public VocableModel(int id)
        {
            this.id = id;
        }

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
            string active = xmlNode.Attributes[$"field{fieldsCount + 3}"].Value;
            Active = !(active.Equals("0"));
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
                (AllowedTypes.Any(type => type == vocableType)) &&
                (Fields[vocableType.InputColumns[0] - 1].Length > 0);
        }

        #endregion
    }
}
