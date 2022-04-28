using Ironwall.MapEditor.UI.ViewModels.ContentControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.Models.Messages.Process
{
    public class ControllerContentUpdateMessageModel
    {
        public ControllerContentUpdateMessageModel(ControllerContentControlViewModel vm = null)
        {
            ViewModel = vm;
        }

        public ControllerContentControlViewModel ViewModel { get; }
    }

    public class ControllerContentRemoveMessageModel
    {
        public ControllerContentRemoveMessageModel
            (SymbolContentControlViewModel vm = null)
        {
            ViewModel = vm;
        }

        public SymbolContentControlViewModel ViewModel { get; }
    }

    public class MapContentAddMessageModel
    {
        public MapContentAddMessageModel(MapContentControlViewModel vm = null)
        {
            ViewModel = vm;
        }
        public MapContentControlViewModel ViewModel { get; private set; }
    }

    public class MapTreeRemoveMessageModel
    {
        public MapTreeRemoveMessageModel(MapContentControlViewModel vm)
        {
            ViewModel = vm;
        }

        public MapContentControlViewModel ViewModel { get; private set; }
    }

    public class MapContentUpdateMessageModel
    {
        public MapContentUpdateMessageModel(MapContentControlViewModel vm = null)
        {
            ViewModel = vm;
        }

        public MapContentControlViewModel ViewModel { get; }
    }

    public class SensorContentUpdateMessageModel
    {
        public SensorContentUpdateMessageModel(SymbolContentControlViewModel vm = null)
        {
            ViewModel = vm;
        }

        public SymbolContentControlViewModel ViewModel { get; }
    }

    public class SensorContentRemoveMessageModel
    {
        public SensorContentRemoveMessageModel(SymbolContentControlViewModel vm = null)
        {
            ViewModel = vm;
        }

        public SymbolContentControlViewModel ViewModel { get; }
    }

    public class CameraContentUpdateMessageModel
    {
        public CameraContentUpdateMessageModel(SymbolContentControlViewModel vm = null)
        {
            ViewModel = vm;
        }
        public SymbolContentControlViewModel ViewModel { get; }
    }

    public class GroupContentUpdateMessageModel
    {
        public GroupContentUpdateMessageModel(SymbolContentControlViewModel vm = null)
        {
            ViewModel = vm;
        }
        public SymbolContentControlViewModel ViewModel { get; }
    }

    public class SymbolContentUpdateMessageModel
    {
        private SensorContentControlViewModel contentControlViewModel;

        public SymbolContentUpdateMessageModel(SymbolContentControlViewModel vm = null
            , string property = null)
        {
            ViewModel = vm;
            Property = property;
        }
        public string Property { get; }
        public SymbolContentControlViewModel ViewModel { get; }
    }
}
