using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nippori.Bases
{
    public abstract class ModelBase : INotifyPropertyChanged
    {
        #region .: Private Methods :.

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region .: INotifyPropertyChanged :.

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
