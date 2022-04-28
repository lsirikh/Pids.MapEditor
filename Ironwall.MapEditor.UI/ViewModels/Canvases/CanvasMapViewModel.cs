using Caliburn.Micro;
using Ironwall.Framework.ViewModels;
using Ironwall.MapEditor.UI.DataProviders;
using Ironwall.MapEditor.UI.Models.Messages.Sections;
using Ironwall.MapEditor.UI.ViewModels.ContentControls;
using Ironwall.MapEditor.UI.ViewModels.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.ViewModels.Canvases
{
    internal class CanvasMapViewModel
        : CanvasEntityViewModel<MapContentControlViewModel>
        
    {
        #region - Ctors -
        public CanvasMapViewModel(MapContentControlViewModel vm,
            IEventAggregator eventAggregator = null)
            : base(vm)
        {
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
        private IEventAggregator _eventAggregator;
        #endregion
    }
}
