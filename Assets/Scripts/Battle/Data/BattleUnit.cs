using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// 战斗单位所处的状态
/// </summary>
public enum BattleUnitState
{
    Wait,        //等待行动
    Cold,        //进行行动中，无法接受别的行动
    Dead,        //死亡
}


public class BattleUnit: 
    CountableInstance,
    IVisualData<BattleUnit, BattleUnitRenderer>
{
    //战斗单位的属性
    public BattleUnitAttribute BattleUnitAttribute;
    
    //资源地址
    public string AssetAddress;
    //中文名
    public string ChessName => BattleUnitAttribute.BattleUnitChineseName;
    //所在战场
    public BattleField BattleField;
    //所属队伍
    public BattleTeam BattleTeam;
    //敌方队伍
    public BattleTeam EnemyTeam;
    //所处的状态
    public BattleUnitState BattleUnitState = BattleUnitState.Wait;
    //目标单位
    public BattleUnit TargetBattleUnit;
    //所在格子
    public GridUnit mapGrid;
    //目标格子
    private GridUnit targetGrid;
    //移动到目标的路径
    private List<GridUnit> toTargetPath = new List<GridUnit>();
    //关联的渲染器
    public BattleUnitRenderer BattleUnitRenderer;
    
    private HpMpBar hpMpBar;
    public HpMpBar HpMpBar
    {
        get => hpMpBar;
        set
        {
            hpMpBar = value;
            barRect = value.GetComponent<RectTransform>();
        }
    }
    private RectTransform barRect;

    public bool CanAction
    {
        get
        {
            return BattleUnitAttribute.CurrentHp > 0;
        }
    }

    public void JoinBattleTeam(BattleTeam battleTeam)
    {
        BattleTeam = battleTeam;
    }

    public void QuitBattleTeam()
    {
        BattleTeam = null;
    }
    

    //棋子自动行动
    public void AutoAction()
    {
        if (BattleUnitState != BattleUnitState.Dead)
            BarUpdate();
        if (BattleUnitState == BattleUnitState.Wait)
        {
            // 目标战斗对象为空或者已死
            if (TargetBattleUnit == null || TargetBattleUnit.CanAction == false)
            {
                TargetBattleUnit = SearchTarget();
            }
            

            // 目标单位在攻击范围内
            if (TargetBattleUnit.mapGrid.Distance(mapGrid) <= BattleUnitAttribute.AttackRange)
            {
                AttackToBattleUnit();
                TargetBattleUnit.BarUpdate();
            }
            //目标单位在攻击范围外
            else
            {
                //得到本次单格移动的目标
                targetGrid = BattleField.BattleMap.GetEmptyGrid(mapGrid, TargetBattleUnit.mapGrid, toTargetPath,
                    BattleUnitAttribute.Mobility);
                
                //目标格子为空，说明无法到达目标战斗单位或者已经在战斗单位一格内
                //目标格子与目标的距离小于等于攻击范围，说明已经可以对目标进行攻击了
                //则应该在移动后停止移动动画
                bool stopMoveAnim = (targetGrid == null || TargetBattleUnit.mapGrid.Distance(targetGrid) <= BattleUnitAttribute.AttackRange);
                
                BattleUnitState = BattleUnitState.Cold;
                BattleUnitRenderer.MoveToTargetGrid(targetGrid, stopMoveAnim);
            }
        }

    }
    
    public void BarUpdate()
    {
        if (barRect != null)
        {
            Vector2 screenPos = UIViewManager.Instance.ConvertWorldPositionToRootCanvasPosition(BattleUnitRenderer.transform.position);
            barRect.anchoredPosition = new Vector2(screenPos.x, screenPos.y + 100f);
            HpMpBar.SetHpBar(BattleUnitAttribute.CurrentHp / (float)BattleUnitAttribute.MaxHp);
            HpMpBar.SetMpBar(BattleUnitAttribute.CurrentMp / (float)BattleUnitAttribute.MaxHp);
        }
    }

    public IEnumerator Die()
    {
        BattleUnitState = BattleUnitState.Dead;
        BattleField.CheckBattleEnd(this);
        yield return new WaitForSeconds(0.5f);
        BattleUnitRenderer.gameObject.SetActive(false);
        barRect.gameObject.SetActive(false);
    }
    
    private void AttackToBattleUnit()
    {
        BattleUnitState = BattleUnitState.Cold;
        BattleUnitAttribute.CalculateAttack(TargetBattleUnit);
        BattleUnitRenderer.AttackToBattleUnit(TargetBattleUnit, 1f/BattleUnitAttribute.AttackSpeed);
    }

    /// <summary>
    /// 寻找到距离该单位最近的地方战斗单位
    /// </summary>
    /// <returns></returns>
    private BattleUnit SearchTarget()
    {
        UtilityObjs.BattleUnits.Clear();
        // 搜寻敌对中可以行动的敌人
        for (int i = 0; i < EnemyTeam.BattleUnits.Count; i++)
        {
            if (EnemyTeam.BattleUnits[i].CanAction)
            {
                UtilityObjs.BattleUnits.Add(EnemyTeam.BattleUnits[i]);
            }
        }
        //没有搜索到敌人
        if (UtilityObjs.BattleUnits.Count == 0)
        {
            return null;
        }

        //对合法的对象进行排序
        BattleUnit newTargetBattleUnit = UtilityObjs.BattleUnits[0];
        int minDistance = newTargetBattleUnit.mapGrid.Distance(mapGrid);

        foreach (BattleUnit battleUnit in UtilityObjs.BattleUnits)
        {
            int distance = battleUnit.mapGrid.Distance(mapGrid);
            if (battleUnit.mapGrid.Distance(mapGrid) == minDistance)
            {
                if (Mathf.Abs(battleUnit.mapGrid.GridPosition.y - mapGrid.GridPosition.y) < 
                    Mathf.Abs(newTargetBattleUnit.mapGrid.GridPosition.y - mapGrid.GridPosition.y))
                {
                    newTargetBattleUnit = battleUnit;
                }
            }
            else if (battleUnit.mapGrid.Distance(mapGrid) < minDistance)
            {
                newTargetBattleUnit = battleUnit;
                minDistance = distance;
            }
        }

        return newTargetBattleUnit;
    }

    //进入格子
    public void EnterGrid(GridUnit grid)
    {
        if (grid == null)
        {
            UtilityHelper.LogError($"战斗单位{Id}进入格子失败。格子为空");
            return;
        }

        if (mapGrid != null)
        {
            LeaveGrid();
        }

        mapGrid = grid;
        
        //通知格子被自己进入了
        grid.OnEnter(this);
    }

    private void LeaveGrid()
    {
        if (mapGrid != null)
        {
            mapGrid.OnLeave();
            mapGrid = null;
        }
    }

    public void ConnectRenderer(BattleUnitRenderer renderer)
    {
        if (renderer == null)
        {
            UtilityHelper.Log("战斗单位连接渲染器失败。渲染器为空");
            return;
        }

        if (BattleUnitRenderer != null)
        {
            DisconnectRenderer();
        }

        BattleUnitRenderer = renderer;
        BattleUnitRenderer.OnConnect(this);
    }

    public void DisconnectRenderer()
    {
        if (BattleUnitRenderer != null)
        {
            BattleUnitRenderer.OnDisconnect();
            BattleUnitRenderer = null;
        }
    }

    public override string ToString()
    {
        return $"BattleUnit_{BattleTeam.Id}_{Id}";
    }

    public string Desc()
    {
        return string.Empty;
    }
    
}
