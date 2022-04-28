using Caliburn.Micro;
using Ironwall.Enums;
using Ironwall.Framework.Services;
using Ironwall.MapEditor.Ui.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.ViewModels.RegisteredItems
{
    public abstract class TreeBaseViewModel<T> 
        : Screen
    {
        #region - Ctors -
        public TreeBaseViewModel()
        {
            Items = new TrulyObservableCollection<TreeContentControlViewModel>();
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            base.OnActivateAsync(cancellationToken);
            Debug.WriteLine($"######### {DisplayName}({this.GetType().Name.ToString()}) OnActivate!! #########");
            InitialTree();
            return InitialProcess();
        }
        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            base.OnDeactivateAsync(close, cancellationToken);
            Debug.WriteLine($"######### {DisplayName}({this.GetType().Name.ToString()}) OnDeactivate!! #########");
            return ClosingProcess();
        }

        /// <summary>
        /// Tree 구조 초기화
        /// </summary>
        protected virtual void InitialTree() 
        {
            ///Root 생성
            SetRootInitialize();

            ///Provider 데이터 트리 구성
            SetTreeWithProvider();
        }

        protected virtual void SetTreeWithProvider(){}

        protected virtual void SetRootInitialize()
        {
            //Tree 초기화
            ClearTree();
        }

        protected virtual void UpdateTree(T viewModel) {}

        /// <summary>
        /// Tree에 Node를 추가하는 과정
        /// </summary>
        /// <param name="parentNode">Node의 부모</param>
        /// <param name="node">추가할 Node</param>
        /// <returns>boolean type</returns>
        protected virtual async void AddTree(TreeContentControlViewModel node)
        {
            try
            {
                var parentNode = node.ParentTree as TreeContentControlViewModel;
                if (parentNode == null)
                    Items.Add(node);
                else
                    parentNode.Children.Add(node);

                ///Tree 추가 node 활성화
                if (!node.IsActive)
                    await node.ActivateAsync();

                ///트리 갯수 갱신
                TreeCount = TreeManager.GetTreeCount(Items);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in AddTree : {ex.Message}");
            }
        }

        /// <summary>
        /// Tree에서 Node를 삭제하는 과정
        /// </summary>
        /// <param name="node">삭제할 Node</param>
        /// <returns>boolean type</returns>
        protected virtual async void RemoveTree(TreeContentControlViewModel node)
        {
            try
            {
                ///node의 Children Tree 모두삭제
                TreeManager.SetTreeClear(node.Children);

                var parentNode = node.ParentTree as TreeContentControlViewModel;
                if (parentNode == null)
                    return;

                ///본인 노드 삭제
                parentNode.Children.Remove(node);
                await node.DeactivateAsync(true);

                ///트리 갯수 갱신
                TreeCount = TreeManager.GetTreeCount(Items);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in RemoveTree : {ex.Message}");
            }
        }

        protected virtual void ClearTree()
        {
            try
            {
                SelectedItem = null;
                TreeManager.SetTreeClear(Items);
                Items.Clear();
                TreeCount = TreeManager.GetTreeCount(Items);
                NotifyOfPropertyChange(() => Items);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in ClearTree : {ex.Message}");
            }
        }

        protected virtual void UpdateSelectedItem(){}
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        /// <summary>
        /// 시작 시, 실행 작업
        /// </summary>
        /// <returns></returns>
        protected virtual Task InitialProcess()
        {
            ///_evnetAggregator 수신
            _eventAggregator?.SubscribeOnPublishedThread(this);
            ///CancellationTokenSource 생성
            _cancellationTokenSource = new CancellationTokenSource();

            return Task.CompletedTask;
        }
        /// <summary>
        /// 종료 시, 실행 작업
        /// </summary>
        /// <returns></returns>
        protected virtual Task ClosingProcess()
        {
            ///_eventAggregator를 수신 취소
            _eventAggregator?.Unsubscribe(this);
            ///Nullify _eventAggregator
            _eventAggregator = null;
            ///CancellationTokenSource Cancel
            _cancellationTokenSource.Cancel();

            return Task.CompletedTask;
        }

        public string SetDataType(int typeDevice)
        {
            switch (typeDevice)
            {
                case (int)EnumDeviceType.Controller:
                    return EnumDeviceType.Controller.ToString();
                case (int)EnumDeviceType.Fence:
                case (int)EnumDeviceType.Multi:
                case (int)EnumDeviceType.PIR:
                case (int)EnumDeviceType.IoController:
                case (int)EnumDeviceType.Contact:
                case (int)EnumDeviceType.Cable:
                case (int)EnumDeviceType.Underground:
                case (int)EnumDeviceType.Laser:
                    return "Sensor";
                case 100:
                    return "Group";
                default:
                    return null;
            }
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public TrulyObservableCollection<TreeContentControlViewModel> Items { get; set; }

        public int TreeCount
        {
            get { return _treeCount; }
            set
            {
                _treeCount = value;
                NotifyOfPropertyChange(() => TreeCount);
            }
        }

        public TreeContentControlViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                NotifyOfPropertyChange(() => SelectedItem);

                if (_selectedItem != null)
                {
                    UpdateSelectedItem();
                }
            }
        }
        #endregion
        #region - Attributes -
        private int _treeCount;
        private TreeContentControlViewModel _selectedItem;
        protected IEventAggregator _eventAggregator;
        protected CancellationTokenSource _cancellationTokenSource;
        #endregion
    }
}
