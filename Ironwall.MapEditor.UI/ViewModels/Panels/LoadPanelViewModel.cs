using Caliburn.Micro;
using Ironwall.Enums;
using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Helpers;
using Ironwall.Framework.Models;
using Ironwall.Framework.Services;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.MapEditor.Ui.Helpers;
using Ironwall.MapEditor.UI.DataProviders;
using Ironwall.MapEditor.UI.Helpers;
using Ironwall.MapEditor.UI.Models;
using Ironwall.MapEditor.UI.Models.Messages.Panels;
using Ironwall.MapEditor.UI.Models.Messages.PopupDialogs;
using Ironwall.MapEditor.UI.Models.Messages.Process;
using Ironwall.MapEditor.UI.ViewModels.ContentControls;
using Ironwall.MapEditor.UI.ViewModels.RegisteredItems;
using Ironwall.MapEditor.UI.ViewModels.Symbols;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.ViewModels.Panels
{
    internal sealed class LoadPanelViewModel
        : BaseViewModel
        , IPanelViewModel
    {

        #region - Ctors -
        public LoadPanelViewModel(
            MapProvider mapProvider
            , ControllerProvider controllerProvider
            , SensorProvider sensorProvider
            , GroupProvider groupProvider
            , CameraProvider cameraProvider

            , SymbolControllerProvider symbolControllerProvider
            , SymbolSensorProvider symbolSensorProvider
            //, SymbolGroupProvider symbolGroupProvider
            , SymbolCameraProvider symbolCameraProvider

            , MapTreeViewModel mapTreeViewModel
            , DeviceTreeViewModel deviceTreeViewModel
            , GroupTreeViewModel groupTreeViewModel
            , CameraTreeViewModel cameraTreeViewModel

            , CanvasMapEntityProvider canvasMapEntityProvider
            , IEventAggregator eventAggregator)
        {
            #region - Settings -
            Id = 17;
            Content = "";
            Category = CategoryEnum.PANEL_SHELL_VM_ITEM;
            #endregion - Settings -

            _eventAggregator = eventAggregator;

            _mapProvider = mapProvider;
            _controllerProvider = controllerProvider;
            _sensorProvider = sensorProvider;
            _groupProvider = groupProvider;
            _cameraProvider = cameraProvider;

            _symbolControllerProvider = symbolControllerProvider;
            _symbolSensorProvider = symbolSensorProvider;
            _symbolCameraProvider = symbolCameraProvider;

            _canvasMapEntityProvider = canvasMapEntityProvider;

            _mapTreeViewModel = mapTreeViewModel;
            _deviceTreeViewModel = deviceTreeViewModel;
            _groupTreeViewModel = groupTreeViewModel;
            _cameraTreeViewModel = cameraTreeViewModel;
        }
        #endregion
        #region - Implementation of Interface -
        public void ModelUpdate()
        {
            NotifyOfPropertyChange(() => MapData);
            NotifyOfPropertyChange(() => ControllerData);
            NotifyOfPropertyChange(() => SensorData);
            NotifyOfPropertyChange(() => GroupData);
            NotifyOfPropertyChange(() => CameraData);
        }

        /// <summary>
        /// NotifyOfPropertyChange(nameof(CanClickOk))이 있어야 갱신이 가능하다.
        /// </summary>
        public bool CanClickOkAsync => true;

        public async void ClickOkAsync()
        {
            try
            {
                ///로딩화면 시현
                await _eventAggregator.PublishOnCurrentThreadAsync(new OpenProgressPopupMessageModel(), _cancellationTokenSource.Token);
                ///파일 불러오기 Task 실행
                if (MapData != null)
                {
                    var task = DoMapTask();
                    await task;
                    Debug.WriteLine($"DoMapTask : {task.Result}");
                }

                if (GroupData != null)
                {
                    var task = DoGroupTask();
                    await task;
                    Debug.WriteLine($"DoGroupTask : {task.Result}");
                }

                if (ControllerData != null)
                {
                    var task = DoControllerTask();
                    await task;
                    Debug.WriteLine($"DoControllerTask : {task.Result}");
                }

                if (SensorData != null)
                {
                    var task = DoSensorTask();
                    await task;
                    Debug.WriteLine($"DoSensorTask : {task.Result}");
                }

                if (CameraData != null)
                {
                    var task = DoCameraTask();
                    await task;
                    Debug.WriteLine($"DoCameraTask : {task.Result}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
                throw;
            }
            finally
            {
                var awaiter = _canvasMapEntityProvider.SetupAsync().GetAwaiter();
                awaiter.OnCompleted(async () =>
                {
                    await _mapTreeViewModel.SelectDefaultMapAsync();
                });
                await _eventAggregator.PublishOnUIThreadAsync(new ClosePopupDialogMessageModel(), _cancellationTokenSource.Token);
                await _eventAggregator.PublishOnUIThreadAsync(new ClosePanelMessageModel(), _cancellationTokenSource.Token);
            }
        }

        private Task<bool> DoMapTask()
        {
            return Task.Run(async () =>
            {
                try
                {
                    var task = FileManager.ReadCSVFile<MapModel>(MapData, _cancellationTokenSource.Token);
                    var data = await task;
                    if (data.Count() > 0)
                    {
                        var treeResult = await RemoveAllTree(_mapTreeViewModel);
                        var contentResult = await RemoveCollectionEntity(_mapProvider);
                        if (treeResult || contentResult)
                            await InsertCollectionEntity(_mapProvider, data, _eventAggregator);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in DoMapTask : {ex.Message}");
                    return false;
                }
            });
        }

        private Task<bool> RemoveCollectionEntity(MapProvider provider)
        {
            return Task.Run(async () =>
            {
                foreach (var item in provider.CollectionEntity)
                {
                    if (item.IsActive)
                        await item.DeactivateAsync(true);
                }
                provider.Clear();
                return true;
            });
        }

        private Task<bool> InsertCollectionEntity(MapProvider provider, IEnumerable<MapModel> data, IEventAggregator eventAggregator = null, CancellationToken token = default)
        {
            return Task.Run(async () =>
            {
                try
                {
                    ///MapProvider를 삭제하기 위해서 삭제 권한을 갖는 구조를 단순화
                    ///Add starting point and Remove starting point를 잡아야 한다.
                    foreach (var item in data)
                    {
                        if (token.IsCancellationRequested)
                            return true;

                        var contentControlViewModel = new MapContentControlViewModel(item, eventAggregator) { DisplayName = $"{item.Id} {EnumDataType.Map.ToString()}", broadCastring = false, };
                        await contentControlViewModel.ActivateAsync();
                        provider.Add(contentControlViewModel);

                        ///TreeContentControlViewModel 생성
                        await AddTreeNode(contentControlViewModel, _mapTreeViewModel);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raise Exception in InsertMapCollectionEntity : {ex.Message}");
                    return false;
                }
            }, cancellationToken: token);
        }

        private Task<bool> AddTreeNode(MapContentControlViewModel contentControlViewModel, MapTreeViewModel treeViewModel)
        {
            return Task.Run(async () =>
            {
                DispatcherService.Invoke((System.Action)(async () =>
                {
                    var id = contentControlViewModel.Id;
                    var treeParent = treeViewModel.Items.FirstOrDefault();
                    var treeNode = new TreeContentControlViewModel(TreeManager.SetTreeMapId(id), contentControlViewModel.MapName, contentControlViewModel.Url, EnumTreeType.LEAF, true, true, treeParent, EnumDataType.Map, _eventAggregator, _mapProvider) { DisplayName = $"[{EnumTreeType.LEAF.ToString()}]{id} {EnumDataType.Map.ToString()}" };
                    ///TreeNode 활성화
                    await treeNode?.ActivateAsync();
                    treeParent.Children.Add(treeNode);
                }));
                return true;
            });

        }

        private Task<bool> DoControllerTask()
        {
            return Task.Run(async () =>
            {
                try
                {
                    var task = FileManager.ReadCSVFile<SymbolModel>(ControllerData, _cancellationTokenSource.Token);
                    var data = await task;
                    if (data.Count() > 0)
                    {
                        var treeResult = await RemoveAllTree(_deviceTreeViewModel, "Controller");
                        var contentResult = await RemoveCollectionEntity(_controllerProvider, _symbolControllerProvider);

                        if (treeResult || contentResult)
                            await InsertCollectionEntity(_controllerProvider, _symbolControllerProvider, data, _eventAggregator);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in DoControllerTask : {ex.Message}");
                    return false;
                }
            });
        }

        private Task<bool> RemoveCollectionEntity(ControllerProvider provider, SymbolControllerProvider symbolProvider)
        {
            return Task.Run(async () =>
            {
                ///ContentControlPrivider
                foreach (var item in provider.CollectionEntity)
                {
                    if (item.IsActive)
                        await item.DeactivateAsync(true);
                }
                provider.Clear();

                ///SymbolProvider
                foreach (var symbolItem in symbolProvider.CollectionEntity)
                {
                    symbolItem.Deactivate();
                }
                symbolProvider.Clear();
                return true;
            });
        }

        private Task<bool> InsertCollectionEntity(ControllerProvider provider, SymbolControllerProvider symbolProvider, IEnumerable<IEntityModel> data, IEventAggregator eventAggregator, CancellationToken token = default)
        {
            return Task.Run(async () =>
            {
                try
                {
                    ///MapProvider를 삭제하기 위해서 삭제 권한을 갖는 구조를 단순화
                    ///Add starting point and Remove starting point를 잡아야 한다.
                    foreach (var item in data)
                    {
                        if (token.IsCancellationRequested)
                            return true;

                        var contentControlViewModel = new ControllerContentControlViewModel(item, eventAggregator, _controllerProvider, _mapProvider);

                        await contentControlViewModel.ActivateAsync();

                        provider.Add(contentControlViewModel);

                        var symbol = new SymbolControllerViewModel(contentControlViewModel, eventAggregator);
                        symbol.Activate();
                        symbolProvider.Add(symbol);

                        ///TreeContentControlViewModel 생성
                        await AddTreeNode(contentControlViewModel, _deviceTreeViewModel);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raise Exception in InsertMapCollectionEntity : {ex.Message}");
                    return false;
                }
            }, token);
        }

        private Task<bool> AddTreeNode(ControllerContentControlViewModel contentControlViewModel, DeviceTreeViewModel treeViewModel)
        {
            return Task.Run(async () =>
            {
                ///TreeContentControlViewModel 생성
                var id = contentControlViewModel.Id;
                var treeParent = treeViewModel.Items.FirstOrDefault();
                var treeNode = new TreeContentControlViewModel(TreeManager.SetTreeControllerId(id), contentControlViewModel.IdController.ToString(), contentControlViewModel.NameDevice, EnumTreeType.BRANCH, true, true, treeParent, EnumDataType.Controller, _eventAggregator, _mapProvider, _groupProvider, _controllerProvider, _sensorProvider) { DisplayName = $"[{EnumTreeType.BRANCH.ToString()}]{contentControlViewModel.IdController} {EnumDataType.Controller.ToString()}" };
                ///5
                await treeNode?.ActivateAsync();
                ///6
                DispatcherService.Invoke((System.Action)(() =>
                {
                    ///TreeNode 추가
                    treeParent.Children.Add(treeNode);
                }));

                return true;
            });

        }

        private Task<bool> DoSensorTask()
        {
            return Task.Run(async () =>
            {
                try
                {
                    var task = FileManager.ReadCSVFile<SymbolModel>(SensorData, _cancellationTokenSource.Token);
                    var data = await task;
                    if (data.Count() > 0)
                    {
                        var treeResult = await RemoveAllTree(_deviceTreeViewModel, "Sensor");
                        var contentResult = await RemoveCollectionEntity(_sensorProvider, _symbolSensorProvider);
                        if (treeResult || contentResult)
                            await InsertCollectionEntity(_sensorProvider, _symbolSensorProvider, data, _eventAggregator);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in DoSensorTask : {ex.Message}");
                    return false;
                }
            });

        }

        private Task<bool> RemoveCollectionEntity(SensorProvider provider, SymbolSensorProvider symbolProvider)
        {
            return Task.Run(async () =>
            {
                try
                {
                    ///ContentControlProvider
                    foreach (var item in provider.CollectionEntity)
                    {
                        if (item.IsActive)
                            await item.DeactivateAsync(true);
                    }
                    provider.Clear();

                    ///SymbolProvider
                    foreach (var symbolItem in symbolProvider.CollectionEntity)
                    {
                        symbolItem.Deactivate();
                    }
                    symbolProvider.Clear();

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        private Task InsertCollectionEntity(SensorProvider provider, SymbolSensorProvider symbolProvider, IEnumerable<IEntityModel> data, IEventAggregator eventAggregator, CancellationToken token = default)
        {
            return Task.Run(async () =>
            {
                try
                {
                    ///MapProvider를 삭제하기 위해서 삭제 권한을 갖는 구조를 단순화
                    ///Add starting point and Remove starting point를 잡아야 한다.
                    foreach (var item in data)
                    {
                        if (token.IsCancellationRequested)
                            return Task.CompletedTask;

                        var contentControlViewModel = new SensorContentControlViewModel(item, eventAggregator, _sensorProvider, _groupProvider, _controllerProvider, _mapProvider);
                        await contentControlViewModel.ActivateAsync();
                        provider.Add(contentControlViewModel);

                        var symbol = new SymbolSensorViewModel(contentControlViewModel, eventAggregator);
                        symbol.Activate();
                        symbolProvider.Add(symbol);

                        ///TreeContentControlViewModel 생성
                        await AddTreeNode(contentControlViewModel, _deviceTreeViewModel);
                    }
                    return Task.CompletedTask;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raise Exception in InsertMapCollectionEntity : {ex.Message}");
                    return Task.CompletedTask;
                }
            }, token);
        }

        private Task<bool> AddTreeNode(SensorContentControlViewModel contentControlViewModel, DeviceTreeViewModel treeViewModel)
        {
            return Task.Run(async () =>
            {
                ///TreeContentControlViewModel 생성
                var id = contentControlViewModel.Id;
                var treeRoot = treeViewModel.Items.FirstOrDefault();
                var treeParent = treeRoot.Children.Where(t => t.Name == contentControlViewModel.IdController.ToString()).FirstOrDefault();
                var treeNode = new TreeContentControlViewModel(TreeManager.SetTreeSensorId(id), contentControlViewModel.IdSensor.ToString(), contentControlViewModel.NameDevice, EnumTreeType.LEAF, true, true, treeParent, EnumDataType.Sensor, _eventAggregator, _mapProvider, _groupProvider, _controllerProvider, _sensorProvider) { DisplayName = $"[{EnumTreeType.LEAF.ToString()}]{contentControlViewModel.IdSensor} {EnumDataType.Sensor.ToString()}" };
                ///5
                await treeNode?.ActivateAsync();
                ///6
                DispatcherService.Invoke((System.Action)(() =>
                    {
                        ///TreeNode 추가
                        treeParent.Children.Add(treeNode);
                    }));
                return true;
            });
        }

        private Task<bool> DoGroupTask()
        {
            return Task.Run(async () =>
            {
                try
                {
                    var task = FileManager.ReadCSVFile<SymbolModel>(GroupData, _cancellationTokenSource.Token);
                    var data = await task;
                    if (data.Count() > 0)
                    {
                        var treeResult = await RemoveAllTree(_groupTreeViewModel);
                        var contentResult = await RemoveCollectionEntity(_groupProvider);
                        if (treeResult || contentResult)
                            await InsertCollectionEntity(_groupProvider, data, _eventAggregator);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in DoGroupTask : {ex.Message}");
                    return false;
                }
            });
        }

        private Task<bool> RemoveCollectionEntity(GroupProvider provider
            //, GroupSymbolProvider symbolProvider
            )
        {
            return Task.Run(async () =>
            {
                try
                {
                    ///ContentControlProvider
                    foreach (var item in provider.CollectionEntity)
                    {
                        if (item.IsActive)
                            await item.DeactivateAsync(true);
                    }
                    provider.Clear();

                    ///SymbolProvider
                    /*foreach (var symbolItem in symbolProvider.CollectionEntity)
                    {
                        symbolItem.Deactivate();
                    }
                    symbolProvider.Clear();*/

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        private Task<bool> InsertCollectionEntity(GroupProvider provider, IEnumerable<IEntityModel> data, IEventAggregator eventAggregator, CancellationToken token = default)
        {
            return Task.Run(async () =>
            {
                try
                {
                    ///MapProvider를 삭제하기 위해서 삭제 권한을 갖는 구조를 단순화
                    ///Add starting point and Remove starting point를 잡아야 한다.
                    foreach (var item in data)
                    {
                        if (token.IsCancellationRequested)
                            return false;

                        var contentControlViewModel = new GroupContentControlViewModel(item, eventAggregator, _groupProvider, _mapProvider);
                        await contentControlViewModel.ActivateAsync();
                        provider.Add(contentControlViewModel);

                        /*var symbol = new SymbolGroupViewModel(contentControlViewModel, eventAggregator);
                        symbol.Activate();
                        symbolProvider.Add(symbol);*/

                        ///TreeContentControlViewModel 생성
                        await AddTreeNode(contentControlViewModel, _groupTreeViewModel);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raise Exception in InsertMapCollectionEntity : {ex.Message}");
                    return false;
                }
            }, token);
        }

        private Task<bool> AddTreeNode(GroupContentControlViewModel contentControlViewModel, GroupTreeViewModel treeViewModel)
        {
            return Task.Run(async () =>
            {
                ///TreeContentControlViewModel 생성
                var id = contentControlViewModel.Id;
                var treeParent = treeViewModel.Items.FirstOrDefault();
                var treeNode = new TreeContentControlViewModel(TreeManager.SetTreeGroupId(id), contentControlViewModel.NameArea, contentControlViewModel.NameArea, EnumTreeType.BRANCH, true, true, treeParent, EnumDataType.Group, _eventAggregator, _mapProvider, _groupProvider, _controllerProvider, _sensorProvider) { DisplayName = $"[{EnumTreeType.BRANCH.ToString()}]{id} {EnumDataType.Group.ToString()}" };
                ///5
                await treeNode?.ActivateAsync();
                ///6
                DispatcherService.Invoke((System.Action)(() =>
                {
                    ///TreeNode 추가
                    treeParent.Children.Add(treeNode);
                }));
                return true;
            });
        }

        private Task<bool> DoCameraTask()
        {
            return Task.Run(async () =>
            {
                try
                {
                    var task = FileManager.ReadCSVFile<SymbolModel>(CameraData, _cancellationTokenSource.Token);
                    var data = await task;
                    if (data.Count() > 0)
                    {
                        var treeResult = await RemoveAllTree(_cameraTreeViewModel);
                        var contentResult = await RemoveCollectionEntity(_cameraProvider, _symbolCameraProvider);
                        if (treeResult && contentResult)
                            await InsertCollectionEntity(_cameraProvider, _symbolCameraProvider, data, _eventAggregator);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in DoCameraTask : {ex.Message}");
                    return false;
                }
            });
        }

        private Task<bool> RemoveCollectionEntity(CameraProvider provider, SymbolCameraProvider symbolProvider)
        {
            return Task.Run(async () =>
            {
                try
                {
                    ///ContentControlProvider
                    foreach (var item in provider.CollectionEntity)
                    {
                        if (item.IsActive)
                            await item.DeactivateAsync(true);
                    }
                    provider.Clear();

                    ///SymbolProvider
                    foreach (var symbolItem in symbolProvider.CollectionEntity)
                    {
                        symbolItem.Deactivate();
                    }
                    symbolProvider.Clear();

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        private Task<bool> InsertCollectionEntity(CameraProvider provider, SymbolCameraProvider symbolProvider, IEnumerable<IEntityModel> data, IEventAggregator eventAggregator, CancellationToken token = default)
        {
            return Task.Run(async () =>
            {
                try
                {
                    ///MapProvider를 삭제하기 위해서 삭제 권한을 갖는 구조를 단순화
                    ///Add starting point and Remove starting point를 잡아야 한다.
                    foreach (var item in data)
                    {
                        if (token.IsCancellationRequested)
                            return false;

                        var contentControlViewModel = new CameraContentControlViewModel(item, eventAggregator, _cameraProvider, _mapProvider);
                        await contentControlViewModel.ActivateAsync();
                        provider.Add(contentControlViewModel);

                        var symbol = new SymbolCameraViewModel(contentControlViewModel, eventAggregator);
                        symbol.Activate();
                        symbolProvider.Add(symbol);

                        ///TreeContentControlViewModel 생성
                        await AddTreeNode(contentControlViewModel, _cameraTreeViewModel);

                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raise Exception in InsertMapCollectionEntity : {ex.Message}");
                    return false;
                }
            }, token);
        }

        private Task<bool> AddTreeNode(CameraContentControlViewModel contentControlViewModel, CameraTreeViewModel treeViewModel)
        {
            return Task.Run(async () =>
            {
                ///TreeContentControlViewModel 생성
                var id = contentControlViewModel.Id;
                var treeParent = treeViewModel.Items.FirstOrDefault();
                var treeNode = new TreeContentControlViewModel(TreeManager.SetTreeCameraId(id), contentControlViewModel.NameDevice, contentControlViewModel.NameDevice, EnumTreeType.LEAF, true, true, treeParent, EnumDataType.Camera, _eventAggregator, _mapProvider, _cameraProvider) { DisplayName = $"[{EnumTreeType.LEAF.ToString()}]{id} {EnumDataType.Camera.ToString()}" };
                ///TreeNode 활성화
                await treeNode?.ActivateAsync();
                DispatcherService.Invoke((System.Action)(() =>
                {
                    ///TreeNode 추가
                    treeParent.Children.Add(treeNode);
                }));
                return true;
            });
        }

        private Task<bool> RemoveAllTree(object viewModel, string type = null)
        {
            return Task.Run(() =>
            {
                if (viewModel.GetType() == typeof(MapTreeViewModel))
                {
                    var vm = viewModel as MapTreeViewModel;
                    var treeRoot = vm.Items.FirstOrDefault();
                    TreeManager.SetTreeClear(treeRoot.Children);
                }
                else if (viewModel.GetType() == typeof(DeviceTreeViewModel))
                {
                    var vm = viewModel as DeviceTreeViewModel;
                    if (type == "Controller")
                    {
                        var treeRoot = vm.Items.FirstOrDefault();
                        TreeManager.SetTreeClear(treeRoot.Children);
                    }
                    else
                    {
                        var treeRoot = vm.Items.FirstOrDefault();
                        foreach (var item in treeRoot.Children)
                        {
                            TreeManager.SetTreeClear(item.Children);
                        }
                    }
                }
                else if (viewModel.GetType() == typeof(GroupTreeViewModel))
                {
                    var vm = viewModel as GroupTreeViewModel;
                    var treeRoot = vm.Items.FirstOrDefault();
                    TreeManager.SetTreeClear(treeRoot.Children);
                }
                else if (viewModel.GetType() == typeof(CameraTreeViewModel))
                {
                    var vm = viewModel as CameraTreeViewModel;
                    var treeRoot = vm.Items.FirstOrDefault();
                    TreeManager.SetTreeClear(treeRoot.Children);
                }
                return true;
            });
        }


        public async void ClickCancelAsync()
        {
            ///AddMapPanel 종료
            await _eventAggregator.PublishOnCurrentThreadAsync(new ClosePanelMessageModel(), _cancellationTokenSource.Token);
        }
        #endregion
        #region - Overrides -
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            base.OnActivateAsync(cancellationToken);
            #region - Initialize Properties -
            MapData = null;
            ControllerData = null;
            SensorData = null;
            GroupData = null;
            CameraData = null;

            ModelUpdate();
            #endregion
            return Task.CompletedTask;
        }
        #endregion
        #region - Binding Methods -


        public bool CanClickToLoadData => true;
        public void ClickToLoadData()
        {
            #region - 파일 불러오기 -
            var dialog = new OpenFileDialog();
            dialog.Filter = "Csv (*.csv;*.txt;)|*.CSV;*.TXT;|" +
                            "All files (*.*)|*.*";
            dialog.Title = "CSV 파일 선택하기";
            dialog.CheckFileExists = true; // 파일 존재 여부 확인
            dialog.CheckPathExists = true; // 폴더 존재 여부 확인
            dialog.InitialDirectory = Directory.GetCurrentDirectory() + "\\" + "Csvs";
            dialog.Multiselect = true;

            dialog.RestoreDirectory = true; // 폴더 위치 저장하기

            if (dialog.ShowDialog() == true)
            {
                foreach (var item in dialog.FileNames)
                {
                    FileInfo fi = new FileInfo(item);
                    if (fi.Exists)
                    {
                        //각 프로퍼티에 할당 이름별로

                        //File경로와 File명을 모두 가지고 온다.
                        if (item.Contains("Map_"))
                        {
                            MapData = Path.GetFullPath(item);
                            NotifyOfPropertyChange(() => MapData);
                        }
                        else if (item.Contains("Controller_"))
                        {
                            ControllerData = Path.GetFullPath(item);
                            NotifyOfPropertyChange(() => ControllerData);
                        }
                        else if (item.Contains("Sensor_"))
                        {
                            SensorData = Path.GetFullPath(item);
                            NotifyOfPropertyChange(() => SensorData);
                        }
                        else if (item.Contains("Group_"))
                        {
                            GroupData = Path.GetFullPath(item);
                            NotifyOfPropertyChange(() => GroupData);
                        }
                        else if (item.Contains("Camera_"))
                        {
                            CameraData = Path.GetFullPath(item);
                            NotifyOfPropertyChange(() => CameraData);
                        }
                    }
                }

            }
            #endregion
        }
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public string MapData { get; set; }
        public string ControllerData { get; set; }
        public string SensorData { get; set; }
        public string GroupData { get; set; }
        public string CameraData { get; set; }
        #endregion
        #region - Attributes -
        private MapProvider _mapProvider;
        private ControllerProvider _controllerProvider;
        private SensorProvider _sensorProvider;
        private GroupProvider _groupProvider;
        private CameraProvider _cameraProvider;

        private SymbolControllerProvider _symbolControllerProvider;
        private SymbolSensorProvider _symbolSensorProvider;
        private SymbolCameraProvider _symbolCameraProvider;
        private CanvasMapEntityProvider _canvasMapEntityProvider;

        private MapTreeViewModel _mapTreeViewModel;
        private DeviceTreeViewModel _deviceTreeViewModel;
        private GroupTreeViewModel _groupTreeViewModel;
        private CameraTreeViewModel _cameraTreeViewModel;
        #endregion
    }
}
