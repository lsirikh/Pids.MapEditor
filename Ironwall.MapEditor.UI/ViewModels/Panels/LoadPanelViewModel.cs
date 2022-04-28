using Caliburn.Micro;
using Ironwall.Enums;
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
            
            //, CanvasMapEntityProvider canvasMapEntityProvider

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
                    await DoMapTask();

                if (GroupData != null)
                    await DoGroupTask();

                if (ControllerData != null)
                    await DoControllerTask();

                if (SensorData != null)
                    await DoSensorTask();

                if (CameraData != null)
                    await DoCameraTask();

                await Task.Delay(500)
                    .ContinueWith((_, t) => _eventAggregator.PublishOnUIThreadAsync(new ClosePopupDialogMessageModel()), _cancellationTokenSource.Token)
                    .ContinueWith((_, t) => _eventAggregator.PublishOnUIThreadAsync(new ClosePanelMessageModel()), _cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
                throw;
            }
            finally
            {
                await Task.Run(() => _canvasMapEntityProvider.Setup());
                _mapTreeViewModel.SelectOne();
            }
            
        }

        private async Task DoMapTask()
        {
            var task = FileManager.ReadCSVFile<MapModel>(MapData, _cancellationTokenSource.Token);
            var data = await task;
            if (data.Count() > 0)
            {
                RemoveAllTree(_mapTreeViewModel);
                await RemoveCollectionEntity(_mapProvider);
                await InsertCollectionEntity(_mapProvider, data, _eventAggregator);
            }
        }

        private async Task RemoveCollectionEntity(MapProvider provider)
        {
            var task = new Task(async () =>
            {
                try
                {
                    foreach (var item in provider.CollectionEntity)
                    {
                        if (item.IsActive)
                            await item.DeactivateAsync(true);
                    }
                    provider.Clear();
                }
                catch (Exception)
                {
                    throw;
                }
            });

            task.Start();
            await task;
        }

        private async Task InsertCollectionEntity(MapProvider provider, IEnumerable<MapModel> data, IEventAggregator eventAggregator = null, CancellationToken token = default)
        {
            var task = new Task(async () =>
            {
                try
                {
                    ///MapProvider를 삭제하기 위해서 삭제 권한을 갖는 구조를 단순화
                    ///Add starting point and Remove starting point를 잡아야 한다.
                    foreach (var item in data)
                    {
                        if (token.IsCancellationRequested)
                            return;

                        var contentControlViewModel = new MapContentControlViewModel(item, eventAggregator);
                        await contentControlViewModel.ActivateAsync();
                        provider.Add(contentControlViewModel);

                        ///TreeContentControlViewModel 생성
                        await AddTreeNode(contentControlViewModel, _mapTreeViewModel);
                    }
                    return;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raise Exception in InsertMapCollectionEntity : {ex.Message}");
                }
            }, token);

            task.Start();
            await task;
        }

        private async Task AddTreeNode(MapContentControlViewModel contentControlViewModel, MapTreeViewModel treeViewModel)
        {
            var id = contentControlViewModel.Id;
            var treeParent = treeViewModel.Items.FirstOrDefault();
            var treeNode = new TreeContentControlViewModel(TreeManager.SetTreeMapId(id), contentControlViewModel.MapName, contentControlViewModel.Url, EnumTreeType.LEAF, true, true, treeParent, EnumDataType.Map, _eventAggregator, _mapProvider) { DisplayName = $"[{EnumTreeType.LEAF.ToString()}]{id} {EnumDataType.Map.ToString()}" };
            ///TreeNode 활성화
            await treeNode?.ActivateAsync();

            DispatcherService.Invoke((System.Action)(() =>
            {
                ///UI 쓰레드에 의해 점유중인 자원에 대해선 자원에 
                ///수정을 가할 때 UI 쓰레드의 Dispatcher에 작업을 위임해야 한다. 

                ///TreeNode 추가
                treeParent.Children.Add(treeNode);
            }));
        }

        private async Task DoControllerTask()
        {
            var task = FileManager.ReadCSVFile<SymbolModel>(ControllerData, _cancellationTokenSource.Token);
            var data = await task;
            if (data.Count() > 0)
            {
                RemoveAllTree(_deviceTreeViewModel);
                await RemoveCollectionEntity(_controllerProvider);
                await InsertCollectionEntity(_controllerProvider, data, _eventAggregator);
            }
        }

        private async Task RemoveCollectionEntity(ControllerProvider provider)
        {
            var task = new Task(async () =>
            {
                try
                {
                    foreach (var item in provider.CollectionEntity)
                    {
                        if (item.IsActive)
                            await item.DeactivateAsync(true);
                    }
                    provider.Clear();
                }
                catch (Exception)
                {
                    throw;
                }
            });

            task.Start();
            await task;
        }

        private async Task InsertCollectionEntity(ControllerProvider provider, IEnumerable<IEntityModel> data, IEventAggregator eventAggregator, CancellationToken token = default)
        {
            var task = new Task(async () =>
            {
                try
                {
                    ///MapProvider를 삭제하기 위해서 삭제 권한을 갖는 구조를 단순화
                    ///Add starting point and Remove starting point를 잡아야 한다.
                    foreach (var item in data)
                    {
                        if (token.IsCancellationRequested)
                            return;

                        var contentControlViewModel = new ControllerContentControlViewModel(item, eventAggregator, _controllerProvider, _mapProvider);

                        await contentControlViewModel.ActivateAsync();

                        provider.Add(contentControlViewModel);

                        ///TreeContentControlViewModel 생성
                        await AddTreeNode(contentControlViewModel, _deviceTreeViewModel);
                    }
                    return;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raise Exception in InsertMapCollectionEntity : {ex.Message}");
                }
            }, token);

            task.Start();
            await task;
        }

        private async Task AddTreeNode(ControllerContentControlViewModel contentControlViewModel, DeviceTreeViewModel treeViewModel)
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
                ///UI 쓰레드에 의해 점유중인 자원에 대해선 자원에 
                ///수정을 가할 때 UI 쓰레드의 Dispatcher에 작업을 위임해야 한다. 

                ///TreeNode 추가
                treeParent.Children.Add(treeNode);
            }));
        }

        private async Task DoSensorTask()
        {
            var task = FileManager.ReadCSVFile<SymbolModel>(SensorData, _cancellationTokenSource.Token);
            var data = await task;
            if (data.Count() > 0)
            {
                RemoveAllSensorTree(_deviceTreeViewModel);
                await RemoveCollectionEntity(_sensorProvider);
                await InsertCollectionEntity(_sensorProvider, data, _eventAggregator);
            }
        }

        private async Task RemoveCollectionEntity(SensorProvider provider)
        {
            var task = new Task(async () =>
            {
                try
                {
                    foreach (var item in provider.CollectionEntity)
                    {
                        if (item.IsActive)
                            await item.DeactivateAsync(true);
                    }
                    provider.Clear();
                }
                catch (Exception)
                {
                    throw;
                }
            });

            task.Start();
            await task;
        }

        private async Task InsertCollectionEntity(SensorProvider provider, IEnumerable<IEntityModel> data, IEventAggregator eventAggregator, CancellationToken token = default)
        {
            var task = new Task(async () =>
            {
                try
                {
                    ///MapProvider를 삭제하기 위해서 삭제 권한을 갖는 구조를 단순화
                    ///Add starting point and Remove starting point를 잡아야 한다.
                    foreach (var item in data)
                    {
                        if (token.IsCancellationRequested)
                            return;

                        var contentControlViewModel = new SensorContentControlViewModel(item, eventAggregator, _sensorProvider, _groupProvider, _controllerProvider, _mapProvider);
                        await contentControlViewModel.ActivateAsync();
                        provider.Add(contentControlViewModel);

                        ///TreeContentControlViewModel 생성
                        await AddTreeNode(contentControlViewModel, _deviceTreeViewModel);
                    }
                    return;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raise Exception in InsertMapCollectionEntity : {ex.Message}");
                }
            }, token);

            task.Start();
            await task;
        }

        private async Task AddTreeNode(SensorContentControlViewModel contentControlViewModel, DeviceTreeViewModel treeViewModel)
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
                ///UI 쓰레드에 의해 점유중인 자원에 대해선 자원에 
                ///수정을 가할 때 UI 쓰레드의 Dispatcher에 작업을 위임해야 한다. 

                ///TreeNode 추가
                treeParent.Children.Add(treeNode);
            }));

            await _eventAggregator.PublishOnUIThreadAsync(new SymbolContentUpdateMessageModel(contentControlViewModel), _cancellationTokenSource.Token);
        }

        private async Task DoGroupTask()
        {
            var task = FileManager.ReadCSVFile<SymbolModel>(GroupData, _cancellationTokenSource.Token);
            var data = await task;
            if (data.Count() > 0)
            {
                RemoveAllTree(_groupTreeViewModel);
                await RemoveCollectionEntity(_groupProvider);
                await InsertCollectionEntity(_groupProvider, data, _eventAggregator);
            }
        }

        private async Task RemoveCollectionEntity(GroupProvider provider)
        {
            var task = new Task(async () =>
            {
                try
                {
                    foreach (var item in provider.CollectionEntity)
                    {
                        if (item.IsActive)
                            await item.DeactivateAsync(true);
                    }
                    provider.Clear();
                }
                catch (Exception)
                {
                    throw;
                }
            });
            task.Start();
            await task;
        }

        private async Task InsertCollectionEntity(GroupProvider provider, IEnumerable<IEntityModel> data, IEventAggregator eventAggregator, CancellationToken token = default)
        {
            var task = new Task(async () =>
            {
                try
                {
                    ///MapProvider를 삭제하기 위해서 삭제 권한을 갖는 구조를 단순화
                    ///Add starting point and Remove starting point를 잡아야 한다.
                    foreach (var item in data)
                    {
                        if (token.IsCancellationRequested)
                            return;

                        var contentControlViewModel = new GroupContentControlViewModel(item, eventAggregator, _groupProvider, _mapProvider);
                        await contentControlViewModel.ActivateAsync();
                        provider.Add(contentControlViewModel);

                        ///TreeContentControlViewModel 생성
                        await AddTreeNode(contentControlViewModel, _groupTreeViewModel);
                    }
                    return;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raise Exception in InsertMapCollectionEntity : {ex.Message}");
                }
            }, token);

            task.Start();
            await task;
        }

        private async Task AddTreeNode(GroupContentControlViewModel contentControlViewModel, GroupTreeViewModel treeViewModel)
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
                ///UI 쓰레드에 의해 점유중인 자원에 대해선 자원에 
                ///수정을 가할 때 UI 쓰레드의 Dispatcher에 작업을 위임해야 한다. 

                ///TreeNode 추가
                treeParent.Children.Add(treeNode);
            }));
        }

        private async Task DoCameraTask()
        {
            var task = FileManager.ReadCSVFile<SymbolModel>(CameraData, _cancellationTokenSource.Token);
            var data = await task;
            if (data.Count() > 0)
            {
                RemoveAllTree(_cameraTreeViewModel);
                await RemoveCollectionEntity(_cameraProvider);
                await InsertCollectionEntity(_cameraProvider, data, _eventAggregator);
            }
        }

        private async Task RemoveCollectionEntity(CameraProvider provider)
        {
            var task = new Task(async () =>
            {
                try
                {
                    foreach (var item in provider.CollectionEntity)
                    {
                        if (item.IsActive)
                            await item.DeactivateAsync(true);
                    }
                    provider.Clear();
                }
                catch (Exception)
                {
                    throw;
                }
            });

            task.Start();
            await task;
        }

        private async Task InsertCollectionEntity(CameraProvider provider, IEnumerable<IEntityModel> data, IEventAggregator eventAggregator, CancellationToken token = default)
        {
            var task = new Task(async () =>
            {
                try
                {
                    ///MapProvider를 삭제하기 위해서 삭제 권한을 갖는 구조를 단순화
                    ///Add starting point and Remove starting point를 잡아야 한다.
                    foreach (var item in data)
                    {
                        if (token.IsCancellationRequested)
                            return;

                        var contentControlViewModel = new CameraContentControlViewModel(item, eventAggregator, _cameraProvider, _mapProvider);
                        await contentControlViewModel.ActivateAsync();
                        provider.Add(contentControlViewModel);

                        ///TreeContentControlViewModel 생성
                        await AddTreeNode(contentControlViewModel, _cameraTreeViewModel);
                    }
                    return;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raise Exception in InsertMapCollectionEntity : {ex.Message}");
                }
            }, token);

            task.Start();
            await task;
        }

        private async Task AddTreeNode(CameraContentControlViewModel contentControlViewModel, CameraTreeViewModel treeViewModel)
        {
            ///TreeContentControlViewModel 생성
            var id = contentControlViewModel.Id;
            var treeParent = treeViewModel.Items.FirstOrDefault();
            var treeNode = new TreeContentControlViewModel(TreeManager.SetTreeCameraId(id), contentControlViewModel.NameDevice, contentControlViewModel.NameDevice, EnumTreeType.LEAF, true, true, treeParent, EnumDataType.Camera, _eventAggregator, _mapProvider, _cameraProvider) { DisplayName = $"[{EnumTreeType.LEAF.ToString()}]{id} {EnumDataType.Camera.ToString()}" };
            ///TreeNode 활성화
            await treeNode?.ActivateAsync();
            DispatcherService.Invoke((System.Action)(() =>
            {
                ///UI 쓰레드에 의해 점유중인 자원에 대해선 자원에 
                ///수정을 가할 때 UI 쓰레드의 Dispatcher에 작업을 위임해야 한다. 

                ///TreeNode 추가
                treeParent.Children.Add(treeNode);
            }));
        }

        private void RemoveAllTree(MapTreeViewModel treeViewModel)
        {
            var treeRoot = treeViewModel.Items.FirstOrDefault();
            TreeManager.SetTreeClear(treeRoot.Children);
        }
        private void RemoveAllTree(DeviceTreeViewModel treeViewModel)
        {
            var treeRoot = treeViewModel.Items.FirstOrDefault();
            TreeManager.SetTreeClear(treeRoot.Children);
        }
        private void RemoveAllSensorTree(DeviceTreeViewModel treeViewModel)
        {
            var treeRoot = treeViewModel.Items.FirstOrDefault();
            foreach (var item in treeRoot.Children)
            {
                TreeManager.SetTreeClear(item.Children);
            }
        }
        private void RemoveAllTree(GroupTreeViewModel treeViewModel)
        {
            var treeRoot = treeViewModel.Items.FirstOrDefault();
            TreeManager.SetTreeClear(treeRoot.Children);
        }
        private void RemoveAllTree(CameraTreeViewModel treeViewModel)
        {
            var treeRoot = treeViewModel.Items.FirstOrDefault();
            TreeManager.SetTreeClear(treeRoot.Children);
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
        public bool CanClickToLoadMapData => true;

        public void ClickToLoadMapData()
        {
            MapData = SetFileLoader();
            NotifyOfPropertyChange(() => MapData);
        }

        public bool CanClickToLoadControllerData => true;

        public void ClickToLoadControllerData()
        {
            ControllerData = SetFileLoader();
            NotifyOfPropertyChange(() => ControllerData);
        }

        public bool CanClickToLoadSensorData => true;

        public void ClickToLoadSensorData()
        {
            SensorData = SetFileLoader();
            NotifyOfPropertyChange(() => SensorData);
        }

        public bool CanClickToLoadGroupData => true;

        public void ClickToLoadGroupData()
        {
            GroupData = SetFileLoader();
            NotifyOfPropertyChange(() => GroupData);
        }

        public bool CanClickToLoadCameraData => true;

        public void ClickToLoadCameraData()
        {
            CameraData = SetFileLoader();
            NotifyOfPropertyChange(() => CameraData);
        }
        #endregion
        #region - Processes -

        private string SetFileLoader()
        {
            #region - 파일 불러오기 -
            var dialog = new OpenFileDialog();
            dialog.Filter = "Csv (*.csv;*.txt;)|*.CSV;*.TXT;|" +
                            "All files (*.*)|*.*";
            dialog.Title = "CSV 파일 선택하기";
            dialog.CheckFileExists = true; // 파일 존재 여부 확인
            dialog.CheckPathExists = true; // 폴더 존재 여부 확인
            dialog.InitialDirectory = Directory.GetCurrentDirectory();

            dialog.RestoreDirectory = true; // 폴더 위치 저장하기

            if (dialog.ShowDialog() == true)
            {
                var relativeUrl = dialog.FileName;
                FileInfo fi = new FileInfo(relativeUrl);
                if (fi.Exists)
                {
                    //File경로와 File명을 모두 가지고 온다.
                    return Path.GetFullPath(relativeUrl);
                }
            }
            return null;
            #endregion
        }
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
        private CanvasMapEntityProvider _canvasMapEntityProvider;
        private MapTreeViewModel _mapTreeViewModel;
        private DeviceTreeViewModel _deviceTreeViewModel;
        private GroupTreeViewModel _groupTreeViewModel;
        private CameraTreeViewModel _cameraTreeViewModel;
        #endregion
    }
}
