using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;

namespace Nippori.Model
{
    /// <summary>
    /// Represents a vocable type in the meaning of its testing.
    /// A type defines which vocable field is presented as question and which ones are answers.
    /// </summary>
    public class TypeModel : GroupModel
    {
        #region .: Properties :.

        /// <summary>
        /// Columns with fields to be shown for examination (as question).
        /// </summary>
        public int[] InputColumns { get; private set; }
        /// <summary>
        /// Columns with fields to be shown after examination (as answers).
        /// </summary>
        public int[] OutputColumns { get; private set; }

        #endregion

        #region .: Constructor :.

        /// <summary>
        /// Creates new instance of <see cref="TypeModel"/> with data imported from an XML node.
        /// </summary>
        /// <param name="xmlNode">XML node with data definition.</param>
        public TypeModel(XmlNode xmlNode)
        {
            Name = xmlNode.Attributes["name"].Value;
            InputColumns = xmlNode.Attributes["in"].Value.Split(',').Select(int.Parse).ToArray();
            OutputColumns = xmlNode.Attributes["out"].Value.Split(',').Select(int.Parse).ToArray();
        }

        #endregion
    }
}
