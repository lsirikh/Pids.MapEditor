using Ironwall.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.Models
{
    public sealed class SymbolModel
        : IEntityModel
    {
        #region - Implementations for IEntityModel -
        public int Id { get; set; }
        public string NameArea { get; set; }
        public int TypeDevice { get; set; }
        public string NameDevice { get; set; }
        public int IdController { get; set; }
        public int IdSensor { get; set; }
        public int TypeShape { get; set; }
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Angle { get; set; }
        public int Map { get; set; }
        public bool Used { get; set; }
        public bool Visibility { get; set; }
        #endregion
    }
}
