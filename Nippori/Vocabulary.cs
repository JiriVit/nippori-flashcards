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
        /// <summary>
        /// Skupiny.
        /// </summary>
        public static NameValueCollection Groups;
        /// <summary>
        /// Typy.
        /// </summary>
        public static List<VocableType> Types;
        /// <summary>
        /// Nadpisy sloupců v tabulce slovíček, musí se zobrazit na formuláři nad poli
        /// pro zadání odpovědí.
        /// </summary>
        public static List<string> ColumnHeaders;
        /// <summary>
        /// Pole povolených skupin, podle kterých se vybírají slovíčka na zkoušení.
        /// </summary>
        public static string[] EnabledGroups;
        /// <summary>
        /// Povolený typ slovíček, který se teď má zkoušet. Číslováno od jedničky.
        /// </summary>
        public static int EnabledType;

        #endregion

        #region Private Fields

        /// <summary>
        /// Seznam všech slovíček, která byla přečtena z Excelu.
        /// </summary>
        private static List<Vocable> allVocables;
        /// <summary>
        /// Seznam povolených slovíček, tedy podmnožiny, která vyhovuje omezením zadaným uživatelem
        /// (povolený typ a povolené skupiny).
        /// </summary>
        private static List<Vocable> enabledVocables;
        private static Random random;
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

        /// <summary>
        /// Načte skupiny slovíček z excelového listu a uloží je do kolekce.
        /// </summary>
        /// <param name="sheet">Excelovský list, z něhož se čtou skupiny.</param>
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
            allVocables = new List<Vocable>(rowCount - 1);
            ColumnHeaders = new List<string>(itemColumns);

            /* načtení hlaviček sloupců */
            for (col = 1; col <= itemColumns; col++)
                ColumnHeaders.Add(sheet.Cells[1, col].Value.ToString());

            /* načtení samotných slovíček */
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
                    importedVocable.Types = Enumerable.Range(1, Types.Count).ToArray();
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

                allVocables.Add(importedVocable);
            }
        }

        /// <summary>
        /// Zjistí, jestli je slovíčko povolené, tedy v souladu s aktuálně nastavenými
        /// povolenými skupinami a typy.
        /// </summary>
        /// <param name="vocable">Slovíčko k posouzení.</param>
        /// <returns>Je-li slovíčko povolené.</returns>
        private static bool IsVocableEnabled(Vocable vocable)
        {
            if (!(vocable.Types.Contains(EnabledType)))
                return false;

            foreach (string g in EnabledGroups)
                if (vocable.Groups.Contains(g))
                    return true;

            return false;
        }

        /// <summary>
        /// Vybere povolená slovíčka na základě povolených typů a skupin.
        /// </summary>
        private static void SelectEnabledVocables()
        {
            var _enabledVocables = from vocable in allVocables
                                   where IsVocableEnabled(vocable)
                                   select vocable;

            enabledVocables = new List<Vocable>(_enabledVocables);
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

            vocableQueue = new Queue<Vocable>(2);
        }

        /// <summary>
        /// Náhodně vytáhne jedno ze slovíček na zásobníku.
        /// <returns>Vytažené slovíčko.</returns>
        /// </summary>
        public static Vocable GetRandomItem()
        {
            CurrentItem = vocableStack[random.Next(vocableStack.Count)];
            CurrentItem.Type = Types[EnabledType - 1];
            Trace.WriteLine(String.Format("Vytahuji slovo {0}, zbývá jich {1}.", CurrentItem, vocableStack.Count));
            return CurrentItem;
        }

        /// <summary>
        /// Odstraní současné slovíčko ze zásobníku. Volat poté, co je slovíčko správně
        /// zodpovězeno.
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

        /// <summary>
        /// Zjistí počet slovíček v zásobníku.
        /// </summary>
        /// <returns></returns>
        public static int GetListCount()
        {
            return vocableStack.Count;
        }

        /// <summary>
        /// Vloží chybně zodpovězené slovíčko do fronty.
        /// </summary>
        public static void PutToQueue()
        {
            vocableQueue.Enqueue(CurrentItem);
            Trace.WriteLine(String.Format("Dávám do fronty slovo {0}, ve frontě je {1}.", CurrentItem, vocableQueue.Count));
        }

        /// <summary>
        /// Obnoví zásobník slovíček.
        /// </summary>
        public static void ReloadList()
        {
            Trace.WriteLine("Obnovuji seznam.");
            vocableStack.Clear();
            vocableStack.AddRange(enabledVocables);
        }

        /// <summary>
        /// Zjistí, jestli je zásobník prázdný.
        /// </summary>
        /// <returns></returns>
        public static bool IsEmpty()
        {
            return vocableStack.Count == 0;
        }

        /// <summary>
        /// Zjistí, jestli je současné slovíčko ve frontě.
        /// </summary>
        /// <returns></returns>
        public static bool IsItemInQueue()
        {
            return vocableQueue.Contains(CurrentItem);
        }

        /// <summary>
        /// Zahájí zkoušení, tj. vynuluje všechny statistiky a aktualizuje seznam
        /// povolených slovíček.
        /// </summary>
        public static void Start()
        {
            SelectEnabledVocables();
            vocableStack = new List<Vocable>(enabledVocables.Count);
            ReloadList();
        }

        #endregion

    }

    /// <summary>
    /// Třída reprezentuje slovíčko.
    /// </summary>
    public class Vocable
    {

        #region .: Properties :.

        /// <summary>
        /// Čte a zapisuje ID slovíčka, neboli číslo řádku v Excelu.
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// Typ slovíčka.
        /// </summary>
        public VocableType Type { get; set; }
        /// <summary>
        /// Položky.
        /// </summary>
        public string[] Items { get; set; }
        /// <summary>
        /// Čte a zapisuje pole typů slovíčka. Je to údaj vyčtený z Excelu, který znamená,
        /// jaké všechny typy tento jeden záznam podporuje.
        /// </summary>
        public int[] Types { get; set; }
        /// <summary>
        /// Čte a zapisuje pole skupin slovíčka. Je to údaj vyčtený z Excelu, který říká,
        /// do jakých všech skupin toto slovíčko patří.
        /// </summary>
        public string[] Groups { get; set; }
        /// <summary>
        /// Čte zadání slovíčka (např. český překlad, zkouší-li se překlad z češtiny).
        /// </summary>
        public string Input
        {
            get
            {
                return Items[Type.InputColumn - 1];
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

        #endregion

        #region .: Public Methods :.

        public string GetOutput(int index)
        {
            if (index < Type.OutputColumns.Count())
                return Items[Type.OutputColumns[index] - 1];
            else
                return String.Empty;
        }

        public int GetOutputCount()
        {
            return Type.OutputColumns.Count();
        }

        public string GetOutputLabel(int index)
        {
            if (index < Type.OutputColumns.Count())
                return Vocabulary.ColumnHeaders[Type.OutputColumns[index] - 1];
            else
                return String.Empty;
        }

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
