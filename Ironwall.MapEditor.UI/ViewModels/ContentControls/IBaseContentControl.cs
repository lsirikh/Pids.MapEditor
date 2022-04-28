namespace Ironwall.MapEditor.UI.ViewModels.ContentControls
{
    public interface IBaseContentControl<T>
    {
        /// <summary>
        /// Insert data as SymbolModel using IEntityModel
        /// </summary>
        /// <param name="model"></param>
        public void Insert(T model);
        /// <summary>
        /// Update properties
        /// </summary>
        void Update();
        /// <summary>
        /// Clear Model
        /// </summary>
        public void Clear();
    }
}