using Ironwall.MapEditor.UI.ViewModels.ContentControls;
using Ironwall.MapEditor.UI.ViewModels.RegisteredItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.Models.Messages.Process
{
    
    public class ControllerTreeAddMessageModel
    {
        public ControllerTreeAddMessageModel(TreeContentControlViewModel vm)
        {
            ViewModel = vm;
        }
        public TreeContentControlViewModel ViewModel { get; }
    }

    public class ControllerTreeRemoveMessageModel
    {
        public ControllerTreeRemoveMessageModel(TreeContentControlViewModel vm)
        {
            ViewModel = vm;
        }

        public TreeContentControlViewModel ViewModel { get; }
    }
}
