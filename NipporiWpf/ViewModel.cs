using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Nippori;

namespace NipporiWpf
{
    public class ViewModel : INotifyPropertyChanged
    {
        #region .: Private Fields :.

        private string debugText;

        private string question;
        private string answer1;
        private string answer2;
        private Visibility visibility1 = Visibility.Hidden;
        private Visibility visibility2 = Visibility.Hidden;

        private Visibility progressBarVisibility = Visibility.Hidden;
        private string openedFileName;

        private int state = 0;

        #endregion

        #region .: Properties :.

        public string DebugText { get { return debugText; } set { debugText = value; NotifyPropertyChanged("DebugText"); } }

        public string Question { get { return question; } set { question = value; NotifyPropertyChanged("Question"); } }
        public string Answer1 { get { return answer1; } set { answer1 = value; NotifyPropertyChanged("Answer1"); } }
        public string Answer2 { get { return answer2; } set { answer2 = value; NotifyPropertyChanged("Answer2"); } }
        public Visibility Visibility1 { get { return visibility1; } set { visibility1 = value; NotifyPropertyChanged("Visibility1"); } }
        public Visibility Visibility2 { get { return visibility2; } set { visibility2 = value; NotifyPropertyChanged("Visibility2"); } }

        public string OpenedFileName { get { return openedFileName; } set { openedFileName = value; NotifyPropertyChanged("OpenedFileName"); } }
        public Visibility ProgressBarVisibility { get { return progressBarVisibility; } set { progressBarVisibility = value; NotifyPropertyChanged("ProgressBarVisibility"); } }

        #endregion

        #region .: Constructor :.

        public ViewModel()
        {
            Vocabulary.Init();
        }

        #endregion

        #region .: Public Methods :.

        public void Confirm()
        {
            if (state == 0)
            {
                state = 1;
            }
            else
            {
                if (Vocabulary.QueueCount > 0)
                    Vocabulary.DequeueVocable();
                else
                    Vocabulary.GetNextVocable();

                ShowVocable();
                state = 0;
            }
            UpdateVisibility();
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
        }

        public void OpenFile(string fileName)
        {
            OpenedFileName = fileName;
            new Task(() => LoadDataTaskFunc()).Start();
        }

        #endregion

        #region .: Private Methods :.

        private void LoadDataTaskFunc()
        {
            ProgressBarVisibility = Visibility.Visible;
            Vocabulary.ReadFile(OpenedFileName);
            ProgressBarVisibility = Visibility.Hidden;

            Vocabulary.EnabledType = 2;
            Vocabulary.Start();
            Vocabulary.GetNextVocable();
            ShowVocable();
            UpdateVisibility();
        }

        private void ShowVocable()
        {
            Question = Vocabulary.CurrentVocable.Input;
            Answer1 = Vocabulary.CurrentVocable.GetOutput(0);
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
