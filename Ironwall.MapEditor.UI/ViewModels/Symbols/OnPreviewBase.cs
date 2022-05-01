using Caliburn.Micro;
using Ironwall.Framework.ViewModels;
using Ironwall.MapEditor.UI.ViewModels.ContentControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.ViewModels.Symbols
{
    public abstract class OnPreviewBase
    : SymbolBase
    {
        protected OnPreviewBase(SymbolContentControlViewModel symbolContentControlViewModel, IEventAggregator eventAggregator) : base(symbolContentControlViewModel, eventAggregator)
        {
        }
    }
}
