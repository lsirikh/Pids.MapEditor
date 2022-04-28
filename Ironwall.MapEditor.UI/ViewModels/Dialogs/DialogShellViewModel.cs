using Caliburn.Micro;
using Ironwall.Enums;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.ViewModels.Dialogs
{
    internal class DialogShellViewModel
        : ConductorOneViewModel

    {
        public DialogShellViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            #region - Settings -
            Id = 2;
            Content = "";
            Category = CategoryEnum.DIALOG_SHELL_VM;
            #endregion - Settings -

            #region - Active Items -
            #endregion - Active Items -
        }

        #region - Overrides -
        #endregion

        #region - Handles -
        #endregion


        #region - Properties -
        #endregion
    }
}
