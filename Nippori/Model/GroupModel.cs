using System;

using Nippori.Bases;

namespace Nippori.Model
{
    /// <summary>
    /// Represents a group (aka category) of a vocable.
    /// We organize vocables to groups (per part of speech, per lesson, per topics etc.).
    /// </summary>
    public class GroupModel : ModelBase
    {
        #region .: Private Fields :.

        private bool isChecked = false;

        #endregion

        #region .: Properties :.

        public string Name { get; protected set; }

        public bool IsChecked
        {
            get => isChecked;
            set
            {
                if (isChecked != value)
                {
                    isChecked = value;
                    NotifyPropertyChanged("IsChecked");
                    OnIsCheckedChanged();
                }
            }
        }

        public bool ClearsAll { get; set; }
        public bool ChecksAll { get; set; }

        #endregion

        #region .: Constructor :.

        public GroupModel() { }

        public GroupModel(string name)
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
}
