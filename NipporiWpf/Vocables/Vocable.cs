using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using Microsoft.Office.Interop.Excel;

namespace NipporiWpf.Vocables
{
    /// <summary>
    /// Represents a vocable.
    /// </summary>
    public class Vocable
    {
        #region .: Properties :.

        #region .: CSV Import :.

        public string English { get; set; }
        public string Translation { get; set; }
        public string Transcription { get; set; }
        public string Categories { get; set; }

        #endregion

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
                return items[Type.InputColumn - 1];
            }
        }
        /// <summary>
        /// Čte popisek zadání slovíčka.
        /// </summary>
        public string InputLabel
        {
            get
            {
                return Vocabulary.ColumnHeaders[Type.InputColumn - 1];
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
        /// <summary>
        /// Pole všech překladů slovíčka.
        /// </summary>
        private string[] items;

        #endregion

        #region .: Constructors :.

        public Vocable(int id)
        {
            this.id = id;
        }

        public Vocable(int id, XmlNode xmlNode)
        {
            this.id = id;
            Import(xmlNode);
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
                return items[Type.OutputColumns[index] - 1];
            else
                return String.Empty;
        }

        /// <summary>
        /// Vrátí název překladu slovíčka, jak je definovaný aktuálně nastaveným typem.
        /// </summary>
        /// <param name="index">Index překladu.</param>
        /// <returns>Název překladu slovíčka.</returns>
        public string GetOutputLabel(int index)
        {
            if (index < Type.OutputColumns.Count())
                return Vocabulary.ColumnHeaders[Type.OutputColumns[index] - 1];
            else
                return String.Empty;
        }

        /// <summary>
        /// Importuje slovíčko z excelového řádku.
        /// </summary>
        /// <param name="excelRow">Excelový řádek.</param>
        /// <returns>TRUE pokud bylo slovíčko importováno, FALSE pokud je řádek prázdný.</returns>
        public bool Import(Range excelRow)
        {
            int col;

            /* řádek je prázdný, nelze nic importovat */
            if (excelRow.Cells[1, 1].Value == null)
                return false;

            /* načtení překladů */
            items = new string[Vocabulary.ItemColumnCount];
            for (col = 1; col <= Vocabulary.ItemColumnCount; col++)
                if (excelRow.Cells[1, col].Value != null)
                    items[col - 1] = excelRow.Cells[1, col].Value.ToString();

            /* načtení typů */
            if (excelRow.Cells[1, Vocabulary.ItemColumnCount + Vocabulary.COL_TYPES_OFFSET].Value == null)
            {
                // no type defined -> assign all of them
                Types = new List<CheckableItem<VocableType>>(Vocabulary.TypesCollection);
            }
            else
            {
                List<string> typeNumbers = new List<string>(excelRow.Cells[1, Vocabulary.ItemColumnCount + Vocabulary.COL_TYPES_OFFSET].Value.ToString().Split(';'));
                Types = new List<CheckableItem<VocableType>>(typeNumbers.Count);
                typeNumbers.ForEach(typeNumber => Types.Add(Vocabulary.TypesCollection[int.Parse(typeNumber) - 1]));
            }

            /* načtení skupin */
            if (excelRow.Cells[1, Vocabulary.ItemColumnCount + Vocabulary.COL_GROUPS_OFFSET].Value == null)
            {
                // no group defined -> assign the first one
                Groups = new List<CheckableItem>(new CheckableItem[] { Vocabulary.GroupsCollection[0] });
            }
            else
            {
                List<string> groupKeys = new List<string>(excelRow.Cells[1, Vocabulary.ItemColumnCount + Vocabulary.COL_GROUPS_OFFSET].Value.ToString().Split(';'));
                Groups = new List<CheckableItem>(groupKeys.Count);
                groupKeys.ForEach(key => Groups.Add(Vocabulary.GroupsDict[key]));
            }

            return true;
        }

        public bool Import(XmlNode xmlNode)
        {
            int i;

            // load translation fields
            items = new string[Vocabulary.ItemColumnCount];
            for (i = 1; i <= Vocabulary.ItemColumnCount; i++)
            {
                items[i - 1] = xmlNode.Attributes[$"field{i}"].Value;
            }

            // load types
            if (xmlNode.Attributes[$"field{i}"].Value.Equals(string.Empty))
            {
                // no type defined -> assign all of them
                Types = new List<CheckableItem<VocableType>>(Vocabulary.TypesCollection);
            }
            else
            {
                List<string> typeNumbers = new List<string>(xmlNode.Attributes[$"field{i}"].Value.Split(';'));
                Types = new List<CheckableItem<VocableType>>(typeNumbers.Count);
                typeNumbers.ForEach(typeNumber => Types.Add(Vocabulary.TypesCollection[int.Parse(typeNumber) - 1]));
            }
            i++;

            // load groups
            if (xmlNode.Attributes[$"field{i}"].Value.Equals(string.Empty))
            {
                // no group defined -> assign the first one
                Groups = new List<CheckableItem>(new CheckableItem[] { Vocabulary.GroupsCollection[0] });
            }
            else
            {
                List<string> groupKeys = new List<string>(xmlNode.Attributes[$"field{i}"].Value.Split(';'));
                Groups = new List<CheckableItem>(groupKeys.Count);
                groupKeys.ForEach(key => Groups.Add(Vocabulary.GroupsDict[key]));
            }

            return true;
        }

        #endregion

        #region .: Public Overriden Methods :.

        public override bool Equals(object obj)
        {
            if (obj is Vocable)
                return ((Vocable)obj).id.Equals(this.id);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0}: {1}", id, items[0]);
        }

        #endregion

    }
}
