using Caliburn.Micro;
using Ironwall.Enums;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.MapEditor.UI.DataProviders;
using Ironwall.MapEditor.UI.Models.Messages.PopupDialogs;
using Ironwall.MapEditor.UI.Models.Messages.Process;
using Ironwall.MapEditor.UI.Models.Messages.Sections;
using Ironwall.MapEditor.UI.ViewModels.Canvases;
using Ironwall.MapEditor.UI.ViewModels.ContentControls;
using Ironwall.MapEditor.UI.ViewModels.Symbols;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ironwall.MapEditor.UI.ViewModels.Sections
{
    internal sealed class CanvasSectionViewModel
        : BaseViewModel
        , IHandle<OpenMapPropertyMessageModel>
        , IHandle<MapTreeRemoveMessageModel>

        , IHandle<SymbolContentUpdateMessageModel>
        , IHandle<OnActivePreviewSymbolMessageModel>
        , IHandle<ClearPreviewSymbolMessageModel>
    {

        #region - Ctors -
        public CanvasSectionViewModel(
            IEventAggregator eventAggregator
            , MapProvider mapProvider
            , ControllerProvider controllerProvider
            , SymbolControllerProvider symbolControllerProvider
            , SymbolSensorProvider symbolSensorProvider
            , SymbolCameraProvider symbolCameraViewModels
            , CanvasMapEntityProvider canvasMapEntityProvider
            //, CanvasControllerViewModel canvasControllerViewModel
            )
        {
            #region - Settings -
            Id = 0;
            Content = "";
            Category = CategoryEnum.SECTION_SHELL_VM_ITEM;
            #endregion - Settings -

            _eventAggregator = eventAggregator;

            _mapProvider = mapProvider;
            _controllerProvider = controllerProvider;
            _symbolControllerProvider = symbolControllerProvider;
            _symbolSensorProvider = symbolSensorProvider;
            _symbolCameraProvider = symbolCameraViewModels;
            
            _canvasMapEntityProvider = canvasMapEntityProvider;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        
        #endregion

        #region - Binding Methods -
        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            var relativePosition = e.GetPosition((ContentPresenter)sender);
            X = relativePosition.X;
            Y = relativePosition.Y;
            if (OnCanvasPreviewSymbol != null)
            {
                OnCanvasPreviewSymbol.X = X - (OnCanvasPreviewSymbol.Width / 2);
                OnCanvasPreviewSymbol.X1 = X - (OnCanvasPreviewSymbol.Width / 2);
                OnCanvasPreviewSymbol.Y = Y - (OnCanvasPreviewSymbol.Height / 2);
                OnCanvasPreviewSymbol.Y1 = Y - (OnCanvasPreviewSymbol.Height / 2);
                //NotifyOfPropertyChange(() => OnCanvasPreviewSymbolViewModel);
            }
        }

        public async void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (OnCanvasPreviewSymbol == null)
                return;

            switch (OnCanvasPreviewSymbol.TypeDevice)
            {
                case (int)EnumDeviceType.NONE:
                    {
                    }
                    break;
                case (int)EnumDeviceType.Controller:
                    {
                        var provider = _symbolControllerProvider;
                        var prevSymbol = provider.CollectionEntity.Where(t => t.Id == OnCanvasPreviewSymbol.Id).FirstOrDefault();
                        
                        if (prevSymbol != null)
                        {
                            provider.Remove(prevSymbol);
                            prevSymbol.Deactivate();
                            prevSymbol = null;
                        }

                        ///////////////////Type Generation//////////////////////
                        var symbol = new SymbolControllerViewModel(OnCanvasPreviewSymbol.SymbolContentControlViewModel, _eventAggregator);
                        ////////////////////////////////////////////////////////
                        symbol.Activate();
                        provider.Add(symbol);

                        ///////////////////Type Settings////////////////////////
                        await _canvasMapEntityProvider.ControllerSetupAsync();
                        CanvasControllerViewModel = await Task.Run(() => _canvasMapEntityProvider.MapCanvasControllerViewModel[SelectedMap.MapNumber]);
                        ////////////////////////////////////////////////////////

                        OnCanvasPreviewSymbol.Deactivate();
                        OnCanvasPreviewSymbol = null;
                    }
                    break;
                case (int)EnumDeviceType.Multi:
                case (int)EnumDeviceType.Fence:
                case (int)EnumDeviceType.Underground:
                case (int)EnumDeviceType.Contact:
                case (int)EnumDeviceType.PIR:
                case (int)EnumDeviceType.IoController:
                case (int)EnumDeviceType.Laser:
                //case (int)EnumDeviceType.Cable:
                    {
                        ///////////////////Provider Setting/////////////////////
                        var provider = _symbolSensorProvider;
                        ////////////////////////////////////////////////////////
                        var prevSymbol = provider.CollectionEntity.Where(t => t.Id == OnCanvasPreviewSymbol.Id).FirstOrDefault();

                        if (prevSymbol != null)
                        {
                            provider.Remove(prevSymbol);
                            prevSymbol.Deactivate();
                            prevSymbol = null;
                        }

                        ///////////////////Type Generation//////////////////////
                        var symbol = new SymbolSensorViewModel(OnCanvasPreviewSymbol.SymbolContentControlViewModel, _eventAggregator);
                        ////////////////////////////////////////////////////////
                        symbol.Activate();
                        provider.Add(symbol);

                        ///////////////////Type Settings////////////////////////
                        await _canvasMapEntityProvider.SensorSetupAsync();
                        CanvasSensorViewModel = await Task.Run(() => _canvasMapEntityProvider.MapCanvasSensorViewModel[SelectedMap.MapNumber]);
                        ////////////////////////////////////////////////////////

                        OnCanvasPreviewSymbol.Deactivate();
                        OnCanvasPreviewSymbol = null;
                    }
                    break;
                case (int)EnumDeviceType.IpCamera:
                    {
                        var provider = _symbolCameraProvider;
                        var prevSymbol = provider.CollectionEntity.Where(t => t.Id == OnCanvasPreviewSymbol.Id).FirstOrDefault();

                        if (prevSymbol != null)
                        {
                            provider.Remove(prevSymbol);
                            prevSymbol.Deactivate();
                            prevSymbol = null;
                        }

                        ///////////////////Type Generation//////////////////////
                        var symbol = new SymbolCameraViewModel(OnCanvasPreviewSymbol.SymbolContentControlViewModel, _eventAggregator);
                        ////////////////////////////////////////////////////////
                        symbol.Activate();
                        provider.Add(symbol);

                        ///////////////////Type Settings////////////////////////
                        await _canvasMapEntityProvider.CameraSetupAsync();
                        CanvasCameraViewModel = await Task.Run(() => _canvasMapEntityProvider.MapCanvasCameraViewModel[SelectedMap.MapNumber]);
                        ////////////////////////////////////////////////////////

                        OnCanvasPreviewSymbol.Deactivate();
                        OnCanvasPreviewSymbol = null;
                    }
                    break;
                default:
                    break;

            }
        }
        #endregion
        #region - IHanldes -
        public async Task HandleAsync(OpenMapPropertyMessageModel message, CancellationToken cancellationToken)
        {
            if (!(message.ViewModel is MapContentControlViewModel vm))
                return;

            SelectedMap = vm;

            await MapEntityUpdate(SelectedMap.MapNumber);

            /*await _eventAggregator.PublishOnUIThreadAsync(new OpenProgressPopupMessageModel(), cancellationToken);
            await Task.Delay(100).ContinueWith((t, _) =>
            {
                CanvasMapViewModel = new CanvasMapViewModel(message?.ViewModel as MapContentControlViewModel, _eventAggregator);
                CanvasMapViewModel.ActivateAsync();
                IsVisible = true;

            }, cancellationToken).ContinueWith((t, _) => _eventAggregator.PublishOnUIThreadAsync(new ClosePopupDialogMessageModel(), cancellationToken), cancellationToken);*/
        }

        public async Task HandleAsync(MapTreeRemoveMessageModel message, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                CanvasMapViewModel = null;

            }, cancellationToken);
        }


        public Task HandleAsync(OnActivePreviewSymbolMessageModel message, CancellationToken cancellationToken)
        {
            try
            {
                var vm = message?.ViewModel;
                //string[] nsTargets = { "Controllers", "Cameras" };
                switch (vm.TypeDevice)
                {
                    case (int)EnumDeviceType.NONE:
                        {
                        }
                        break;
                    case (int)EnumDeviceType.Controller:
                        {
                            //var dataContext = IoC.Get<OnCanvasPreviewSymbolViewModel>();
                            //var view = new SymbolCameraView() { DataContext = dataContext };
                            //ViewLocator.AddNamespaceMapping("Ironwall.MapEditor.UI.ViewModels.Symbols", "Ironwall.MapEditor.UI.ViewModels.Symbols.Controllers");
                            OnCanvasPreviewSymbol = new OnPreviewControllerViewModel(vm, _eventAggregator);
                            OnCanvasPreviewSymbol.Activate();
                        }
                        break;
                    case (int)EnumDeviceType.Multi:
                    case (int)EnumDeviceType.Fence:
                    case (int)EnumDeviceType.Underground:
                    case (int)EnumDeviceType.Contact:
                    case (int)EnumDeviceType.PIR:
                    case (int)EnumDeviceType.IoController:
                    case (int)EnumDeviceType.Laser:
                    case (int)EnumDeviceType.Cable:
                        {
                            OnCanvasPreviewSymbol = new OnPreviewSensorViewModel(vm, _eventAggregator);
                            OnCanvasPreviewSymbol.Activate();
                        }
                        break;
                    case (int)EnumDeviceType.IpCamera:
                        {
                            //ViewLocator.AddNamespaceMapping("", ".Cameras");
                            OnCanvasPreviewSymbol = new OnPreviewCameraViewModel(vm, _eventAggregator);
                            OnCanvasPreviewSymbol.Activate();
                        }
                        break;
                    default:
                        break;
                }
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Raised Exception in OnActivePreviewSymbolMessageModel : {ex.Message}");
                return Task.CompletedTask;
            }
            finally
            {
                
            }

        }

        public Task HandleAsync(SymbolContentUpdateMessageModel message, CancellationToken cancellationToken)
        {
            if(OnCanvasPreviewSymbol != null)
                OnCanvasPreviewSymbol.Update();
            
            return Task.CompletedTask;
        }

        public Task HandleAsync(ClearPreviewSymbolMessageModel message, CancellationToken cancellationToken)
        {
            OnCanvasPreviewSymbol?.Deactivate();
            OnCanvasPreviewSymbol = null;
            return Task.CompletedTask;
        }
        #endregion
        #region - Processes -
        private async Task MapEntityUpdate(int mapNumber, CancellationToken cancellationToken = default)
        {
            await _eventAggregator.PublishOnCurrentThreadAsync(new OpenProgressPopupMessageModel(), cancellationToken);
            //Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss.ffff")}] MapEntityUpdate started...");
            await Task.Delay(500);

            CanvasMapViewModel = await Task.Run(() => _canvasMapEntityProvider?.MapCanvasMapViewModel[mapNumber], cancellationToken);
            CanvasControllerViewModel = await Task.Run(() => _canvasMapEntityProvider?.MapCanvasControllerViewModel[mapNumber], cancellationToken);
            CanvasSensorViewModel = await Task.Run(() => _canvasMapEntityProvider?.MapCanvasSensorViewModel[mapNumber], cancellationToken);
            CanvasCameraViewModel = await Task.Run(() => _canvasMapEntityProvider?.MapCanvasCameraViewModel[mapNumber], cancellationToken);
            //CanvasGroupDetectViewModel = await Task.Run(() => _canvasMapEntityProvider.MapCanvasGroupDetectViewModel[mapNumber], cancellationToken);
            //CanvasGroupFaultViewModel = await Task.Run(() => _canvasMapEntityProvider.MapCanvasGroupFaultViewModel[mapNumber], cancellationToken);
            //CanvasGroupLabelViewModel = await Task.Run(() => _canvasMapEntityProvider.MapCanvasGroupLabelViewModel[mapNumber], cancellationToken);

            //Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss.ffff")}] MapEntityUpdate finished...");
            await _eventAggregator.PublishOnCurrentThreadAsync(new ClosePopupDialogMessageModel(), cancellationToken);
        }
        #endregion
        #region - Properties -

        #region - SuppressedCode-
        /*private OnCanvasPreviewSymbolViewModel _onCanvasPreviewSymbolViewModel;
        public OnCanvasPreviewSymbolViewModel OnCanvasPreviewSymbolViewModel
        {
            get { return _onCanvasPreviewSymbolViewModel; }
            set
            {
                _onCanvasPreviewSymbolViewModel = value;
                NotifyOfPropertyChange(() => OnCanvasPreviewSymbolViewModel);
            }
        }*/
        #endregion

        private OnPreviewBase _onCanvasPreviewSymbol;

        public OnPreviewBase OnCanvasPreviewSymbol
        {
            get { return _onCanvasPreviewSymbol; }
            set 
            { 
                _onCanvasPreviewSymbol = value;
                NotifyOfPropertyChange(() => OnCanvasPreviewSymbol);
            }
        }

        public CanvasMapViewModel CanvasMapViewModel
        {
            get { return _canvasMapViewModel; }
            set
            {
                _canvasMapViewModel = value;
                NotifyOfPropertyChange(() => CanvasMapViewModel);
            }
        }

        public CanvasControllerViewModel CanvasControllerViewModel
        {
            get { return _canvasControllerViewModel; }
            set
            {
                _canvasControllerViewModel = value;
                NotifyOfPropertyChange(() => CanvasControllerViewModel);
            }
        }

        public CanvasSensorViewModel CanvasSensorViewModel
        {
            get { return _canvasSensorViewModel; }
            set 
            { 
                _canvasSensorViewModel = value;
                NotifyOfPropertyChange(() => CanvasSensorViewModel);
            }
        }


        public CanvasCameraViewModel CanvasCameraViewModel
        {
            get { return _canvasCameraViewModel; }
            set 
            { 
                _canvasCameraViewModel = value;
                NotifyOfPropertyChange(() => CanvasCameraViewModel);
            }
        }


        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                NotifyOfPropertyChange(() => IsVisible);
            }
        }

        public double X
        {
            get => _x;
            set
            {
                _x = value;
                NotifyOfPropertyChange(() => X);
            }
        }

        public double Y
        {
            get => _y;
            set
            {
                _y = value;
                NotifyOfPropertyChange(() => Y);
            }
        }


        public MapContentControlViewModel SelectedMap
        {
            get { return _selectedMap; }
            set 
            { 
                _selectedMap = value;
                NotifyOfPropertyChange(() => SelectedMap);
            }
        }


        public bool Coordinate { get; }

        public MapProvider MapProvider { get; set; }


        #endregion
        #region - Attributes -
        
        private MapProvider _mapProvider;
        private ControllerProvider _controllerProvider;
        private MapContentControlViewModel _selectedMap;
        private SymbolControllerProvider _symbolControllerProvider;
        private SymbolSensorProvider _symbolSensorProvider;
        private SymbolCameraProvider _symbolCameraProvider;
        private CanvasMapEntityProvider _canvasMapEntityProvider;
        private bool _isVisible;

        private double _x;
        private double _y;


        private CanvasMapViewModel _canvasMapViewModel;
        private CanvasControllerViewModel _canvasControllerViewModel;
        private CanvasSensorViewModel _canvasSensorViewModel;
        private CanvasCameraViewModel _canvasCameraViewModel;


        #endregion
    }
}
