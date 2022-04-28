using Caliburn.Micro;
using Ironwall.Framework.ViewModels;
using Ironwall.MapEditor.UI.Models.Messages.Sections;
using Ironwall.MapEditor.UI.ViewModels.Conductors;
using Ironwall.MapEditor.UI.ViewModels.Sections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ironwall.MapEditor.UI.ViewModels
{
    internal sealed class ExShellViewModel
        : ShellViewModel
    {
        #region - Ctors -
        public ExShellViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            DisplayName = nameof(ExShellViewModel);
        }
        #endregion

        #region - Overrides -
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            base.OnActivateAsync(cancellationToken);
            TopMenuSectionViewModel.ActivateAsync();
            RegisteredStateSectionViewModel.ActivateAsync();
            PropertySectionViewModel.ActivateAsync();
            CanvasSectionViewModel.ActivateAsync();
            ConductorControlViewModel.ActivateAsync();
            return Task.CompletedTask;
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            base.OnDeactivateAsync(close, cancellationToken);
            _eventAggregator.Unsubscribe(this);
            return Task.CompletedTask;
        }
        #endregion
        #region - Processes -
        public void PressEscKey(object sender, EventArgs e)
        {
            if (!(e is KeyEventArgs ke))
                return;

            if (ke.Key == Key.Escape)
            {
                _eventAggregator.PublishOnUIThreadAsync(new ClearPreviewSymbolMessageModel());
            }
        }
        #endregion

        #region - Properties - 
        public TopMenuSectionViewModel TopMenuSectionViewModel => IoC.Get<TopMenuSectionViewModel>();
        public RegisteredStateSectionViewModel RegisteredStateSectionViewModel => IoC.Get<RegisteredStateSectionViewModel>();
        public CanvasSectionViewModel CanvasSectionViewModel => IoC.Get<CanvasSectionViewModel>();
        public PropertySectionViewModel PropertySectionViewModel => IoC.Get<PropertySectionViewModel>();
        public ConductorControlViewModel ConductorControlViewModel => IoC.Get<ConductorControlViewModel>();
        #endregion

        #region - Attributes - 
        #endregion
    }
}
