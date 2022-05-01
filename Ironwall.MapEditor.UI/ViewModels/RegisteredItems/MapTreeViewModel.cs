using Caliburn.Micro;
using Ironwall.Enums;
using Ironwall.Framework.Services;
using Ironwall.MapEditor.Ui.Helpers;
using Ironwall.MapEditor.UI.DataProviders;
using Ironwall.MapEditor.UI.Helpers;
using Ironwall.MapEditor.UI.Models;
using Ironwall.MapEditor.UI.Models.Messages.Process;
using Ironwall.MapEditor.UI.Models.Messages.Sections;
using Ironwall.MapEditor.UI.Services;
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
    public sealed class MapTreeViewModel
        : TreeBaseViewModel<MapContentControlViewModel>
        , IHandle<MapContentUpdateMessageModel>
    {
        #region - Ctors -
        /// <summary>
        /// Default constructor
        /// </summary>
        public MapTreeViewModel(
            MapProvider provider
            , IEventAggregator eventAggregator)
        {
            _provider = provider;
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

            //Root가 null인경우 초기화
            AddTree(new TreeContentControlViewModel($"R{0}", "전체맵", "맵 전체 구성", EnumTreeType.ROOT, true, true, null, EnumDataType.MapRoot, _eventAggregator, _provider) { DisplayName = $"[{EnumTreeType.ROOT.ToString()}]{0} {EnumDataType.MapRoot.ToString()}" });

            //Root Node 활성화 상태 확인
            if (!Items.FirstOrDefault().IsActive)
                await Items.FirstOrDefault().ActivateAsync();
        }
        /// <summary>
        /// 해당 Provider를 이용하여, 트리노드 Branch 혹은 Leaf 형성
        /// </summary>
        protected override void SetTreeWithProvider()
        {
            base.SetTreeWithProvider();

            ///맵 등록 리스트 생성
            ///***********************TreeContentControlViewModel 생성*********************
            var itemList = _provider.Select(mapItem => new TreeContentControlViewModel(TreeManager.SetTreeMapId(mapItem.Id), mapItem.MapName, mapItem.Url, EnumTreeType.LEAF, mapItem.Used, mapItem.Visibility, Items.FirstOrDefault(), EnumDataType.Map, _eventAggregator, _provider) { DisplayName = $"[{EnumTreeType.LEAF.ToString()}]{mapItem.Id} {EnumDataType.Map.ToString()}" });

            ///맵 등록
            itemList.ToList().ForEach(item => AddTree(item));
        }

        public async Task SelectDefaultMapAsync()
        {
            var task = Task.Run(() =>
            {
                var SelectedMap = Items.FirstOrDefault().Children.FirstOrDefault();
                SelectedMap.IsSelected = true;
                SelectedItem = SelectedMap;
            });

            await task;
        }

        /// <summary>
        /// 선택된 트리 노드를 기준으로 변경사항 업데이트
        /// </summary>
        /// <param name="viewModel">변경을 위한 ViewModel 데이터</param>
        protected override void UpdateTree(MapContentControlViewModel viewModel = null)
        {
            ///해당 viewModel의 속성을 변경
            try
            {
                ///해당 viewModel의 Id와 같은 Id를
                ///갖는 TreeItemViewModel을 찾고,
                ///나머지 정보를 업데이트 해준다.

                if (viewModel == null)
                    return;

                var item = TreeManager.GetMatchedId(Items, TreeManager.SetTreeMapId(viewModel.Id));

                if (item == null)
                    return;

                item.Name = viewModel.MapName;
                item.Description = viewModel.Url;
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
            var viewModel = _provider.CollectionEntity
                .Where(item => TreeManager.SetTreeMapId(item.Id) == SelectedItem.Id)
                .SingleOrDefault();

            if (viewModel != null)
                _eventAggregator.PublishOnUIThreadAsync(new OpenMapPropertyMessageModel(viewModel));
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        /// <summary>
        /// ContentControlVM의 내용을 업데이트 시켜주기 위한 메소드
        /// </summary>
        /// <param name="message">업데이트에 필요한 TreeItemViewModel을 포함한 메시지</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>Task.CompletedTask</returns>
        public Task HandleAsync(MapContentUpdateMessageModel message, CancellationToken cancellationToken)
        {
            if (message?.ViewModel != null)
            {
                UpdateTree(message.ViewModel);
            }
            else
            {
                InitialTree();
            }
            return Task.CompletedTask;
        }


        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private MapProvider _provider;
        #endregion
    }
}
