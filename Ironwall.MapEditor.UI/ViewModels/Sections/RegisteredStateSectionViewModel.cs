using Caliburn.Micro;
using Ironwall.Enums;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.MapEditor.UI.DataProviders;
using Ironwall.MapEditor.UI.Services;
using Ironwall.MapEditor.UI.ViewModels.RegisteredItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.ViewModels.Sections
{
    internal sealed class RegisteredStateSectionViewModel
        : BaseViewModel
    {
        
        #region - Ctors -
        public RegisteredStateSectionViewModel(
            IEventAggregator eventAggregator
            , MapTreeViewModel treeMapUserControlViewModel
            //, ControllerTreeViewModel controllerTreeViewModel
            //, SensorTreeViewModel sensorTreeViewModel
            , DeviceTreeViewModel deviceTreeViewModel
            , GroupTreeViewModel groupTreeViewModel
            , GroupSymbolTreeViewModel groupSymbolTreeViewModel
            , CameraTreeViewModel cameraTreeViewModel
            )
        {
            #region - Settings -
            Id = 0;
            Content = "";
            Category = CategoryEnum.SECTION_SHELL_VM_ITEM;
            #endregion - Settings -

            _eventAggregator = eventAggregator;
            MapTreeViewModel = treeMapUserControlViewModel;
            //ControllerTreeViewModel = controllerTreeViewModel;
            //SensorTreeViewModel = sensorTreeViewModel;
            DeviceTreeViewModel = deviceTreeViewModel;
            GroupTreeViewModel = groupTreeViewModel;
            GroupSymbolTreeViewModel = groupSymbolTreeViewModel;
            CameraTreeViewModel = cameraTreeViewModel;
        }

        #endregion

        #region - Implementation of Interface -
        #endregion

        #region - Overrides -
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            base.OnActivateAsync(cancellationToken);

            MapTreeViewModel.ActivateAsync();
            //ControllerTreeViewModel.ActivateAsync();
            //SensorTreeViewModel.ActivateAsync();
            DeviceTreeViewModel.ActivateAsync();
            GroupTreeViewModel.ActivateAsync();
            GroupSymbolTreeViewModel.ActivateAsync();
            CameraTreeViewModel.ActivateAsync();
            return Task.CompletedTask;
        }
        #endregion

        #region - Binding Methods -
        #endregion

        #region - IHanldes -
        #endregion

        #region - Properties -
        public MapTreeViewModel MapTreeViewModel { get; }
        public DeviceTreeViewModel DeviceTreeViewModel { get; }
        public GroupTreeViewModel GroupTreeViewModel { get; }
        public GroupSymbolTreeViewModel GroupSymbolTreeViewModel { get; }
        public CameraTreeViewModel CameraTreeViewModel { get; }

        private TreeContentControlViewModel _selectedMap;

        public TreeContentControlViewModel MyProperty
        {
            get { return _selectedMap; }
            set { _selectedMap = value; }
        }


        #endregion

        #region - Attributes -
        #endregion
    }
}
