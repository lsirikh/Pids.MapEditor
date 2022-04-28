using Caliburn.Micro;
using Ironwall.Enums;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.ViewModels.Panels
{
    internal class PanelShellViewModel
       : ConductorOneViewModel

    {
        public PanelShellViewModel(
            IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            #region - Settings -
            Id = 1;
            Name = this.GetType().Name.ToString();
            Content = "";
            Category = CategoryEnum.PANEL_SHELL_VM;
            #endregion - Settings -

            #region - Active Items -
            #endregion - Active Items -
        }

        #region - Overrides -
        #endregion

        #region - IHandles -
        #endregion

        #region - Properties -
        #endregion
    }
}
