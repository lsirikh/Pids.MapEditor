using Caliburn.Micro;
using Ironwall.Enums;
using Ironwall.Framework.DataProviders;
using Ironwall.MapEditor.Ui.Helpers;
using Ironwall.MapEditor.UI.DataProviders;
using Ironwall.MapEditor.UI.Helpers;
using Ironwall.MapEditor.UI.Models.Messages.Process;
using Ironwall.MapEditor.UI.Models.Messages.Sections;
using Ironwall.MapEditor.UI.ViewModels.ContentControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.ViewModels.RegisteredItems
{
    public sealed class DeviceTreeViewModel
        : TreeBaseViewModel<SymbolContentControlViewModel>
        //, IHandle<ControllerTreeAddMessageModel>
        //, IHandle<ControllerTreeRemoveMessageModel>
        //, IHandle<ControllerContentUpdateMessageModel>
        //, IHandle<SensorTreeAddMessageModel>
        //, IHandle<SensorTreeRemoveMessageModel>
        //, IHandle<SensorContentUpdateMessageModel>
        , IHandle<SymbolContentUpdateMessageModel>
    {
        #region - Ctors -
        public DeviceTreeViewModel(
            ControllerProvider controllerProvider
            , SensorProvider sensorProvider
            , GroupProvider groupProvider
            , MapProvider mapProvider
            , IEventAggregator eventAggregator)
        {
            _controllerProvider = controllerProvider;
            _sensorProvider = sensorProvider;
            _groupProvider = groupProvider;
            _mapProvider = mapProvider;
            _eventAggregator = eventAggregator;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        /// <summary>
        /// Tree구조 전체 초기화 후, Root 등록
        /// </summary>
        protected override async void SetRootInitialize()
        {
            base.SetRootInitialize();

            ///DiviceRoot 생성
            ///Root가 null인경우 초기화
            ///****************TreeContetnControlViewMdoel 생성************************
            AddTree(new TreeContentControlViewModel($"R{0}", "전체 장비", "사이트 전체 장비 구성", EnumTreeType.ROOT, true, true, null, EnumDataType.DeviceRoot, _eventAggregator, _mapProvider, _groupProvider, _controllerProvider, _sensorProvider) { DisplayName = $"[{EnumTreeType.ROOT.ToString()}]{0} {EnumDataType.DeviceRoot.ToString()}" });

            ///Root는 Items의 첫 오브젝트를 할당
            var root = Items.FirstOrDefault();
            ///Root Node 활성화 상태 확인
            if (!Items.FirstOrDefault().IsActive)
                await root.ActivateAsync();
        }
        /// <summary>
        /// 해당 Provider를 이용하여, 트리노드 Branch 혹은 Leaf 형성
        /// </summary>
        protected override void SetTreeWithProvider()
        {
            base.SetTreeWithProvider();

            ///제어기 등록
            ///$"C{0}"
            ///기존의 등록하던 방식과 동일하게 등록
            ///****************TreeContetnControlViewMdoel 생성************************
            var controllerList = _controllerProvider.Select(controller => new TreeContentControlViewModel(TreeManager.SetTreeControllerId(controller.Id), controller.IdController.ToString(), controller.NameDevice, EnumTreeType.BRANCH, controller.Used, controller.Visibility, Items.FirstOrDefault(), EnumDataType.Controller, _eventAggregator) { DisplayName = $"[{EnumTreeType.BRANCH.ToString()}]{controller.IdController} {EnumDataType.Controller.ToString()}"});

            foreach (var item in controllerList)
            {
                var parentNode = item.ParentTree as TreeContentControlViewModel;
                parentNode.Children.Add(item);
            }
            //controllerList.ToList().ForEach(item => AddTree(item.ParentTree as TreeContentControlViewModel, item));

            ///센서 등록
            ///$"S{0}"
            ///각 센서별 등록된 제어기 번호로,
            ///controllerList를 활용해서 검색 후,
            ///해당 트리를 부모로 할당
            ///****************TreeContetnControlViewMdoel 생성************************
            var sensorList = _sensorProvider.Select(sensor => new TreeContentControlViewModel(TreeManager.SetTreeSensorId(sensor.Id), sensor.IdSensor.ToString(), sensor.NameDevice, EnumTreeType.LEAF, sensor.Used, sensor.Visibility, controllerList.Where(controller => controller.Name == sensor.IdController.ToString()).FirstOrDefault(), EnumDataType.Sensor, _eventAggregator) { DisplayName = $"[{EnumTreeType.LEAF.ToString()}]{sensor.IdSensor} {EnumDataType.Sensor.ToString()}" });
            foreach (var item in sensorList)
            {
                var parentNode = item.ParentTree as TreeContentControlViewModel;
                parentNode.Children.Add(item);
            }
            //sensorList.ToList().ForEach(item => AddTree(item.ParentTree as TreeContentControlViewModel, item));
        }
        /// <summary>
        /// 선택된 트리 노드를 기준으로 변경사항 업데이트
        /// </summary>
        /// <param name="viewModel">변경을 위한 ViewModel 데이터</param>
        protected override void UpdateTree(SymbolContentControlViewModel viewModel = null)
        {
            ///해당 viewModel의 속성을 변경
            try
            {
                ///해당 viewModel의 Id와 같은 Id를
                ///갖는 TreeItemViewModel을 찾고,
                ///나머지 정보를 업데이트 해준다.

                if (viewModel == null)
                    return;


                var itemId = "";
                if (viewModel.TypeDevice == (int)EnumDeviceType.Controller)
                    itemId = TreeManager.SetTreeControllerId(viewModel.Id);
                else
                    itemId = TreeManager.SetTreeSensorId(viewModel.Id);


                var item = TreeManager.GetMatchedId(Items, itemId);
                if (item == null)
                    return;

                ///Root객체 찾고, root할당
                if (!(Items.FirstOrDefault() is TreeContentControlViewModel root) || root == null)
                    return;

                ///Update항목이 Controller인지, Sensor인지 구분
                var dataType = DataConvertionHelper.IntDeviceToDataConverter(viewModel.TypeDevice);

                switch (dataType)
                {
                    case EnumDataType.Controller:
                        var searchedItem = root.Children.Where(t => t.Id == TreeManager.SetTreeControllerId(viewModel.Id)).FirstOrDefault();
                        /// IdController의 변경이 있는 경우
                        /// 자녀 노드를 모두 삭제
                        if (viewModel.IdController.ToString() != searchedItem.Name)
                        {
                            ///변경 전, Controller Id
                            var prevIdController = int.Parse(searchedItem.Name);
                            ///Tree Node에서 자녀 Tree 모두 제거
                            TreeManager.SetTreeClear(searchedItem.Children);
                            ///Provider에서 제거
                            RemoveSensorsInProvider(prevIdController);
                        }
                        item.Name = viewModel.IdController.ToString(); //viewModel의 타입이 제어기
                        break;
                    case EnumDataType.Sensor:
                        if((item.ParentTree as TreeContentControlViewModel).Name != viewModel.IdController.ToString())
                            MoveSensorNode(viewModel);

                        item.Name = viewModel.IdSensor.ToString(); //viewModel의 타입이 센서
                        break;
                    #region Inactive
                    /*case EnumDataType.None:
                    case EnumDataType.MapRoot:
                    case EnumDataType.Map:
                    case EnumDataType.DeviceRoot:
                    case EnumDataType.GroupRoot:
                    case EnumDataType.Group:
                    case EnumDataType.CameraRoot:
                    case EnumDataType.Camera:
                        return;*/
                    #endregion
                    default:
                        return;
                }

                item.Description = viewModel.NameDevice;
                item.Used = viewModel.Used;
                item.Visibility = viewModel.Visibility;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in UpdateTree : {ex.Message}");
                return;
            }
            finally
            {
                NotifyOfPropertyChange(() => Items);
            }
        }

        private void MoveSensorNode(SymbolContentControlViewModel viewModel)
        {
            var itemId = TreeManager.SetTreeSensorId(viewModel.Id);
            var item = TreeManager.GetMatchedId(Items, itemId);
            //var prevParent = Items.FirstOrDefault().Children.Where(t => t.Id == (item.ParentTree as TreeContentControlViewModel).Id).FirstOrDefault();
            var prevParent = item.ParentTree as TreeContentControlViewModel;
            ///기존 부모 Tree에서 삭제
            prevParent.Children.Remove(item);
            var newParent = Items.FirstOrDefault().Children.Where(t => t.Name == viewModel.IdController.ToString()).FirstOrDefault();
            item.ParentTree = newParent;
            
            if (newParent == null)
                return;

            ///신규 부모 Tree에 추가
            newParent.Children.Add(item);
        }

        /*/// <summary>
        /// Tree에 노드 등록
        /// </summary>
        /// <param name="parentTree">등록할 노드의 부모노드</param>
        /// <param name="childTree">등록할 Tree Node</param>
        /// <returns>boolean Type</returns>
        protected override bool AddTree(TreeContentControlViewModel parent, TreeContentControlViewModel child)
        {
            return base.AddTree(parent, child);
        }*/

        /// <summary>
        /// Tree에 노드 삭제
        /// </summary>
        /// <param name="node">등록할 Tree Node</param>
        /// <returns>boolean Type</returns>
        /*protected override bool RemoveTree(TreeContentControlViewModel node)
        {
            var nodeId = node.Id;
            var nodeType = node.DataType;

            if (base.RemoveTree(node))
            {
                ///제어기
                if (nodeType == EnumDataType.Controller)
                {
                    ///provider에 등록된 아이템 찾기`
                    ///
                    RemoveControllerInProvider(nodeId);

                    RemoveSensorsInProvider(nodeId);
                }
                else
                {
                    RemoveSensorsInProvider(nodeId);
                }

                ///센서
                return true;
            }
            else
                return false;

        }*/

        /// <summary>
        /// RemoveControllerInProvider - Provider에 등록된 ControllerContetntViewModel을 삭제하는 메소드
        /// </summary>
        /// <param name="id">삭제할 Controller ID</param>
        /*private void RemoveControllerInProvider(string id)
        {
            if (_controllerProvider.Count() > 0)
            {
                ///id와 동일한 값을 갖는 Controller 찾기
                var contentControlViewModel = _controllerProvider?
                    .Where(t => TreeManager.SetTreeControllerId(t.Id) == id)?.FirstOrDefault();

                ///MapContentControlViewModel 비활성화
                contentControlViewModel?.DeactivateAsync(true);

                ///provider에서 해당 아이템 제거
                _controllerProvider.Remove(contentControlViewModel);
                ///메시징
                _eventAggregator.PublishOnUIThreadAsync(new ControllerContentUpdateMessageModel());
            }
        }*/

        /// <summary>
        /// RemoveSensorsInProvider - Provider에 등록된 SensorContentViewModel을 삭제하는 메소드
        /// </summary>
        /// <param name="id">삭제할 Sensor Id</param>
        /*private void RemoveSensorsInProvider(string id)
        {
            if (_sensorProvider.Count() > 0)
            {
                ///id와 동일한 값을 갖는 Sensor 찾기
                var sensors = _sensorProvider?
                    .Where(t => TreeManager.SetTreeSensorId(t.Id) == id)
                    .Select(t => t).ToList();

                ///매칭된 아이템 수량
                var count = sensors.Count();
                ///매칭된 아이템을 Deactivate 및 provider에서 제거
                foreach (var sensor in sensors)
                {
                    sensor.DeactivateAsync(true);
                    _sensorProvider.Remove(sensor);
                }
                ///메시징
                if (count > 0)
                    _eventAggregator.PublishOnUIThreadAsync(new SensorContentUpdateMessageModel());
            }
        }*/
        /// <summary>
        /// RemoveSensorsInProvider - (Method Overriding) Controller Id를 통해 Provider 속해있는 Sensor를 모두 삭제하는 메소드
        /// </summary>
        /// <param name="controllerId">해당 Controller Id에 속한 Sensor는 삭제</param>
        private void RemoveSensorsInProvider(int controllerId)
        {
            if (_sensorProvider.Count() > 0)
            {
                ///id와 동일한 값을 갖는 Sensor 찾기
                var sensors = _sensorProvider?
                    .Where(t => t.IdController == controllerId)
                    .Select(t => t).ToList();

                ///매칭된 아이템 수량
                var count = sensors.Count();
                ///매칭된 아이템을 Deactivate 및 provider에서 제거
                foreach (var sensor in sensors)
                {
                    sensor.DeactivateAsync(true);
                    _sensorProvider.Remove(sensor);
                }
                ///메시징
                if (count > 0)
                    _eventAggregator.PublishOnUIThreadAsync(new SensorContentUpdateMessageModel());
            }
        }

        /// <summary>
        /// 트리 노드를 선택할 경우, 해당 아이템의 세부내역(PropertySection)을
        /// 활성화 시키기 위한 이벤트 호출 메소드
        /// </summary>
        protected override void UpdateSelectedItem()
        {
            if (SelectedItem == null)
                return;

            base.UpdateSelectedItem();
            
            ///DataType을 이용한 Controller, Sensor 구분
            if (SelectedItem.DataType == EnumDataType.Controller)
            {
                var viewModel = _controllerProvider.CollectionEntity
                .Where(item => TreeManager.SetTreeControllerId(item.Id) == SelectedItem.Id)
                .SingleOrDefault();

                if (viewModel != null)
                    _eventAggregator.PublishOnUIThreadAsync(new OpenControllerPropertyMessageModel(viewModel));
            }
            else
            {
                var viewModel = _sensorProvider.CollectionEntity
                .Where(item => TreeManager.SetTreeSensorId(item.Id) == SelectedItem.Id)
                .SingleOrDefault();

                if (viewModel != null)
                    _eventAggregator.PublishOnUIThreadAsync(new OpenSensorPropertyMessageModel(viewModel));
            }
            
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        /// <summary>
        /// Tree Node의 상위(Root)에서 Contenxt Menu를 활용한 추가 이벤트 수신 메소드
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /*public Task HandleAsync(ControllerTreeAddMessageModel message, CancellationToken cancellationToken)
        {
            //Get ViewModel
            var viewModel = message.ViewModel;

            //Get idController
            var id = _controllerProvider.GetMaxId() + 1;
            var idController = ProviderManager.GetMaxControllerID(_controllerProvider) + 1;

            //*************************ControllerContentControlViewModel 생성, 활성화 및 추가****************************
            var contentControlViewModel = new ControllerContentControlViewModel(id, "Untitle", (int)EnumDeviceType.Controller, $"{idController}", idController, 0, 0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0, false, false, _eventAggregator, _controllerProvider, _mapProvider) { DisplayName = $"{idController} {EnumDataType.Controller.ToString()}" };
            //MapContentControlViewModel 활성화
            contentControlViewModel.ActivateAsync();
            ///Provider에 데이터 추가
            _controllerProvider.Add(contentControlViewModel);
            //***********************************************************************************************************

            //초기화된 TreeContentControlViewModel인 경우
            if (viewModel.Id == TreeManager.SetTreeControllerId(0))
            {
                //트리 노드 중 최대 ID 값 보다 1증가한 값을 ID로 할당
                viewModel.Id = TreeManager.SetTreeControllerId(id);
                viewModel.Name = contentControlViewModel.IdController.ToString();
                viewModel.Description = contentControlViewModel.NameDevice;
                viewModel.DisplayName = $"[{EnumTreeType.BRANCH.ToString()}]{idController} {EnumDataType.Controller.ToString()}";
            }
            //AddTree Tree Node 추가 프로세스
            AddTree(viewModel.ParentTree as TreeContentControlViewModel, viewModel);
            
            return Task.CompletedTask;
        }*/

        /*private void RefreshTree(TreeContentControlViewModel viewModel)
        {
            
            if(viewModel.DataType == EnumDataType.Controller)
            {
                ///해당 컨트롤러에 속하는 센서를 찾아서 등록
                ///****************TreeContetnControlViewMdoel 생성************************
                var sensorList = _sensorProvider.Select(sensor => new TreeContentControlViewModel(TreeManager.SetTreeSensorId(sensor.Id), sensor.IdSensor.ToString(), sensor.NameDevice, EnumTreeType.LEAF, sensor.Used, sensor.Visibility, viewModel, EnumDataType.Sensor, _eventAggregator) { DisplayName = $"[{EnumTreeType.LEAF.ToString()}]{sensor.IdSensor} {EnumDataType.Sensor.ToString()}" });

                sensorList.ToList().ForEach(item => AddTree(item.ParentTree as TreeContentControlViewModel, item));

            }
            else if (viewModel.DataType == EnumDataType.Sensor)
            {
                ///1.해당 트리노드를 트리에서 찾는다.
                ///2.찾은 노드의 부모노드의 정보와 수정된 부모노드 정보를확인한다.
                ///3-1.맞는 경우는 그대로 유지
                ///3-2.맞지 않는 경우는 해당 부모노드로 트리노드를 이동한다.
                ///     이동방법
                ///     1. 기존트리 노드를 새로운 위치에 Add
                ///     2. 기존트리의 노드는 Remove
                ///     
                var parentNode = viewModel.ParentTree as TreeContentControlViewModel;
            }
            else
            {

            }
        }*/

        /// <summary>
        /// AddController Panel을 활용한 Controller Tree Node의 추가(Add) 요청 이벤트 수신 메소드
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /*public Task HandleAsync(ControllerContentUpdateMessageModel message, CancellationToken cancellationToken)
        {
            InitialTree();
            return Task.CompletedTask;
        }*/
        /// <summary>
        /// 선택된 Tree Node를 삭제하기 위한 요청 수신 메소드
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /*public Task HandleAsync(ControllerTreeRemoveMessageModel message, CancellationToken cancellationToken)
        {
            ///최초 브랜치가 DeviceRoot이면,
            ///DeviceTreeViewModel에서 처리

            var firstBranch = TreeManager.GetFirstBranch(message.ViewModel);
            if (firstBranch.DataType == EnumDataType.DeviceRoot)
                RemoveTree(message.ViewModel);

            return Task.CompletedTask;
        }*/
        /// <summary>
        /// Tree Node의 상위(Branch)에서 Contenxt Menu를 활용한 추가 이벤트 수신 메소드
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /*public Task HandleAsync(SensorTreeAddMessageModel message, CancellationToken cancellationToken)
        {
            //Get ViewModel
            var viewModel = message.ViewModel;

            //Get ParentNode 
            var parentNode = viewModel.ParentTree as TreeContentControlViewModel;

            if (parentNode == null)
                return Task.CompletedTask;

            //Get idSensor
            var id = _sensorProvider.GetMaxId() + 1;
            var idController = int.Parse(parentNode.Name);
            var idSensor = ProviderManager.GetMaxSensorID(_sensorProvider, idController) + 1;

            //****************SensorContentControlViewModel 생성, 활성화 및 추가*************************
            var contentControlViewModel = new SensorContentControlViewModel(id, "Untitle", (int)EnumDeviceType.Fence, $"{idSensor}", idController, idSensor, 0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0, false, false, _eventAggregator, _sensorProvider, _groupProvider, _controllerProvider, _mapProvider) { DisplayName = $"{idSensor} {EnumDataType.Sensor.ToString()}" };
            //SensorContentControlViewModel 활성화
            contentControlViewModel.ActivateAsync();
            ///Provider에 데이터 추가
            _sensorProvider.Add(contentControlViewModel);
            //*******************************************************************************************

            //초기화된 TreeContentControlViewModel인 경우
            if (viewModel.Id == TreeManager.SetTreeSensorId(0))
            {
                //트리 노드 중 최대 ID 값 보다 1증가한 값을 ID로 할당
                viewModel.Id = TreeManager.SetTreeSensorId(id);
                viewModel.Name = contentControlViewModel.IdSensor.ToString();
                viewModel.Description = contentControlViewModel.NameDevice;
                viewModel.DisplayName = $"[{EnumTreeType.LEAF.ToString()}]{idSensor} {EnumDataType.Sensor.ToString()}";
            }
            //Add Tree Node
            AddTree(parentNode, viewModel);
            
            return Task.CompletedTask;
        }*/
        /// <summary>
        /// AddSensor Panel을 활용한 Sensor Tree Node의 추가(Add) 요청 이벤트 수신 메소드
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /*public Task HandleAsync(SensorContentUpdateMessageModel message, CancellationToken cancellationToken)
        {
            InitialTree();
            return Task.CompletedTask;
        }*/
        /// <summary>
        /// Sensor Tree Node의 삭제 요청 이벤트 수신 메소드
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /*public Task HandleAsync(SensorTreeRemoveMessageModel message, CancellationToken cancellationToken)
        {
            ///최초 브랜치가 DeviceRoot이면,
            ///DeviceTreeViewModel에서 처리

            var firstBranch = TreeManager.GetFirstBranch(message.ViewModel);
            if (firstBranch.DataType == EnumDataType.DeviceRoot)
                RemoveTree(message.ViewModel);
            return Task.CompletedTask;
        }*/
        /// <summary>
        /// 등록된 Tree Node의 세부내역 변경 요청 이벤트 수신 메소드 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task HandleAsync(SymbolContentUpdateMessageModel message, CancellationToken cancellationToken)
        {
            var viewModel = message?.ViewModel;
            if (viewModel == null)
                return Task.CompletedTask;

            if (viewModel.GetType() == typeof(ControllerContentControlViewModel)
                || viewModel.GetType() == typeof(SensorContentControlViewModel))
                UpdateTree(message?.ViewModel);

            return Task.CompletedTask;
        }
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private ControllerProvider _controllerProvider;
        private SensorProvider _sensorProvider;
        private GroupProvider _groupProvider;
        private MapProvider _mapProvider;
        #endregion
    }
}
