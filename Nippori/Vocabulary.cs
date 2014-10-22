using System;
using System.Collections.Generic;
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
        #region Properties

        /// <summary>
        /// Aktuálně vybrané slovíčko.
        /// </summary>
        public static Vocable CurrentItem;
        /// <summary>
        /// Konfigurace.
        /// </summary>
        public static Dictionary<string, string> Configuration;

        #endregion

        #region Private Fields

        private static Vocable[] internalList;
        private static Dictionary<string, VocableType> vocableTypes;
        private static VocableType[] typeList;
        private static Random random;

        private static List<Vocable> vocableStack;
        private static Queue<Vocable> vocableQueue;

        #endregion

        #region Private Methods

        /// <summary>
        /// Načte typy slovíček z excelového listu a uloží je do slovníku.
        /// </summary>
        /// <param name="sheet">Excelovský list, z něhož se čtou typy.</param>
        private static void ImportTypes(Worksheet sheet)
        {
            int row, rowCount, column;
            List<string> localList;

            rowCount = sheet.UsedRange.Rows.Count;
            localList = new List<string>(5);
            vocableTypes = new Dictionary<string, VocableType>(rowCount - 1);

            /* procházím řádky a čtu typy */
            for (row = 2; row <= rowCount; row++)
            {
                localList.Clear();

                /* procházím sloupce a čtu jména překladů */
                for (column = 2; column <= 6; column++)
                {
                    if (sheet.Cells[row, column].Value != null)
                        localList.Add(sheet.Cells[row, column].Value.ToString());
                    else
                        break;
                }
                
                /* ze seznamu jmen překladů vytvořím nový typ a založím ho do slovníku */
                vocableTypes.Add(sheet.Cells[row, 1].Value.ToString(), new VocableType(localList.ToArray()));
            }
        }

        /// <summary>
        /// Načte slovíčka z excelového listu a uloží je do slovníku.
        /// </summary>
        /// <param name="sheet">Excelovský list, z něhož se čtou slovíčka.</param>
        private static void ImportVocables(Worksheet sheet)
        {
            int row, rowCount, column;
            string czech, typeId;
            List<Vocable> localList;
            List<string> transList;

            rowCount = sheet.UsedRange.Rows.Count;
            localList = new List<Vocable>(rowCount);
            transList = new List<string>(5);

            /* procházím řádky a čtu slovíčka */
            for (row = 2; row <= rowCount; row++)
            {
                /* je-li slovíčko vůbec aktivní */
                if ((null != sheet.Cells[row, 8].Value) && 
                    (!sheet.Cells[row, 8].Value.ToString().Equals("1")))
                    continue;
                
                czech = sheet.Cells[row, 1].Value.ToString();
                if (null == sheet.Cells[row, 7].Value)
                    typeId = "1";
                else
                    typeId = sheet.Cells[row, 7].Value.ToString();                
                transList.Clear();

                /* procházím sloupce a čtu překlady */
                for (column = 2; column <= 6; column++)
                {
                    if (sheet.Cells[row, column].Value != null)
                        transList.Add(sheet.Cells[row, column].Value.ToString());
                    else
                        break;
                }

                /* ze všech údajů vytvořím nové slovíčko a založím ho do seznamu */
                localList.Add(new Vocable(vocableTypes[typeId], czech, transList.ToArray()));
            }

            internalList = localList.ToArray();
            Trace.WriteLine(String.Format("Načítám {0} slovíček ze souboru.", localList.Count));
        }

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
                Configuration.Add(sheet.Cells[row, 1].Value.ToString(), sheet.Cells[row, 2].Value.ToString());
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
                ImportTypes(excel.ActiveWorkbook.Worksheets[2]);
                ImportVocables(excel.ActiveWorkbook.Worksheets[1]);
                ImportConfiguration(excel.ActiveWorkbook.Worksheets[3]);
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

            types.Add(new VocableType(new string[] {"Česky", "Deutsch"}));
            types.Add(new VocableType(new string[] {"Česky", "Singular", "Plural"}));

            typeList = types.ToArray();

            list.Add(new Vocable(typeList[0], "rád", new string[] {"gern"}));
            list.Add(new Vocable(typeList[0], "přirozeně", new string[] {"natürlich"}));

            internalList = list.ToArray();

            vocableStack = new List<Vocable>(internalList);
            vocableQueue = new Queue<Vocable>(2);
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
            vocableStack.AddRange(internalList);
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

        /// <summary>
        /// Typ slovíčka.
        /// </summary>
        public VocableType Type { get; set; }
        /// <summary>
        /// Český překlad.
        /// </summary>
        public string Czech { get; set; }
        /// <summary>
        /// Cizí překlady.
        /// </summary>
        public string[] Translation { get; set; }

        #endregion

        #region .: Constructor :.

        /// <summary>
        /// Vytvoří novou instanci třídy Vocable.
        /// </summary>
        /// <param name="type">Typ slovíčka.</param>
        /// <param name="czech">Český překlad.</param>
        /// <param name="translation">Cizí překlady.</param>
        public Vocable(VocableType type, string czech, string[] translation)
        {
            this.Type = type;
            this.Czech = czech;
            this.Translation = (string[])translation.Clone();
        }

        #endregion

        #region .: Public Overriden Methods :.

        public override string ToString()
        {
            return String.Format("'{0}'", Czech);
        }

        public override bool Equals(object obj)
        {
            if (obj is Vocable)
                return ((Vocable)obj).Czech.Equals(this.Czech);
            else
                return false;
        }

        #endregion
    }

    /// <summary>
    /// Reprezentuje typ slovíčka, neboli seznam jmen různých překladů.
    /// </summary>
    public class VocableType
    {
        /// <summary>
        /// Název zadání.
        /// </summary>
        public string InputName { get; set; }

        /// <summary>
        /// Pole jmen překladů.
        /// </summary>
        public string[] Name { get; set; }

        /// <summary>
        /// Konstruktor, vytvoří nový typ z předaného pole jmen.
        /// </summary>
        /// <param name="names">Pole jmen překladů, na prvním místě je jméno zadání.</param>
        public VocableType(string[] names)
        {
            InputName = names[0];
            Name = new string[names.Length - 1];
            Array.Copy(names, 1, Name, 0, names.Length - 1);
        }
    }

}
