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

        //, IHandle<DeviceTreeSelectedMessageModel>
        , IHandle<CameraTreeSelectedMessageModel>
        , IHandle<GroupTreeSelectedMessageModel>

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

                SelectedItem = item;
                item.IsSelected = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in UpdateTree : {ex.Message}");
                return;
            }
            
        }

        private void MoveSensorNode(SymbolContentControlViewModel viewModel)
        {
            try
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

                item.IsSelected = true;
                SelectedItem = item;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in MoveSensorNode : {ex.Message}");
                return;
            }
        }

       
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
            ///DataType을 이용한 Controller, Sensor 구분
            if (SelectedItem.DataType == EnumDataType.Controller)
            {
                var viewModel = _controllerProvider.CollectionEntity
                .Where(item => TreeManager.SetTreeControllerId(item.Id) == SelectedItem.Id)
                .SingleOrDefault();

                if (viewModel != null)
                {
                    _eventAggregator.PublishOnUIThreadAsync(new OpenControllerPropertyMessageModel(viewModel));
                }
            }
            else
            {
                var viewModel = _sensorProvider.CollectionEntity
                .Where(item => TreeManager.SetTreeSensorId(item.Id) == SelectedItem.Id)
                .SingleOrDefault();

                if (viewModel != null)
                    _eventAggregator.PublishOnUIThreadAsync(new OpenSensorPropertyMessageModel(viewModel));
            }

            _eventAggregator.PublishOnUIThreadAsync(new DeviceTreeSelectedMessageModel());

        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        
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

        public async Task HandleAsync(CameraTreeSelectedMessageModel message, CancellationToken cancellationToken)
        {
            await Task.Run(() => TreeManager.SetTreeUnselected(Items) );
        }

        public async Task HandleAsync(GroupTreeSelectedMessageModel message, CancellationToken cancellationToken)
        {
            await Task.Run(() => TreeManager.SetTreeUnselected(Items) );
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
