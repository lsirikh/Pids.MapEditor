using Caliburn.Micro;
using Ironwall.MapEditor.UI.ViewModels.Dialogs;
using Ironwall.MapEditor.UI.ViewModels.Panels;
using Ironwall.MapEditor.UI.ViewModels.PopupDialogs;

namespace Ironwall.MapEditor.UI.ViewModels.Conductors
{
    internal interface IConductorControlViewModel
    {
        PanelShellViewModel PanelShellViewModel { get; }
        DialogShellViewModel DialogShellViewModel { get; }
        PopupDialogShellViewModel PopupDialogShellViewModel { get; }
    }
}