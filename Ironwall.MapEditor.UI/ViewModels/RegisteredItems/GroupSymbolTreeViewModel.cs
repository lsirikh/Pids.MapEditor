using Caliburn.Micro;
using Ironwall.Enums;
using Ironwall.MapEditor.Ui.Helpers;
using Ironwall.MapEditor.UI.DataProviders;
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
    public sealed class GroupSymbolTreeViewModel
        : TreeBaseViewModel<SymbolContentControlViewModel>

        , IHandle<SymbolContentUpdateMessageModel>
    {
        
        #region - Ctors -
        public GroupSymbolTreeViewModel()
        {
            _mapProvider = IoC.Get<MapProvider>();
            _groupProvider = IoC.Get<GroupProvider>();
            _groupSymbolProvider = IoC.Get<GroupSymbolProvider>();
            _eventAggregator = IoC.Get<IEventAggregator>();
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

            ///Root가 null인경우 초기화
            ///V0 카메라 ID 규격
            AddTree(new TreeContentControlViewModel($"R{0}", "전체 그룹", "카메라 전체 구성", EnumTreeType.ROOT, true, true, null, EnumDataType.GroupSymbolRoot, _eventAggregator, _mapProvider) { DisplayName = $"[{EnumTreeType.ROOT.ToString()}]{0} {EnumDataType.GroupRoot.ToString()}" });

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

            ///Provider를 이용한 카메라 등록
            ///$"G{0}"
            ///기존의 등록하던 방식과 동일하게 등록
            ///****************TreeContetnControlViewMdoel 생성************************
            var groupList = _groupProvider.Select(group => new TreeContentControlViewModel(TreeManager.SetTreeGroupId(group.Id), group.NameArea, group.NameDevice, EnumTreeType.BRANCH, group.Used, group.Visibility, Items.FirstOrDefault(), EnumDataType.Group, _eventAggregator) { DisplayName = $"[{EnumTreeType.BRANCH.ToString()}]{group.Id} {EnumDataType.Group.ToString()}" });

            groupList.ToList().ForEach(item => AddTree(item));

            var groupSymbolList = _groupSymbolProvider.Select(gSymbol => new TreeContentControlViewModel(TreeManager.SetTreeGroupSymbolId(gSymbol.Id), gSymbol.NameArea, gSymbol.NameDevice, EnumTreeType.LEAF, gSymbol.Used, gSymbol.Visibility, Items.FirstOrDefault(), EnumDataType.GroupSymbol, _eventAggregator) { DisplayName = $"[{EnumTreeType.LEAF.ToString()}]{gSymbol.Id} {EnumDataType.GroupSymbol.ToString()}" });


        }
        /// <summary>
        /// 선택된 트리 노드를 기준으로 변경사항 업데이트
        /// </summary>
        /// <param name="viewModel">변경을 위한 ViewModel 데이터</param>
        protected override void UpdateTree(SymbolContentControlViewModel viewModel)
        {
            ///해당 viewModel의 속성을 변경
            try
            {
                ///해당 viewModel의 Id와 같은 Id를
                ///갖는 TreeItemViewModel을 찾고,
                ///나머지 정보를 업데이트 해준다.

                if (viewModel == null)
                    return;

                var item = TreeManager.GetMatchedId(Items, TreeManager.SetTreeCameraId(viewModel.Id));

                if (item == null)
                    return;

                item.Name = viewModel.NameArea;
                item.Description = viewModel.NameDevice;
                item.Used = viewModel.Used;
                item.Visibility = viewModel.Visibility;

                item.IsSelected = true;
                SelectedItem = item;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in UpdateTree : {ex.Message}");
                return;
            }

        }

        /// <summary>
        /// 트리 노드를 선택할 경우, 해당 아이템의 세부내역(PropertySection)을
        /// 활성화 시키기 위한 이벤트 호출 메소드
        /// </summary>
        protected override void UpdateSelectedItem()
        {
            var viewModel = _groupSymbolProvider.CollectionEntity
            .Where(item => TreeManager.SetTreeCameraId(item.Id) == SelectedItem.Id)
            .SingleOrDefault();

            if (viewModel != null)
                _eventAggregator.PublishOnUIThreadAsync(new OpenGroupSymbolPropertyMessageModel(viewModel));

            _eventAggregator.PublishOnUIThreadAsync(new GroupSymbolSelectedMessageModel());
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        public Task HandleAsync(SymbolContentUpdateMessageModel message, CancellationToken cancellationToken)
        {
            //Add Function
            return Task.CompletedTask;
        }

        private MapProvider _mapProvider;
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private GroupProvider _groupProvider;
        private GroupSymbolProvider _groupSymbolProvider;
        #endregion
    }
}
