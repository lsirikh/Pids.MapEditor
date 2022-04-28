using Caliburn.Micro;
using Ironwall.Enums;
using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models;
using Ironwall.Framework.ViewModels;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.MapEditor.UI.DataProviders;
using Ironwall.MapEditor.UI.Helpers;
using Ironwall.MapEditor.UI.Models.Messages.PopupDialogs;
using Ironwall.MapEditor.UI.Models.Messages.Process;
using Ironwall.MapEditor.UI.Models.Messages.Sections;
using Ironwall.MapEditor.UI.ViewModels.Canvases;
using Ironwall.MapEditor.UI.ViewModels.ContentControls;
using Ironwall.MapEditor.UI.ViewModels.Symbols;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
            _canvasMapEntityProvider = canvasMapEntityProvider;
            //CanvasControllerViewModel = canvasControllerViewModel;
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
            if (OnCanvasPreviewSymbolViewModel != null)
            {
                OnCanvasPreviewSymbolViewModel.X = X - (OnCanvasPreviewSymbolViewModel.Width / 2);
                OnCanvasPreviewSymbolViewModel.X1 = X - (OnCanvasPreviewSymbolViewModel.Width / 2);
                OnCanvasPreviewSymbolViewModel.Y = Y - (OnCanvasPreviewSymbolViewModel.Height / 2);
                OnCanvasPreviewSymbolViewModel.Y1 = Y - (OnCanvasPreviewSymbolViewModel.Height / 2);
                //NotifyOfPropertyChange(() => OnCanvasPreviewSymbolViewModel);
            }
        }

        public async void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (OnCanvasPreviewSymbolViewModel == null)
                return;

            switch (OnCanvasPreviewSymbolViewModel.TypeDevice)
            {
                case (int)EnumDeviceType.NONE:
                    {
                    }
                    break;
                case (int)EnumDeviceType.Controller:
                    {
                        var prevSymbol = _symbolControllerProvider.CollectionEntity.Where(t => t.IdController == OnCanvasPreviewSymbolViewModel.IdController).FirstOrDefault();
                        if(prevSymbol != null)
                        {
                            _symbolControllerProvider.Remove(prevSymbol);
                            prevSymbol.Deactivate();
                            prevSymbol = null;
                        }

                        var symbol = new SymbolControllerViewModel(OnCanvasPreviewSymbolViewModel.SymbolContentControlViewModel, _eventAggregator);
                        symbol.Activate();
                        _symbolControllerProvider.Add(symbol);
                        await _canvasMapEntityProvider.ControllerSetup();

                        CanvasControllerViewModel = await Task.Run(() => _canvasMapEntityProvider.MapCanvasControllerViewModel[SelectedMap.MapNumber]);

                        OnCanvasPreviewSymbolViewModel.Deactivate();
                        OnCanvasPreviewSymbolViewModel = null;
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
                    }
                    break;
                case (int)EnumDeviceType.IpCamera:
                    {

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
            var vm = message?.ViewModel;
            OnCanvasPreviewSymbolViewModel = new OnCanvasPreviewSymbolViewModel(vm, _eventAggregator);
            OnCanvasPreviewSymbolViewModel.Activate();
            return Task.CompletedTask;
        }

        public Task HandleAsync(SymbolContentUpdateMessageModel message, CancellationToken cancellationToken)
        {
            if(OnCanvasPreviewSymbolViewModel != null)
                OnCanvasPreviewSymbolViewModel.Update();
            
            return Task.CompletedTask;
        }

        public Task HandleAsync(ClearPreviewSymbolMessageModel message, CancellationToken cancellationToken)
        {
            OnCanvasPreviewSymbolViewModel.Deactivate();
            OnCanvasPreviewSymbolViewModel = null;
            return Task.CompletedTask;
        }
        #endregion
        #region - Processes -
        private async Task MapEntityUpdate(int mapNumber, CancellationToken cancellationToken = default)
        {
            await _eventAggregator.PublishOnCurrentThreadAsync(new OpenProgressPopupMessageModel(), cancellationToken);
            //Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss.ffff")}] MapEntityUpdate started...");
            CanvasMapViewModel = await Task.Run(() => _canvasMapEntityProvider.MapCanvasMapViewModel[mapNumber], cancellationToken);
            CanvasControllerViewModel = await Task.Run(() => _canvasMapEntityProvider.MapCanvasControllerViewModel[mapNumber], cancellationToken);
            //CanvasSensorViewModel = await Task.Run(() => _canvasMapEntityProvider.MapCanvasSensorViewModel[mapNumber], cancellationToken);
            //CanvasCameraViewModel = await Task.Run(() => _canvasMapEntityProvider.MapCanvasCameraViewModel[mapNumber], cancellationToken);
            //CanvasGroupDetectViewModel = await Task.Run(() => _canvasMapEntityProvider.MapCanvasGroupDetectViewModel[mapNumber], cancellationToken);
            //CanvasGroupFaultViewModel = await Task.Run(() => _canvasMapEntityProvider.MapCanvasGroupFaultViewModel[mapNumber], cancellationToken);
            //CanvasGroupLabelViewModel = await Task.Run(() => _canvasMapEntityProvider.MapCanvasGroupLabelViewModel[mapNumber], cancellationToken);

            //Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss.ffff")}] MapEntityUpdate finished...");
            await _eventAggregator.PublishOnCurrentThreadAsync(new ClosePopupDialogMessageModel(), cancellationToken);
        }
        #endregion
        #region - Properties -
        public OnCanvasPreviewSymbolViewModel OnCanvasPreviewSymbolViewModel
        {
            get { return _onCanvasPreviewSymbolViewModel; }
            set
            {
                _onCanvasPreviewSymbolViewModel = value;
                NotifyOfPropertyChange(() => OnCanvasPreviewSymbolViewModel);

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

        private MapContentControlViewModel _selectedMap;

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
        private OnCanvasPreviewSymbolViewModel _onCanvasPreviewSymbolViewModel;
        private MapProvider _mapProvider;
        private ControllerProvider _controllerProvider;
        private SymbolControllerProvider _symbolControllerProvider;
        private CanvasMapEntityProvider _canvasMapEntityProvider;
        private bool _isVisible;

        private double _x;
        private double _y;


        private CanvasMapViewModel _canvasMapViewModel;
        private CanvasControllerViewModel _canvasControllerViewModel;


        #endregion
    }
}
