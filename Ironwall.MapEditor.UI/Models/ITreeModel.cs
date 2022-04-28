using Ironwall.Enums;
using System.Collections.Generic;

namespace Ironwall.MapEditor.UI.Models
{
    public interface ITreeModel
    {
        /// <summary>
        /// Id - 해당 트리의 특정 번호
        /// C1 제어기 + 제어기 ID번호
        /// S1 센서 + 센서 ID 번호
        /// G1 그룹 + 그룹 ID 번호
        /// (단, 중복일 수 없음)
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Name - 해당 트리의 이름
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Description - 해당 트리의 내역
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Type - 해당 트리의 타입
        /// ROOT : 부모가 없고, 자식만 존재 최초 노드
        /// BRANCH : 부모와 자식이 둘다 존재 중간 노드
        /// LEAF : 자식은 없고 부모만 존재 말단 노드
        /// </summary>
        public EnumTreeType Type { get; set; }
        /// <summary>
        /// Used - IXXModel 계열의 Used와 연동
        /// </summary>
        public bool Used { get; set; }
        /// <summary>
        /// Visibility - IXXModel계열의 Visibility와 연동
        /// </summary>
        public bool Visibility { get; set; }
        /// <summary>
        /// ParentTree - 트리의 상위(부모) 노드 객체
        /// (단, ParentTree가 null 부모노드 없음)
        /// </summary>
        public object ParentTree { get; set; }
        /// <summary>
        /// DataType - 해당 데이터 타입을 활용하여,
        /// 호출 클래스를 구분할 수 있음.
        /// </summary>
        public EnumDataType DataType { get; set; }

    }
}