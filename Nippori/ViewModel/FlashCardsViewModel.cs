using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml;

using Nippori.Bases;
using Nippori.Japanese;
using Nippori.Model;

namespace Nippori.ViewModel
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
                //FontFamilyForSigns = new FontFamily("Yu Mincho"),
                FontFamilyForSigns = new FontFamily("Yu Gothic UI SemiLight"),
            };

            public static readonly Language Chinese = new Language()
            {
                Name = "Chinese",
                Key = "cn",
                //FontFamilyForSigns = new FontFamily("SimSun"),
                FontFamilyForSigns = new FontFamily("Microsoft YaHei UI Light"),
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
            /// <summary>
            /// Gets font family to be used for signs.
            /// </summary>
            public FontFamily FontFamilyForSigns { get; private set; }

            #endregion
        }

        #endregion

        #region .: Private Fields :.

        #region .: Status :.

        private string openedFileName;
        private bool fileLoaded;
        private bool noVocablesForExamination = true;
        private int rounds = 0;
        private int state = 0;

        #endregion

        #region .: XML Import :.

        XmlDocument xmlDocument;

        #endregion

        #region .: Collections :.

        private Dictionary<string, GroupModel> groupsDict;

        #endregion

        #region .: Debug :.

        private string debugText;

        #endregion

        private Visibility progressBarVisibility = Visibility.Hidden;
        private List<VocableModel> allVocables = new List<VocableModel>();
        private List<VocableModel> vocableStack;
        private Random random = new Random();
        private Language currentLanguage;
        private bool jlptKanjiOnly = false;

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
        public ObservableCollection<GroupModel> GroupsCollection { get; private set; }

        /// <summary>
        /// Observable collection of supported vocable types.
        /// </summary>
        public ObservableCollection<TypeModel> TypesCollection { get; private set; }

        #endregion

        #region .: Debug :.

        public string DebugText { get => debugText; set { debugText = value; NotifyPropertyChanged("DebugText"); } }
        public bool TestButtonVisible { get; private set; } = false;

        #endregion

        #region .: Fields :.

        public string[] Fields { get; } = new string[4];
        public Visibility[] FieldsVisibility { get; } = new Visibility[4];
        public bool[] FieldsEmphasized { get; } = new bool[4];
        public FontFamily[] FieldFontFamily { get; } = new FontFamily[4];

        #endregion

        #region .: Settings :.

        public bool JlptKanjiOnly
        {
            get => jlptKanjiOnly;
            set
            {
                if (value != jlptKanjiOnly)
                {
                    jlptKanjiOnly = value;
                    NotifyPropertyChanged("JlptKanjiOnly");
                    StartExam();
                }
            }
        }

        #endregion

        public Visibility ProgressBarVisibility { get { return progressBarVisibility; } set { progressBarVisibility = value; NotifyPropertyChanged("ProgressBarVisibility"); } }
        public VocableModel CurrentVocable { get; set; }
        public TypeModel EnabledType { get; set; }

        #endregion

        #region .: Constructor :.

        public FlashCardsViewModel()
        {
#if DEBUG
            TestButtonVisible = true;
#endif

            for (int i = 0; i < FieldFontFamily.Length; i++)
            {
                FieldFontFamily[i] = SystemFonts.MessageFontFamily;
            }
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

        /// <summary>
        /// Disables current vocable so it won't be examined after stack refill.
        /// </summary>
        public void DisableCurrentVocable()
        {
            CurrentVocable.Enabled = false;
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

        public void EnableAllVocables()
        {
            allVocables.ForEach(v => v.Enabled = true);
            StartExam();
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
            TypesCollection = new ObservableCollection<TypeModel>();
            foreach (XmlNode child in typesNode.ChildNodes)
            {
                TypeModel type = new TypeModel(child);

                type.IsCheckedChanged += TypeItem_IsCheckedChanged;
                TypesCollection.Add(type);
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
            groupsDict = new Dictionary<string, GroupModel>(groupsNode.ChildNodes.Count);
            GroupsCollection = new ObservableCollection<GroupModel>
            {
                new GroupModel("Clear all") { ClearsAll = true },
                new GroupModel("Select all") { ChecksAll = true },
            };

            // fill in the groups from the XML file
            foreach (XmlNode child in groupsNode.ChildNodes)
            {
                GroupModel item = new GroupModel(child.Attributes["name"].Value);

                groupsDict.Add(child.Attributes["key"].Value, item);
                GroupsCollection.Add(item);
            }

            // register event handler for each item
            foreach (GroupModel item in GroupsCollection)
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
                VocableModel vocable = new VocableModel(child, fieldsCount);

                // assign types
                string types = child.Attributes[$"field{fieldsCount + 1}"].Value;
                if (types.Equals(string.Empty))
                {
                    // no type defined -> assign all of them
                    vocable.Types = new List<TypeModel>(TypesCollection);
                }
                else
                {
                    List<string> typeNumbers = new List<string>(types.Split(';'));

                    vocable.Types = new List<TypeModel>(typeNumbers.Count);
                    typeNumbers.ForEach(typeNumber => vocable.Types.Add(TypesCollection[int.Parse(typeNumber) - 1]));
                }

                // assign groups
                string groups = child.Attributes[$"field{fieldsCount + 2}"].Value;
                if (groups.Equals(string.Empty))
                {
                    // no group defined -> assign the first one
                    vocable.Groups = new List<GroupModel>(new GroupModel[] { GroupsCollection[0] });
                }
                else
                {
                    List<string> groupKeys = new List<string>(groups.Split(';'));
                    vocable.Groups = new List<GroupModel>(groupKeys.Count);
                    groupKeys.ForEach(key => vocable.Groups.Add(groupsDict[key]));
                }

                allVocables.Add(vocable);
            }

            TypesCollection[0].IsChecked = true;
            foreach (GroupModel item in GroupsCollection)
            {
                item.IsChecked = true;
            }
        }

        #endregion

        #region .: Migrated from Vocabulary class :.

        public void GetNextVocable()
        {
            VocableModel candidateVocable;

            if (CurrentVocable != null)
            {
                vocableStack.Remove(CurrentVocable);
            }

            if (vocableStack.Count == 0)
            {
                RefillStack();
                Rounds++;
            }

            if (!noVocablesForExamination)
            {
                if (vocableStack.Count > 1)
                {
                    while (true)
                    {
                        candidateVocable = vocableStack[random.Next(vocableStack.Count)];
                        if (candidateVocable.Equals(CurrentVocable))
                            continue;
                        break;
                    }
                }
                else
                {
                    candidateVocable = vocableStack[0];
                }
                CurrentVocable = candidateVocable;
            }
            else
            {
                CurrentVocable = null;
            }
        }

        /// <summary>
        /// Finds out if the vocable is enabled according to current settings
        /// of enabled types and groups.
        /// </summary>
        /// <param name="vocable">Vocable to be checked.</param>
        /// <returns>Vocable enabled or not.</returns>
        private bool IsVocableEnabled(VocableModel vocable)
        {
            bool jlptKanjiRestriction = false;

            if ((currentLanguage == Language.Japanese) && JlptKanjiOnly)
            {
                jlptKanjiRestriction = !vocable.Fields[2].Any(c => JlptUtils.IsKanjiUpToJlptLevel(c, JlptLevels.N4));
            }

            return 
                vocable.Enabled && 
                vocable.IsType(EnabledType) && 
                !jlptKanjiRestriction &&
                (vocable.Groups.Any(group => group.IsChecked));
        }

        private void RefillStack()
        {
            var enabledVocables = from vocable in allVocables
                                  where IsVocableEnabled(vocable)
                                  select vocable;

            vocableStack = new List<VocableModel>(enabledVocables);
            noVocablesForExamination = (vocableStack.Count == 0);
        }

        public void VocabularyStart()
        {
            CurrentVocable = null;
            RefillStack();
            Rounds = 0;
        }

        #endregion

        private bool IsAsianCharacter(char character)
        {
            bool isAsian = false;

            string[] regexs =
            {
                @"\p{IsCJKUnifiedIdeographs}",
                @"\p{IsKatakana}",
                @"\p{IsHiragana}",
            };

            foreach (string rx in regexs)
            {
                if (Regex.IsMatch(character.ToString(), rx))
                {
                    isAsian = true;
                    break;
                }
            }

            return isAsian;
        }

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
            if (!noVocablesForExamination)
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

                    if (Fields[i].Any(c => IsAsianCharacter(c)))
                    {
                        FieldFontFamily[i] = currentLanguage.FontFamilyForSigns;
                    }
                    else
                    {
                        FieldFontFamily[i] = SystemFonts.MessageFontFamily;
                    }
                }
            }
            else
            {
                Fields[0] = "(no vocables in selected set)";
                Fields[1] = Fields[2] = Fields[3] = string.Empty;
            }

            NotifyPropertyChanged("Fields");
            NotifyPropertyChanged("FieldFontFamily");
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
            if (currentLanguage.Equals(Language.Japanese))
            {
                if ((vocableStack.Count > 0) &&
                     (TypesCollection[0].IsChecked) &&
                     CurrentVocable.Fields[2].Any(c => JlptUtils.IsKanjiUpToJlptLevel(c, JlptLevels.N4))
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
            TypeModel senderItem2 = (TypeModel)sender;

            if (senderItem2.IsChecked)
            {
                foreach (TypeModel item in TypesCollection)
                {
                    if (item != sender)
                    {
                        item.IsChecked = false;
                    }
                    else
                    {
                        EnabledType = item;
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
            GroupModel senderItem = (GroupModel)sender;

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
