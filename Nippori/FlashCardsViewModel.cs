using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

using Nippori.Bases;
using Nippori.Vocables;

namespace Nippori
{
    public class FlashCardsViewModel : ModelBase
    {
        #region .: Constants :.

        private const int ColOffsetActive = 3;

        /// <summary>
        /// Array with supported languages.
        /// </summary>
        private readonly Language[] SupportedLanguages = new Language[]
        {
            Language.Chinese,
            Language.Japanese,
        };

        #endregion

        #region .: Nested Classes :.

        /// <summary>
        /// Represents a language being examined, holds configuration specific for the language.
        /// </summary>
        public class Language
        {
            #region .: Constants :.

            public static readonly Language Japanese = new Language()
            {
                Name = "Japanese",
                Key = "jp",
            };

            public static readonly Language Chinese = new Language()
            {
                Name = "Chinese",
                Key = "cn",
            };

            #endregion

            #region .: Properties :.

            /// <summary>
            /// Gets name of the language.
            /// </summary>
            public string Name { get; private set; }
            /// <summary>
            /// Gets key, i.e. shortcut of the language, defined in Excel sheet.
            /// </summary>
            public string Key { get; private set; }

            #endregion
        }

        #endregion

        #region .: Private Fields :.

        #region .: Status :.

        private string openedFileName;
        private bool fileLoaded;
        private int rounds = 0;
        private int state = 0;

        #endregion

        #region .: XML Import :.

        XmlDocument xmlDocument;

        #endregion

        #region .: Collections :.

        private Dictionary<string, CheckableItemBase> groupsDict;

        #endregion

        #region .: Debug :.

        private string debugText;

        #endregion

        private Visibility progressBarVisibility = Visibility.Hidden;
        private List<Vocable> allVocables = new List<Vocable>();
        private List<Vocable> vocableStack;
        private Random random = new Random();
        private Language currentLanguage;

        #endregion

        #region .: Properties :.

        #region .: Status :.

        public string StackCount { get => fileLoaded ? vocableStack.Count.ToString() : "--"; }
        public int Rounds { get => rounds; set { rounds = value; NotifyPropertyChanged("Rounds"); } }
        public string OpenedFileName { get => openedFileName; set { openedFileName = value; NotifyPropertyChanged("OpenedFileName"); } }

        #endregion

        #region .: Collections :.

        /// <summary>
        /// Observable collection of supported vocable groups.
        /// </summary>
        public ObservableCollection<CheckableItemBase> GroupsCollection { get; private set; }

        /// <summary>
        /// Observable collection of supported vocable types.
        /// </summary>
        public ObservableCollection<CheckableItem<VocableType>> TypesCollection { get; private set; }

        #endregion

        #region .: Debug :.

        public string DebugText { get => debugText; set { debugText = value; NotifyPropertyChanged("DebugText"); } }
        public bool TestButtonVisible { get; private set; } = false;

        #endregion

        public string[] Fields { get; } = new string[4];
        public Visibility[] FieldsVisibility { get; } = new Visibility[4];
        public bool[] FieldsEmphasized { get; } = new bool[4];
        public Visibility ProgressBarVisibility { get { return progressBarVisibility; } set { progressBarVisibility = value; NotifyPropertyChanged("ProgressBarVisibility"); } }
        public Vocable CurrentVocable { get; set; }
        public VocableType EnabledType { get; set; }

        #endregion

        #region .: Constructor :.

        public FlashCardsViewModel()
        {
#if DEBUG
            TestButtonVisible = true;
#endif
        }

        #endregion

        #region .: Public Methods :.

        public void Confirm()
        {
            if (vocableStack.Count > 0)
            {
                if (state == 1)
                {
                    GetNextVocable();

                    ShowVocable();
                    state = 0;
                }
                else
                {
                    state++;
                }
                UpdateVisibility();
                NotifyPropertyChanged("StackCount");
            }
        }

        public void Reject()
        {
            // rejecting temporarily removed, to be added later and better
            Confirm();
        }

        public void Disable()
        {
            CurrentVocable.Active = false;
            Confirm();
        }

        public void OpenFile(string fileName)
        {
            OpenedFileName = fileName;
            new Task(() => LoadDataTaskFunc()).Start();
        }

        public void StartExam()
        {
            VocabularyStart();
            GetNextVocable();
            ShowVocable();
            UpdateVisibility();
            NotifyPropertyChanged("StackCount");
        }

        /// <summary>
        /// Invokes a test procedure.
        /// </summary>
        public void Test()
        {
        }

        #endregion

        #region .: Private Methods :.

        #region .: XML Import :.

