using Caliburn.Micro;
using Ironwall.Framework.Services;
using Ironwall.MapEditor.UI.Models.Messages.Process;
using Ironwall.MapEditor.UI.ViewModels.ComboBoxSource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.DataProviders
{
    public class MapComboBox : INotifyPropertyChanged
    {
        #region - Ctors -
        public MapComboBox()
        {
            _source = new TrulyObservableCollection<ComboBoxViewModel>();
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
        public Task HandleAsync(MapContentUpdateMessageModel message, CancellationToken cancellationToken)
        {
            RaisePropertyChanged("Source");
            return Task.CompletedTask;
        }
        #endregion
        #region - Properties -
        public TrulyObservableCollection<ComboBoxViewModel> Source
        {
            get {return _source; }
            set
            {
                _source = value;
                RaisePropertyChanged("Source");
            }
        }

        //private IEventAggregator _eventAggregator;
        #endregion
        #region - Attributes -
        private TrulyObservableCollection<ComboBoxViewModel> _source;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
