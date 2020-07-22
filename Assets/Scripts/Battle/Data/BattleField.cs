using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum BattleState
{
    Prepare,        //准备中
    Ready,          //准备就绪
    Fighting,       //战斗中
    End,            //战斗结束
    Exception,      //战斗异常
}

public class BattleField : 
    CountableInstance,
    IVisualData<BattleField, BattleFieldRenderer>
{
    public BattleState BattleState = BattleState.Prepare;

    public BattleMap BattleMap;
    public List<BattleTeam> Teams = new List<BattleTeam>();

    public List<BattleUnit> ActionUnitList = new List<BattleUnit>();

    private BattleFieldRenderer battleFieldRenderer;
    
    private Dictionary<BattleUnit, HpMpBar> battleUnitBarDic = new Dictionary<BattleUnit, HpMpBar>();

    public void Init(int width, int height, List<string> teamA, List<string> teamB)
    {
        //将Grid铺起来，但并未连接渲染器
        BattleMap = BattleMapCreator.Instance.Create(width, height);
        
        //生成战斗单位小组，加GridUnit加入Team中
        GenerateBattleTeam(teamA, teamB);
    }

    private void GenerateBattleTeam(List<string> teamA, List<string> teamB)
    {
        int teamCount = 2;
        for (int i = 0; i < teamCount; i++)
        {
            BattleTeam team =
                BattleTeamCreator.Instance.Create(i == 0 ? teamA : teamB, BattleMap.GetBronGrid(i, false));
            foreach (BattleUnit battleUnit in team.BattleUnits)
            {
                ActionUnitList.Add(battleUnit);
            }
            Teams.Add(team);
        }

        for (int i = 0; i < teamCount; i++)
        {
            foreach (BattleUnit battleUnit in Teams[i].BattleUnits)
            {
                battleUnit.BattleField = this;
                battleUnit.BattleTeam = Teams[i];
                battleUnit.EnemyTeam = Teams[Mathf.Abs(i - 1)];
            }
        }
    }

    /// <summary>
    /// 获取一个Team
    /// </summary>
    /// <param name="battleUnit">战斗单位</param>
    /// <param name="sameTeam">是否返回该战斗单位的team</param>
    /// <returns></returns>
    public BattleTeam GetBattleTeam(BattleUnit battleUnit, bool sameTeam)
    {
        if (battleUnit == null)
        {
            return null;
        }

        if (Teams[0].Id == battleUnit.BattleTeam.Id)
        {
            return sameTeam ? Teams[0] : Teams[1];
        }
        else if (Teams[1].Id == battleUnit.BattleTeam.Id)
        {
            return sameTeam ? Teams[1] : Teams[0];
        }
        
        UtilityHelper.LogError(
            $"Get battle team failed.Target teamID = {battleUnit.BattleTeam.Id}, team 0 id = {Teams[0].Id}, team 1 id = {Teams[1].Id}");

        return null;
    }

    public bool CheckBattleEnd(BattleUnit deadUnit)
    {
        BattleTeam deadTeam = GetBattleTeam(deadUnit, true);
        int unitsCount = deadTeam.BattleUnits.Count;
        for (int i = 0; i < unitsCount; i++)
        {
            if (deadTeam.BattleUnits[i].BattleUnitState != BattleUnitState.Dead)
                break;
            if (i == unitsCount - 1)
            {
                BattleState = BattleState.End;
                return true;
            }
        }

        return false;
    }

    public async void Run()
    {
        Debug.Log("开始战斗");
        UIViewHpMpBar uiViewHpMpBar = null;
        Task<UIViewBase> barHandle = UIViewManager.Instance.ShowView(UIViewName.HpMpBar);
        await barHandle;
        Task<UIViewBase> damageHandel = UIViewManager.Instance.ShowView(UIViewName.DamageFigure);
        await damageHandel;
        if (barHandle.Status == TaskStatus.RanToCompletion && damageHandel.Status == TaskStatus.RanToCompletion)
        {
            uiViewHpMpBar = barHandle.Result as UIViewHpMpBar;
            
            foreach (BattleUnit battleUnit in ActionUnitList)
            {
                HpMpBar hpMpBar = uiViewHpMpBar.CreateHpMpBar();
                battleUnitBarDic.Add(battleUnit, hpMpBar);
                battleUnit.HpMpBar = hpMpBar;
            }
            
            battleFieldRenderer.uiViewDamageFigure = damageHandel.Result as UIViewDamageFigure;
        }
        
        BattleState = BattleState.Ready;
    }

    public void ConnectRenderer(BattleFieldRenderer renderer)
    {
        if (battleFieldRenderer != null)
            return;

        battleFieldRenderer = renderer;
        battleFieldRenderer.OnConnect(this);
        UtilityHelper.Log($"{this}连接到渲染器{renderer}");
    }

    public void DisconnectRenderer()
    {
        if (battleFieldRenderer != null)
        {
            foreach (GridUnit gridUnit in BattleMap.MapGrids)
            {
                gridUnit.DisconnectRenderer();
            }
            
            battleFieldRenderer.OnDisconnect();
            battleFieldRenderer = null;
            UtilityHelper.Log($"{this}断开渲染");
            
        }
    }
}