        /// <summary>
        /// Imports vocable type definitions from node 'types' in the XML document and stores them
        /// to <see cref="TypesCollection"/>.
        /// </summary>
        private void ImportVocableTypeDefinitions()
        {
            XmlNode typesNode = xmlDocument.GetElementsByTagName("types")[0];
            TypesCollection = new ObservableCollection<CheckableItem<VocableType>>();
            foreach (XmlNode child in typesNode.ChildNodes)
            {
                VocableType type = new VocableType(child);

                CheckableItem<VocableType> item = new CheckableItem<VocableType>(type.Name, type);
                item.IsCheckedChanged += TypeItem_IsCheckedChanged;

                TypesCollection.Add(item);
            }

            NotifyPropertyChanged("TypesCollection");
        }

        /// <summary>
        /// Imports group definitions from node 'groups' in the XML document and stores them
        /// to <see cref="GroupsCollection"/> and <see cref="groupsDict"/>.
        /// </summary>
        private void ImportGroupDefinitions()
        {
            XmlNode groupsNode = xmlDocument.GetElementsByTagName("groups")[0];

            // initialize dictionary and observable collection
            groupsDict = new Dictionary<string, CheckableItemBase>(groupsNode.ChildNodes.Count);
            GroupsCollection = new ObservableCollection<CheckableItemBase>
            {
                new CheckableItemBase("Clear all") { ClearsAll = true },
                new CheckableItemBase("Select all") { ChecksAll = true },
            };

            // fill in the groups from the XML file
            foreach (XmlNode child in groupsNode.ChildNodes)
            {
                CheckableItemBase item = new CheckableItemBase(child.Attributes["name"].Value);

                groupsDict.Add(child.Attributes["key"].Value, item);
                GroupsCollection.Add(item);
            }

            // register event handler for each item
            foreach (CheckableItemBase item in GroupsCollection)
            {
                item.IsCheckedChanged += GroupItem_IsCheckedChanged;
            }

            NotifyPropertyChanged("GroupsCollection");
        }

        /// <summary>
        /// Imports vocables and settings from an XML file.
        /// </summary>
        /// <param name="path">Path to the XML file.</param>
        private void ReadXmlFile(string path)
        {
            XmlNode node;
            int fieldsCount = 0;

            xmlDocument = new XmlDocument();
            xmlDocument.Load(path);

            /* import configuration */
            node = xmlDocument.GetElementsByTagName("config")[0];
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Attributes["key"].Value.Equals("columns"))
                {
                    fieldsCount = int.Parse(child.Attributes["value"].Value);
                }
                if (child.Attributes["key"].Value.Equals("lang"))
                {
                    string key = child.Attributes["value"].Value;
                    foreach (Language lang in SupportedLanguages)
                    {
                        if (lang.Key.Equals(key))
                        {
                            currentLanguage = lang;
                            break;
                        }
                    }
                }
            }

            ImportVocableTypeDefinitions();
            ImportGroupDefinitions();

            /* import vocables */
            node = xmlDocument.GetElementsByTagName("vocabulary")[0];
            foreach (XmlNode child in node.ChildNodes)
            {
                Vocable vocable = new Vocable(child, fieldsCount);

                // assign types
                string types = child.Attributes[$"field{fieldsCount + 1}"].Value;
                if (types.Equals(string.Empty))
                {
                    // no type defined -> assign all of them
                    vocable.AllowedTypes = new List<CheckableItem<VocableType>>(TypesCollection);
                }
                else
                {
                    List<string> typeNumbers = new List<string>(types.Split(';'));
                    vocable.AllowedTypes = new List<CheckableItem<VocableType>>(typeNumbers.Count);
                    typeNumbers.ForEach(typeNumber => vocable.AllowedTypes.Add(TypesCollection[int.Parse(typeNumber) - 1]));
                }

                // assign groups
                string groups = child.Attributes[$"field{fieldsCount + 2}"].Value;
                if (groups.Equals(string.Empty))
                {
                    // no group defined -> assign the first one
                    vocable.Groups = new List<CheckableItemBase>(new CheckableItemBase[] { GroupsCollection[0] });
                }
                else
                {
                    List<string> groupKeys = new List<string>(groups.Split(';'));
                    vocable.Groups = new List<CheckableItemBase>(groupKeys.Count);
                    groupKeys.ForEach(key => vocable.Groups.Add(groupsDict[key]));
                }

                allVocables.Add(vocable);
            }

