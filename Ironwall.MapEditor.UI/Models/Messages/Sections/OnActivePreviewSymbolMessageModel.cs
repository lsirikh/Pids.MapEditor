using Ironwall.MapEditor.UI.ViewModels.ContentControls;
using Ironwall.MapEditor.UI.ViewModels.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.Models.Messages.Sections
{
    public class OnActivePreviewSymbolMessageModel
    {
        public OnActivePreviewSymbolMessageModel(SymbolContentControlViewModel vm)
        {
            ViewModel = vm;
        }

        public SymbolContentControlViewModel ViewModel { get; }
    }
}
