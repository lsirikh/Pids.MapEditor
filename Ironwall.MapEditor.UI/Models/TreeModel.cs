using Ironwall.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.Models
{
    public sealed class TreeModel
        : ITreeModel
    {
        #region - Ctors -
        public TreeModel(){}

        public TreeModel(
            string id, 
            string name, 
            string description, 
            EnumTreeType type,
            bool used, 
            bool visibility, 
            object parentTree,
            EnumDataType dataType)
        {
            Id = id;
            Name = name;
            Description = description;
            Type = type;
            Used = used;
            Visibility = visibility;
            ParentTree = parentTree;
            DataType = dataType;
        }
        #endregion
        #region - Implementation of Interface -
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public EnumTreeType Type { get; set; }
        public bool Used { get; set; }
        public bool Visibility { get; set; }
        public object ParentTree { get; set; }
        public EnumDataType DataType { get; set; }
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
        #endregion
        #region - Attributes -
        #endregion
    }
}
