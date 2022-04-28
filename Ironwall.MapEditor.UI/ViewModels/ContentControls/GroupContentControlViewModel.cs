using Caliburn.Micro;
using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models;
using Ironwall.MapEditor.UI.DataProviders;
using Ironwall.MapEditor.UI.Models;
using Ironwall.MapEditor.UI.Models.Messages.Process;
using Ironwall.MapEditor.UI.ModelValidators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.ViewModels.ContentControls
{
    public class GroupContentControlViewModel
        : SymbolContentControlViewModel
    {
        #region - Ctors -
        public GroupContentControlViewModel(int id, IEventAggregator eventAggregator = null, EntityCollectionProvider<SymbolContentControlViewModel> provider = null) : base(id, eventAggregator, provider)
        {
        }

        public GroupContentControlViewModel(IEntityModel model, IEventAggregator eventAggregator, EntityCollectionProvider<SymbolContentControlViewModel> provider = null, MapProvider mapProvider = null) : base(model, eventAggregator, provider)
        {
            MapProvider = mapProvider;
        }

        public GroupContentControlViewModel(int id, string nameArea, int typeDevice, string nameDevice, int idController, int idSensor, int typeShape, double x1, double y1, double x2, double y2, double width, double height, double angle, int map, bool used, bool visibility, IEventAggregator eventAggregator = null, EntityCollectionProvider<SymbolContentControlViewModel> provider = null) : base(id, nameArea, typeDevice, nameDevice, idController, idSensor, typeShape, x1, y1, x2, y2, width, height, angle, map, used, visibility, eventAggregator, provider)
        {
        }
        public GroupContentControlViewModel(int id, string nameArea, int typeDevice, string nameDevice, int idController, int idSensor, int typeShape, double x1, double y1, double x2, double y2, double width, double height, double angle, int map, bool used, bool visibility, IEventAggregator eventAggregator = null, EntityCollectionProvider<SymbolContentControlViewModel> provider = null, MapProvider mapProvider = null) : base(id, nameArea, typeDevice, nameDevice, idController, idSensor, typeShape, x1, y1, x2, y2, width, height, angle, map, used, visibility, eventAggregator, provider)
        {
            MapProvider = mapProvider;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public MapProvider MapProvider { get; }

        public MapContentControlViewModel SelectedMap
        {
            //get { return _selectedMap; }
            get { return MapProvider?.Where(t => t.MapNumber == Map).FirstOrDefault(); }
            set
            {
                if (value == null)
                {
                    Map = 0;
                    NotifyOfPropertyChange(() => SelectedMap);
                    return;
                }
                Map = value.MapNumber;
                NotifyOfPropertyChange(() => SelectedMap);
            }
        }
        #endregion
        #region - Attributes -
        #endregion

    }
}
