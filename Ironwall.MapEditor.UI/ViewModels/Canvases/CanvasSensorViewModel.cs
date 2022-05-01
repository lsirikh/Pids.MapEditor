using Ironwall.Framework.ViewModels;
using Ironwall.MapEditor.UI.DataProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.ViewModels.Canvases
{
    internal class CanvasSensorViewModel
        : CanvasEntityViewModel<SymbolSensorProvider>
    {
        public CanvasSensorViewModel(SymbolSensorProvider entityProvider, bool visibility = true) : base(entityProvider, visibility)
        {
        }
    }
}
