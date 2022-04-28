using Caliburn.Micro;
using Ironwall.Enums;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.MapEditor.UI.Models.Messages.Dialogs;
using Ironwall.MapEditor.UI.Models.Messages.Panels;
using Ironwall.MapEditor.UI.Models.Messages.PopupDialogs;
using Ironwall.MapEditor.UI.ViewModels.Dialogs;
using Ironwall.MapEditor.UI.ViewModels.Panels;
using Ironwall.MapEditor.UI.ViewModels.PopupDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.ViewModels.Conductors
{
    internal class ConductorControlViewModel
        : BaseViewModel
        , IConductorControlViewModel
    #region -Receive Handles -
        , IHandle<ClosePanelMessageModel>
        , IHandle<OpenShowListPanelMessageModel>
        , IHandle<OpenAddMapPanelMessageModel>
        , IHandle<OpenAddControlPanelMessageModel>
        , IHandle<OpenAddSensorPanelMessageModel>
        , IHandle<OpenAddGroupPanelMessageModel>
        , IHandle<OpenAddCameraPanelMessageModel>
        , IHandle<OpenSavePanelMessageModel>
        , IHandle<OpenLoadPanelMessageModel>

        , IHandle<CloseDialogMessageModel>

        , IHandle<ClosePopupDialogMessageModel>
        , IHandle<OpenProgressPopupMessageModel>
    #endregion
    {
        #region - Ctors -
        public ConductorControlViewModel(
            IEventAggregator eventAggregator
            , PanelShellViewModel panelShellViewModel
            , DialogShellViewModel dialogShellViewModel
            , PopupDialogShellViewModel popupDialogShellViewModel)
        {
            #region - Settings -
            Id = 0;
            Content = "";
            Category = CategoryEnum.SECTION_SHELL_VM;
            #endregion - Settings -

            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnPublishedThread(this);

            PanelShellViewModel = panelShellViewModel;
            DialogShellViewModel = dialogShellViewModel;
            PopupDialogShellViewModel = popupDialogShellViewModel;
        }
        #endregion

        #region - Panels -
        public Task HandleAsync(ClosePanelMessageModel message, CancellationToken cancellationToken)
        {
            var conductorShell = PanelShellViewModel;

            //현재 Conductor가 Active 상태인지 확인한다.
            if (!conductorShell.IsActive)
                conductorShell.ActivateAsync(); //Active가 아니면 활성화 시킨다.

            //이전에 담긴 Item은 무시한다.
            conductorShell.Items.Clear();
            //현재 Conductor에 담긴 Item은 Deactivate 시킨다.
            conductorShell.DeactivateItemAsync(conductorShell.ActiveItem, true, cancellationToken);
            //결론적으로 Conductor를 Deactivate 시킨다.
            return conductorShell.TryCloseAsync();
        }

        public Task HandleAsync(OpenLoadPanelMessageModel message, CancellationToken cancellationToken)
        {
            var conductorShell = PanelShellViewModel;

            if (!conductorShell.IsActive)
                conductorShell.ActivateAsync();

            return conductorShell.ActivateItemAsync(IoC.Get<LoadPanelViewModel>(), cancellationToken);
        }

        public Task HandleAsync(OpenSavePanelMessageModel message, CancellationToken cancellationToken)
        {
            var conductorShell = PanelShellViewModel;

            if (!conductorShell.IsActive)
                conductorShell.ActivateAsync();

            return conductorShell.ActivateItemAsync(IoC.Get<SavePanelViewModel>(), cancellationToken);
        }

        public Task HandleAsync(OpenShowListPanelMessageModel message, CancellationToken cancellationToken)
        {
            var conductorShell = PanelShellViewModel;

            if (!conductorShell.IsActive)
                conductorShell.ActivateAsync();

            return conductorShell.ActivateItemAsync(IoC.Get<ShowListPanelViewModel>(), cancellationToken);
        }

        public Task HandleAsync(OpenAddMapPanelMessageModel message, CancellationToken cancellationToken)
        {
            var conductorShell = PanelShellViewModel;

            if (!conductorShell.IsActive)
                conductorShell.ActivateAsync();

            return conductorShell.ActivateItemAsync(IoC.Get<AddMapPanelViewModel>(), cancellationToken);
        }

        public Task HandleAsync(OpenAddControlPanelMessageModel message, CancellationToken cancellationToken)
        {
            var conductorShell = PanelShellViewModel;

            if (!conductorShell.IsActive)
                conductorShell.ActivateAsync();

            return conductorShell.ActivateItemAsync(IoC.Get<AddControllerPanelViewModel>(), cancellationToken);
        }

        public Task HandleAsync(OpenAddSensorPanelMessageModel message, CancellationToken cancellationToken)
        {
            var conductorShell = PanelShellViewModel;

            if (!conductorShell.IsActive)
                conductorShell.ActivateAsync();

            return conductorShell.ActivateItemAsync(IoC.Get<AddSensorPanelViewModel>(), cancellationToken);
        }

        public Task HandleAsync(OpenAddGroupPanelMessageModel message, CancellationToken cancellationToken)
        {
            var conductorShell = PanelShellViewModel;

            if (!conductorShell.IsActive)
                conductorShell.ActivateAsync();

            return conductorShell.ActivateItemAsync(IoC.Get<AddGroupPanelViewModel>(), cancellationToken);
        }

        public Task HandleAsync(OpenAddCameraPanelMessageModel message, CancellationToken cancellationToken)
        {
            var conductorShell = PanelShellViewModel;

            if (!conductorShell.IsActive)
                conductorShell.ActivateAsync();

            return conductorShell.ActivateItemAsync(IoC.Get<AddCameraPanelViewModel>(), cancellationToken);
        }

        #endregion

        #region - Dialogs -.
        public Task HandleAsync(CloseDialogMessageModel message, CancellationToken cancellationToken)
        {
            var conductorShell = DialogShellViewModel;

            //현재 Conductor가 Active 상태인지 확인한다.
            if (!conductorShell.IsActive)
                conductorShell.ActivateAsync(); //Active가 아니면 활성화 시킨다.

            //이전에 담긴 Item은 무시한다.
            conductorShell.Items.Clear();
            //현재 Conductor에 담긴 Item은 Deactivate 시킨다.
            conductorShell.DeactivateItemAsync(conductorShell.ActiveItem, true, cancellationToken);
            //결론적으로 Conductor를 Deactivate 시킨다.
            return conductorShell.TryCloseAsync();
        }
        #endregion

        #region - PopupDialogs -
        public Task HandleAsync(ClosePopupDialogMessageModel message, CancellationToken cancellationToken)
        {
            var conductorShell = PopupDialogShellViewModel;

            //현재 Conductor가 Active 상태인지 확인한다.
            if (!conductorShell.IsActive)
                conductorShell.ActivateAsync(); //Active가 아니면 활성화 시킨다.

            //이전에 담긴 Item은 무시한다.
            conductorShell.Items.Clear();
            //현재 Conductor에 담긴 Item은 Deactivate 시킨다.
            conductorShell.DeactivateItemAsync(conductorShell.ActiveItem, true, cancellationToken);
            //결론적으로 Conductor를 Deactivate 시킨다.
            return conductorShell.TryCloseAsync();
        }

        public Task HandleAsync(OpenProgressPopupMessageModel message, CancellationToken cancellationToken)
        {
            var conductorShell = PopupDialogShellViewModel;

            if (!conductorShell.IsActive)
                conductorShell.ActivateAsync();

            return conductorShell.ActivateItemAsync(IoC.Get<ProgressPopupDialogViewModel>(), cancellationToken);
        }

        






        #endregion

        #region - Properties -
        public PanelShellViewModel PanelShellViewModel { get; }
        public DialogShellViewModel DialogShellViewModel { get; }
        public PopupDialogShellViewModel PopupDialogShellViewModel { get; }
        #endregion

        #region - Attributes -
        #endregion
    }
}
