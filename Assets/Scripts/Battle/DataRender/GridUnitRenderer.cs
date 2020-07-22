using TMPro;
using UnityEngine;

public class GridUnitRenderer : BaseBehaviour, IVisualRenderer<GridUnit, GridUnitRenderer>
{
    //瓦片的精灵渲染器
    [SerializeField] private SpriteRenderer tileRenderer = null;
    //显示格子的名字
    [SerializeField] private TextMeshPro gridInfo = null;
    //特效节点
    [SerializeField] private Transform effectNode = null;

    public GridUnit GridUnit;

    public override void Init(params object[] args)
    {
        tileRenderer.sortingLayerID = GameConst.SortingLayer_Battle_Map;
        gridInfo.sortingLayerID = GameConst.SortingLayer_Battle_Map;
    }

    private void UpdateLocalPosition()
    {
        if (GridUnit != null)
        {
            transform.localPosition = GridUnit.LocalPosition;
            tileRenderer.sortingOrder = GridUnit.GridPosition.x * GameConst.OrderGapPerRow;
            gridInfo.sortingOrder = GridUnit.GridPosition.x * GameConst.OrderGapPerRow;
        }
    }

    public override bool Equals(object other)
    {
        if (other is GridUnitRenderer)
        {
            return GetInstanceID() == ((GridUnitRenderer) other).GetInstanceID();
        }

        return false;
    }

    public void OnConnect(GridUnit data)
    {
        GridUnit = data;
        if (GridUnit != null)
        {
            transform.name = GridUnit.ToString();
            UpdateLocalPosition();
            gridInfo.text = $"{GridUnit.GridPosition}\n{GridUnit.GetCubePosFromPos(GridUnit.GridPosition)}";
            gameObject.SetActive(true);
        }
    }

    public void OnDisconnect()
    {
        gridInfo = null;
        transform.SetUnused(false, GameConst.STR_GRID);
    }
}