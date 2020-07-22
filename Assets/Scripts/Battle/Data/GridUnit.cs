using System;
using UnityEngine;

public enum GridType
{
    None,
    Obstacle,
    Normal,
    Born,
}

public class GridUnit: IVisualData<GridUnit, GridUnitRenderer>
{
    public int MapId;
    //格子类型
    private GridType gridType;
    //当前停留的战斗单位
    public BattleUnit BattleUnit;
    //格子的可通行信息
    public int passes;
    //格子的行列
    public Vector2Int GridPosition;
    //格子在空间中的位置
    public Vector3 LocalPosition;
    //指向obj类型的临时指针
    public System.Object tempRef;
    //格子的渲染器
    public GridUnitRenderer GridUnitRenderer;
    
    public GridUnit(int mapId, int row, int column)
    {
        GridType = GridType.None;
        this.MapId = mapId;
        this.GridPosition = new Vector2Int(row, column);
    }

    public GridType GridType
    {
        get { return gridType; }
        set
        {
            gridType = value;
            switch (value)
            {
                case GridType.None:
                    passes = 0;
                    break;
                case GridType.Normal:
                case GridType.Born:
                    passes = 63;
                    break;
                default:
                    passes = 63;
                    break;
            }
        }
    }

    //计算两个格子的距离
    public int Distance(GridUnit target)
    {
        Vector3Int thisVector3Int = GetCubePosFromPos(GridPosition);
        Vector3Int targetVector3Int = GetCubePosFromPos(target.GridPosition);
        int distance = (Mathf.Abs(thisVector3Int.x - targetVector3Int.x) +
                        Mathf.Abs(thisVector3Int.y - targetVector3Int.y) +
                        Mathf.Abs(thisVector3Int.z - targetVector3Int.z)) / 2;
        //Debug.Log($"({target.GridPosition.x},{target.GridPosition.y}->{GridPosition.x},{GridPosition.y})=>{distance}");
        return distance;
    }

    public Vector3Int GetCubePosFromPos(Vector2Int pos)
    {
        int x = pos.x;
        int y = pos.y - pos.x / 2;
        return new Vector3Int(x, y, -x - y);
    }

    public override bool Equals(object obj)
    {
        if (obj is GridUnit)
        {
            GridUnit data = (GridUnit) obj;
            return data.MapId == MapId &&
                   data.GridPosition.x == GridPosition.x &&
                   data.GridPosition.y == GridPosition.y;
        }

        return false;
    }

    public override string ToString()
    {
        return $"{GridPosition}";
    }


    public void ConnectRenderer(GridUnitRenderer renderer)
    {
        if (renderer == null)
        {
            UtilityHelper.LogError("GridUnit连接渲染器失败");
            return;
        }

        if (GridUnitRenderer != null)
            DisconnectRenderer();

        GridUnitRenderer = renderer;
        GridUnitRenderer.OnConnect(this);
    }

    public void DisconnectRenderer()
    {
        if (GridUnitRenderer != null)
        {
            GridUnitRenderer.OnDisconnect();
            GridUnitRenderer = null;
        }
    }

    public void OnEnter(BattleUnit battleUnit)
    {
        this.BattleUnit = battleUnit;
        
    }

    public void OnLeave()
    {
        this.BattleUnit = null;
    }
}
