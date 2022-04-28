using Caliburn.Micro;
using Ironwall.Framework.Services;
using Ironwall.MapEditor.UI.DataProviders;
using Ironwall.MapEditor.UI.ViewModels.ContentControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.ViewModels.DataGridItems
{
    public class DataGridSensorViewModel
        : Screen
    {
        #region - Ctors -
        /// <summary>
        /// 독립적인 DataGrid Panel
        /// </summary>
        public DataGridSensorViewModel(SensorProvider provider)
        {
            _provider = provider;
        }

        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await base.OnActivateAsync(cancellationToken);
            await GetList(cancellationToken);
        }
        #endregion
        #region - Binding Methods -
        public bool CanButtonAdd() => false;
        public async void ButtonAdd()
        {
            await GetList();
        }

        public bool CanButtonRemove() => false;
        public void ButtonRemove()
        {
        }

        #endregion
        #region - Processes -
        public async Task GetList(CancellationToken cancellationToken = default)
        {
            CollectionEntity = new ObservableCollection<SymbolContentControlViewModel>();
            ///화면상의 지연을 감소하기 위한 지연 테스크
            await Task.Delay(1000).ContinueWith((_, t) =>
            {
                ///리스트 상에서 속성 변경하는 내용 Broadcasting하기위한 설정
                foreach (var item in _provider.CollectionEntity)
                {
                    item.broadCastring = true;
                }

                ///_provider의 CollectionEntity를 DataGridMapViewModel의 
                ///속성으로 연결해주는 작업....
                CollectionEntity = _provider.CollectionEntity;
            }, cancellationToken);
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public ObservableCollection<SymbolContentControlViewModel> CollectionEntity
        {
            get => _collectionEntity;
            set
            {
                _collectionEntity = value;
                NotifyOfPropertyChange(() => CollectionEntity);
            }
        }
        #endregion
        #region - Attributes -
        private ObservableCollection<SymbolContentControlViewModel> _collectionEntity;
        private SensorProvider _provider;
        #endregion
    }
}
