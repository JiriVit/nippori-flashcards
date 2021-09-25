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
using Nippori.Enums;
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
        private bool setupWasChanged;
        private int state = 0;

        #endregion

        #region .: XML Import :.

        XmlDocument xmlDocument;

        #endregion

        #region .: Collections :.

        private Dictionary<string, GroupModel> groupsDict;

        #endregion

        #region .: Training Setup :.

        private bool activeVocablesEnabled = true;
        private bool easyVocablesEnabled = false;
        private bool difficultVocablesEnabled = true;

        private bool jlptKanjiOnly = false;
        private bool jlptKanjiOnlyVisible = false;

        #endregion

        #region .: Debug :.

        private string debugText;

        #endregion

        private List<VocableModel> allVocables = new List<VocableModel>();
        /// <summary>
        /// Stack of vocables to be trained in current round.
        /// </summary>
        private List<VocableModel> vocableStack;
        private Random random = new Random();
        private Language currentLanguage;

        #endregion

        #region .: Properties :.

        #region .: Status :.

        public string StackCount
        {
            get => fileLoaded ? vocableStack.Count.ToString() : "--";
            set => NotifyPropertyChanged("StackCount");
        }
        public string OpenedFileName { get => openedFileName; set { openedFileName = value; NotifyPropertyChanged("OpenedFileName"); } }
        public bool FileLoaded
        {
            get => fileLoaded;
            private set
            {
                fileLoaded = value;
                NotifyPropertyChanged("FileLoaded");
            }
        }

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

        #region .: Training Setup :.

        public bool ActiveVocablesEnabled
        {
            get => activeVocablesEnabled;
            set
            {
                activeVocablesEnabled = value;
                NotifyPropertyChanged("ActiveVocablesEnabled");
                StartTraining();
            }
        }

        public bool EasyVocablesEnabled
        {
            get => easyVocablesEnabled;
            set
            {
                easyVocablesEnabled = value;
                NotifyPropertyChanged("DifficultVocablesEnabled");
                StartTraining();
            }
        }

        public bool DifficultVocablesEnabled
        {
            get => difficultVocablesEnabled;
            set
            {
                difficultVocablesEnabled = value;
                NotifyPropertyChanged("DifficultVocablesEnabled");
                StartTraining();
            }
        }

        public bool JlptKanjiOnly
        {
            get => jlptKanjiOnly;
            set
            {
                if (value != jlptKanjiOnly)
                {
                    jlptKanjiOnly = value;
                    NotifyPropertyChanged("JlptKanjiOnly");
                    StartTraining();
                }
            }
        }

        public bool JlptKanjiOnlyVisible
        {
            get => jlptKanjiOnlyVisible;
            private set
            {
                if (value != jlptKanjiOnlyVisible)
                {
                    jlptKanjiOnlyVisible = value;
                    NotifyPropertyChanged("JlptKanjiOnlyVisible");
                    StartTraining();
                }
            }
        }

        public bool KanjiToEnglishActive
        {
            get => (TypesCollection != null) && (TypesCollection.Count > 0) && TypesCollection[0].IsChecked;
            set
            {
                if ((TypesCollection != null) && (TypesCollection.Count > 0))
                {
                    TypesCollection[0].IsChecked = value;
                    RestartTrainingIfSetupChanged();
                }
            }
        }

        public bool EnglishToKanjiActive
        {
            get => (TypesCollection != null) && (TypesCollection.Count > 1) && TypesCollection[1].IsChecked;
            set
            {
                if ((TypesCollection != null) && (TypesCollection.Count > 1))
                {
                    TypesCollection[1].IsChecked = value;
                    RestartTrainingIfSetupChanged();
                }
            }
        }

        #endregion

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

        /// <summary>
        /// Moves to next vocale in the training set.
        /// </summary>
        public void MoveToNextVocable()
        {
            if (vocableStack.Count > 0)
            {
                if (state == 1)
                {
                    SelectNextVocable();

                    ShowVocable();
                    state = 0;
                }
                else
                {
                    state++;
                }
                UpdateVisibility();
                StackCount = "";
            }
        }

        /// <summary>
        /// Disables current vocable so it won't be examined after stack refill.
        /// </summary>
        public void DisableCurrentVocable()
        {
            CurrentVocable.Enabled = false;
            MoveToNextVocable();
        }

        /// <summary>
        /// Loads vocables from given XML file.
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadVocablesFromXmlFile(string fileName)
        {
            OpenedFileName = fileName;
            ReadXmlFile(OpenedFileName);
            FileLoaded = true;
        }

        /// <summary>
        /// Starts the training.
        /// </summary>
        public void StartTraining()
        {
            CurrentVocable = null;
            state = 0;
            
            RefillStack();
            SelectNextVocable();
            ShowVocable();
            UpdateVisibility();

            StackCount = "";
        }

        /// <summary>
        /// Makes all vocables enabled for training, ie. resets all that were disabled by the user.
        /// </summary>
        public void EnableAllVocables()
        {
            allVocables.ForEach(v => v.Enabled = true);
            StartTraining();
        }

        /// <summary>
        /// Restarts the training, but only if setup was changed since last call of this methods.
        /// This to be called when a submenu with type/group setup is closed.
        /// </summary>
        public void RestartTrainingIfSetupChanged()
        {
            if (setupWasChanged)
            {
                StartTraining();
                setupWasChanged = false;
            }
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
                            JlptKanjiOnlyVisible = (lang == Language.Japanese);
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
                if (!item.ChecksAll && !item.ClearsAll)
                {
                    item.IsChecked = true;
                }
            }

            // clear this flag, we only want it to carry changes perfomed by user
            setupWasChanged = false;
        }

        #endregion

        /// <summary>
        /// Finds next vocable for training and exposes it in <see cref="CurrentVocable"/> property.
        /// The next vocable is chosen randomly from the stack. Previously trained vocable is removed
        /// from the stack. If the stack runs out of vocables, it is refilled.
        /// </summary>
        public void SelectNextVocable()
        {
            VocableModel candidateVocable;

            if (CurrentVocable != null)
            {
                // remove previously trained vocable from the stack
                vocableStack.Remove(CurrentVocable);
            }

            if (vocableStack.Count == 0)
            {
                // we ran out of vocables, need to refill
                RefillStack();
            }

            if (!noVocablesForExamination)
            {
                if (vocableStack.Count > 1)
                {
                    // make sure that the next vocable is not the same as the previous one
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

            bool enabledBySetup =
                ((vocable.Status == VocableStatus.Active) && ActiveVocablesEnabled) ||
                ((vocable.Status == VocableStatus.Easy) && EasyVocablesEnabled) ||
                ((vocable.Status == VocableStatus.Difficult) && DifficultVocablesEnabled);

            return
                vocable.Enabled && 
                enabledBySetup &&
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

        /// <summary>
        /// Determines if a given character is Chinese (hanzi) or Japanese (kanji, hiragana, katakana).
        /// </summary>
        /// <param name="character">Character to be evaluated.</param>
        /// <returns>Boolean result.</returns>
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
            TypeModel typeModel = (TypeModel)sender;

            if (typeModel.IsChecked)
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
            else
            {
                int index = TypesCollection.IndexOf(typeModel);
                if (index == 0)
                {
                    NotifyPropertyChanged("KanjiToEnglishActive");
                }
                if (index == 1)
                {
                    NotifyPropertyChanged("EnglishToKanjiActive");
                }
            }

            setupWasChanged = true;
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
            else if (senderItem.ChecksAll && senderItem.IsChecked)
            {
                GroupsCollection.ToList().ForEach(item => item.IsChecked = (!item.ClearsAll && !item.ChecksAll));
            }
            else
            {
                setupWasChanged = true;
            }
        }

        #endregion
    }
}
