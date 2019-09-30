﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

using NipporiWpf.Vocables;

namespace NipporiWpf
{
    public class ViewModel : INotifyPropertyChanged
    {
        #region .: Constants :.

        private const int ColOffsetActive = 3;

        #endregion

        #region .: Private Fields :.

        #region .: Backing for properties :.

        private string question;
        private string answer1;
        private string answer2;
        private Visibility visibility1 = Visibility.Hidden;
        private Visibility visibility2 = Visibility.Hidden;

        #endregion

        #region .: Status :.

        private bool fileLoaded;

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

        public string Question { get { return question; } set { question = value; NotifyPropertyChanged("Question"); } }
        public string Answer1 { get { return answer1; } set { answer1 = value; NotifyPropertyChanged("Answer1"); } }
        public string Answer2 { get { return answer2; } set { answer2 = value; NotifyPropertyChanged("Answer2"); } }
        public string Note { get; set; }

        public string DebugText { get { return debugText; } set { debugText = value; NotifyPropertyChanged("DebugText"); } }

        public Visibility Visibility1 { get { return visibility1; } set { visibility1 = value; NotifyPropertyChanged("Visibility1"); } }
        public Visibility Visibility2 { get { return visibility2; } set { visibility2 = value; NotifyPropertyChanged("Visibility2"); } }
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
            if (state == CurrentVocable.Type.OutputColumns.Length)
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
            string path = @"C:\Users\jiriv\OneDrive\Dokumenty\Excel\Japonština\Genki 2-21.xml";

            ReadXmlFile(path);
        }

        #endregion

        #region .: Private Methods :.

        #region .: Migrated from Vocabulary class :.

        public Vocable GetNextVocable()
        {
            Vocable candidateVocable;

            while (true)
            {
                candidateVocable = vocableStack[random.Next(vocableStack.Count)];
                if (candidateVocable.Equals(CurrentVocable))
                    continue;
                break;
            }

            vocableStack.Remove(candidateVocable);
            candidateVocable.Type = EnabledType;
            CurrentVocable = candidateVocable;

            if (vocableStack.Count == 0)
            {
                RefillStack();
                Rounds++;
            }

            return candidateVocable;
        }

        /// <summary>
        /// Finds out if the vocable is enabled according to current settings
        /// of enabled types and groups.
        /// </summary>
        /// <param name="vocable">Vocable to be checked.</param>
        /// <returns>Vocable enabled or not.</returns>
        private bool IsVocableEnabled(Vocable vocable)
        {
            bool isEnabled = false;

            if (vocable.Types.Any(type => type.Data == EnabledType))
            {
                if (vocable.Groups.Any(group => group.IsChecked))
                {
                    isEnabled = true;
                }
            }

            return isEnabled;
        }

        /// <summary>
        /// Imports vocables and settings from an XML file.
        /// </summary>
        /// <param name="path">Path to the XML file.</param>
        private void ReadXmlFile(string path)
        {
            XmlDocument xmlDoc;
            XmlNode node;
            int fieldsCount = 0;

            xmlDoc = new XmlDocument();
            xmlDoc.Load(path);

            /* import configuration */
            node = xmlDoc.GetElementsByTagName("config")[0];
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Attributes["key"].Value.Equals("columns"))
                {
                    fieldsCount = int.Parse(child.Attributes["value"].Value);
                    break;
                }
            }

            /* import vocable type definitions */
            node = xmlDoc.GetElementsByTagName("types")[0];
            TypesCollection = new ObservableCollection<CheckableItem<VocableType>>();
            foreach (XmlNode child in node.ChildNodes)
            {
                VocableType type = new VocableType(child);

                CheckableItem<VocableType> item = new CheckableItem<VocableType>(type.Name, type);
                item.IsCheckedChanged += TypeItem_IsCheckedChanged;

                TypesCollection.Add(item);
            }

            /* import group definitions */
            node = xmlDoc.GetElementsByTagName("groups")[0];
            GroupsDict = new Dictionary<string, CheckableItem>(node.ChildNodes.Count);
            GroupsCollection = new ObservableCollection<CheckableItem>();
            foreach (XmlNode child in node.ChildNodes)
            {
                CheckableItem item = new CheckableItem(child.Attributes["name"].Value);
                item.IsCheckedChanged += GroupItem_IsCheckedChanged;

                GroupsDict.Add(child.Attributes["key"].Value, item);
                GroupsCollection.Add(item);
            }

            /* import vocables */
            node = xmlDoc.GetElementsByTagName("vocabulary")[0];
            foreach (XmlNode child in node.ChildNodes)
            {
                Vocable vocable = new Vocable(child, fieldsCount);

                // assign types
                string types = child.Attributes[$"field{fieldsCount + 1}"].Value;
                if (types.Equals(string.Empty))
                {
                    // no type defined -> assign all of them
                    vocable.Types = new List<CheckableItem<VocableType>>(TypesCollection);
                }
                else
                {
                    List<string> typeNumbers = new List<string>(types.Split(';'));
                    vocable.Types = new List<CheckableItem<VocableType>>(typeNumbers.Count);
                    typeNumbers.ForEach(typeNumber => vocable.Types.Add(TypesCollection[int.Parse(typeNumber) - 1]));
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
            Question = CurrentVocable.Input;
            Answer1 = CurrentVocable.GetOutput(0);
            Answer2 = CurrentVocable.GetOutput(1);
        }

        private void UpdateVisibility()
        {
            switch (state)
            {
                case 0:
                    Visibility1 = Visibility.Hidden;
                    Visibility2 = Visibility.Hidden;
                    break;
                case 1:
                    Visibility1 = Visibility.Visible;
                    Visibility2 = Visibility.Hidden;
                    break;
                case 2:
                    Visibility1 = Visibility.Visible;
                    Visibility2 = Visibility.Visible;
                    break;
            }
        }

        #region .: Debug :.

        private void ShowSampleData()
        {
            Question = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit.";
            Answer1 = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit.";
            Visibility1 = Visibility.Visible;
            Answer2 = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit.";
            Visibility2 = Visibility.Visible;
            Note = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit.";
        }

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
        private void GroupItem_IsCheckedChanged(object sender, EventArgs e)
        {
            //RefillStack();
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
