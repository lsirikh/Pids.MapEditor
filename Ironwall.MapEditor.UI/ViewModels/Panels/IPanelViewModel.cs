namespace Ironwall.MapEditor.UI.ViewModels.Panels
{
    internal interface IPanelViewModel
    {
        public void ModelUpdate();
        public void ClickOkAsync();
        public void ClickCancelAsync();
    }
}