using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.ViewModels.ComboBoxSource
{
    public class ComboBoxViewModel : INotifyPropertyChanged
    {
        #region - Ctors -
        public ComboBoxViewModel()
        {

        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        public override string ToString()
        {
            return Number.ToString();
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public int Number
        {
            get { return _number; }
            set
            {
                _number = value;
                RaisePropertyChanged("Number");
            }
        }
        #endregion
        #region - Attributes -
        private int _number;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

    }
}
