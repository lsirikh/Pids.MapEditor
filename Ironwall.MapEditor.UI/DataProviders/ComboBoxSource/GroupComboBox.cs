using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.DataProviders
{
    public class GroupComboBox : INotifyPropertyChanged
    {
        #region - Ctors -
        public GroupComboBox()
        {
            _source = new ObservableCollection<string>();
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
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
        public ObservableCollection<string> Source
        {
            get { return _source; }
            set
            {
                _source = value;
                RaisePropertyChanged("NameAreaSource");
            }
        }
        #endregion
        #region - Attributes -
        private ObservableCollection<string> _source;

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
