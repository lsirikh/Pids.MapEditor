using Caliburn.Micro;
using Ironwall.Enums;
using Ironwall.Framework.Services;
using Ironwall.MapEditor.Ui.Helpers;
using Ironwall.MapEditor.UI.DataProviders;
using Ironwall.MapEditor.UI.Helpers;
using Ironwall.MapEditor.UI.Models;
using Ironwall.MapEditor.UI.Models.Messages.Process;
using Ironwall.MapEditor.UI.Services;
using Ironwall.MapEditor.UI.ViewModels.ContentControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ironwall.MapEditor.UI.ViewModels.RegisteredItems
{
    public class TreeContentControlViewModel
        : BaseContentControl<ITreeModel>
        , ITreeContentControlViewModel
    {
        #region - Ctors -
        public TreeContentControlViewModel(IEventAggregator eventAggregator = null)
        {
            _eventAggregator = eventAggregator;

            DisplayName = "Untitled Tree Node";
            Model = new TreeModel();
            Children = new TrulyObservableCollection<TreeContentControlViewModel>();

        }

        public TreeContentControlViewModel(string id, string name, string description, EnumTreeType type, bool used, bool visibility, object parentTree, EnumDataType dataType, IEventAggregator eventAggregator = null)
        {
            DisplayName = name;
            _eventAggregator = eventAggregator;

            Model = new TreeModel(id, name, description, type, used, visibility, parentTree, dataType);
            Children = new TrulyObservableCollection<TreeContentControlViewModel>();
            Update();
        }

        public TreeContentControlViewModel(string id, string name, string description, EnumTreeType type, bool used, bool visibility, object parentTree, EnumDataType dataType, IEventAggregator eventAggregator = null, MapProvider mapProvider = null)
        {
            DisplayName = name;
            _eventAggregator = eventAggregator;

            _mapProvider = mapProvider;

            Model = new TreeModel(id, name, description, type, used, visibility, parentTree, dataType);
            Children = new TrulyObservableCollection<TreeContentControlViewModel>();
            Update();
        }

        public TreeContentControlViewModel(string id, string name, string description, EnumTreeType type, bool used, bool visibility, object parentTree, EnumDataType dataType, IEventAggregator eventAggregator = null, MapProvider mapProvider = null, CameraProvider cameraProvider = null)
        {
            DisplayName = name;
            _eventAggregator = eventAggregator;

            _mapProvider = mapProvider;
            _cameraProvider = cameraProvider;

            Model = new TreeModel(id, name, description, type, used, visibility, parentTree, dataType);
            Children = new TrulyObservableCollection<TreeContentControlViewModel>();
            Update();
        }

        public TreeContentControlViewModel(string id, string name, string description, EnumTreeType type, bool used, bool visibility, object parentTree, EnumDataType dataType, IEventAggregator eventAggregator = null, MapProvider mapProvider = null, GroupProvider groupProvider = null, ControllerProvider controllerProvider = null, SensorProvider sensorProvider = null)
        {
            DisplayName = name;
            _eventAggregator = eventAggregator;

            _sensorProvider = sensorProvider;
            _controllerProvider = controllerProvider;
            _groupProvider = groupProvider;
            _mapProvider = mapProvider;

            Model = new TreeModel(id, name, description, type, used, visibility, parentTree, dataType);
            Children = new TrulyObservableCollection<TreeContentControlViewModel>();
            Update();
        }

        #endregion
        #region - Implementation of Interface -
        public override void Update()
        {
            NotifyOfPropertyChange(() => Id);
            NotifyOfPropertyChange(() => Name);
            NotifyOfPropertyChange(() => Description);
            NotifyOfPropertyChange(() => Used);
            NotifyOfPropertyChange(() => Type);
            NotifyOfPropertyChange(() => Visibility);
            NotifyOfPropertyChange(() => ParentTree);
            NotifyOfPropertyChange(() => DataType);
        }

        public override void Clear()
        {
            Model = new TreeModel();
            Update();
        }
        #endregion
        #region - Overrides -
        protected override Task InitialProcess()
        {
            return base.InitialProcess();
        }
        #endregion
        #region - Binding Methods -
        public async void ClickAddTree(object sender, RoutedEventArgs e)
        {
            if (!((sender as FrameworkElement).DataContext is TreeContentControlViewModel viewModel))
                return;

            switch (viewModel.DataType)
            {
                case EnumDataType.MapRoot:
                    ///등록방법
                    ///1. TreeNode와 연동을 위한 MapContentControlViewModel 생성
                    ///2. ControllerContentControlViewModel을 ActivateAsync
                    ///3. ControllerContentControlViewModel을 ControllerProvider에 등록
                    ///4. TreeNode 생성
                    ///5. TreeNode를 ActivateAsync
                    ///6. TreeNode를 ControllerProvider에 등록
                    {
                        var id = (int)(_mapProvider?.GetMaxId()) + 1;
                        var mapNumber = ProviderManager.GetMaxMapID(_mapProvider) + 1;

                        ///1
                        var contentControlViewModel = new MapContentControlViewModel(id, $"{mapNumber}번 맵", mapNumber, null, null, null, 0.0, 0.0, false, false, _eventAggregator, _mapProvider) { DisplayName = $"{id} {EnumDataType.Map.ToString()}" };
                        ///2
                        await contentControlViewModel ?.ActivateAsync();
                        ///3
                        _mapProvider?.Add(contentControlViewModel);

                        await _eventAggregator?.PublishOnUIThreadAsync(new MapContentAddMessageModel());

                        ///4
                        var treeNode = new TreeContentControlViewModel(TreeManager.SetTreeMapId(id), contentControlViewModel.MapName, contentControlViewModel.Url, EnumTreeType.LEAF, true, true, viewModel, EnumDataType.Map, _eventAggregator, _mapProvider) { DisplayName = $"[{EnumTreeType.LEAF.ToString()}]{id} {EnumDataType.Map.ToString()}" };
                        
                        ///5
                        await treeNode ?.ActivateAsync();

                        ///6
                        viewModel.Children.Add(treeNode);
                    }
                    break;

                case EnumDataType.Controller:
                    ///등록방법
                    ///1. TreeNode와 연동을 위한 SensorContentControlViewModel 생성
                    ///2. ControllerContentControlViewModel을 ActivateAsync
                    ///3. ControllerContentControlViewModel을 ControllerProvider에 등록
                    ///4. TreeNode 생성
                    ///5. TreeNode를 ActivateAsync
                    ///6. TreeNode를 ControllerProvider에 등록
                    {
                        var id = _sensorProvider.GetMaxId() + 1;
                        var idController = int.Parse(viewModel.Name);
                        var idSensor = ProviderManager.GetMaxSensorID(_sensorProvider, idController) + 1;

                        var map = _mapProvider.CollectionEntity?.FirstOrDefault();
                        var mapNumber = map != null ? map.MapNumber : 0;

                        //1
                        var contentControlViewModel = new SensorContentControlViewModel(id, null, (int)EnumDeviceType.Fence, $"{idSensor}", idController, idSensor, 0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, mapNumber, false, false, _eventAggregator, _sensorProvider, _groupProvider, _controllerProvider, _mapProvider) { DisplayName = $"{idSensor} {EnumDataType.Sensor.ToString()}" };
                        //2
                        await contentControlViewModel?.ActivateAsync();
                        ///3
                        _sensorProvider.Add(contentControlViewModel);
                        ///4
                        var treeNode = new TreeContentControlViewModel(TreeManager.SetTreeSensorId(id), contentControlViewModel.IdSensor.ToString(), contentControlViewModel.NameDevice, EnumTreeType.LEAF, true, true, viewModel, EnumDataType.Sensor, _eventAggregator, _mapProvider, _groupProvider, _controllerProvider, _sensorProvider) { DisplayName = $"[{EnumTreeType.LEAF.ToString()}]{idSensor} {EnumDataType.Sensor.ToString()}" };
                        ///5
                        await treeNode?.ActivateAsync();
                        ///6
                        viewModel.Children.Add(treeNode);

                    }

                    //await _eventAggregator?.PublishOnUIThreadAsync(new SensorTreeAddMessageModel(sensor));
                    break;

                case EnumDataType.DeviceRoot:
                    ///등록방법
                    ///1. TreeNode와 연동을 위한 ControllerContentControlViewModel 생성
                    ///2. ControllerContentControlViewModel을 ActivateAsync
                    ///3. ControllerContentControlViewModel을 ControllerProvider에 등록
                    ///4. TreeNode 생성
                    ///5. TreeNode를 ActivateAsync
                    ///6. TreeNode를 ControllerProvider에 등록
                    {
                        var id = _controllerProvider.GetMaxId() + 1;
                        var idController = ProviderManager.GetMaxControllerID(_controllerProvider) + 1;

                        var map = _mapProvider.CollectionEntity?.FirstOrDefault();
                        var mapNumber = map != null ? map.MapNumber : 0;

                        ///1 
                        var contentControlViewModel = new ControllerContentControlViewModel(id, $"{idController}제어기", (int)EnumDeviceType.Controller, $"{idController}", idController, 0, 0, 0.0, 0.0, 0.0, 0.0, 30.0, 30.0, 0.0, mapNumber, false, false, _eventAggregator, _controllerProvider, _mapProvider) { DisplayName = $"{idController} {EnumDataType.Controller.ToString()}" };
                        ///2
                        await contentControlViewModel?.ActivateAsync();
                        ///3
                        _controllerProvider.Add(contentControlViewModel);
                        ///4
                        var treeNode = new TreeContentControlViewModel(TreeManager.SetTreeControllerId(id), contentControlViewModel.IdController.ToString(), contentControlViewModel.NameDevice, EnumTreeType.BRANCH, true, true, viewModel, EnumDataType.Controller, _eventAggregator, _mapProvider, _groupProvider, _controllerProvider, _sensorProvider) { DisplayName = $"[{EnumTreeType.BRANCH.ToString()}]{idController} {EnumDataType.Controller.ToString()}" };
                        ///5
                        await treeNode?.ActivateAsync();
                        ///6
                        viewModel.Children.Add(treeNode);
                    }
                    //await _eventAggregator?.PublishOnUIThreadAsync(new ControllerTreeAddMessageModel(controller));
                    break;

                case EnumDataType.GroupRoot:
                    ///등록방법
                    ///1. TreeNode와 연동을 위한 ControllerContentControlViewModel 생성
                    ///2. ControllerContentControlViewModel을 ActivateAsync
                    ///3. ControllerContentControlViewModel을 ControllerProvider에 등록
                    ///4. TreeNode 생성
                    ///5. TreeNode를 ActivateAsync
                    ///6. TreeNode를 ControllerProvider에 등록
                    {
                        var id = _groupProvider.GetMaxId() + 1;
                        var nameArea = int.Parse(ProviderManager.GetMaxNameArea(_groupProvider))+1;

                        var map = _mapProvider.CollectionEntity?.FirstOrDefault();
                        var mapNumber = map != null ? map.MapNumber : 0;

                        //1
                        var contentControlViewModel = new GroupContentControlViewModel(id, nameArea.ToString(), (int)EnumDeviceType.NONE, nameArea.ToString(), 0, 0, 0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, mapNumber, false, false, _eventAggregator, _groupProvider, _mapProvider) { DisplayName = $"{id} {EnumDataType.Group.ToString()}" };
                        //2
                        await contentControlViewModel?.ActivateAsync();
                        ///3
                        _groupProvider.Add(contentControlViewModel);
                        ///4
                        var treeNode = new TreeContentControlViewModel(TreeManager.SetTreeGroupId(id), contentControlViewModel.NameArea, contentControlViewModel.NameArea, EnumTreeType.BRANCH, true, true, viewModel, EnumDataType.Group, _eventAggregator, _mapProvider, _groupProvider, _controllerProvider, _sensorProvider) { DisplayName = $"[{EnumTreeType.BRANCH.ToString()}]{id} {EnumDataType.Group.ToString()}" };
                        ///5
                        await treeNode?.ActivateAsync();
                        ///6
                        viewModel.Children.Add(treeNode);
                    }
                    //await _eventAggregator?.PublishOnUIThreadAsync(new GroupTreeAddMessageModel(group));
                    break;

                case EnumDataType.CameraRoot:
                    ///등록방법
                    ///1. TreeNode와 연동을 위한 CameraContentControlViewModel 생성
                    ///2. ControllerContentControlViewModel을 ActivateAsync
                    ///3. ControllerContentControlViewModel을 ControllerProvider에 등록
                    ///4. TreeNode 생성
                    ///5. TreeNode를 ActivateAsync
                    ///6. TreeNode를 ControllerProvider에 등록
                    {
                        var id = _cameraProvider.GetMaxId() + 1;

                        var map = _mapProvider.CollectionEntity?.FirstOrDefault();
                        var mapNumber = map != null ? map.MapNumber : 0;

                        //1
                        var contentControlViewModel = new CameraContentControlViewModel(id, $"{id}번 카메라", (int)EnumDeviceType.IpCamera, $"{id}번 카메라", 0, 0, 0, 0.0, 0.0, 0.0, 0.0, 30.0, 30.0, 0.0, mapNumber, false, false, _eventAggregator, null, _mapProvider) { DisplayName = $"{id} {EnumDataType.Camera.ToString()}" };
                        //2
                        contentControlViewModel?.ActivateAsync();
                        ///3
                        _cameraProvider.Add(contentControlViewModel);
                        
                        ///4
                        var treeNode = new TreeContentControlViewModel(TreeManager.SetTreeCameraId(id), contentControlViewModel.NameDevice, contentControlViewModel.NameDevice, EnumTreeType.LEAF, true, true, viewModel, EnumDataType.Camera, _eventAggregator, _mapProvider, _cameraProvider){ DisplayName = $"[{EnumTreeType.LEAF.ToString()}]{id} {EnumDataType.Camera.ToString()}" };
                        ///5
                        await treeNode?.ActivateAsync();
                        ///6
                        viewModel.Children.Add(treeNode);
                    }
                    //await _eventAggregator?.PublishOnUIThreadAsync(new CameraTreeAddMessageModel(camera));
                    break;

                default:
                    Debug.WriteLine("아무것도 선택되지 않았습니다. It was not selected anyone.");
                    break;
            }

        }

        public async void ClickRemoveTree(object sender, RoutedEventArgs e)
        {
            if (!((sender as FrameworkElement).DataContext is TreeContentControlViewModel viewModel))
                return;

            switch (viewModel.DataType)
            {
                case EnumDataType.Map:
                    ///삭제 순서
                    ///1. _mapProvider에서 viewModel과 동일한 Id를 검색한 후,
                    ///2. 해당 contentControlViewModel을 Remove
                    ///3. 해당 contentControlViewModel을 Deactivate
                    ///4. viewModel을 통해 부모 TreeNode를 parentNode로 할당
                    ///5. 부모 TreeNode에서 해당 자식노드를 Remove
                    ///6. 해당 TreeContentControlViewModel을 Deactivate
                    {
                        //1
                        var contentControlViewModel = _mapProvider.Where(t => t.Id == TreeManager.GetMapProviderId(viewModel.Id)).FirstOrDefault();
                        //2
                        _mapProvider.Remove(contentControlViewModel);
                        //3
                        await contentControlViewModel?.DeactivateAsync(true);
                        //4
                        var parentNode = viewModel.ParentTree as TreeContentControlViewModel;
                        //5
                        parentNode?.Children?.Remove(viewModel);
                        //6
                        await viewModel.DeactivateAsync(true);
                    }
                    break;
                case EnumDataType.Controller:

                    ///최상위 RootNode를 확인하고, GroupRoot와 DeviceRoot는
                    ///서로 다르게 동작
                    ///GroupRoot인 경우,
                    ///ControllerTree 하위의 SensorTree의 그룹정보를 모두 Untitled로 변경,
                    ///Controllertree 및 Children Tree 모두 삭제
                    ///DeviceRoot인 경우,
                    ///Controllertree 및 Children Tree 모두 삭제
                    ///ControllerProvider 및 SensorProivder에서도 모두 삭제
                    ///삭제 순서
                    ///1. _controllerProvider에서 viewModel과 동일한 Id를 검색한 후,
                    ///2. 해당 contentControlViewModel을 Remove
                    ///3. 해당 contentControlViewModel을 Deactivate
                    ///4. viewModel을 통해 부모 TreeNode를 parentNode로 할당
                    ///5. 부모 TreeNode에서 해당 자식노드를 Remove
                    ///6. 해당 TreeContentControlViewModel을 Deactivate

                    var rootNode = TreeManager.GetRootNode(viewModel);
                    
                    if(rootNode.DataType == EnumDataType.DeviceRoot)
                    {
                        //1
                        var contentControlViewModel = _controllerProvider.Where(t => t.Id == TreeManager.GetControllerProviderId(viewModel.Id)).FirstOrDefault();
                        //2
                        await _eventAggregator.PublishOnUIThreadAsync(new ControllerContentRemoveMessageModel(contentControlViewModel));
                        _controllerProvider.Remove(contentControlViewModel);
                        //3
                        await contentControlViewModel?.DeactivateAsync(true);
                        //3-1 
                        var sensorList = _sensorProvider.Where(t => t.IdController == TreeManager.GetControllerProviderId(viewModel.Id)).ToList();
                        foreach (var item in sensorList)
                        {
                            _sensorProvider.Remove(item);
                            await item.DeactivateAsync(true);
                            await _eventAggregator.PublishOnUIThreadAsync(new SensorContentRemoveMessageModel(item));
                        }
                        //4
                        var parentNode = viewModel.ParentTree as TreeContentControlViewModel;
                        //5
                        TreeManager.SetTreeClear(viewModel.Children);
                        parentNode?.Children?.Remove(viewModel);
                        //6
                        await viewModel.DeactivateAsync(true);
                    }else if(rootNode.DataType == EnumDataType.GroupRoot)
                    {
                        _sensorProvider.Where(t => t.IdController == TreeManager.GetControllerProviderId(viewModel.Id)).Select(t => t.NameArea = null);
                        //4
                        var parentNode = viewModel.ParentTree as TreeContentControlViewModel;
                        //5
                        TreeManager.SetTreeClear(viewModel.Children);
                        parentNode?.Children?.Remove(viewModel);
                        //6
                        await viewModel.DeactivateAsync(true);
                    }
                    break;
                case EnumDataType.Sensor:
                    ///삭제 순서
                    ///1. _sensorProvider에서 viewModel과 동일한 Id를 검색한 후,
                    ///2. 해당 contentControlViewModel을 Remove
                    ///3. 해당 contentControlViewModel을 Deactivate
                    ///4. viewModel을 통해 부모 TreeNode를 parentNode로 할당
                    ///5. 부모 TreeNode에서 해당 자식노드를 Remove
                    ///6. 해당 TreeContentControlViewModel을 Deactivate
                    rootNode = TreeManager.GetRootNode(viewModel);
                    if (rootNode.DataType == EnumDataType.DeviceRoot)
                    {
                        //1
                        var contentControlViewModel = _sensorProvider.Where(t => t.Id == TreeManager.GetSensorProviderId(viewModel.Id)).FirstOrDefault();
                        //2
                        _sensorProvider.Remove(contentControlViewModel);
                        await _eventAggregator.PublishOnUIThreadAsync(new SensorContentRemoveMessageModel(contentControlViewModel))
                            .ContinueWith(async (t,_)=> await contentControlViewModel?.DeactivateAsync(true), null, TaskScheduler.FromCurrentSynchronizationContext());
                        //3
                        //4
                        var parentNode = viewModel.ParentTree as TreeContentControlViewModel;
                        //5
                        parentNode?.Children?.Remove(viewModel);
                        //6
                        await viewModel.DeactivateAsync(true);
                    }
                    else if (rootNode.DataType == EnumDataType.GroupRoot)
                    {
                        _sensorProvider.Where(t => t.Id == TreeManager.GetSensorProviderId(viewModel.Id)).Select(t=>t.NameArea = null);
                        //4
                        var parentNode = viewModel.ParentTree as TreeContentControlViewModel;
                        //5
                        TreeManager.SetTreeClear(viewModel.Children);
                        parentNode?.Children?.Remove(viewModel);
                        //6
                        await viewModel.DeactivateAsync(true);
                    }
                    //await _eventAggregator?.PublishOnUIThreadAsync(new SensorTreeRemoveMessageModel(viewModel));

                    break;
                case EnumDataType.Group:
                    ///삭제 순서
                    ///1. _groupProvider에서 viewModel과 동일한 Id를 검색한 후,
                    ///2. 해당 contentControlViewModel을 Remove
                    ///3. 해당 contentControlViewModel을 Deactivate
                    ///4. viewModel을 통해 부모 TreeNode를 parentNode로 할당
                    ///5. 부모 TreeNode에서 해당 자식노드를 Remove
                    ///6. 해당 TreeContentControlViewModel을 Deactivate
                    {
                        var sensorList = _sensorProvider.Where(t => t.NameArea == viewModel.Name).ToList();
                        foreach (var item in sensorList)
                        {
                            item.NameArea = null;
                        }

                        //1
                        var contentControlViewModel = _groupProvider.Where(t => t.Id == TreeManager.GetGroupProviderId(viewModel.Id)).FirstOrDefault();
                        //2
                        _groupProvider.Remove(contentControlViewModel);
                        //3
                        await contentControlViewModel?.DeactivateAsync(true);
                        //4
                        var parentNode = viewModel.ParentTree as TreeContentControlViewModel;
                        //5
                        TreeManager.SetTreeClear(viewModel.Children);
                        parentNode?.Children?.Remove(viewModel);
                        //6
                        await viewModel.DeactivateAsync(true);
                    }
                    //await _eventAggregator?.PublishOnUIThreadAsync(new GroupTreeRemoveMessageModel(viewModel));
                    break;
                case EnumDataType.Camera:
                    ///삭제 순서
                    ///1. _cameraProvider에서 viewModel과 동일한 Id를 검색한 후,
                    ///2. 해당 contentControlViewModel을 Remove
                    ///3. 해당 contentControlViewModel을 Deactivate
                    ///4. viewModel을 통해 부모 TreeNode를 parentNode로 할당
                    ///5. 부모 TreeNode에서 해당 자식노드를 Remove
                    ///6. 해당 TreeContentControlViewModel을 Deactivate
                    {
                        //1
                        var contentControlViewModel = _cameraProvider.Where(t => t.Id == TreeManager.GetCameraProviderId(viewModel.Id)).FirstOrDefault();
                        //2
                        _cameraProvider.Remove(contentControlViewModel);
                        //3
                        await contentControlViewModel?.DeactivateAsync(true);
                        //4
                        var parentNode = viewModel.ParentTree as TreeContentControlViewModel;
                        //5
                        parentNode?.Children?.Remove(viewModel);
                        //6
                        await viewModel.DeactivateAsync(true);
                    }
                    //await _eventAggregator?.PublishOnUIThreadAsync(new CameraTreeRemoveMessageModel(viewModel));
                    break;
            }
        }
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        /// <summary>
        /// 트리 노드의 아이디
        /// 연동된 모델로 부터 부여 받은 고유번호에 해당한다.
        /// </summary>
        public string Id
        {
            get => Model.Id;
            set
            {
                Model.Id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }
        /// <summary>
        /// 트리 노드의 이름 -
        /// 연동된 모델로 부터 부여 받게되는 이름
        /// </summary>
        public string Name
        {
            get => Model.Name;
            set
            {
                Model.Name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }
        /// <summary>
        /// 트리 노드의 해설 -
        /// 트리 노드를 묘사하는 문구를 입력
        /// </summary>
        public string Description
        {
            get => Model.Description;
            set
            {
                Model.Description = value;
                NotifyOfPropertyChange(() => Description);
            }
        }
        /// <summary>
        /// 트리 노드의 타입 -
        /// 트리 노드의 타입을 지정할 수 있다.
        /// 1.ROOT : 부모가 없고, 자식만 존재 최초 노드
        /// 2.BRANCH : 부모와 자식이 둘다 존재 중간 노드
        /// 3.LEAF : 자식은 없고 부모만 존재 말단 노드
        /// </summary>
        public EnumTreeType Type
        {
            get => Model.Type;
            set
            {
                Model.Type = value;
                NotifyOfPropertyChange(() => Type);
            }
        }
        /// <summary>
        /// 사용 속성 -
        /// 연동된 모델로 부터 부여받은 속성으로
        /// 해당 트리 노드의 사용 여부를 결정
        /// </summary>
        public bool Used
        {
            get => Model.Used;
            set
            {
                Model.Used = value;
                NotifyOfPropertyChange(() => Used);
            }
        }
        /// <summary>
        /// 시현 속성 -
        /// 연동된 모델로 부터 부여받은 속성으로
        /// 해당 트리 노드의 시현 여부를 결정
        /// </summary>
        public bool Visibility
        {
            get => Model.Visibility;
            set
            {
                Model.Visibility = value;
                NotifyOfPropertyChange(() => Visibility);
            }
        }

        /// <summary>
        /// 상위 노드 객체 - 
        /// ParentTree는 상위 객체를 담는 오브젝트로
        /// 존재 유무에 따라 객체 혹은 null이 될 수 있으며,
        /// 객체는 TreeItemViewModel타입을 활용
        /// </summary>
        public object ParentTree
        {
            get => Model?.ParentTree;
            set
            {
                Model.ParentTree = value;
                NotifyOfPropertyChange(() => ParentTree);
            }
        }

        public EnumDataType DataType
        {
            get { return Model.DataType; }
            set
            {
                Model.DataType = value;
                NotifyOfPropertyChange(() => DataType);
            }
        }

        /// <summary>
        /// A list of all children contained inside this item
        /// </summary>
        public TrulyObservableCollection<TreeContentControlViewModel> Children
        {
            get { return _children; }
            set
            {
                _children = value;
                NotifyOfPropertyChange(() => Children);
            }
        }
        #endregion
        #region Helper Methods
        #endregion
        #region - Attributes -
        private TrulyObservableCollection<TreeContentControlViewModel> _children;
        private MapProvider _mapProvider;
        private CameraProvider _cameraProvider;
        private SensorProvider _sensorProvider;
        private ControllerProvider _controllerProvider;
        private GroupProvider _groupProvider;
        #endregion
    }
}
