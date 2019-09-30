using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nippori.Bases;

namespace Nippori.Vocables
{
    public class CheckableItem : ModelBase
    {
        #region .: Private Fields :.

        private string name;
        private bool isChecked = false;

        #endregion

        #region .: Properties :.

        public string Name
        {
            get { return name; }
            set { name = value; NotifyPropertyChanged("Name"); }
        }

        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                bool previousValue = isChecked;
                isChecked = value;
                NotifyPropertyChanged("IsChecked");
                if (previousValue != isChecked)
                {
                    OnIsCheckedChanged();
                }
            }
        }

        #endregion

        #region .: Constructor :.

        public CheckableItem(string name)
        {
            Name = name;
        }

        #endregion

        #region .: Private Methods :.

        private void OnIsCheckedChanged()
        {
            IsCheckedChanged?.Invoke(this, new EventArgs());
        }

        #endregion

        #region .: Events :.

        public event EventHandler IsCheckedChanged;

        #endregion
    }

    public class CheckableItem<T> : CheckableItem
    {
        public T Data { set; get; }

        public CheckableItem(string name, T data) : base(name)
        {
            Data = data;
        }
    }
}
