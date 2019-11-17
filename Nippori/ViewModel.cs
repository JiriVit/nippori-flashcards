using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

using Nippori.Vocables;

namespace Nippori
{
    public class ViewModel : INotifyPropertyChanged
    {
        #region .: Constants :.

        private const int ColOffsetActive = 3;

        #endregion

        #region .: Private Fields :.

        #region .: Backing for properties :.

        #endregion

        #region .: Status :.

        private bool fileLoaded;

        #endregion

        #region .: XML Import :.

        XmlDocument xmlDocument;

        #endregion

        private string debugText;

        private int rounds = 0;

        private Visibility progressBarVisibility = Visibility.Hidden;
        private string openedFileName;

        private ObservableCollection<CheckableItem<VocableType>> types;
        private ObservableCollection<CheckableItem> groups;

        private int state = 0;

        // this is migrated from Vocabulary class, to be ordered later

        private List<Vocable> allVocables = new List<Vocable>();
        private List<Vocable> vocableStack;

        private Random random = new Random();

        #endregion

        #region .: Properties :.

        public string DebugText { get { return debugText; } set { debugText = value; NotifyPropertyChanged("DebugText"); } }

        public string[] Fields { get; } = new string[4];
        public Visibility[] FieldsVisibility { get; } = new Visibility[4];
        public bool[] FieldsEmphasized { get; } = new bool[4];

        public int Rounds
        {
            get => rounds;
            set { rounds = value; NotifyPropertyChanged("Rounds"); }
        }
        public string StackCount { get => fileLoaded ? vocableStack.Count.ToString() : "--"; }

        public string OpenedFileName { get { return openedFileName; } set { openedFileName = value; NotifyPropertyChanged("OpenedFileName"); } }
        public Visibility ProgressBarVisibility { get { return progressBarVisibility; } set { progressBarVisibility = value; NotifyPropertyChanged("ProgressBarVisibility"); } }

        public ObservableCollection<CheckableItem<VocableType>> Types { get { return types; } set { types = value; NotifyPropertyChanged("Types"); } }
        public ObservableCollection<CheckableItem> Groups { get { return groups; } set { groups = value; NotifyPropertyChanged("Groups"); } }

        #region .: Migrated from Vocabulary class :.

        /// <summary>
        /// Observable collection of supported vocable types.
        /// </summary>
        public ObservableCollection<CheckableItem<VocableType>> TypesCollection { get; set; }
        /// <summary>
        /// Observable collection of supported vocable groups.
        /// </summary>
        public ObservableCollection<CheckableItem> GroupsCollection { get; set; }
        public Dictionary<string, CheckableItem> GroupsDict;
        public Vocable CurrentVocable { get; set; }

        #endregion

        public VocableType EnabledType { get; set; }

        #region .: Debug & Test :.

        public bool TestButtonVisible { get; private set; } = false;

        #endregion

        #endregion

        #region .: Constructor :.

        public ViewModel()
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
        }

        /// <summary>
        /// Imports group definitions from node 'groups' in the XML document and stores them
        /// to <see cref="GroupsCollection"/> and <see cref="GroupsDict"/>.
        /// </summary>
        private void ImportGroupDefinitions()
        {
            XmlNode groupsNode = xmlDocument.GetElementsByTagName("groups")[0];

            // create special first item which works as a command for clearing selections
            CheckableItem groupClearAll = new CheckableItem("Clear all")
            {
                ClearsAll = true,
            };
            CheckableItem groupCheckAll = new CheckableItem("Select all")
            {
                ChecksAll = true,
            };
            groupClearAll.IsCheckedChanged += GroupItem_IsCheckedChanged;
            groupCheckAll.IsCheckedChanged += GroupItem_IsCheckedChanged;

            // initialize dictionary and observable collection
            GroupsDict = new Dictionary<string, CheckableItem>(groupsNode.ChildNodes.Count);
            GroupsCollection = new ObservableCollection<CheckableItem>
            {
                groupClearAll,
                groupCheckAll
            };

            // fill in the groups from the XML file
            foreach (XmlNode child in groupsNode.ChildNodes)
            {
                CheckableItem item = new CheckableItem(child.Attributes["name"].Value);
                item.IsCheckedChanged += GroupItem_IsCheckedChanged;

                GroupsDict.Add(child.Attributes["key"].Value, item);
                GroupsCollection.Add(item);
            }
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
                    break;
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
                    vocable.Groups = new List<CheckableItem>(new CheckableItem[] { GroupsCollection[0] });
                }
                else
                {
                    List<string> groupKeys = new List<string>(groups.Split(';'));
                    vocable.Groups = new List<CheckableItem>(groupKeys.Count);
                    groupKeys.ForEach(key => vocable.Groups.Add(GroupsDict[key]));
                }

                allVocables.Add(vocable);
            }

            TypesCollection[0].IsChecked = true;
            foreach (CheckableItem item in GroupsCollection)
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

            Types = TypesCollection;
            Groups = GroupsCollection;

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
            if ( (vocableStack.Count > 0) &&
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
            CheckableItem senderItem = (CheckableItem)sender;

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

        #region .: INotifyPropertyChanged :.

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
