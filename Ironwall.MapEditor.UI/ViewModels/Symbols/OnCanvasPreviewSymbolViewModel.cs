using Caliburn.Micro;
using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models;
using Ironwall.Framework.ViewModels;
using Ironwall.MapEditor.UI.ViewModels.ContentControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.ViewModels.Symbols
{
    public class OnCanvasPreviewSymbolViewModel
        : ISymbolViewMdoelBase
        , IEntityViewModel
        , INotifyPropertyChanged
    {
        #region - Ctors -
        public OnCanvasPreviewSymbolViewModel(
            //IEntityModel entityModel
            SymbolContentControlViewModel symbolContentControlViewModel
            , IEventAggregator eventAggregator
            )
        {
            _eventAggregator = eventAggregator;
            SymbolContentControlViewModel = symbolContentControlViewModel;
            //UpdateProperty(entityModel);
        }

        /*private void UpdateProperty(IEntityModel entityViewModel)
        {
            Id = entityViewModel.Id;
            NameArea = entityViewModel.NameArea;
            TypeDevice = entityViewModel.TypeDevice;
            NameDevice = entityViewModel.NameDevice;
            IdController = entityViewModel.IdController;
            IdSensor = entityViewModel.IdSensor;
            TypeShape = entityViewModel.TypeShape;
            X1 = entityViewModel.X1;
            Y1 = entityViewModel.Y1;
            X2 = entityViewModel.X2;
            Y2 = entityViewModel.Y2;
            Width = entityViewModel.Width;
            Height = entityViewModel.Height;
            Angle = entityViewModel.Angle;
            Map = entityViewModel.Map;
            Used = entityViewModel.Used;
            Visibility = entityViewModel.Visibility;
        }*/
        #endregion
        #region - Implementation of Interface -
        public void Activate()
        {
            _eventAggregator.SubscribeOnUIThread(this);
        }

        public void Deactivate()
        {
            _eventAggregator.Unsubscribe(this);
        }
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        public void Update()
        {
            RaisePropertyChanged("X");
            RaisePropertyChanged("Y");
            RaisePropertyChanged("NameArea");
            RaisePropertyChanged("TypeDevice");
            RaisePropertyChanged("NameDevice");
            RaisePropertyChanged("IdController");
            RaisePropertyChanged("IdSensor");
            RaisePropertyChanged("TypeShape");
            RaisePropertyChanged("X1");
            RaisePropertyChanged("Y1");
            RaisePropertyChanged("X2");
            RaisePropertyChanged("Y2");
            RaisePropertyChanged("Width");
            RaisePropertyChanged("Height");
            RaisePropertyChanged("Angle");
            RaisePropertyChanged("Map");
            RaisePropertyChanged("Used");
            RaisePropertyChanged("Visibility");
        }
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
        public double X
        {
            get => x;
            set
            {
                x = value;
                RaisePropertyChanged("X");
            }
        }

        public double Y
        {
            get => y;
            set
            {
                y = value;
                RaisePropertyChanged("Y");
            }
        }
        public int Id
        {
            get => SymbolContentControlViewModel.Id;
            set
            {
                SymbolContentControlViewModel.Id = value;
                RaisePropertyChanged("Id");
            }
        }

        public string NameArea
        {
            get => SymbolContentControlViewModel.NameArea;
            set
            {
                SymbolContentControlViewModel.NameArea = value;
                RaisePropertyChanged("NameArea");
            }
        }

        public int TypeDevice
        {
            get => SymbolContentControlViewModel.TypeDevice;
            set
            {
                SymbolContentControlViewModel.TypeDevice = value;
                RaisePropertyChanged("TypeDevice");
            }
        }

        public string NameDevice
        {
            get => SymbolContentControlViewModel.NameDevice;
            set
            {
                SymbolContentControlViewModel.NameDevice = value;
                RaisePropertyChanged("NameDevice");
            }
        }

        public int TypeShape
        {
            get => SymbolContentControlViewModel.TypeShape;
            set
            {
                SymbolContentControlViewModel.TypeShape = value;
                RaisePropertyChanged("TypeShape");
            }
        }

        public double X1
        {
            get => SymbolContentControlViewModel.X1;
            set
            {
                SymbolContentControlViewModel.X1 = value;
                RaisePropertyChanged("X1");
            }
        }

        public double Y1
        {
            get => SymbolContentControlViewModel.Y1;
            set
            {
                SymbolContentControlViewModel.Y1 = value;
                RaisePropertyChanged("Y1");
            }
        }

        public double X2
        {
            get => SymbolContentControlViewModel.X2;
            set
            {
                SymbolContentControlViewModel.X2 = value;
                RaisePropertyChanged("X2");
            }
        }

        public double Y2
        {
            get => SymbolContentControlViewModel.Y2;
            set
            {
                SymbolContentControlViewModel.Y2 = value;
                RaisePropertyChanged("Y2");
            }
        }

        public double Width
        {
            get => SymbolContentControlViewModel.Width;
            set
            {
                SymbolContentControlViewModel.Width = value;
                RaisePropertyChanged("Width");
            }
        }

        public double Height
        {
            get => SymbolContentControlViewModel.Height;
            set
            {
                SymbolContentControlViewModel.Height = value;
                RaisePropertyChanged("Height");
            }
        }

        public double Angle
        {
            get => SymbolContentControlViewModel.Angle;
            set
            {
                SymbolContentControlViewModel.Angle = value;
                RaisePropertyChanged("Angle");
            }
        }
        public int IdController
        {
            get => SymbolContentControlViewModel.IdController;
            set
            {
                SymbolContentControlViewModel.IdController = value;
                RaisePropertyChanged("IdController");
            }
        }
        public int IdSensor
        {
            get => SymbolContentControlViewModel.IdSensor;
            set
            {
                SymbolContentControlViewModel.IdSensor = value;
                RaisePropertyChanged("IdSensor");
            }
        }

        public int Map
        {
            get => SymbolContentControlViewModel.Map;
            set
            {
                SymbolContentControlViewModel.Map = value;
                RaisePropertyChanged("Map");
            }
        }

        public bool Used
        {
            get => SymbolContentControlViewModel.Used;
            set
            {
                SymbolContentControlViewModel.Used = value;
                RaisePropertyChanged("Used");
            }
        }

        public bool Visibility
        {
            get => SymbolContentControlViewModel.Visibility;
            set
            {
                SymbolContentControlViewModel.Visibility = value;
                RaisePropertyChanged("Visibility");
            }
        }
        public SymbolContentControlViewModel SymbolContentControlViewModel { get; private set; }
        #endregion

        #region - Attributes -
        private double x;
        private double y;
        private IEventAggregator _eventAggregator;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
