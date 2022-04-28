using Ironwall.MapEditor.UI.Models;
using System.Collections.Generic;

namespace Ironwall.MapEditor.UI.Services
{
    public interface IInsertTreeItemService
    {
        public List<ITreeModel> GetTreeData();
    }
}