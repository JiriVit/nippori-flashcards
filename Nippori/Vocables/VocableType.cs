using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;

namespace Nippori.Vocables
{
    /// <summary>
    /// Represents a vocable type in the meaning of its testing.
    /// A type defines which vocable field is presented as question and which ones are answers.
    /// TODO Add support for output columns defined in format "2+3,4" which says that columns 2 and 3
    /// are to be shown in one step and column 4 in the next one.
    /// </summary>
    public class VocableType
    {
        public string Name { get; set; }
        public int InputColumn { get; set; }
        public int[] OutputColumns { get; set; }

        #region .: Constructor :.

        /// <summary>
        /// Creates new instance of <see cref="VocableType"/>.
        /// </summary>
        public VocableType()
        {

        }

        /// <summary>
        /// Creates new instance of <see cref="VocableType"/> with data imported from an XML node.
        /// </summary>
        /// <param name="xmlNode">XML node with data definition.</param>
        public VocableType(XmlNode xmlNode)
        {
            Name = xmlNode.Attributes["name"].Value;
            InputColumn = int.Parse(xmlNode.Attributes["in"].Value);
            OutputColumns = xmlNode.Attributes["out"].Value.Split(';').Select(int.Parse).ToArray();
        }

        #endregion

        public override string ToString()
        {
            return String.Format("{0} | {1} | {2}", Name, InputColumn,
                String.Join(",", OutputColumns.Select(x => x.ToString())));
        }
    }
}
