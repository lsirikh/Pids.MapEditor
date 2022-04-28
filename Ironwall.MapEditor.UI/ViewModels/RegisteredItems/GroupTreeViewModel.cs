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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.ViewModels.RegisteredItems
{
    public sealed class GroupTreeViewModel
        : TreeBaseViewModel<SymbolContentControlViewModel>
        //, IHandle<GroupTreeAddMessageModel>
        , IHandle<GroupTreeRemoveMessageModel>
        , IHandle<GroupContentUpdateMessageModel>
        , IHandle<ControllerContentRemoveMessageModel>
        , IHandle<ControllerTreeAddMessageModel>
        , IHandle<ControllerTreeRemoveMessageModel>
        , IHandle<ControllerContentUpdateMessageModel>
        , IHandle<SensorTreeAddMessageModel>
        , IHandle<SensorTreeRemoveMessageModel>
        , IHandle<SensorContentRemoveMessageModel>
        , IHandle<SensorContentUpdateMessageModel>
        , IHandle<SymbolContentUpdateMessageModel>
    {
        #region - Ctors -
        public GroupTreeViewModel(
            GroupProvider groupProvider
            , ControllerProvider controllerProvider
            , SensorProvider sensorProvider
            , MapProvider mapProvider
            , IEventAggregator eventAggregator)
        {
            _groupProvider = groupProvider;
            _controllerProvider = controllerProvider;
            _sensorProvider = sensorProvider;
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

            ///GroupRoot 생성
            ///Root가 null인경우 초기화
            ///****************TreeContetnControlViewMdoel 생성************************
            AddTree(new TreeContentControlViewModel($"G{0}", "전체 그룹", "사이트 전체 그룹 구성", EnumTreeType.ROOT, true, true, null, EnumDataType.GroupRoot, _eventAggregator, _mapProvider, _groupProvider, _controllerProvider, _sensorProvider) { DisplayName = $"[{EnumTreeType.ROOT.ToString()}]{0} {EnumDataType.GroupRoot.ToString()}" });

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

            ///그룹 등록
            ///$"G{0}"
            ///****************TreeContetnControlViewMdoel 생성 및 등록*****************************
            var groupList = _groupProvider.Select(group => new TreeContentControlViewModel(TreeManager.SetTreeGroupId(group.Id), group.NameArea, group.NameArea, EnumTreeType.BRANCH, group.Used, group.Visibility, Items.FirstOrDefault(), EnumDataType.Group, _eventAggregator) { DisplayName = $"[{EnumTreeType.BRANCH.ToString()}]{group.NameArea} {EnumDataType.Group.ToString()}" });
            ///**************************************************************************************
            groupList.ToList().ForEach(item => AddTree(item));


            ///1. 그룹리스트에 존재하면 그룹을 등록한다. check
            ///2. sensorProvider에 데이터가 존재하면, 
            ///2-1. 해당 데이터의 그룹을 조회하고, 그룹과 매칭되는 그룹이 존재하면
            ///2-2. 해당 그룹의 제어기번호와 일치하는 제어기가 controllerProvider에 존재하는지 확인한다.
            ///2-3. controllerProvider에 매칭되는 제어기가 존재하면, 
            ///2-4. 제어기의 Tree node를 만들기 전에 해당 트리가 존재하는지 확인
            ///2-5. 존재하면, 넘어가고, 존재하지 않으면, 생성한다.
            ///2-6. 제어기 Tree node를 부모로 갖는 센서 Tree node를 생성한다.
            foreach (var sensor in _sensorProvider)
            {
                //2-1
                //Items에서 데이터를 긁어와야 정상적인 데이터 바인딩 및 
                var selectedGroup = Items.FirstOrDefault().Children.Where(t => t.Name == sensor.NameArea).FirstOrDefault();
                if (selectedGroup == null)
                    continue;

                //2-2
                var selectedController = _controllerProvider.Where(t => t.IdController == sensor.IdController).FirstOrDefault();
                if (selectedController == null)
                    continue;

                var searchedController = TreeManager.GetMatchedId(Items, TreeManager.SetTreeControllerId(selectedController.Id));
                ///parentNode와 같은 그룹인지 확인
                ///

                if (searchedController != null && (searchedController.ParentTree as TreeContentControlViewModel).Id == selectedGroup.Id)
                {
                    //2-3
                    ///센서 등록
                    ///$"S{0}"
                    ///****************TreeContetnControlViewMdoel 생성 및 등록*****************************
                    var node = new TreeContentControlViewModel(TreeManager.SetTreeSensorId(sensor.Id), sensor.IdSensor.ToString(), sensor.NameDevice, EnumTreeType.LEAF, sensor.Used, sensor.Visibility, searchedController, EnumDataType.Sensor, _eventAggregator) { DisplayName = $"[{EnumTreeType.LEAF.ToString()}]{sensor.IdSensor} {EnumDataType.Sensor.ToString()}" };
                    ///**************************************************************************************
                    AddTree(node);
                }
                else
                {
                    //2-4
                    ///제어기 등록
                    ///$"C{0}"
                    ///****************TreeContetnControlViewMdoel 생성 및 등록*****************************
                    var parentNode = new TreeContentControlViewModel(TreeManager.SetTreeControllerId(selectedController.Id), selectedController.IdController.ToString(), selectedController.NameDevice, EnumTreeType.LEAF, selectedController.Used, selectedController.Visibility, selectedGroup, EnumDataType.Controller, _eventAggregator) { DisplayName = $"[{EnumTreeType.BRANCH.ToString()}]{sensor.IdController} {EnumDataType.Controller.ToString()}" };
                    ///**************************************************************************************
                    AddTree(parentNode);

                    ///센서 등록
                    ///$"S{0}"
                    ///****************TreeContetnControlViewMdoel 생성 및 등록*****************************
                    var node = new TreeContentControlViewModel(TreeManager.SetTreeSensorId(sensor.Id), sensor.IdSensor.ToString(), sensor.NameDevice, EnumTreeType.LEAF, sensor.Used, sensor.Visibility, parentNode, EnumDataType.Sensor, _eventAggregator) { DisplayName = $"[{EnumTreeType.LEAF.ToString()}]{sensor.IdSensor} {EnumDataType.Sensor.ToString()}" };
                    ///**************************************************************************************
                    AddTree(node);
                }
            }
        }
        /// <summary>
        /// AddTree 메소드 - TreeNode를 추가하는 메소드로 재정의
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        /*protected override bool AddTree(TreeContentControlViewModel parentNode, TreeContentControlViewModel node)
        {
            try
            {
                ///parentTree노드를 기준으로
                ///하위에 childTree를 추가
                if (parentNode == null)
                {
                    Items.Add(node);
                }
                else
                {
                    var tree = parentNode.Children;
                    tree?.Add(node);
                }

                ///Tree 추가 node 활성화
                if (!node.IsActive)
                    node.ActivateAsync();

                ///트리 갯수 갱신
                TreeCount = TreeManager.GetTreeCount(Items);
                Debug.WriteLine($"전체 트리갯수: {TreeCount}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in AddTree : {ex.Message}");
                return false;
            }
        }*/


        /// <summary>
        /// 선택된 트리 노드를 기준으로 변경사항 업데이트
        /// </summary>
        /// <param name="viewModel">변경을 위한 ViewModel 데이터</param>
        protected override async void UpdateTree(SymbolContentControlViewModel viewModel = null)
        {
            ///해당 viewModel의 속성을 변경
            try
            {
                ///해당 viewModel의 Id와 같은 Id를
                ///갖는 TreeItemViewModel을 찾고,
                ///나머지 정보를 업데이트 해준다.
                if (viewModel == null)
                    return;


                var itemId = SetTreeNodeId(viewModel);
                var dataType = DataConvertionHelper.IntDeviceToDataConverter(viewModel.TypeDevice);

                TreeContentControlViewModel item;
                
                ///Root객체 찾고, root할당
                if (!(Items.FirstOrDefault() is TreeContentControlViewModel root) || root == null)
                    return;

                ///Update항목이 Group인지, Sensor인지 구분
                if (dataType == EnumDataType.Group)
                {
                    item = TreeManager.GetMatchedId(Items, itemId);
                    if (item == null)
                        return;


                    ///Group인 경우, 동일 ID의 Tree Node를 찾아 수정
                    var searchedItem = root.Children.Where(t => t.Id == TreeManager.SetTreeGroupId(viewModel.Id)).FirstOrDefault();

                    if (item.Name != viewModel.NameArea)
                    {
                        ///자녀트리 삭제 처리
                        TreeManager.SetTreeClear(item.Children);
                        ///변경된 NameArea 정보 수정
                        item.Name = viewModel.NameArea;

                        ///자녀트리 추가
                        AddGroupSubTree(item);
                    }
                    item.Used = viewModel.Used;
                    item.Visibility = viewModel.Visibility;

                }
                else if(dataType == EnumDataType.Sensor)
                {
                    item = TreeManager.GetMatchedId(Items, itemId);
                    var controller = item?.ParentTree as TreeContentControlViewModel;
                    var group = controller?.ParentTree as TreeContentControlViewModel;
                    if (item ==null)
                    {
                        ///이미 수정된 트리가 소속되지 않은 경우
                        ///해당 ContentControlViewModel에 부모노드에 해당하는 
                        ///Controller TreeNode를 등록
                        TreeContentControlViewModel curController = AddControllerTree(viewModel);
                        if (curController == null)
                            return;
                        ///등록된 부모노드를 갖고 하위 노드에 viewModel을 등록
                        TreeContentControlViewModel curSensor = AddSensorTree(curController, viewModel);
                    }else if (item != null
                        && group.Name != viewModel.NameArea)
                    {
                        ///Tree node에 자신과 동일한 아이디를 갖는 노드를 찾으면,
                        ///해당 노드를 삭제
                        //RemoveSensorTree(item);
                        controller.Children.Remove(item);

                        if (!(controller.Children.Count() > 0))
                        {
                            group.Children.Remove(controller);
                            await controller?.DeactivateAsync(true);
                        }
                        ///Controller Tree Node 등록
                        item.ParentTree = AddControllerTree(viewModel);
                        AddTree(item);
                        
                        ///등록된 부모노드를 갖고 하위 노드에 viewModel을 등록
                        //TreeContentControlViewModel curSensor = AddSensorTree(curController, viewModel);
                    }
                    else
                    {
                        item.Name = viewModel.IdSensor.ToString();
                        item.Description = viewModel.NameDevice;
                        item.Used = viewModel.Used;
                        item.Visibility = viewModel.Visibility;
                    }
                }

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
        

        private TreeContentControlViewModel AddSensorTree(TreeContentControlViewModel parentNode, SymbolContentControlViewModel viewModel)
        {
            ///센서 트리노드 생성
            var sensorTree = new TreeContentControlViewModel(TreeManager.SetTreeSensorId(viewModel.Id), viewModel.IdSensor.ToString(), viewModel.NameDevice, EnumTreeType.LEAF, viewModel.Used, viewModel.Visibility, parentNode, EnumDataType.Sensor, _eventAggregator, _mapProvider, _groupProvider, _controllerProvider, _sensorProvider) { DisplayName = $"[{EnumTreeType.LEAF.ToString()}]{viewModel.IdSensor} {EnumDataType.Sensor.ToString()}" };

            ///센서 트리노드 부모노드 확인
            ///Items, 센서 아이디
            AddTree(sensorTree);
            return sensorTree;
        }

        private TreeContentControlViewModel AddControllerTree(SymbolContentControlViewModel viewModel)
        {
            ///변경될 Group Tree 확인
            var curGroup = Items.FirstOrDefault() // GroupRoot
                .Children.Where(t => t.Name == viewModel.NameArea).FirstOrDefault(); //Group

            if (curGroup == null)
                return null;

            var curController = curGroup.Children.Where(t => t.Name == viewModel.IdController.ToString()).FirstOrDefault();

            ///제어기가 없으면,
            ///제어기 노드를 추가 생성
            if (curController == null)
            {
                ///제어기 TreeContetnControlViewMdoel 등록을 위한
                ///_controllerProvider의 ContentControlViewModel 검색
                var selectedController = _controllerProvider.Where(t => t.IdController == viewModel.IdController).FirstOrDefault();
                if (selectedController == null)
                    return null;

                //2-4
                ///제어기 등록
                ///$"C{0}"
                ///****************TreeContetnControlViewMdoel 생성 및 등록*****************************
                curController = new TreeContentControlViewModel(TreeManager.SetTreeControllerId(selectedController.Id), selectedController.IdController.ToString(), selectedController.NameDevice, EnumTreeType.BRANCH, selectedController.Used, selectedController.Visibility, curGroup, EnumDataType.Controller, _eventAggregator, _mapProvider, _groupProvider, _controllerProvider, _sensorProvider) { DisplayName = $"[{EnumTreeType.BRANCH.ToString()}]{selectedController.IdController} {EnumDataType.Controller.ToString()}" };
                ///**************************************************************************************
                //curGroup.Children.Add(curController);
                AddTree(curController);
            }
            return curController;
        }

        private string SetTreeNodeId(SymbolContentControlViewModel viewModel)
        {
            var itemId = "";

            if ((viewModel.TypeDevice == (int)EnumDeviceType.NONE)
                && (viewModel.IdController == 0)
                && (viewModel.IdSensor == 0))
                itemId = TreeManager.SetTreeGroupId(viewModel.Id);
            else if (viewModel.TypeDevice == (int)EnumDeviceType.Controller)
                itemId = TreeManager.SetTreeControllerId(viewModel.Id);
            else if (viewModel.TypeDevice != (int)EnumDeviceType.NONE
                && viewModel.TypeDevice != (int)EnumDeviceType.Controller)
                itemId = TreeManager.SetTreeSensorId(viewModel.Id);
            else
                throw new Exception(message: "DeviceType was Not matched with EnumDeviceType.");

            return itemId;
        }

        /// <summary>
        /// Tree에 노드 삭제
        /// </summary>
        /// <param name="node">등록할 Tree Node</param>
        /// <returns>boolean Type</returns>
        protected override void RemoveTree(TreeContentControlViewModel node)
        {
            switch (node.DataType)
            {
                case EnumDataType.GroupRoot:
                    break;
                case EnumDataType.Group:
                    break;
                case EnumDataType.Sensor:
                    var parent = node.ParentTree as TreeContentControlViewModel;
                    if (parent.Children.Count() == 1)
                        node = parent;
                    break;
                case EnumDataType.Controller:
                    break;
                default:
                    break;
            }
            base.RemoveTree(node);
        }
        /*protected override void RemoveTree(TreeContentControlViewModel node)
        {
            ///자녀트리 삭제 처리
            TreeManager.SetTreeClearChildrenWithProvidersInGroup(node.Children, _controllerProvider, _sensorProvider, _groupProvider);
            
            ///본인 트리 삭제 처리
            ///1. 부모노드 식별
            ///2. node의 타입 식별
            ///3. 트리노드 삭제
            ///4. provider 삭제 혹은 초기화
            try
            {
                //1
                var parentNode = node.ParentTree as TreeContentControlViewModel;

                ///GroupRoot 트리 삭제
                if (parentNode == null)
                {
                    Items.Remove(node);
                }
                else
                {
                    //2
                    switch (node.DataType)
                    {
                        case EnumDataType.GroupRoot:
                            ///parentNode가 null일 경우로 처리
                            break;
                        case EnumDataType.Group:
                            ///Items로 부터 부모트리 할당
                            var groupRoot = Items.FirstOrDefault();
                            ///트리노드 삭제
                            groupRoot.Children.Remove(node);

                            ///트리노드에 해당하는 _groupProvider의 아이템을 찾는다.
                            var item = _groupProvider.Where(t => t.Id == TreeManager.GetGroupProviderId(node.Id)).FirstOrDefault();
                            ///_groupProvider에서 삭제
                            _groupProvider.Remove(item);
                            break;
                        case EnumDataType.Controller:
                            ///Items로 부터 GroupRoot를 할당
                            var group = Items.FirstOrDefault() //GroupRoot
                                .Children.Where(group => group.Id == parentNode.Id).FirstOrDefault(); //Group
                            group.Children.Remove(node);

                            ///Controller는 별도의 provider의 아이템 처리가 필요하지 않다.
                            break;
                        case EnumDataType.Sensor:
                            ///NameArea를 _sensorProvider에서 찾는다.
                            var searchedItem = _sensorProvider
                                .Where(t => t.Id == TreeManager.GetSensorProviderId(node.Id)).FirstOrDefault();


                            ///해당 NameArea를 갖는 Group에 속한 Controller를 찾는다.
                            var controller = Items.FirstOrDefault() //GroupRoot
                                .Children.Where(group => group.Name == searchedItem.NameArea).FirstOrDefault()//Group
                                .Children.Where(con => con.Id == TreeManager.SetTreeControllerId(searchedItem.IdController)).FirstOrDefault();//Controller

                            ///controller 트리에서 삭제
                            controller.Children.Remove(node);
                            ///_sensorProvidr 아이템 초기화
                            searchedItem.NameArea = "Untitle";
                            break;
                        default:
                            Debug.WriteLine($"아무것도 선택 되지 않았습니다.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception at RemoveTree in GroupTree : {ex.Message}");
            }
            finally
            {
                node.DeactivateAsync(true);
                node = null;
            }
        }*/


        /// <summary>
        /// 트리 노드를 선택할 경우, 해당 아이템의 세부내역(PropertySection)을
        /// 활성화 시키기 위한 이벤트 호출 메소드
        /// </summary>
        protected override void UpdateSelectedItem()
        {
            if (SelectedItem == null)
                return;

            base.UpdateSelectedItem();

            ///DataType을 이용한 Group, Sensor 구분
            if (SelectedItem.DataType == EnumDataType.Group)
            {
                var viewModel = _groupProvider.CollectionEntity
                .Where(item => TreeManager.SetTreeGroupId(item.Id) == SelectedItem.Id)
                .SingleOrDefault();

                if (viewModel != null)
                    _eventAggregator.PublishOnUIThreadAsync(new OpenGroupPropertyMessageModel(viewModel));
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
        /*public Task HandleAsync(GroupTreeAddMessageModel message, CancellationToken cancellationToken)
        {
            //Get ViewModel
            var viewModel = message.ViewModel;
            //최대그룹번호+1의 값을 찾음
            var id = _groupProvider.GetMaxId() + 1;
            var nameArea = 0;
            int.TryParse(ProviderManager.GetMaxNameArea(_groupProvider), out nameArea);
            nameArea++;

            //*************************GroupContentControlViewModel 생성, 활성화 및 추가****************************
            var contentControlViewModel = new GroupContentControlViewModel(id, nameArea.ToString(), (int)EnumDeviceType.NONE, nameArea.ToString(), 0, 0, 0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0, false, false, _eventAggregator, _groupProvider, _mapProvider) { DisplayName = $"{id} {EnumDataType.Group.ToString()}" };
            //MapContentControlViewModel 활성화
            contentControlViewModel.ActivateAsync();
            ///Provider에 데이터 추가
            _groupProvider.Add(contentControlViewModel);
            //************************************************************************************************************

            ///초기화된 TreeContentControlViewModel인 경우
            ///아래와 같이 TreeContetnControlViewModel과 GroupContentControlViewModel을
            ///매칭하는 프로세스를 진행
            if (viewModel.Id == TreeManager.SetTreeGroupId(0))
            {
                //트리 노드 중 최대 ID 값 보다 1증가한 값을 ID로 할당
                viewModel.Id = TreeManager.SetTreeGroupId(id);
                viewModel.Name = contentControlViewModel.NameArea;
                viewModel.Description = contentControlViewModel.NameArea;
                viewModel.DisplayName = $"[{EnumTreeType.BRANCH.ToString()}]{id} {EnumDataType.Group.ToString()}";
            }
            //AddTree Tree Node 추가 프로세스
            AddTree(viewModel);

            ///센서리스트 조회
            AddGroupSubTree(viewModel);

            return Task.CompletedTask;
        }*/

        private void AddGroupSubTree(TreeContentControlViewModel viewModel)
        {
            if (!(_sensorProvider.Where(t => t.NameArea == viewModel.Name).Count() > 0))
                return;

            ///해당 센서의 제어기 등록여부 확인 
            ///미 등록 시, 등록
            ///

            var sensorList = _sensorProvider.Where(t => t.NameArea == viewModel.Name);
            foreach (var sensor in sensorList)
            {
                ///해당 센서를 포함하는 제어기를 조회
                var selectedController = _controllerProvider.Where(t => t.IdController == sensor.IdController).FirstOrDefault();
                ///해당 센서를 포함시킬 제어기가 없으면, 
                ///센서 등록을 할 수 없음
                if (selectedController == null)
                    continue;

                ///제어기가 _controllerProvider에 있으며, 
                ///해당 그룹범위의 TreeNode로 존재하는지 확인
                var searchedController = TreeManager.GetMatchedId(viewModel.Children, TreeManager.SetTreeControllerId(selectedController.Id));
                
                ///해당 제어기가 TreeNode로 존재하는 경우
                if (searchedController != null)
                {
                    //2-3
                    ///센서 등록
                    ///$"S{0}"
                    ///****************TreeContetnControlViewMdoel 생성 및 등록*****************************
                    var node = new TreeContentControlViewModel(TreeManager.SetTreeSensorId(sensor.Id), sensor.IdSensor.ToString(), sensor.NameDevice, EnumTreeType.LEAF, sensor.Used, sensor.Visibility, searchedController, EnumDataType.Sensor, _eventAggregator) { DisplayName = $"[{EnumTreeType.LEAF.ToString()}]{sensor.IdSensor} {EnumDataType.Sensor.ToString()}" };
                    ///**************************************************************************************
                    AddTree(node);
                }
                else
                {
                    //2-4
                    ///제어기 등록
                    ///$"C{0}"
                    ///****************TreeContetnControlViewMdoel 생성 및 등록*****************************
                    var parentNode = new TreeContentControlViewModel(TreeManager.SetTreeControllerId(selectedController.Id), selectedController.IdController.ToString(), selectedController.NameDevice, EnumTreeType.LEAF, selectedController.Used, selectedController.Visibility, viewModel, EnumDataType.Controller, _eventAggregator) { DisplayName = $"[{EnumTreeType.BRANCH.ToString()}]{sensor.IdController} {EnumDataType.Controller.ToString()}" };
                    ///**************************************************************************************
                    AddTree(parentNode);

                    ///센서 등록
                    ///$"S{0}"
                    ///****************TreeContetnControlViewMdoel 생성 및 등록*****************************
                    var node = new TreeContentControlViewModel(TreeManager.SetTreeSensorId(sensor.Id), sensor.IdSensor.ToString(), sensor.NameDevice, EnumTreeType.LEAF, sensor.Used, sensor.Visibility, parentNode, EnumDataType.Sensor, _eventAggregator) { DisplayName = $"[{EnumTreeType.LEAF.ToString()}]{sensor.IdSensor} {EnumDataType.Sensor.ToString()}" };
                    ///**************************************************************************************
                    AddTree(node);
                }
            }
        }

        public Task HandleAsync(GroupTreeRemoveMessageModel message, CancellationToken cancellationToken)
        {
            RemoveTree(message.ViewModel);
            return Task.CompletedTask;
        }

        public Task HandleAsync(GroupContentUpdateMessageModel message, CancellationToken cancellationToken)
        {
            InitialTree();
            return Task.CompletedTask;
        }

        public Task HandleAsync(ControllerTreeAddMessageModel message, CancellationToken cancellationToken)
        {
            InitialTree();
            return Task.CompletedTask;
        }

        public Task HandleAsync(ControllerTreeRemoveMessageModel message, CancellationToken cancellationToken)
        {
            ///최초 브랜치가 GroupRoot이면,
            ///GroupTreeViewModel에서 처리

            var root = TreeManager.GetRootNode(message.ViewModel);
            if (root.DataType == EnumDataType.GroupRoot)
                RemoveTree(message.ViewModel);
            return Task.CompletedTask;
        }

        public Task HandleAsync(ControllerContentUpdateMessageModel message, CancellationToken cancellationToken)
        {
            InitialTree();
            return Task.CompletedTask;
        }

        public Task HandleAsync(SensorTreeAddMessageModel message, CancellationToken cancellationToken)
        {
            InitialTree();
            return Task.CompletedTask;
        }

        public Task HandleAsync(SensorTreeRemoveMessageModel message, CancellationToken cancellationToken)
        {
            ///최초 브랜치가 GroupRoot이면,
            ///GroupTreeViewModel에서 처리
            var root = TreeManager.GetRootNode(message.ViewModel);
            if (root.DataType == EnumDataType.GroupRoot)
                RemoveTree(message.ViewModel);
            return Task.CompletedTask;
        }

        public Task HandleAsync(SensorContentUpdateMessageModel message, CancellationToken cancellationToken)
        {
            InitialTree();
            return Task.CompletedTask;
        }

        public Task HandleAsync(SymbolContentUpdateMessageModel message, CancellationToken cancellationToken)
        {
            var viewModel = message?.ViewModel;
            var property = message?.Property;
            if (viewModel == null)
                return Task.CompletedTask;

            if (viewModel.GetType() == typeof(GroupContentControlViewModel)
                || viewModel.GetType() == typeof(SensorContentControlViewModel))
                UpdateTree(message?.ViewModel);
            //else if(property == "NameArea")

            return Task.CompletedTask;
        }

        public Task HandleAsync(SensorContentRemoveMessageModel message, CancellationToken cancellationToken)
        {
            var item = TreeManager.GetMatchedId(Items, TreeManager.SetTreeSensorId((int)message?.ViewModel.Id));
            if (item == null)
                return Task.CompletedTask;

            var root = TreeManager.GetRootNode(item);
            if (root.DataType == EnumDataType.GroupRoot)
                RemoveTree(item);

            return Task.CompletedTask;

        }

        public Task HandleAsync(ControllerContentRemoveMessageModel message, CancellationToken cancellationToken)
        {
            var viewModel = message?.ViewModel;
            
            //해당 Controller만 있는 Group을 List로
            var groupRoot = Items.FirstOrDefault(); //GroupRoot

            foreach (var group in groupRoot.Children)
            {
                var controllerList = group.Children.Where(t => t.Name == message?.ViewModel.IdController.ToString()).ToList();
                foreach (var controller in controllerList)
                {
                    var root = TreeManager.GetRootNode(controller);
                    if (root.DataType == EnumDataType.GroupRoot)
                        RemoveTree(controller);
                }
            }

            return Task.CompletedTask;
        }
        #endregion

        #region - Properties -
        #endregion
        #region - Attributes -
        private GroupProvider _groupProvider;
        private ControllerProvider _controllerProvider;
        private SensorProvider _sensorProvider;
        private MapProvider _mapProvider;
        #endregion

    }
}
