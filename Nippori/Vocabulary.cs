﻿using System;
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

        public static int ItemColumnCount { get { return itemColumns; } }

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

        #region Private Methods

        #region ... Data Import

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

            if (vocable.Types.Contains(EnabledType))
            {
                if ((EnabledGroups == null) || (EnabledGroups.Length == 0))
                {
                    isEnabled = true;
                }
                else
                {
                    foreach (string g in EnabledGroups)
                    {
                        if (vocable.Groups.Contains(g))
                        {
                            isEnabled = true;
                        }
                    }
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
            candidateVocable.Type = Types[EnabledType - 1];
            CurrentVocable = candidateVocable;
            Trace.WriteLine(String.Format("[zásobník] Vytahuji \"{0}\", zbývá {1}.", 
                CurrentVocable, vocableStack.Count));

            if (vocableStack.Count == 0)
                RefillStack();

            return candidateVocable;
        }

        /// <summary>
        /// Inicializace slovníku.
        /// </summary>
        public static void Init()
        {
            List<VocableType> types = new List<VocableType>();
            List<Vocable> list = new List<Vocable>();

            random = new Random();

            vocableQueue = new Queue<Vocable>(QUEUE_LEN);
        }

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
        /// Typ slovíčka.
        /// </summary>
        public VocableType Type { get; set; }
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
                Types = Enumerable.Range(1, Vocabulary.Types.Count).ToArray();
            }
            else
            {
                items = excelRow.Cells[1, Vocabulary.ItemColumnCount + Vocabulary.COL_TYPES_OFFSET].Value.ToString().Split(';');
                Types = items.Select(Int32.Parse).ToArray();
            }
            
            /* načtení skupin */
            if (excelRow.Cells[1, Vocabulary.ItemColumnCount + Vocabulary.COL_GROUPS_OFFSET].Value == null)
                Groups = new string[] { Vocabulary.Groups.Keys[0] };
            else
                Groups = excelRow.Cells[1, Vocabulary.ItemColumnCount + Vocabulary.COL_GROUPS_OFFSET].Value.ToString().Split(';');

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
