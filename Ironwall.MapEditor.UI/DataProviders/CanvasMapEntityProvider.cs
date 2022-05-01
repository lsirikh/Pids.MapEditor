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
            , SymbolControllerProvider symbolControllerProvider
            , SymbolSensorProvider symbolSensorProvider
            , SymbolCameraProvider symbolCameraProvider)
        {
            _mapProvider = mapProvider;
            _symbolControllerProvider = symbolControllerProvider;
            _symbolSensorProvider = symbolSensorProvider;
            _symbolCameraProvider = symbolCameraProvider;
            
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);



            MapCanvasControllerViewModel = new Dictionary<int, Task<CanvasControllerViewModel>>();
            MapCanvasControllerViewModel = new Dictionary<int, Task<CanvasControllerViewModel>>();
            MapCanvasSensorViewModel = new Dictionary<int, Task<CanvasSensorViewModel>>();
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        /// <summary>
        /// 전체 엔티티 업데이트
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SetupAsync(CancellationToken cancellationToken = default)
        {
            return Task.Run(async() => {
                await MapSetupAsync(cancellationToken);
                await ControllerSetupAsync(cancellationToken);
                await SensorSetupAsync(cancellationToken);
                await CameraSetupAsync(cancellationToken);
                /*for (int i = 0; i < _mapProvider.Count; i++)
                {
                    var mapNumber = i + 1;
                    await MapSetupAsync(cancellationToken);
                    await ControllerSetupAsync(cancellationToken);
                    await SensorSetupAsync(cancellationToken);
                    await CameraSetupAsync(cancellationToken);

                    #region - Deprecated Code -
                    //Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss.ffff")}]MapCanvasSensorViewModel.Add");
                    //MapCanvasMapViewModel.Add(mapNumber, MappedMapProvider(mapNumber, _mapProvider));
                    //MapCanvasControllerViewModel.Add(mapNumber, MappedSymbolControllerProvider(mapNumber, _symbolControllerProvider));
                    //MapCanvasSensorViewModel.Add(mapNumber, MappedSymbolSensorProvider(mapNumber, _symbolSensorProvider));
                    //MapCanvasCameraViewModel.Add(mapNumber, MappedSymbolCameraProvider(mapNumber, _symbolCameraProvider));
                    //MapCanvasGroupDetectViewModel.Add(mapNumber, MappdedSymbolGroupDetectProvider(mapNumber, _symbolGroupDetectProvider));
                    //MapCanvasGroupFaultViewModel.Add(mapNumber, MappedSymbolGroupFaultProvider(mapNumber, _symbolGroupFaultProvider));
                    //MapCanvasGroupLabelViewModel.Add(mapNumber, MappedSymbolGroupLabelProvider(mapNumber, _symbolGroupLabelProvider));
                    #endregion
                }*/
            }, cancellationToken);
        }

        
        /// <summary>
        /// 맵 엔티티 업데이트
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task MapSetupAsync(CancellationToken cancellationToken = default)
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

        /// <summary>
        /// 제어기 엔터티 업데이트
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task ControllerSetupAsync(CancellationToken cancellationToken = default)
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

        /// <summary>
        /// 센서 엔티티 업데이트
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SensorSetupAsync(CancellationToken cancellationToken = default)
        {
            MapCanvasSensorViewModel = new Dictionary<int, Task<CanvasSensorViewModel>>();

            var task = Task.Run(() =>
            {
                for (int i = 0; i < _mapProvider.Count; i++)
                {
                    var mapNumber = i + 1;
                    MapCanvasSensorViewModel.Add(mapNumber, MappedSymbolSensorProvider(mapNumber, _symbolSensorProvider));
                }
            }, cancellationToken);

            await task;
        }


        /// <summary>
        /// 카메라 엔티티 업데이트
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task CameraSetupAsync(CancellationToken cancellationToken = default)
        {
            MapCanvasCameraViewModel = new Dictionary<int, Task<CanvasCameraViewModel>>();

            var task = Task.Run(() =>
            {
                for (int i = 0; i < _mapProvider.Count; i++)
                {
                    var mapNumber = i + 1;
                    MapCanvasCameraViewModel.Add(mapNumber, MappedSymbolCameraProvider(mapNumber, _symbolCameraProvider));
                }
            }, cancellationToken);

            await task;
        }

        /// <summary>
        /// 맵 번호에 따른 맵핑 맵 데이터 생성
        /// </summary>
        /// <param name="mapNumber"></param>
        /// <param name="inputProvider"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 맵 번호에 따른 맵핑 제어기 데이터 생성
        /// </summary>
        /// <param name="mapNumber"></param>
        /// <param name="inputProvider"></param>
        /// <returns></returns>
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

        private async Task<CanvasSensorViewModel> MappedSymbolSensorProvider(int mapNumber, EntityCollectionProvider<SymbolSensorViewModel> inputProvider)
        {
            if (!(inputProvider != null || inputProvider.Count > 0))
                throw new Exception();

            SymbolSensorProvider provider = new SymbolSensorProvider();

            Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss.ffff")}]MappedSymbolSensorProvider started!");

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
                Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss.ffff")}]MappedSymbolSensorProvider finished!");

            return new CanvasSensorViewModel(provider);
        }

        /// <summary>
        /// 맵 번호에 따른 맵핑 카메라 데이터 생성
        /// </summary>
        /// <param name="mapNumber"></param>
        /// <param name="inputProvider"></param>
        /// <returns></returns>
        private async Task<CanvasCameraViewModel> MappedSymbolCameraProvider(int mapNumber, SymbolCameraProvider inputProvider)
        {
            if (!(inputProvider != null || inputProvider.Count > 0))
                throw new Exception();

            SymbolCameraProvider provider = new SymbolCameraProvider();

            Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss.ffff")}]MappedSymbolCameraProvider started!");

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
                Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss.ffff")}]MappedSymbolCameraProvider finished!");

            return new CanvasCameraViewModel(provider);
        }

        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        public async Task HandleAsync(MapContentAddMessageModel message, CancellationToken cancellationToken)
        {
            await SetupAsync();
        }
        #endregion
        #region - Properties -

        public Dictionary<int, Task<CanvasMapViewModel>> MapCanvasMapViewModel { get; set; }
        public Dictionary<int, Task<CanvasControllerViewModel>> MapCanvasControllerViewModel { get; set; }
        public Dictionary<int, Task<CanvasSensorViewModel>> MapCanvasSensorViewModel { get; set; }
        public Dictionary<int, Task<CanvasCameraViewModel>> MapCanvasCameraViewModel { get; set; }
        #endregion
        #region - Attributes -
        private MapProvider _mapProvider;
        private SymbolControllerProvider _symbolControllerProvider;
        private SymbolSensorProvider _symbolSensorProvider;
        private SymbolCameraProvider _symbolCameraProvider;
        private IEventAggregator _eventAggregator;
        #endregion
    }
}
