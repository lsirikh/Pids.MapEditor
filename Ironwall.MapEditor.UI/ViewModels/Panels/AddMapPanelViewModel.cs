using Caliburn.Micro;
using FluentValidation.Results;
using Ironwall.Enums;
using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Helpers;
using Ironwall.Framework.Services;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.MapEditor.Ui.Helpers;
using Ironwall.MapEditor.UI.DataProviders;
using Ironwall.MapEditor.UI.Helpers;
using Ironwall.MapEditor.UI.Models;
using Ironwall.MapEditor.UI.Models.Messages.Panels;
using Ironwall.MapEditor.UI.Models.Messages.PopupDialogs;
using Ironwall.MapEditor.UI.Models.Messages.Process;
using Ironwall.MapEditor.UI.Models.Messages.Sections;
using Ironwall.MapEditor.UI.ModelValidators;
using Ironwall.MapEditor.UI.ViewModels.ContentControls;
using Ironwall.MapEditor.UI.ViewModels.RegisteredItems;
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
    internal sealed class AddMapPanelViewModel
        : BaseViewModel
        , IPanelViewModel
        , IHandle<AddPanelUpdateMessageModel>
    {
        public AddMapPanelViewModel(
            IEventAggregator eventAggregator
            , MapProvider mapProvider
            , MapTreeViewModel treeViewModel)
        {
            #region - Settings -
            Id = 11;
            Content = "";
            Category = CategoryEnum.PANEL_SHELL_VM_ITEM;
            #endregion - Settings -

            _eventAggregator = eventAggregator;
            _mapProvider = mapProvider;

            _treeViewModel = treeViewModel;
        }

        #region - Implementation of Interface -
        public void ModelUpdate()
        {
            ContentControlViewModel.Update();
            NotifyOfPropertyChange(nameof(CanClickOkAsync));
        }

        public int GetMaxId()
        {
            var count = 0;
            _mapProvider.Select(t => t).ToList().ForEach(t =>
            {
                if (t.Id > count)
                    count = t.Id;
            });
            return count;
        }

        /// <summary>
        /// NotifyOfPropertyChange(nameof(CanClickOk))이 있어야 갱신이 가능하다.
        /// </summary>
        public bool CanClickOkAsync => !ContentControlViewModel.IsValidationError;

        public async void ClickOkAsync()
        {
            ///로딩화면 시현
            await _eventAggregator.PublishOnCurrentThreadAsync(new OpenProgressPopupMessageModel(), _cancellationTokenSource.Token);

            ///500ms 지연 후(지연이 없으면 로딩화면을 볼 수 없음),
            ///ClickOkAsync의 로직 수행
            await Task.Delay(500).ContinueWith(async(_, t) => 
            {
                try
                {
                    ///MapContentControlViewModel의 데이터가 정상적인지 확인

                    ///MapContentControlViewModel Activate 시킴
                    await ContentControlViewModel.ActivateAsync();
                    ///추가
                    _mapProvider.Add(ContentControlViewModel);

                    ///TreeContentControlViewModel 생성
                    var id = ContentControlViewModel.Id;
                    var treeParent = _treeViewModel.Items.FirstOrDefault();
                    var treeNode = new TreeContentControlViewModel(TreeManager.SetTreeMapId(id), ContentControlViewModel.MapName, ContentControlViewModel.Url, EnumTreeType.LEAF, true, true, treeParent, EnumDataType.Map, _eventAggregator, _mapProvider) { DisplayName = $"[{EnumTreeType.LEAF.ToString()}]{id} {EnumDataType.Map.ToString()}" };
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
                catch (Exception ex)
                {
                    Debug.WriteLine($"Exception was raised : {ex.Message}");
                }
                finally
                {
                    ///로딩화면 종료
                    await _eventAggregator.PublishOnCurrentThreadAsync(new ClosePopupDialogMessageModel(), _cancellationTokenSource.Token);
                }

            }, _cancellationTokenSource.Token).ContinueWith(async(_, t) => 
            {
                ///AddMapPanel 종료
                await _eventAggregator.PublishOnCurrentThreadAsync(new ClosePanelMessageModel(), _cancellationTokenSource.Token);
            }, _cancellationTokenSource.Token);
        }

        public async void ClickCancelAsync()
        {
            ///AddMapPanel 종료
            await _eventAggregator.PublishOnCurrentThreadAsync(new ClosePanelMessageModel(), _cancellationTokenSource.Token);
            await TryCloseAsync();
        }

        
        #endregion

        #region - Handlers -
        /// <summary>
        /// 이 Handler는 MapContentControlViewModel에서 속성 변경 및
        /// ValidationCheck내용을 반영하기 위해서 수신하는 용도
        /// </summary>
        /// <param name="message">N/A</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task HandleAsync(AddPanelUpdateMessageModel message, CancellationToken cancellationToken)
        {
            ModelUpdate();
            return Task.CompletedTask;
        }
        #endregion

        #region - Overrides -
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            base.OnActivateAsync(cancellationToken);

            var id = _mapProvider.GetMaxId() + 1;
            var mapNumber = ProviderManager.GetMaxMapID(_mapProvider)+1;

            ///초기 화면이 Activate되면,
            ///MapContentControlViewModel의 인스턴스 생성
            ContentControlViewModel = new MapContentControlViewModel(id, $"{mapNumber}", mapNumber, null, null, null, 0.0, 0.0, false, false, _eventAggregator, _mapProvider)
            {
                DisplayName = $"{id} {EnumDataType.Map.ToString()}",
                broadCastring = false,
            };
            ModelUpdate();
            return Task.CompletedTask;
        }
        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            base.OnDeactivateAsync(close, cancellationToken);
            ContentControlViewModel = null;
            return Task.CompletedTask;
        }

        
        #endregion

        #region - Prcesses -
        #endregion

        #region - Properties -

        public MapContentControlViewModel ContentControlViewModel { get; set; }
        #endregion

        #region - Attributes -
        private MapProvider _mapProvider;
        private MapTreeViewModel _treeViewModel;
        #endregion
    }
}
