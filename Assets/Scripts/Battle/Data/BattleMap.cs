using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleMap: CountableInstance
{
    //地图Id
    public int MapId;
    //地图宽高
    public int MapWidth = 0;
    public int MapHeight = 0;
    
    //地图格子信息
    public GridUnit[,] MapGrids;
    //出生格子
    private List<GridUnit> bornAGrids = new List<GridUnit>();
    private List<GridUnit> bornBGrids = new List<GridUnit>();
    //普通格子
    private List<GridUnit> normalGrids = new List<GridUnit>();
    
    //格子总数量
    public int GridCount => MapHeight * MapWidth;
    
    //战场中铺设格子（信息,不做显示）
    public void Init(int width, int height)
    {
        if (width <= 0 || height <= 0)
            return;

        MapWidth = width;
        MapHeight = height;
        MapGrids = new GridUnit[MapWidth, MapHeight];

        for (int i = 0; i < MapHeight; i++)
        {
            for (int j = 0; j < MapWidth; j++)
            {
                GridUnit gridUnit = new GridUnit(MapId, i, j);
                gridUnit.LocalPosition = new Vector3(
                    j * GameConst.Map_GridWith + (((i & 1) == 1) ? GameConst.Map_GridWith * 0.5f : 0f),
                    0,
                    -i * GameConst.Map_GridOffsetY);
                gridUnit.GridType = GridType.Normal;
                MapGrids[j, i] = gridUnit;
            }
        }
        
        GenerateBorn();
        TidyGridList();
    }

    private void GenerateBorn()
    {
        for (int i = 1; i < MapWidth; i += 3)
        {
            MapGrids[i, 0].GridType = GridType.Born;
            bornAGrids.Add(MapGrids[i, MapHeight - 1]);
        }
        
        for (int i = 1; i < MapWidth; i += 3)
        {
            MapGrids[i, MapHeight - 1].GridType = GridType.Born;
            bornBGrids.Add(MapGrids[i, 0]);
        }
    }

    private void TidyGridList()
    {
        normalGrids.Clear();

        foreach (GridUnit grid in MapGrids)
        {
            switch (grid.GridType)
            {
                case GridType.Normal:
                    normalGrids.Add(grid);
                    break;
                default:
                    break;
            }
        }
    }
    
    public GridUnit[] GetBronGrid(int side, bool rand)
    {
        if (rand)
            throw new NotImplementedException();
        
        List<GridUnit> bornGrids = new List<GridUnit>();
        bornGrids = (side == 0) ? bornAGrids : bornBGrids;
        GridUnit[] gridUnits = new GridUnit[bornGrids.Count];
        bornGrids.CopyTo(gridUnits);
        return gridUnits;
    }

    public GridUnit GetGridData(int row, int column)
    {
        if (row < 0 || row >= MapHeight || column <0 || column >= MapWidth)
        {
            return null;
        }

        return MapGrids[column, row];
    }
    
    /// <summary>
    /// 得到真正的目标点
    /// </summary>
    /// <param name="from">起点</param>
    /// <param name="to">终点</param>
    /// <param name="path">路径</param>
    /// <param name="mobility">移动力</param>
    /// <returns>通常可以到达了，离终点最近的一个空的地板，终点就在眼前时返回终点，死路返回null</returns>
    public GridUnit GetEmptyGrid(GridUnit from, GridUnit to, List<GridUnit> path, int mobility)
    {
        // 保存寻路结果的容器
        if (path == null)
            path = new List<GridUnit>();
        
        path.Clear();

        // 发起从起点到终点的导航
        if (MapNavigator.Instance.Navigate(this, from, to, path, null, mobility))
        {//可以到达
            if (path.Count > 1)
            {//需要走几步
                if (path[path.Count - 1].Equals(to))
                {
                    //移除最后一个，因为他是目标
                    path.RemoveAt(path.Count - 1);
                    return path[path.Count - 1];
                }
                else
                    return path[path.Count - 1];
            }
            else if (path[path.Count-1].Equals(to))
            {//目标就在眼前，不用动
                path.Clear();
                return null;
            }
            else if (path.Count>0)
            {
                return path[0];
            }
            else
            {
                return null;
            }
        }
        return null;
    }
    
    public GridUnit GetGridByDir(int row, int column, int dir)
    {
        switch (dir)
        {
            //9点钟方向
            case 0:
                return GetGridData(row, column - 1);
            //7点钟
            case 1:
                return GetGridData(row + 1,
                    ((row & 1) == 0) ? column - 1 : column);
            //5点钟
            case 2:
                return GetGridData(row + 1,
                    ((row & 1) == 0) ? column : column + 1);
            //3点钟
            case 3:
                return GetGridData(row, column + 1);
            //1点钟
            case 4:
                return GetGridData(row - 1,
                    ((row & 1) == 0) ? column : column + 1);
            //11点钟
            case 5:
                return GetGridData(row - 1,
                    ((row & 1) == 0) ? column - 1 : column);
            default:
                return null;
        }
    }
    
    public override bool Equals(object obj)
    {
        if (obj is BattleMap)
            return MapId == ((BattleMap) obj).MapId;
        return false;
    }


}
