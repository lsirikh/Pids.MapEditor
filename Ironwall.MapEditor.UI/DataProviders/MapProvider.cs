using Ironwall.Framework.DataProviders;
using Ironwall.MapEditor.UI.ViewModels.ComboBoxSource;
using Ironwall.MapEditor.UI.ViewModels.ContentControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.DataProviders
{
    public sealed class MapProvider
        : EntityCollectionProvider<MapContentControlViewModel>
    {
        #region - Ctors -
        public MapProvider()
        {
        }

        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public int GetMaxId()
        {
            var count = 0;
            CollectionEntity.Select(t => t).ToList().ForEach(t =>
            {
                if (t.Id > count)
                    count = t.Id;
            });
            return count;
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        #endregion

    }
}
