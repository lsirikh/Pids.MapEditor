using Ironwall.MapEditor.UI.ViewModels.RegisteredItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.Models.Messages.Process
{
    public class SensorTreeAddMessageModel
    {
        public SensorTreeAddMessageModel(TreeContentControlViewModel vm)
        {
            ViewModel = vm;
        }
        public TreeContentControlViewModel ViewModel { get; }
    }

    public class SensorTreeRemoveMessageModel
    {
        public SensorTreeRemoveMessageModel(TreeContentControlViewModel vm)
        {
            ViewModel = vm;
        }

        public TreeContentControlViewModel ViewModel { get; }
    }


}
