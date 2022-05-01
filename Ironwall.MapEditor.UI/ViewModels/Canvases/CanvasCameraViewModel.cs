using Ironwall.Framework.ViewModels;
using Ironwall.MapEditor.UI.DataProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.ViewModels.Canvases
{
    internal class CanvasCameraViewModel
        : CanvasEntityViewModel<SymbolCameraProvider>
    {
        #region -Ctors-
        public CanvasCameraViewModel(
            SymbolCameraProvider entityProvider
            , bool visibility = true) 
            : base(entityProvider, visibility)
        {
        }
        #endregion
    }
}
