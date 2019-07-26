using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using NipporiWpf.Vocables;

namespace NipporiWpf
{
    public class ViewModel : INotifyPropertyChanged
    {
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
        public string StackCount { get => fileLoaded ? Vocabulary.StackCount.ToString() : "--"; }

        public string OpenedFileName { get { return openedFileName; } set { openedFileName = value; NotifyPropertyChanged("OpenedFileName"); } }
        public Visibility ProgressBarVisibility { get { return progressBarVisibility; } set { progressBarVisibility = value; NotifyPropertyChanged("ProgressBarVisibility"); } }

        public ObservableCollection<CheckableItem<VocableType>> Types { get { return types; } set { types = value; NotifyPropertyChanged("Types"); } }
        public ObservableCollection<CheckableItem> Groups { get { return groups; } set { groups = value; NotifyPropertyChanged("Groups"); } }

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
            if (state == Vocabulary.CurrentVocable.Type.OutputColumns.Length)
            {
                if (Vocabulary.QueueCount > 0)
                    Vocabulary.DequeueVocable();
                else
                    Vocabulary.GetNextVocable();

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
            if (state == 0)
            {
                state = 1;
            }
            else
            {
                Vocabulary.EnqueueCurrentVocable();
                if (Vocabulary.QueueCount > 1)
                    Vocabulary.DequeueVocable();
                else
                    Vocabulary.GetNextVocable();

                ShowVocable();
                state = 0;
            }
            UpdateVisibility();
            NotifyPropertyChanged("StackCount");
        }

        public void OpenFile(string fileName)
        {
            OpenedFileName = fileName;
            new Task(() => LoadDataTaskFunc()).Start();
        }

        public void StartExam()
        {
            Vocabulary.Start();
            Vocabulary.GetNextVocable();
            ShowVocable();
            UpdateVisibility();
            NotifyPropertyChanged("StackCount");
        }

        /// <summary>
        /// Invokes a test procedure.
        /// </summary>
        public void Test()
        {
            // right now do nothing
        }

        #endregion

        #region .: Private Methods :.

        private void LoadDataTaskFunc()
        {
            ProgressBarVisibility = Visibility.Visible;
            if (OpenedFileName.EndsWith("xml"))
            {
                Vocabulary.ReadXmlFile(OpenedFileName);
            }
            else
            {
                Vocabulary.ReadFile(OpenedFileName);
            }
            ProgressBarVisibility = Visibility.Hidden;

            Types = Vocabulary.TypesCollection;
            Groups = Vocabulary.GroupsCollection;

            fileLoaded = true;
            StartExam();
        }

        private void ShowVocable()
        {
            Question = Vocabulary.CurrentVocable.Input;
            Answer1 = Vocabulary.CurrentVocable.GetOutput(0);
            Answer2 = Vocabulary.CurrentVocable.GetOutput(1);
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

        #region .: INotifyPropertyChanged :.

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
