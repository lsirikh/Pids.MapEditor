﻿using Caliburn.Micro;
using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models;
using Ironwall.Framework.ViewModels;
using Ironwall.MapEditor.UI.Models;
using Ironwall.MapEditor.UI.Models.Messages.Process;
using Ironwall.MapEditor.UI.ViewModels.ContentControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Ironwall.MapEditor.UI.ViewModels.Symbols
{
    public class SymbolControllerViewModel
      : SymbolBase
        , IHandle<SymbolContentUpdateMessageModel>
    {
        public SymbolControllerViewModel(SymbolContentControlViewModel symbolContentControlViewModel, IEventAggregator eventAggregator) : base(symbolContentControlViewModel, eventAggregator)
        {
        }
        public Task HandleAsync(SymbolContentUpdateMessageModel message, CancellationToken cancellationToken)
        {
            Update();
            return Task.CompletedTask;
        }
    }
}
