using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;

namespace Nippori
{
    /// <summary>
    /// Reprezentuje slovník.
    /// </summary>
    public static class Vocabulary
    {
        #region Constants

        const int COL_TYPES_OFFSET = 1;
        const int COL_GROUPS_OFFSET = 2;
        const int COL_ACTIVE_OFFSET = 3;

        #endregion

        #region Properties

        /// <summary>
        /// Aktuálně vybrané slovíčko.
        /// </summary>
        public static Vocable CurrentItem;
        /// <summary>
        /// Konfigurace.
        /// </summary>
        public static Dictionary<string, string> Configuration;
        public static NameValueCollection Groups;
        public static List<VocableType> Types;

        #endregion

        #region Private Fields

        public static List<Vocable> vocables;
        private static Random random;
        private static List<Vocable> vocableStack;
        private static Queue<Vocable> vocableQueue;
        private static int itemColumns;

        #endregion

        #region Private Methods

        /// <summary>
        /// Načte konfiguraci z excelového listu a uloží ji do slovníku.
        /// </summary>
        /// <param name="sheet">Excelovský list, z něhož se čte konfigurace.</param>
        private static void ImportConfiguration(Worksheet sheet)
        {
            int row, rowCount;

            rowCount = sheet.UsedRange.Rows.Count;
            Configuration = new Dictionary<string, string>(rowCount - 1);

            /* procházím řádky a čtu dvojice klíč + hodnota */
            for (row = 2; row <= rowCount; row++)
            {
                /* eliminace prázdných buněk zahrnutých do UsedRange */
                if (sheet.Cells[row, 1].Value == null)
                    break;

                Configuration.Add(sheet.Cells[row, 1].Value.ToString(),
                    sheet.Cells[row, 2].Value.ToString());
            }

            itemColumns = Int32.Parse(Configuration["columns"]);
        }

        private static void ImportGroups(Worksheet sheet)
        {
            int row, rowCount;

            rowCount = sheet.UsedRange.Rows.Count;
            Groups = new NameValueCollection(rowCount);

            /* procházím řádky a čtu dvojice klíč + hodnota */
            for (row = 2; row <= rowCount; row++)
            {
                /* eliminace prázdných buněk zahrnutých do UsedRange */
                if (sheet.Cells[row, 1].Value == null)
                    break;

                Groups.Add(sheet.Cells[row, 1].Value.ToString(),
                    sheet.Cells[row, 2].Value.ToString());
            }
        }

        /// <summary>
        /// Načte typy slovíček z excelového listu a uloží je do slovníku.
        /// </summary>
        /// <param name="sheet">Excelovský list, z něhož se čtou typy.</param>
        private static void ImportTypes(Worksheet sheet)
        {
            VocableType importedType;
            int row, rowCount;
            string[] outputColumns;

            rowCount = sheet.UsedRange.Rows.Count;
            Types = new List<VocableType>(rowCount - 1);

            for (row = 2; row <= rowCount; row++)
            {
                /* eliminace prázdných buněk zahrnutých do UsedRange */
                if (sheet.Cells[row, 1].Value == null)
                    break;

                importedType = new VocableType();

                importedType.Name = sheet.Cells[row, 2].Value.ToString();
                importedType.InputColumn = Convert.ToInt32(sheet.Cells[row, 3].Value.ToString());
                outputColumns = sheet.Cells[row, 4].Value.ToString().Split(';');
                importedType.OutputColumns = outputColumns.Select(Int32.Parse).ToArray();

                Types.Add(importedType);
            }
        }

