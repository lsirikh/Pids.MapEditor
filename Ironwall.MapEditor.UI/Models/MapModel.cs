using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.Models
{
    public sealed class MapModel
        : IMapModel
    {
        #region - Implementations for IMapModel -
        public int Id { get; set; }             //Not Counted
        public string MapName { get; set; }     //1
        public int MapNumber { get; set; }      //2
        public string FileName { get; set; }    //3
        public string FileType { get; set; }    //4
        public string Url { get; set; }         //5
        public double Width { get; set; }       //6
        public double Height { get; set; }      //7
        public bool Used { get; set; }          //8
        public bool Visibility { get; set; }    //9
        #endregion
    }
}
