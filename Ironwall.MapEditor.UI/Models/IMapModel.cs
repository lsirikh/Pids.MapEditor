namespace Ironwall.MapEditor.UI.Models
{
    public interface IMapModel
    {
        #region - Properties -
        public int Id { get; set; }
        public string MapName { get; set; }
        public int MapNumber { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string Url { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public bool Used { get; set; }
        public bool Visibility { get; set; }
        #endregion
    }
}