            TypesCollection[0].IsChecked = true;
            foreach (CheckableItemBase item in GroupsCollection)
            {
                item.IsChecked = true;
            }
        }

        #endregion

        #region .: Migrated from Vocabulary class :.

        public void GetNextVocable()
        {
            Vocable candidateVocable;

            if (vocableStack.Count > 0)
            {
                while (true)
                {
                    candidateVocable = vocableStack[random.Next(vocableStack.Count)];
                    if (candidateVocable.Equals(CurrentVocable))
                        continue;
                    break;
                }

                vocableStack.Remove(candidateVocable);
                CurrentVocable = candidateVocable;

                if (vocableStack.Count == 0)
                {
                    RefillStack();
                    Rounds++;
                }
            }
        }

        /// <summary>
        /// Finds out if the vocable is enabled according to current settings
        /// of enabled types and groups.
        /// </summary>
        /// <param name="vocable">Vocable to be checked.</param>
        /// <returns>Vocable enabled or not.</returns>
        private bool IsVocableEnabled(Vocable vocable)
        {
            return 
                vocable.Active && 
                vocable.IsType(EnabledType) && 
                (vocable.Groups.Any(group => group.IsChecked));
        }

        private void RefillStack()
        {
            var enabledVocables = from vocable in allVocables
                                  where IsVocableEnabled(vocable)
                                  select vocable;

            vocableStack = new List<Vocable>(enabledVocables);
        }

        public void VocabularyStart()
        {
            RefillStack();
            Rounds = 0;
        }

        #endregion

        private void LoadDataTaskFunc()
        {
            ProgressBarVisibility = Visibility.Visible;
            ReadXmlFile(OpenedFileName);
            ProgressBarVisibility = Visibility.Hidden;

            fileLoaded = true;
            StartExam();
        }

        private void ShowVocable()
        {
            if (vocableStack.Count > 0)
            {
                int fieldsCount = EnabledType.InputColumns.Length + EnabledType.OutputColumns.Length;

                for (int i = 0; i < Fields.Length; i++)
                {
                    if (i < fieldsCount)
                    {
                        if (i < EnabledType.InputColumns.Length)
                        {
                            Fields[i] = CurrentVocable.Fields[EnabledType.InputColumns[i] - 1];
                        }
                        else
                        {
                            Fields[i] = CurrentVocable.Fields[EnabledType.OutputColumns[i - EnabledType.InputColumns.Length] - 1];
                        }
                    }
                    else if (i == (Fields.Length - 1))
                    {
                        Fields[i] = CurrentVocable.Fields[CurrentVocable.Fields.Length - 1];
                    }
                    else
                    {
                        Fields[i] = string.Empty;
                    }
                }
            }
            else
            {
                Fields[0] = "(no vocables in selected set)";
                Fields[1] = Fields[2] = Fields[3];
            }

            NotifyPropertyChanged("Fields");
        }
        
        private void UpdateVisibility()
        {
            int visibleFields;

            if (vocableStack.Count > 0)
            {
                if (state == 0)
                {
                    visibleFields = EnabledType.InputColumns.Length;
                    FieldsVisibility[FieldsVisibility.Length - 1] = Visibility.Hidden;
                }
                else
                {
                    visibleFields = EnabledType.InputColumns.Length + EnabledType.OutputColumns.Length;
                    FieldsVisibility[FieldsVisibility.Length - 1] = Visibility.Visible;
                }

                for (int i = 0; i < (FieldsVisibility.Length - 1); i++)
                {
                    FieldsVisibility[i] = (i < visibleFields) ? Visibility.Visible : Visibility.Hidden;
                }
            }
            else
            {
                FieldsVisibility[0] = Visibility.Visible;
                FieldsVisibility[1] = FieldsVisibility[2] = FieldsVisibility[3] = Visibility.Hidden;
            }

            NotifyPropertyChanged("FieldsVisibility");

            // nasty hack for emphasizing kanji
            // TODO Rework this so the field to be emphasized can be defined in Excel, not here.
            if (currentLanguage.Equals(Language.Japanese))
            {
                if ((vocableStack.Count > 0) &&
                     (TypesCollection[0].IsChecked) &&
                     CurrentVocable.IsType(TypesCollection[2].Data)
                   )
                {
                    FieldsEmphasized[1] = true;
                }
                else
                {
                    FieldsEmphasized[1] = false;
                }

                NotifyPropertyChanged("FieldsEmphasized");
            }
        }

        #region .: Debug :.

        #endregion

        #endregion

        #region .: Event Handlers :.

        /// <summary>
        /// Unchecks all exercise types except of the one just checked (which raised the event)
        /// and triggers stack refill (to reflect the change).
        /// This is to make sure that only one exercise type is checked at a time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TypeItem_IsCheckedChanged(object sender, EventArgs e)
        {
            CheckableItem<VocableType> senderItem = (CheckableItem<VocableType>)sender;

            if (senderItem.IsChecked)
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
            }
        }

        /// <summary>
        /// Triggers stack refill, to reflect change of enabled groups.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupItem_IsCheckedChanged(object sender, EventArgs e)
        {
            CheckableItemBase senderItem = (CheckableItemBase)sender;

            // check for special 'group' item which is actually a command to clear selection 
            if (senderItem.ClearsAll && senderItem.IsChecked)
            {
                GroupsCollection.ToList().ForEach(item => item.IsChecked = false);
            }
            if (senderItem.ChecksAll && senderItem.IsChecked)
            {
                GroupsCollection.ToList().ForEach(item => item.IsChecked = (!item.ClearsAll && !item.ChecksAll));
            }
        }

        #endregion
    }
}
