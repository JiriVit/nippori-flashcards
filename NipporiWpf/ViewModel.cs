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

        private int state = 0;

        #endregion

        #region .: Properties :.

        public string DebugText { get { return debugText; } set { debugText = value; NotifyPropertyChanged("DebugText"); } }

        public string Question { get { return question; } set { question = value; NotifyPropertyChanged("Question"); } }
        public string Answer1 { get { return answer1; } set { answer1 = value; NotifyPropertyChanged("Answer1"); } }
        public string Answer2 { get { return answer2; } set { answer2 = value; NotifyPropertyChanged("Answer2"); } }
        public Visibility Visibility1 { get { return visibility1; } set { visibility1 = value; NotifyPropertyChanged("Visibility1"); } }
        public Visibility Visibility2 { get { return visibility2; } set { visibility2 = value; NotifyPropertyChanged("Visibility2"); } }

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
            Question = "ひらがな";
            Answer1 = "hiragana";
            Answer2 = "prdel";

            state = (state + 1) % 3;
            UpdateVisibility();
        }

        public void Reject()
        {
            state = (Math.Abs(state + 3) - 1) % 3;
            UpdateVisibility();
        }

        #endregion

        #region .: Private Methods :.

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
