using Caliburn.Micro;
using Ironwall.MapEditor.UI.Models.Messages.Process;
using Ironwall.MapEditor.UI.ViewModels.ContentControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.ViewModels.Symbols
{
    public class SymbolSensorViewModel
        : SymbolBase
        , IHandle<SymbolContentUpdateMessageModel>
    {
        public SymbolSensorViewModel(SymbolContentControlViewModel symbolContentControlViewModel, IEventAggregator eventAggregator) : base(symbolContentControlViewModel, eventAggregator)
        {
        }

        public Task HandleAsync(SymbolContentUpdateMessageModel message, CancellationToken cancellationToken)
        {
            Update();
            return Task.CompletedTask;
        }
    }
}
