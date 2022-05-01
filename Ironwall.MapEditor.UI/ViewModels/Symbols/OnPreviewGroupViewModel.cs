using Caliburn.Micro;
using Ironwall.MapEditor.UI.ViewModels.ContentControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.ViewModels.Symbols
{
    public class OnPreviewGroupViewModel
        : OnPreviewBase
    {
        public OnPreviewGroupViewModel(SymbolContentControlViewModel symbolContentControlViewModel, IEventAggregator eventAggregator) : base(symbolContentControlViewModel, eventAggregator)
        {
        }
    }
}
