using Caliburn.Micro;
using Ironwall.Enums;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.MapEditor.UI.DataProviders;
using Ironwall.MapEditor.UI.Models.Messages.Panels;
using Ironwall.MapEditor.UI.ViewModels.DataGridItems;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Ironwall.MapEditor.UI.ViewModels.Panels
{
    internal sealed class ShowListPanelViewModel
        : BaseViewModel
        , IPanelViewModel
    {
        #region - Ctors -
        public ShowListPanelViewModel(
            IEventAggregator eventAggregator
            , MapProvider mapProvider
            , ControllerProvider controllerProvider
            , SensorProvider sensorProvider
            , GroupProvider groupProvider
            , CameraProvider cameraProvider)
        {
            #region - Settings -
            Id = 10;
            Content = "";
            Category = CategoryEnum.PANEL_SHELL_VM_ITEM;
            #endregion - Settings -

            _eventAggregator = eventAggregator;

            DataGridMapViewModel = new DataGridMapViewModel(mapProvider);
            DataGridControllerViewModel = new DataGridControllerViewModel(controllerProvider);
            DataGridSensorViewModel = new DataGridSensorViewModel(sensorProvider);
            DataGridGroupViewModel = new DataGridGroupViewModel(groupProvider);
            DataGridCameraViewModel = new DataGridCameraViewModel(cameraProvider);
        }


        #endregion
        #region - Implementation of Interface -
        public async void ClickCancelAsync()
        {
            await _eventAggregator.PublishOnCurrentThreadAsync(new ClosePanelMessageModel(), _cancellationTokenSource.Token);
        }

        public async void ClickOkAsync()
        {
            await _eventAggregator.PublishOnCurrentThreadAsync(new ClosePanelMessageModel(), _cancellationTokenSource.Token);
        }

        public void ModelUpdate()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region - Overrides -
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            base.OnActivateAsync(cancellationToken);
            return Task.CompletedTask;
        }
        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            base.OnDeactivateAsync(close, cancellationToken);

            DataGridMapViewModel.DeactivateAsync(true);
            DataGridControllerViewModel.DeactivateAsync(true);
            DataGridSensorViewModel.DeactivateAsync(true);
            DataGridGroupViewModel.DeactivateAsync(true);
            DataGridCameraViewModel.DeactivateAsync(true);

            return Task.CompletedTask;
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        //상속자를 가져다 쓰면 부모의 인스턴스와 맵핑된 UserControl을 불러올 수있을까?
        public DataGridMapViewModel DataGridMapViewModel { get; set; }
        public DataGridControllerViewModel DataGridControllerViewModel { get; set; }
        public DataGridSensorViewModel DataGridSensorViewModel { get; set; }
        public DataGridGroupViewModel DataGridGroupViewModel { get; set; }
        public DataGridCameraViewModel DataGridCameraViewModel { get; set; }

        private object _selectedItem;

        public object SelectedItem
        {
            get { return _selectedItem; }
            set 
            { 
                _selectedItem = value;
                NotifyOfPropertyChange(() => SelectedItem);

                if (_selectedItem == null)
                    return;

                if((_selectedItem as TabItem).Tag.ToString() == "Map")
                {
                    DataGridMapViewModel?.ActivateAsync();
                    //NotifyOfPropertyChange(()=> DataGridMapViewModel);
                }
                else if((_selectedItem as TabItem).Tag.ToString() == "Controller") 
                {
                    DataGridControllerViewModel?.ActivateAsync();
                    //NotifyOfPropertyChange(() => DataGridControllerViewModel);
                }
                else if ((_selectedItem as TabItem).Tag.ToString() == "Sensor")
                {
                    DataGridSensorViewModel?.ActivateAsync();
                    //NotifyOfPropertyChange(() => DataGridSensorViewModel);
                }
                else if ((_selectedItem as TabItem).Tag.ToString() == "Group")
                {
                    DataGridGroupViewModel?.ActivateAsync();
                    //NotifyOfPropertyChange(() => DataGridGroupViewModel);
                }
                else if ((_selectedItem as TabItem).Tag.ToString() == "Camera")
                {
                    DataGridCameraViewModel?.ActivateAsync();
                    //NotifyOfPropertyChange(() => DataGridCameraViewModel);
                }
                
            }
        }


        #endregion
        #region - Attributes -
        #endregion
    }
}
