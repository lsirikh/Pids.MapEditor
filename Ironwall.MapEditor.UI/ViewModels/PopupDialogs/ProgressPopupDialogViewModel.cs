using Caliburn.Micro;
using Ironwall.Enums;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.ViewModels.PopupDialogs
{
    internal sealed class ProgressPopupDialogViewModel
        : BaseViewModel
    {
        #region - Ctors -
        public ProgressPopupDialogViewModel(IEventAggregator eventAggregator)
        {
            #region - Settings -
            Id = 31;
            Content = "";
            Category = CategoryEnum.POPUP_DIALOG_SHELL_VM_ITEM;
            #endregion - Settings -

            _eventAggregator = eventAggregator;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        #endregion
    }
}
