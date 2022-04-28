using Ironwall.MapEditor.UI.ViewModels.RegisteredItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.Models.Messages.Process
{
    public class SymbolTreeAddMessageModel
    {
        public SymbolTreeAddMessageModel(TreeContentControlViewModel vm)
        {
            ViewModel = vm;
        }
        public TreeContentControlViewModel ViewModel { get; }
    }

    public class SymbolTreeRemoveMessageModel
    {
        public SymbolTreeRemoveMessageModel(TreeContentControlViewModel vm)
        {
            ViewModel = vm;
        }

        public TreeContentControlViewModel ViewModel { get; }
    }
}
