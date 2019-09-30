using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace Nippori.Vocables
{
    /// <summary>
    /// Represents a vocable.
    /// </summary>
    public class Vocable
    {
        #region .: Properties :.

        public string[] Fields { get; private set; }

        /// <summary>
        /// Typ slovíčka.
        /// </summary>
        public VocableType Type { get; set; }
        /// <summary>
        /// Čte a zapisuje pole typů slovíčka. Je to údaj vyčtený z Excelu, který znamená,
        /// jaké všechny typy tento jeden záznam podporuje.
        /// </summary>
        public List<CheckableItem<VocableType>> Types { get; set; }
        /// <summary>
        /// Čte a zapisuje pole skupin slovíčka. Je to údaj vyčtený z Excelu, který říká,
        /// do jakých všech skupin toto slovíčko patří.
        /// </summary>
        public List<CheckableItem> Groups { get; set; }
        /// <summary>
        /// Čte zadání slovíčka (např. český překlad, zkouší-li se překlad z češtiny).
        /// </summary>
        public string Input
        {
            get
            {
                return Fields[Type.InputColumn - 1];
            }
        }
        /// <summary>
        /// Čte počet překladů slovíčka.
        /// </summary>
        public int OutputCount { get { return Type.OutputColumns.Count(); } }

        #endregion

        #region .: Private Fields :.

        /// <summary>
        /// Číslo řádku v Excelu, z kterého bylo slovíčko načteno.
        /// </summary>
        private int id;

        #endregion

        #region .: Constructors :.

        public Vocable(int id)
        {
            this.id = id;
        }

        public Vocable(XmlNode xmlNode, int fieldsCount)
        {
            int i;

            // load translation fields
            Fields = new string[fieldsCount];
            for (i = 1; i <= fieldsCount; i++)
            {
                Fields[i - 1] = xmlNode.Attributes[$"field{i}"].Value;
            }
        }

        #endregion

        #region .: Public Methods :.

        /// <summary>
        /// Vrátí překlad slovíčka, jak je definovaný aktuálně nastaveným typem.
        /// </summary>
        /// <param name="index">Index překladu.</param>
        /// <returns>Překlad slovíčka.</returns>
        public string GetOutput(int index)
        {
            if (index < Type.OutputColumns.Count())
                return Fields[Type.OutputColumns[index] - 1];
            else
                return String.Empty;
        }

        #endregion

    }
}
