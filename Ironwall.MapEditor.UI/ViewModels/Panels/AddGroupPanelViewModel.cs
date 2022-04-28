using Caliburn.Micro;
using Ironwall.Enums;
using Ironwall.Framework.Services;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.MapEditor.Ui.Helpers;
using Ironwall.MapEditor.UI.DataProviders;
using Ironwall.MapEditor.UI.Helpers;
using Ironwall.MapEditor.UI.Models.Messages.Panels;
using Ironwall.MapEditor.UI.Models.Messages.PopupDialogs;
using Ironwall.MapEditor.UI.Models.Messages.Process;
using Ironwall.MapEditor.UI.ViewModels.ContentControls;
using Ironwall.MapEditor.UI.ViewModels.RegisteredItems;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.ViewModels.Panels
{
    internal sealed class AddGroupPanelViewModel
        : BaseViewModel
        , IPanelViewModel
        , IHandle<AddPanelUpdateMessageModel>
    {
        #region - Ctors -
        public AddGroupPanelViewModel(
            IEventAggregator eventAggregator
            , ControllerProvider controllerProvider
            , SensorProvider sensorProvider
            , GroupProvider groupProvider
            , MapProvider mapProvider
            , GroupTreeViewModel treeViewModel
            )
        {
            #region - Settings -
            Id = 13;
            Content = "";
            Category = CategoryEnum.PANEL_SHELL_VM_ITEM;
            #endregion - Settings -

            _eventAggregator = eventAggregator;
            _controllerProvider = controllerProvider;
            _mapProvider = mapProvider;
            _sensorProvider = sensorProvider;
            _groupProvider = groupProvider;

            _treeViewModel = treeViewModel;
        }
        #endregion

        #region - Implementation of Interface -
        public void ModelUpdate()
        {
            ContentControlViewModel.Update();
            NotifyOfPropertyChange(nameof(CanClickOkAsync));
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
            await Task.Delay(500).ContinueWith(async (_, t) =>
            {
                try
                {
                    ///ContentControlViewModel의 데이터가 정상적인지 확인

                    ///ContentControlViewModel Activate 시킴
                    await ContentControlViewModel.ActivateAsync();

                    ///추가
                    _groupProvider.Add(ContentControlViewModel);

                    ///TreeContentControlViewModel 생성
                    var id = ContentControlViewModel.Id;
                    var treeParent = _treeViewModel.Items.FirstOrDefault();
                    var treeNode = new TreeContentControlViewModel(TreeManager.SetTreeGroupId(id), ContentControlViewModel.NameArea, ContentControlViewModel.NameArea, EnumTreeType.BRANCH, true, true, treeParent, EnumDataType.Group, _eventAggregator, _mapProvider, _groupProvider, _controllerProvider, _sensorProvider) { DisplayName = $"[{EnumTreeType.BRANCH.ToString()}]{id} {EnumDataType.Group.ToString()}" };
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
                catch (Exception ex)
                {
                    Debug.WriteLine($"Exception was raised : {ex.Message}");
                }
                finally
                {
                   ///로딩화면 종료
                    await _eventAggregator.PublishOnCurrentThreadAsync(new ClosePopupDialogMessageModel(), _cancellationTokenSource.Token);
                }
            }, _cancellationTokenSource.Token).ContinueWith(async (_, t) =>
            {
                ///AddMapPanel 종료
                await _eventAggregator.PublishOnCurrentThreadAsync(new ClosePanelMessageModel(), _cancellationTokenSource.Token);
            }, _cancellationTokenSource.Token);
        }

        public async void ClickCancelAsync()
        {
            await _eventAggregator.PublishOnCurrentThreadAsync(new ClosePanelMessageModel(), _cancellationTokenSource.Token);
        }
        #endregion

        #region - Overrides -
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            base.OnActivateAsync(cancellationToken);

            var id = _groupProvider.GetMaxId() + 1;
            var nameArea = int.Parse(ProviderManager.GetMaxNameArea(_groupProvider)) + 1;

            ///초기 화면이 Activate되면,
            ///SymbolContentControlViewModel의 인스턴스 생성
            ContentControlViewModel = new GroupContentControlViewModel(id, nameArea.ToString(), (int)EnumDeviceType.NONE, nameArea.ToString(), 0, 0, 0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0, false, false, _eventAggregator, _groupProvider, _mapProvider) { DisplayName = $"{id} {EnumDataType.Group.ToString()}" };

            ModelUpdate();
            return Task.CompletedTask;
        }

        public Task HandleAsync(AddPanelUpdateMessageModel message, CancellationToken cancellationToken)
        {
            ModelUpdate();
            return Task.CompletedTask;
        }
        #endregion

        #region - Processes -
        #endregion

        #region - Binding Methods -
        #endregion

        #region - IHanldes -
        #endregion
        #region - Properties -
        public GroupContentControlViewModel ContentControlViewModel { get; set; }
        #endregion
        #region - Attributes -
        private GroupProvider _groupProvider;
        private GroupTreeViewModel _treeViewModel;
        private ControllerProvider _controllerProvider;
        private MapProvider _mapProvider;
        private SensorProvider _sensorProvider;
        #endregion
    }
}
