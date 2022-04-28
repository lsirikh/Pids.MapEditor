using Ironwall.Enums;
using Ironwall.MapEditor.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.Services
{
    public class InsertTreeItemService
        : IInsertTreeItemService
    {

        #region - Ctors -
        public InsertTreeItemService()
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
        public List<ITreeModel> GetTreeData()
        {
            /*_item = new List<ITreeModel>();
            _item.Add(new TreeModel(1, "전체맵", "맵 최상위 레벨", true, true));

            _item[0].SubItems = new List<ITreeModel>();

            //Insert Child Item
            _item[0].SubItems.Add(new TreeModel(11, "맵1", "부산발전소", true, true));
            _item[0].SubItems.Add(new TreeModel(12, "맵2", "군산발전소", true, true));
            _item[0].SubItems.Add(new TreeModel(13, "맵3", "서인천발전소", true, true));
            _item[0].SubItems.Add(new TreeModel(14, "맵4", "영남발전소", true, true));
            _item[0].SubItems.Add(new TreeModel(15, "맵5", "강원발전소", true, true));

            return _item;*/

            var listItem = new List<ITreeModel>();

            //listItem.Add(new TreeModel(1, "부산발전소", "송도바다인근", EnumTreeType.LEAF, true, true, false ,"MAP"));
            //listItem.Add(new TreeModel(2, "군산발전소", "군산 해만금간척지구", EnumTreeType.LEAF, true, true, false, "MAP"));
            //listItem.Add(new TreeModel(3, "서인천발전소", "서인천지방", EnumTreeType.LEAF, true, true, false, "MAP"));
            //listItem.Add(new TreeModel(4, "영남발전소", "영남지방", EnumTreeType.LEAF, true, true, false, "MAP"));
            //listItem.Add(new TreeModel(5, "강원발전소", "강원도 영월지방", EnumTreeType.LEAF, true, true, false, "MAP"));

            return listItem;

        }
        #endregion
        #region - IHanldes - 
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        #endregion

        private List<ITreeModel> _item;
    }
}
