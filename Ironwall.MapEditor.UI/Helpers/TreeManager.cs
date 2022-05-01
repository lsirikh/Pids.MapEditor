using Caliburn.Micro;
using Ironwall.Framework.Services;
using Ironwall.MapEditor.UI.DataProviders;
using Ironwall.MapEditor.UI.ViewModels.RegisteredItems;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.Ui.Helpers
{
    public static class TreeManager
    {
        /// <summary>
        /// GetTreeCount - 트리 노드의 갯수를 구하기 위한 메소드
        /// </summary>
        /// <param name="collection">검색할 트리 범위, ObservableCollection형태의 TreeItemViewModel타입 데이터를 인자로 받음</param>
        /// <returns></returns>
        public static int GetTreeCount(ObservableCollection<TreeContentControlViewModel> collection)
        {
            /// Get The number of node with the recursive travers of tree structure
            /// node0
            ///   ㄴnode1
            ///     ㄴnode4
            ///        ㄴnode7
            ///   ㄴnode2
            ///     ㄴnode5
            ///   ㄴnode3
            ///     ㄴnode6
            /// 
           
            ///initialize Count
            int count = 0;

            TreeRecursizeCounter(collection);
            return count;

            void TreeRecursizeCounter(ObservableCollection<TreeContentControlViewModel> collection)
            {
                if (!(collection != null && collection.Count() > 0))
                {
                    //Debug.WriteLine($"자손없음");
                    return;
                }

                foreach (var item in collection)
                {
                    count++;
                    //Debug.WriteLine($"{item.DisplayName} 도착!! 총 노드 : {count}개");
                    if (item.Children.Count() > 0)
                    {
                        //Debug.WriteLine($"하위 레벨 진입");
                        TreeRecursizeCounter(item.Children);
                        //Debug.WriteLine($"상위 레벨 진입");
                    }
                }
            }
        }

        /// <summary>
        /// 트리의 노드 중 가장 큰 값을 갖는 ID를 식별하는 메소드
        /// </summary>
        /// <param name="collection">검색할 트리 범위, ObservableCollection형태의 TreeItemViewModel타입 데이터를 인자로 받음</param>
        /// <returns>int 타입, 가장 큰 아이디를 반환</returns>
        public static int GetTreeMaxNumber(ObservableCollection<TreeContentControlViewModel> collection)
        {
            ///최대값을 전달할 
            int max = 0;

            TreeRecursiveMaxNumber(collection);
            return max;

            void TreeRecursiveMaxNumber(ObservableCollection<TreeContentControlViewModel> collection)
            {
                ///입력 파라미터 collection 검증
                if (!(collection != null && collection.Count() > 0))
                {
                    return;
                }

                for (int i = 0; i < collection.Count(); i++)
                {
                    if (collection[i].Children.Count() > 0)
                    {
                        TreeRecursiveMaxNumber(collection[i].Children);
                    }
                    var id = int.Parse(collection[i].Id.Substring(1));
                    if (id > max)
                        max = id;
                }

            }
        }

        /// <summary>
        /// GetMatchedTree - 트리에서 일치하는 노드 찾아 주는 메소드
        /// </summary>
        /// <param name="collection">검색할 트리 범위, ObservableCollection형태의 TreeItemViewModel타입 데이터를 인자로 받음</param>
        /// <param name="node">찾아볼 트리 노드</param>
        /// <returns>boolean 타입 데이터</returns>
        public static bool GetMatchedTree(ObservableCollection<TreeContentControlViewModel> collection, TreeContentControlViewModel node)
        {
            var _isMatched = false;

            TreeRecursiveSearch(collection, node);
            
            return _isMatched;

            void TreeRecursiveSearch(ObservableCollection<TreeContentControlViewModel> collection, TreeContentControlViewModel node)
            {
                //입력 파라미터 collection 검증
                if (!(collection != null && collection.Count() > 0))
                    return;

                foreach (var item in collection)
                {
                    //일치하는 아이템을 찾으면 더이상 순환을 하지 않음
                    if (_isMatched)
                        return;

                    //Debug.WriteLine($"{item.DisplayName} 도착!!");

                    //일치하는 아이디를 찾으면 _isMatched를 true 전환
                    if (item == node)
                    {
                        _isMatched = true;
                        return;
                    }
                    else if (item.Children.Count() > 0)
                    {
                        TreeRecursiveSearch(item.Children, node);
                    }
                }
            }
        }

        /// <summary>
        /// GetMatchedId - collection 범위의 트리에서 parameter로 받은 Id가 일치하는 TreeItemViewModel을 반환하는 헬퍼 메소드
        /// </summary>
        /// <param name="collection">검색할 트리 범위, ObservableCollection형태의 TreeItemViewModel타입 데이터를 인자로 받음</param>
        /// <param name="id">찾을 Id 정보</param>
        /// <returns>TreeItemViewModel 인스턴스 반환</returns>
        public static TreeContentControlViewModel GetMatchedId(ObservableCollection<TreeContentControlViewModel> collection, string id)
        {
            TreeContentControlViewModel searchTree = null;

            TreeRecursiveSearchId(collection, id);

            return searchTree;

            void TreeRecursiveSearchId(ObservableCollection<TreeContentControlViewModel> collection, string id)
            {
                //입력 파라미터 collection 검증
                if (!(collection != null && collection.Count() > 0))
                {
                    return;
                }

                for (int i = 0; i < collection.Count(); i++)
                {
                    if (collection[i].Children.Count() > 0)
                    {
                        TreeRecursiveSearchId(collection[i].Children, id);
                    }

                    if (collection[i].Id == id)
                        searchTree = collection[i];
                }
            }
        }

        /*public static TreeContentControlViewModel GetMatchedIdWithDataType(ObservableCollection<TreeContentControlViewModel> collection, int id, string dataType)
        {
            TreeContentControlViewModel searchTree = null;

            TreeRecursiveSearchId(collection, id, dataType);

            return searchTree;

            void TreeRecursiveSearchId(ObservableCollection<TreeContentControlViewModel> collection, int id, string dataType)
            {
                //입력 파라미터 collection 검증
                if (!(collection != null && collection.Count() > 0))
                {
                    return;
                }

                for (int i = 0; i < collection.Count(); i++)
                {
                    if (collection[i].Children.Count() > 0)
                    {
                        TreeRecursiveSearchId(collection[i].Children, id, dataType);
                    }
                    ///ID 및 DataType까지 일치해야 원하는 TreeContentControlViewModel임
                    if (collection[i].Id == id && collection[i].DataType == dataType)
                        searchTree = collection[i];
                }
            }
        }*/
        public static TreeContentControlViewModel GetControllerTreeInGroup(ObservableCollection<TreeContentControlViewModel> collection, string id)
        {
            var tree = GetMatchedId(collection, id);

            //return tree?.ParentTree as TreeContentControlViewModel;
            return null;

        }

        public static TreeContentControlViewModel GetRootNode(TreeContentControlViewModel node)
        {
            if (!(node?.ParentTree is TreeContentControlViewModel parentNode)
                || parentNode == null)
                return node;
            
            return GetRootNode(parentNode);
        }


        public static void SetTreeUnselected(TrulyObservableCollection<TreeContentControlViewModel> collection)
        {
            ///입력 파라미터 collection 검증
            if (!(collection != null && collection.Count() > 0))
                return;

            foreach (var item in collection.ToList())
            {
                if (item.Children?.Count() > 0)
                    SetTreeUnselected(item.Children);

                item.IsSelected = false;
            }
        }


        /// <summary>
        /// TreeRecursiveRemover - 트리 구조를 삭제하기 위한 기능을 제공. 동일 레벨의 노드 및 자식 트리의 개별 노드를 삭제, Deactivate 시키는 메소드
        /// </summary>
        /// <param name="collection">검색할 트리 범위, ObservableCollection형태의 TreeItemViewModel타입 데이터를 인자로 받음</param>
        public static async void SetTreeClear(TrulyObservableCollection<TreeContentControlViewModel> collection)
        {
            ///입력 파라미터 collection 검증
            if (!(collection != null && collection.Count() > 0))
                return;

            foreach (var item in collection.ToList())
            {
                if (item.Children?.Count() > 0)
                    SetTreeClear(item.Children);

                if (item.ParentTree is TreeContentControlViewModel parent)
                {
                    await item.DeactivateAsync(true);
                    DispatcherService.Invoke((System.Action)(() =>
                    {
                        ///TreeNode 추가
                        //treeParent.Children.Add(treeNode);
                        parent.Children.Remove(item);
                    }));
                }
                else
                    await item.DeactivateAsync(true);

                GC.Collect();
            }
        }

        public static void SetTreeClearChildrenWithProvidersInGroup(ObservableCollection<TreeContentControlViewModel> collection
            //, MapProvider mapProvider = null
            , ControllerProvider controllerProvider = null
            , SensorProvider sensorProvider = null
            , GroupProvider groupProvider = null
            //, CameraProvider cameraProvider = null
            )
        {
            ///입력 파라미터 collection 검증
            if (!(collection != null && collection.Count() > 0))
                return;

            for (int i = 0; i < collection.Count(); i++)
            {
                var child = collection[i];
                //Debug.WriteLine($"{collection[i].DisplayName} 도착!!");
                if (collection[i].Children.Count() > 0)
                {
                    SetTreeClearChildrenWithProvidersInGroup(collection[i].Children
                        //, mapProvider
                        , controllerProvider
                        , sensorProvider
                        , groupProvider
                        //, cameraProvider
                        );
                }

                if (collection[i].ParentTree is TreeContentControlViewModel parent)
                {
                    switch (collection[i].DataType)
                    {
                        case Enums.EnumDataType.None:
                            break;
                        case Enums.EnumDataType.MapRoot:
                            break;
                        case Enums.EnumDataType.Map:
                            break;
                        case Enums.EnumDataType.DeviceRoot:
                            break;
                        case Enums.EnumDataType.Controller:
                            break;
                        case Enums.EnumDataType.Sensor:
                            var selectedSensors = sensorProvider?
                                .Where(t => collection[i].Id == TreeManager.SetTreeSensorId(t.Id));
                            if (selectedSensors != null)
                                foreach (var item in selectedSensors)
                                    item.NameArea = "Untitle";
                            break;
                        case Enums.EnumDataType.GroupRoot:
                            break;
                        case Enums.EnumDataType.Group:
                            var selectedGroups = groupProvider?
                                .Where(t => collection[i].Id == TreeManager.SetTreeGroupId(t.Id));

                            if (selectedGroups != null)
                                foreach (var item in selectedGroups)
                                    groupProvider?.Remove(item);
                            break;
                        case Enums.EnumDataType.CameraRoot:
                            break;
                        case Enums.EnumDataType.Camera:
                            break;
                        default:
                            break;
                    }
                   
                    ///트리 노드 삭제 및 비활성화
                    collection[i].DeactivateAsync(true);
                    parent.Children.Remove(collection[i]);

                    ///collection 삭제에 따른 순환문 옵션 제어
                    if (parent.Children.Count() > 0)
                        i--;
                    else
                        break;
                }
                else
                {
                    collection[i].DeactivateAsync(true);
                }
            }
        }

        

        /// <summary>
        /// Map Tree에서 활용하는 Id로 변경
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string SetTreeMapId(int id)
        {
            return $"M{id}";
        }
        /// <summary>
        /// MapProvider에서 활용하는 Id로 변경
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int GetMapProviderId(string id)
        {
            return int.Parse(id.Replace("M", ""));
        }
        /// <summary>
        /// Device, Group Tree에서 활용하는 Id로 변경
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string SetTreeControllerId(int id)
        {
            return $"C{id}";
        }
        /// <summary>
        /// ControllerProvider에서 활용하는 Id로 변경
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int GetControllerProviderId(string id)
        {
            return int.Parse(id.Replace("C", ""));
        }
        /// <summary>
        /// Camera Tree에서 활용하는 Id로 변경
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string SetTreeCameraId(int id)
        {
            return $"V{id}";
        }
        /// <summary>
        /// CameraProvider에서 활용하는 Id로 변경
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int GetCameraProviderId(string id)
        {
            return int.Parse(id.Replace("V", ""));
        }
        /// <summary>
        /// Device, Group Tree에서 활용하는 Id로 변경
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string SetTreeSensorId(int id)
        {
            return $"S{id}";
        }
        /// <summary>
        /// SensorProvider에서 활용하는 Id로 변경
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int GetSensorProviderId(string id)
        {
            return int.Parse(id.Replace("S", ""));
        }
        /// <summary>
        /// Group Tree에서 활용하는 Id로 변경
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string SetTreeGroupId(int id)
        {
            return $"G{id}";
        }
        public static string SetTreeGroupId(string id)
        {
            return $"G{id}";
        }
        /// <summary>
        /// GroupProvider에서 활용하는 Id로 변경
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int GetGroupProviderId(string id)
        {
            return int.Parse(id.Replace("G", ""));
        }
    }
}
