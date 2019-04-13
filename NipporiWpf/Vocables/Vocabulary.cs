using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;

namespace NipporiWpf.Vocables
{
    /// <summary>
    /// Reprezentuje slovník.
    /// </summary>
    public static class Vocabulary
    {

        #region Constants

        public const int COL_TYPES_OFFSET = 1;
        public const int COL_GROUPS_OFFSET = 2;
        public const int COL_ACTIVE_OFFSET = 3;

        const int QUEUE_LEN = 2;

        #endregion

        #region Properties

        /// <summary>
        /// Aktuálně vybrané slovíčko.
        /// </summary>
        public static Vocable CurrentVocable;
        /// <summary>
        /// Konfigurace.
        /// </summary>
        public static Dictionary<string, string> Configuration;
        /// <summary>
        /// Dictionary of groups, indexed by their keys.
        /// </summary>
        public static Dictionary<string, CheckableItem> GroupsDict;
        /// <summary>
        /// Nadpisy sloupců v tabulce slovíček, musí se zobrazit na formuláři nad poli
        /// pro zadání odpovědí.
        /// </summary>
        public static List<string> ColumnHeaders;
        /// <summary>
        /// Povolený typ slovíček, který se teď má zkoušet. Číslováno od jedničky.
        /// </summary>
        public static VocableType EnabledType;
        /// <summary>
        /// Čte počet slovíček ve frontě.
        /// </summary>
        public static int QueueCount
        {
            get
            {
                return vocableQueue.Count;
            }
        }
        /// <summary>
        /// Gets elapsed number of rounds, i.e. whole sets being examined.
        /// </summary>
        public static int Rounds { get; set; }
        public static int StackCount => vocableStack.Count;
        public static int ItemColumnCount { get { return itemColumns; } }

        public static ObservableCollection<CheckableItem<VocableType>> TypesCollection { get; set; }
        /// <summary>
        /// Observable collection of supported vocable groups.
        /// </summary>
        public static ObservableCollection<CheckableItem> GroupsCollection { get; set; }

        #endregion

        #region Private Fields

        private static Random random;

        /// <summary>
        /// Seznam všech slovíček z Excelu. Plní se ve chvíli, kdy čteme Excel.
        /// </summary>
        private static List<Vocable> allVocables;
        /// <summary>
        /// Zásobník slovíček, z něj se vytahují slovíčka na formulář a po správném zodpovězení se z něj vyhodí.
        /// Když je zásobník prázdný, znovu se naplní.
        /// </summary>
        /// <remarks>
        /// Sloučit s enabledVocables, tyto dva seznamy mají shodnou funkci, stačí nám jeden.
        /// </remarks>
        private static List<Vocable> vocableStack;
        /// <summary>
        /// Fronta špatně zodpovězených slovíček.
        /// </summary>
        private static Queue<Vocable> vocableQueue;
        /// <summary>
        /// Počet datových sloupců v tabulce slovíček (tj. vyjma informací o typech apod.).
        /// </summary>
        private static int itemColumns;

        #endregion

        #region .: Constructor :.

        static Vocabulary()
        {
            random = new Random();
            vocableQueue = new Queue<Vocable>(QUEUE_LEN);
        }

        #endregion

        #region .: Private Methods :.

        #region .: Data Import :.

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

        /// <summary>
        /// Načte skupiny slovíček z excelového listu a uloží je do kolekce.
        /// </summary>
        /// <param name="sheet">Excelovský list, z něhož se čtou skupiny.</param>
        private static void ImportGroups(Worksheet sheet)
        {
            int row, rowCount;
            string key, name;

            rowCount = sheet.UsedRange.Rows.Count;
            GroupsDict = new Dictionary<string, CheckableItem>(rowCount);
            GroupsCollection = new ObservableCollection<CheckableItem>();

            /* procházím řádky a čtu dvojice klíč + hodnota */
            for (row = 2; row <= rowCount; row++)
            {
                /* eliminace prázdných buněk zahrnutých do UsedRange */
                if (sheet.Cells[row, 1].Value == null)
                    break;

                key = sheet.Cells[row, 1].Value.ToString();
                name = sheet.Cells[row, 2].Value.ToString();
                CheckableItem item = new CheckableItem(name);
                item.IsCheckedChanged += GroupItem_IsCheckedChanged;
                GroupsDict.Add(key, item);
                GroupsCollection.Add(item);
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

            TypesCollection = new ObservableCollection<CheckableItem<VocableType>>();

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

                CheckableItem<VocableType> item = new CheckableItem<VocableType>(importedType.Name, importedType);
                item.IsCheckedChanged += TypeItem_IsCheckedChanged;

                TypesCollection.Add(item);
            }
        }