        /// <summary>
        /// Načte slovíčka z excelového listu a uloží je do slovníku.
        /// </summary>
        /// <param name="sheet">Excelovský list, z něhož se čtou slovíčka.</param>
        private static void ImportVocables(Worksheet sheet)
        {
            Vocable importedVocable;
            int row, rowCount, col;
            string[] importedColumns;
            

            rowCount = sheet.UsedRange.Rows.Count;
            vocables = new List<Vocable>(rowCount - 1);

            for (row = 2; row <= rowCount; row++)
            {
                /* eliminace prázdných buněk zahrnutých do UsedRange */
                if (sheet.Cells[row, 1].Value == null)
                    break;

                if ( (sheet.Cells[row, itemColumns + COL_ACTIVE_OFFSET].Value != null) &&
                     (!sheet.Cells[row, itemColumns + COL_ACTIVE_OFFSET].Value.ToString().Equals("1")))
                    continue;

                importedVocable = new Vocable();
                importedVocable.ID = row - 2;
                importedVocable.Items = new string[itemColumns];

                for (col = 1; col <= itemColumns; col++)
                    importedVocable.Items[col - 1] = sheet.Cells[row, col].Value.ToString();
                if (sheet.Cells[row, itemColumns + COL_TYPES_OFFSET].Value == null)
                {
                    importedVocable.Types = new int[] { 1 };
                }
                else
                {
                    importedColumns = sheet.Cells[row, itemColumns + COL_TYPES_OFFSET].Value.ToString().Split(';');
                    importedVocable.Types = importedColumns.Select(Int32.Parse).ToArray();
                }
                if (sheet.Cells[row, itemColumns + COL_GROUPS_OFFSET].Value == null)
                    importedVocable.Groups = new string[] { Groups.Keys[0] };
                else
                    importedVocable.Groups = sheet.Cells[row, itemColumns + COL_GROUPS_OFFSET].Value.ToString().Split(';');

                vocables.Add(importedVocable);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Načte slovíčka ze souboru. Nedělá nic jiného - refresh se musí provést ručně.
        /// </summary>
        /// <param name="fileName">Jméno souboru.</param>
        public static void ReadFile(string fileName)
        {
            Application excel = new Application();

            try
            {
                excel.Workbooks.Open(fileName);

                ImportConfiguration(excel.ActiveWorkbook.Worksheets["CONFIG"]);
                ImportTypes(excel.ActiveWorkbook.Worksheets["TYPES"]);
                ImportGroups(excel.ActiveWorkbook.Worksheets["GROUPS"]);
                ImportVocables(excel.ActiveWorkbook.Worksheets["LIST"]);
            }
            finally
            {
                excel.Workbooks.Close();
            }
        }

        /// <summary>
        /// Inicializace slovníku.
        /// </summary>
        public static void Init()
        {
            List<VocableType> types = new List<VocableType>();
            List<Vocable> list = new List<Vocable>();

            random = new Random();

            //types.Add(new VocableType(new string[] {"Česky", "Deutsch"}));
            //types.Add(new VocableType(new string[] {"Česky", "Singular", "Plural"}));

            //typeList = types.ToArray();

            //list.Add(new Vocable(typeList[0], "rád", new string[] {"gern"}));
            //list.Add(new Vocable(typeList[0], "přirozeně", new string[] {"natürlich"}));

            //internalList = list.ToArray();

            //vocableStack = new List<Vocable>(internalList);
            //vocableQueue = new Queue<Vocable>(2);
        }

        /// <summary>
        /// Náhodně vytáhne jedno ze zbývajících slovíček a označí je jako vybrané.
        /// </summary>
        public static void GetRandomItem()
        {
            CurrentItem = vocableStack[random.Next(vocableStack.Count)];
            Trace.WriteLine(String.Format("Vytahuji slovo {0}, zbývá jich {1}.", CurrentItem, vocableStack.Count));
        }

        /// <summary>
        /// Odstraní ze seznamu vybrané slovíčko.
        /// </summary>
        public static void RemoveItem()
        {
            Trace.WriteLine(String.Format("Odstraňuji slovo {0}.", CurrentItem));
            vocableStack.Remove(CurrentItem);            
        }

        /// <summary>
        /// Vytáhne slovíčko z fronty špatně zodpovězených a nastaví jako aktuální.
        /// </summary>
        public static void GetQueueItem()
        {
            CurrentItem = vocableQueue.Dequeue();
            Trace.WriteLine(String.Format("Vytahuji z fronty slovo {0}, ve frontě zbývá {1}.", CurrentItem, vocableQueue.Count));
        }

        /// <summary>
        /// Zjistí počet slovíček ve frontě.
        /// </summary>
        /// <returns>Počet slovíček ve frontě.</returns>
        public static int GetQueueCount()
        {
            return vocableQueue.Count();
        }

        public static int GetListCount()
        {
            return vocableStack.Count;
        }

        public static void PutToQueue()
        {
            vocableQueue.Enqueue(CurrentItem);
            Trace.WriteLine(String.Format("Dávám do fronty slovo {0}, ve frontě je {1}.", CurrentItem, vocableQueue.Count));
        }

        public static void ReloadList()
        {
            Trace.WriteLine("Obnovuji seznam.");
            vocableStack.Clear();
            //vocableStack.AddRange(internalList);
        }

        public static bool IsEmpty()
        {
            return vocableStack.Count == 0;
        }

        public static bool IsItemInQueue()
        {
            return vocableQueue.Contains(CurrentItem);
        }

        #endregion

    }

    /// <summary>
    /// Třída reprezentuje slovíčko.
    /// </summary>
    public class Vocable
    {
        #region .: Properties :.

        public int ID { get; set; }
        /// <summary>
        /// Typ slovíčka.
        /// </summary>
        public VocableType Type { get; set; }
        /// <summary>
        /// Položky.
        /// </summary>
        public string[] Items { get; set; }
        public int[] Types { get; set; }
        public string[] Groups { get; set; }

        #endregion

        #region .: Public Overriden Methods :.

        public override bool Equals(object obj)
        {
            if (obj is Vocable)
                return ((Vocable)obj).ID.Equals(this.ID);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0}: {1}", ID, Items[0]);
        }


        #endregion
    }

    /// <summary>
    /// Reprezentuje typ slovíčka.
    /// </summary>
    public class VocableType
    {
        public string Name { get; set; }
        public int InputColumn { get; set; }
        public int[] OutputColumns { get; set; }

        public override string ToString()
        {
            return String.Format("{0} | {1} | {2}", Name, InputColumn, 
                String.Join(",", OutputColumns.Select(x => x.ToString())));
        }
    }

}
