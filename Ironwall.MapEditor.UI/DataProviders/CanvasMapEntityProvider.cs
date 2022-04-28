using Caliburn.Micro;
using Ironwall.Framework.DataProviders;
using Ironwall.Framework.ViewModels;
using Ironwall.MapEditor.UI.Models.Messages.Process;
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

namespace Ironwall.MapEditor.UI.DataProviders
{
    internal class CanvasMapEntityProvider
        : IHandle<MapContentAddMessageModel>
    {
        
        #region - Ctors -
        public CanvasMapEntityProvider(
            MapProvider mapProvider
            , IEventAggregator eventAggregator
            , SymbolControllerProvider symbolControllerProvider)
        {
            _mapProvider = mapProvider;
            _symbolControllerProvider = symbolControllerProvider;
            
            
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -

        public async Task Setup(CancellationToken cancellationToken = default)
        {
            var task = Task.Run(async () => {
                for (int i = 0; i < _mapProvider.Count; i++)
                {
                    var mapNumber = i + 1;
                    await MapSetup(cancellationToken);
                    await ControllerSetup(cancellationToken);
                    //Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss.ffff")}]MapCanvasSensorViewModel.Add");
                    //MapCanvasMapViewModel.Add(mapNumber, MappedMapProvider(mapNumber, _mapProvider));
                    //MapCanvasControllerViewModel.Add(mapNumber, MappedSymbolControllerProvider(mapNumber, _symbolControllerProvider));
                    //MapCanvasSensorViewModel.Add(mapNumber, MappedSymbolSensorProvider(mapNumber, _symbolSensorProvider));
                    //MapCanvasCameraViewModel.Add(mapNumber, MappedSymbolCameraProvider(mapNumber, _symbolCameraProvider));
                    //MapCanvasGroupDetectViewModel.Add(mapNumber, MappdedSymbolGroupDetectProvider(mapNumber, _symbolGroupDetectProvider));
                    //MapCanvasGroupFaultViewModel.Add(mapNumber, MappedSymbolGroupFaultProvider(mapNumber, _symbolGroupFaultProvider));
                    //MapCanvasGroupLabelViewModel.Add(mapNumber, MappedSymbolGroupLabelProvider(mapNumber, _symbolGroupLabelProvider));
                }
            }, cancellationToken);

            await task;
        }

        public async Task MapSetup(CancellationToken cancellationToken = default)
        {
            MapCanvasMapViewModel = new Dictionary<int, Task<CanvasMapViewModel>>();
            
            var task = Task.Run(() => {
                for (int i = 0; i < _mapProvider.Count; i++)
                {
                    var mapNumber = i + 1;
                    MapCanvasMapViewModel.Add(mapNumber, MappedMapProvider(mapNumber, _mapProvider));
                }
            }, cancellationToken);
            
            await task;
        }

        public async Task ControllerSetup(CancellationToken cancellationToken = default)
        {
            MapCanvasControllerViewModel = new Dictionary<int, Task<CanvasControllerViewModel>>();

            var task = Task.Run(() => 
            {
                for (int i = 0; i < _mapProvider.Count; i++)
                {
                    var mapNumber = i + 1;
                    MapCanvasControllerViewModel.Add(mapNumber, MappedSymbolControllerProvider(mapNumber, _symbolControllerProvider));
                }
            }, cancellationToken);

            await task;
        }

        private async Task<CanvasMapViewModel> MappedMapProvider(int mapNumber, MapProvider inputProvider)
        {
            if (!(inputProvider != null || inputProvider.Count > 0))
                throw new Exception();

            MapContentControlViewModel mapViewModel = null;

            Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss.ffff")}]MappedMapProvider started!");

            var task = Task.Run(() =>
            {
                foreach (var item in inputProvider)
                {
                    if (item.MapNumber == mapNumber)
                    {
                        mapViewModel = item;
                        break;
                    }
                }
                return true;
            });

            var res = await task;
            if (res)
                Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss.ffff")}]MappedMapProvider finished!");

            return new CanvasMapViewModel(mapViewModel);
        }

        private async Task<CanvasControllerViewModel> MappedSymbolControllerProvider(int mapNumber, EntityCollectionProvider<SymbolControllerViewModel> inputProvider)
        {
            if (!(inputProvider != null || inputProvider.Count > 0))
                throw new Exception();

            SymbolControllerProvider provider = new SymbolControllerProvider();

            Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss.ffff")}]MappedSymbolControllerProvider started!");

            var task = Task.Run(() =>
            {
                foreach (var item in inputProvider)
                {
                    if (item.Map == mapNumber)
                        provider.Add(item);
                }
                return true;
            });

            var res = await task;
            if (res)
                Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss.ffff")}]MappedSymbolControllerProvider finished!");

            return new CanvasControllerViewModel(provider);
        }


        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        public async Task HandleAsync(MapContentAddMessageModel message, CancellationToken cancellationToken)
        {
            await Setup();
        }
        #endregion
        #region - Properties -

        public Dictionary<int, Task<CanvasControllerViewModel>> MapCanvasControllerViewModel { get; set; }
        public Dictionary<int, Task<CanvasMapViewModel>> MapCanvasMapViewModel { get; set; }
        #endregion
        #region - Attributes -
        private MapProvider _mapProvider;
        private SymbolControllerProvider _symbolControllerProvider;
        private IEventAggregator _eventAggregator;
        #endregion
    }
}