        /// <summary>
        /// Načte slovíčka z excelového listu a uloží je do slovníku.
        /// </summary>
        /// <param name="sheet">Excelovský list, z něhož se čtou slovíčka.</param>
        private static void ImportVocables(Worksheet sheet)
        {
            Vocable importedVocable;
            bool usedRow;
            int row, rowCount, col;
            
            rowCount = sheet.UsedRange.Rows.Count;
            allVocables = new List<Vocable>(rowCount - 1);
            ColumnHeaders = new List<string>(itemColumns);

            /* načtení hlaviček sloupců */
            for (col = 1; col <= itemColumns; col++)
                ColumnHeaders.Add(sheet.Cells[1, col].Value.ToString());

            /* načtení samotných slovíček */
            for (row = 2; row <= rowCount; row++)
            {
                if ( (sheet.Cells[row, itemColumns + COL_ACTIVE_OFFSET].Value != null) &&
                     (!sheet.Cells[row, itemColumns + COL_ACTIVE_OFFSET].Value.ToString().Equals("1")))
                    continue;

                importedVocable = new Vocable(row);
                usedRow = importedVocable.Import(sheet.Rows[row]);
                if (!usedRow)
                    break;
                allVocables.Add(importedVocable);
            }
        }

        #endregion

        /// <summary>
        /// Finds out if the vocable is enabled according to current settings
        /// of enabled types and groups.
        /// </summary>
        /// <param name="vocable">Vocable to be checked.</param>
        /// <returns>Vocable enabled or not.</returns>
        private static bool IsVocableEnabled(Vocable vocable)
        {
            bool isEnabled = false;

            if (vocable.Types.Any(type => type.Data == Vocabulary.EnabledType))
            {
                if (vocable.Groups.Any(group => group.IsChecked))
                {
                    isEnabled = true;
                }
            }

            return isEnabled;
        }

        /// <summary>
        /// Naplní zásobník slovíčky vyhovujícími požadavkům uživatele.
        /// </summary>
        private static void RefillStack()
        {
            var enabledVocables = from vocable in allVocables
                                   where IsVocableEnabled(vocable)
                                   select vocable;

            vocableStack = new List<Vocable>(enabledVocables);

            Trace.WriteLine(String.Format("[zásobník] Naplňuji {0} slovy.",
                vocableStack.Count));            
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Vytáhne slovíčko z fronty špatně zodpovězených a nastaví jako aktuální.
        /// <returns>Slovíčko vytažené z fronty.</returns>
        /// </summary>
        public static Vocable DequeueVocable()
        {
            CurrentVocable = vocableQueue.Dequeue();            
            Trace.WriteLine(String.Format("[fronta] Vytahuji \"{0}\", nyní {1} slov.", 
                CurrentVocable, vocableQueue.Count));
            return CurrentVocable;
        }

        /// <summary>
        /// Vloží chybně zodpovězené slovíčko do fronty.
        /// </summary>
        public static void EnqueueCurrentVocable()
        {
            vocableQueue.Enqueue(CurrentVocable);
            Trace.WriteLine(String.Format("[fronta] Vkládám \"{0}\", nyní {1} slov.", 
                CurrentVocable, vocableQueue.Count));
        }

        /// <summary>
        /// Vytáhne ze zásobníku náhodně vybrané slovíčko, které není ve frontě.
        /// Slovíčko je ze zásobníku odstraněno. Pokud se tím zásobník vyprázdní,
        /// je znovu naplněn.
        /// <returns>Vytažené slovíčko.</returns>
        /// </summary>
        public static Vocable GetNextVocable()
        {
            Vocable candidateVocable;

            while (true)
            {
                candidateVocable = vocableStack[random.Next(vocableStack.Count)];
                if (candidateVocable.Equals(CurrentVocable))
                    continue;
                if (vocableQueue.Contains(candidateVocable))
                    continue;
                break;
            }

            vocableStack.Remove(candidateVocable);
            candidateVocable.Type = EnabledType;
            CurrentVocable = candidateVocable;
            Trace.WriteLine(String.Format("[zásobník] Vytahuji \"{0}\", zbývá {1}.", 
                CurrentVocable, vocableStack.Count));

            if (vocableStack.Count == 0)
            {
                RefillStack();
                Rounds++;
            }

            return candidateVocable;
        }

        /// <summary>
        /// Imports vocables and settings from Excel workbook.
        /// </summary>
        /// <param name="fileName">Name of the Excel file.</param>
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

                TypesCollection[0].IsChecked = true;
                foreach (CheckableItem item in GroupsCollection)
                {
                    item.IsChecked = true;
                }
            }
            finally
            {
                excel.Workbooks.Close();
                excel.Quit();
            }
        }

        /// <summary>
        /// Zahájí zkoušení, tj. vynuluje všechny statistiky a aktualizuje seznam
        /// povolených slovíček.
        /// </summary>
        public static void Start()
        {
            RefillStack();
            Rounds = 0;
        }

        #endregion

        #region .: Event Handlers :.

        /// <summary>
        /// Unchecks all exercise types except of the one just checked (which raised the event)
        /// and triggers stack refill (to reflect the change).
        /// This is to make sure that only one exercise type is checked at a time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void TypeItem_IsCheckedChanged(object sender, EventArgs e)
        {
            foreach (CheckableItem<VocableType> item in TypesCollection)
            {
                if (item != sender)
                {
                    item.IsChecked = false;
                }
                else
                {
                    EnabledType = item.Data;
                }
            }

            //RefillStack();
        }

        /// <summary>
        /// Triggers stack refill, to reflect change of enabled groups.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void GroupItem_IsCheckedChanged(object sender, EventArgs e)
        {
            //RefillStack();
        }

        #endregion

    }
}
