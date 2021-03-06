using Caliburn.Micro;
using Ironwall.Enums;
using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Services;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.MapEditor.UI.DataProviders;
using Ironwall.MapEditor.UI.Models.Messages.Panels;
using Ironwall.MapEditor.UI.Models.Messages.Process;
using Ironwall.MapEditor.UI.Models.Messages.Sections;
using Ironwall.MapEditor.UI.ViewModels.ContentControls;
using Ironwall.MapEditor.UI.ViewModels.Panels;
using Ironwall.MapEditor.UI.ViewModels.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Ironwall.MapEditor.UI.ViewModels.Sections
{
    internal sealed class TopMenuSectionViewModel
        : BaseViewModel
        , IHandle<OpenMapPropertyMessageModel>
        , IHandle<OpenControllerPropertyMessageModel>
        , IHandle<OpenSensorPropertyMessageModel>
        , IHandle<OpenCameraPropertyMessageModel>
        , IHandle<OpenGroupPropertyMessageModel>
    {
        #region - Ctors -
        public TopMenuSectionViewModel(
            IEventAggregator eventAggregator
            , IWindowManager windowManager
            , ControllerProvider controllerProvider
            , SymbolControllerProvider symbolControllerProvider
            , AddMapPanelViewModel addMapPanelViewModel)
        {
            #region - Settings -
            Id = 0;
            Content = "";
            Category = CategoryEnum.SECTION_SHELL_VM_ITEM;
            #endregion - Settings -

            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnPublishedThread(this);
            _windowManager = windowManager;

            _symbolControllerProvider = symbolControllerProvider;
            _controllerProvider = controllerProvider;
            _addMapPanelViewModel = addMapPanelViewModel;
        }
        #endregion

        #region - Binding Methods -
        public void OnClickSave(object sender, RoutedEventArgs e)
        {
            CancellationToken cancellationToken = new CancellationToken();
            _eventAggregator.PublishOnCurrentThreadAsync(new OpenSavePanelMessageModel(), cancellationToken);
        }

        public void OnClickLoad(object sender, RoutedEventArgs e)
        {
            CancellationToken cancellationToken = new CancellationToken();
            _eventAggregator.PublishOnCurrentThreadAsync(new OpenLoadPanelMessageModel(), cancellationToken);
        }
        public void OnClickExit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public void OnClickShowList(object sender, RoutedEventArgs e)
        {
            CancellationToken cancellationToken = new CancellationToken();
            _eventAggregator.PublishOnCurrentThreadAsync(new OpenShowListPanelMessageModel(), cancellationToken);
        }

        public void OnClickAddMap(object sender, RoutedEventArgs e)
        {
            CancellationToken cancellationToken = new CancellationToken();
            _eventAggregator.PublishOnCurrentThreadAsync(new OpenAddMapPanelMessageModel(), cancellationToken);
        }

        public void OnClickAddController(object sender, RoutedEventArgs e)
        {
            CancellationToken cancellationToken = new CancellationToken();
            _eventAggregator.PublishOnCurrentThreadAsync(new OpenAddControlPanelMessageModel(), cancellationToken);
        }

        public void OnClickAddSensor(object sender, RoutedEventArgs e)
        {
            CancellationToken cancellationToken = new CancellationToken();
            _eventAggregator.PublishOnCurrentThreadAsync(new OpenAddSensorPanelMessageModel(), cancellationToken);
        }

        public void OnClickAddGroup(object sender, RoutedEventArgs e)
        {
            CancellationToken cancellationToken = new CancellationToken();
            _eventAggregator.PublishOnCurrentThreadAsync(new OpenAddGroupPanelMessageModel(), cancellationToken);
        }

        public void OnClickAddCamera(object sender, RoutedEventArgs e)
        {
            CancellationToken cancellationToken = new CancellationToken();
            _eventAggregator.PublishOnCurrentThreadAsync(new OpenAddCameraPanelMessageModel(), cancellationToken);
        }


        public void OnClickAddSymbolController(object sender, RoutedEventArgs e)
        {
            if (SymbolContentControlViewModel == null)
                return;
            
            //var vm = EntityViewModelFactory.Build<SymbolControllerViewModel>(SymbolContentControlViewModel.Model);


            /*vm.EventAggregator = _eventAggregator;
            vm.EventAggregator.SubscribeOnPublishedThread(vm);
            vm.X = SymbolContentControlViewModel.X1;
            vm.Y = SymbolContentControlViewModel.Y1;
            vm.Visibility = SymbolContentControlViewModel.Visibility;

            _symbolControllerProvider.Add(vm);*/

            

            /*if (_symbolControllerProvider.CollectionEntity.Where(t => t.Id == SymbolContentControlViewModel.Id).Count() > 0)
            {
                var entity = _symbolControllerProvider.CollectionEntity.Where(t => t.Id == SymbolContentControlViewModel.Id).FirstOrDefault();
                DispatcherService.Invoke((System.Action)(() =>
                {
                    ///UI 쓰레드에 의해 점유중인 자원에 대해선 자원에 
                    ///수정을 가할 때 UI 쓰레드의 Dispatcher에 작업을 위임해야 한다. 
                    _symbolControllerProvider.Remove(entity);
                }));
            }
            else
                _symbolControllerProvider.Add(symbolController);*/

            //_windowManager.ShowDialogAsync(_addMapPanelViewModel);
            //_windowManager.ShowPopupAsync(_addMapPanelViewModel);

            //_symbolControllerProvider.Add(SymbolContentControlViewModel);


            _eventAggregator.PublishOnUIThreadAsync(new OnActivePreviewSymbolMessageModel(SymbolContentControlViewModel));
            //_eventAggregator.PublishOnUIThreadAsync(new UpdateSymbolControllerMessageModel());
        }

        #endregion


        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        public Task HandleAsync(OpenMapPropertyMessageModel message, CancellationToken cancellationToken)
        {


            DrawingType = EnumDrawingType.NONE;
            return Task.CompletedTask;
        }

        public Task HandleAsync(OpenControllerPropertyMessageModel message, CancellationToken cancellationToken)
        {
            DrawingType = EnumDrawingType.Controller;
            SymbolContentControlViewModel = message.ViewModel as ControllerContentControlViewModel;

            
            return Task.CompletedTask;
        }

        public Task HandleAsync(OpenSensorPropertyMessageModel message, CancellationToken cancellationToken)
        {
            DrawingType = EnumDrawingType.Sensor;
            return Task.CompletedTask;
        }

        public Task HandleAsync(OpenGroupPropertyMessageModel message, CancellationToken cancellationToken)
        {
            DrawingType = EnumDrawingType.Group;
            return Task.CompletedTask;
        }
        
        public Task HandleAsync(OpenCameraPropertyMessageModel message, CancellationToken cancellationToken)
        {
            DrawingType = EnumDrawingType.IpCamera;
            return Task.CompletedTask;
        }
        #endregion
        #region - Properties -

        public EnumDrawingType DrawingType
        {
            get { return _drawingType; }
            set 
            { 
                _drawingType = value;
                NotifyOfPropertyChange(() => DrawingType);
            }
        }
        private SymbolContentControlViewModel _symbolContentControlViewModel;

        public SymbolContentControlViewModel SymbolContentControlViewModel
        {
            get { return _symbolContentControlViewModel; }
            set { _symbolContentControlViewModel = value; }
        }


        #endregion
        #region - Attributes -
        private EnumDrawingType _drawingType;
        private IWindowManager _windowManager;
        private SymbolControllerProvider _symbolControllerProvider;
        private ControllerProvider _controllerProvider;
        private AddMapPanelViewModel _addMapPanelViewModel;
        #endregion
    }
}
