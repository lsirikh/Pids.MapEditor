using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.Models.Messages.Sections
{
    public abstract class OpenPropertyMessageModel
    {
        public OpenPropertyMessageModel(Screen vm)
        {
            ViewModel = vm;
        }

        public Screen ViewModel { get; }
    }

    public class OpenMapPropertyMessageModel
         : OpenPropertyMessageModel
    {
        public OpenMapPropertyMessageModel(Screen vm) : base(vm)
        {
        }
    }

    public class OpenControllerPropertyMessageModel
         : OpenPropertyMessageModel
    {
        public OpenControllerPropertyMessageModel(Screen vm) : base(vm)
        {
        }
    }

    public class OpenSensorPropertyMessageModel
         : OpenPropertyMessageModel
    {
        public OpenSensorPropertyMessageModel(Screen vm) : base(vm)
        {
        }
    }

    public class OpenGroupPropertyMessageModel
         : OpenPropertyMessageModel
    {
        public OpenGroupPropertyMessageModel(Screen vm) : base(vm)
        {
        }
    }

    public class OpenGroupSymbolPropertyMessageModel
        : OpenPropertyMessageModel
    {
        public OpenGroupSymbolPropertyMessageModel(Screen vm) : base(vm)
        {
        }
    }

    public class OpenCameraPropertyMessageModel
        : OpenPropertyMessageModel
    {
        public OpenCameraPropertyMessageModel(Screen vm) : base(vm)
        {
        }
    }



    
}
