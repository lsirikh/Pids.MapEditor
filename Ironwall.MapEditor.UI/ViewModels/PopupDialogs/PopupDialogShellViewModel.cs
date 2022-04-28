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
    internal class PopupDialogShellViewModel
        : ConductorOneViewModel

    {
        #region - Ctors -
        public PopupDialogShellViewModel(
            IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            #region - Settings -
            Id = 3;
            Content = "";
            Category = CategoryEnum.POPUP_DIALOG_SHELL_VM;
            #endregion - Settings -

            #region - Active Items -
            #endregion - Active Items -
        }
        #endregion

        #region - Handles -
        #endregion

        #region - Properties -
        #endregion
    }
}
