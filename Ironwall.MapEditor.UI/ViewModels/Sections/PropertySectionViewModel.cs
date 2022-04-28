using Caliburn.Micro;
using Ironwall.Enums;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.MapEditor.UI.Models.Messages.Sections;
using Ironwall.MapEditor.UI.ModelValidators;
using Ironwall.MapEditor.UI.ViewModels.ContentControls;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.ViewModels.Sections
{
    internal sealed class PropertySectionViewModel
        : BaseViewModel
        , IHandle<OpenMapPropertyMessageModel>
        , IHandle<OpenControllerPropertyMessageModel>
        , IHandle<OpenSensorPropertyMessageModel>
        , IHandle<OpenCameraPropertyMessageModel>
        , IHandle<OpenGroupPropertyMessageModel>
    {
        #region - Ctors -
        public PropertySectionViewModel(IEventAggregator eventAggregator)
        {
            #region - Settings -
            Id = 0;
            Content = "";
            Category = CategoryEnum.SECTION_SHELL_VM_ITEM;
            #endregion - Settings -

            _eventAggregator = eventAggregator;
        }
        #endregion

        #region - Overrides -
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            base.OnActivateAsync(cancellationToken);

            
            return Task.CompletedTask;
        }

       
        #endregion

        #region - IHanldes -
        public async Task HandleAsync(OpenMapPropertyMessageModel message, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                ContentViewModel = message?.ViewModel;
                if (!(ContentViewModel is MapContentControlViewModel vm))
                    return;
                if (!vm.IsActive)
                    vm.ActivateAsync();

            }, cancellationToken);
        }

        public async Task HandleAsync(OpenControllerPropertyMessageModel message, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                ContentViewModel = message?.ViewModel;
                if (!(ContentViewModel is ControllerContentControlViewModel vm))
                    return;
                if (!vm.IsActive)
                    vm.ActivateAsync();

            }, cancellationToken);
        }

        public async Task HandleAsync(OpenSensorPropertyMessageModel message, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                ContentViewModel = message?.ViewModel;
                if (!(ContentViewModel is SensorContentControlViewModel vm))
                    return;
                if (!vm.IsActive)
                    vm.ActivateAsync();

            }, cancellationToken);
        }

        public async Task HandleAsync(OpenCameraPropertyMessageModel message, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                ContentViewModel = message?.ViewModel;
                if (!(ContentViewModel is CameraContentControlViewModel vm))
                    return;
                if(!vm.IsActive)
                    vm.ActivateAsync();

            }, cancellationToken);
        }

        public async Task HandleAsync(OpenGroupPropertyMessageModel message, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                ContentViewModel = message?.ViewModel;
                if (!(ContentViewModel is GroupContentControlViewModel vm))
                    return;
                if (!vm.IsActive)
                    vm.ActivateAsync();

            }, cancellationToken);
        }
        #endregion

        #region - Binding Methods -
        #endregion

        #region - Properties -
        public Screen ContentViewModel
        {
            get { return _contentViewModel; }
            set 
            {
                _contentViewModel = value;
                NotifyOfPropertyChange(() => ContentViewModel);
            }
        }
        #endregion

        #region - Attributes -
        private Screen _contentViewModel;
        #endregion

    }
}
