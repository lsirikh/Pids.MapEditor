using Caliburn.Micro;
using Ironwall.Framework.Services;
using Ironwall.MapEditor.UI.DataProviders;
using Ironwall.MapEditor.UI.ViewModels.ContentControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Ironwall.MapEditor.UI.ViewModels.DataGridItems
{
    //DataGridContentControlView
    public class DataGridMapViewModel
        : Screen
    {
        #region - Ctors -
        /// <summary>
        /// 독립적인 DataGrid Panel
        /// </summary>
        public DataGridMapViewModel(MapProvider provider)
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
        public void ClickOpenFile(object sender, RoutedEventArgs e)
        {
            if (!((sender as Button).DataContext is MapContentControlViewModel vm))
                return;

            vm.ClickOpenFile();
        }
        #endregion
        #region - Processes -
        public async Task GetList(CancellationToken cancellationToken = default)
        {
            ///초기화된 CollectionEntity를 설정한다.
            CollectionEntity = new ObservableCollection<MapContentControlViewModel>();
            
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

            # region Test Code
            /*await Task.Run(() =>
            {
                var rand = (new Random()).Next(20, 50);

                for (int i = 0; i < rand; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;
                    DispatcherService.Invoke((System.Action)(() =>
                    {
                        ///UI 쓰레드에 의해 점유중인 자원에 대해선 자원에 
                        ///수정을 가할 때 UI 쓰레드의 Dispatcher에 작업을 위임해야 한다. 
                        CollectionEntity.Add(new MapContentControlViewModel(i, $"맵{i}", i, $"맵{i}.png", "png", $"Maps/맵{i}.png", 1920.0, 1080.0, true, true));
                    }));
                    //CollectionEntity.Add(new MapContentControlViewModel(i, $"맵{i}", i, $"맵{i}.png", "png", "Maps/맵{i}.png", 1920.0, 1080.0, true, true));

                    Task.Delay(50, cancellationToken);
                }
            }, cancellationToken);*/
            #endregion
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public ObservableCollection<MapContentControlViewModel> CollectionEntity
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
        private ObservableCollection<MapContentControlViewModel> _collectionEntity;
        private MapProvider _provider;
        #endregion
    }
}
