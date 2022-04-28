using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.Models.Messages.Sections
{
    public class OpenMapPropertyMessageModel
    {
        public OpenMapPropertyMessageModel(Screen vm)
        {
            ViewModel = vm;
        }

        public Screen ViewModel { get; }
    }

    public class OpenControllerPropertyMessageModel
    {
        public OpenControllerPropertyMessageModel(Screen vm)
        {
            ViewModel = vm;
        }

        public Screen ViewModel { get; }
    }

    public class OpenSensorPropertyMessageModel
    {
        public OpenSensorPropertyMessageModel(Screen vm)
        {
            ViewModel = vm;
        }

        public Screen ViewModel { get; }
    }

    public class OpenGroupPropertyMessageModel
    {
        public OpenGroupPropertyMessageModel(Screen vm)
        {
            ViewModel = vm;
        }

        public Screen ViewModel { get; }
    }

    public class OpenCameraPropertyMessageModel
    {
        public OpenCameraPropertyMessageModel(Screen vm)
        {
            ViewModel = vm;
        }

        public Screen ViewModel { get; }
    }
}
