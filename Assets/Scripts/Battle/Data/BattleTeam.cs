using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BattleTeam: CountableInstance
{
    public BattleField BattleField;
    public List<BattleUnit> BattleUnits = new List<BattleUnit>();
    
    private Dictionary<int, BattleUnit> battleUnitsDic = new Dictionary<int, BattleUnit>();

    /// <summary>
    /// 添加战斗单位
    /// </summary>
    /// <param name="battleUnit"></param>
    public void AddBattleUnit(BattleUnit battleUnit)
    {
        if (battleUnit == null)
        {
            return;
        }

        if (battleUnit.BattleTeam != null)
        {
            //重复加入
            if (battleUnit.BattleTeam.Equals(this))
            {
                return;
            }
            UtilityHelper.LogError("添加战斗单位到队伍中失败，该单位已经有队伍");
            return;
        }

        //重复添加
        if (BattleUnits.Contains(battleUnit))
        {
            return;
        }
        
        BattleUnits.Add(battleUnit);
        battleUnitsDic.Add(battleUnit.Id, battleUnit);

        battleUnit.JoinBattleTeam(this);
    }

    /// <summary>
    /// 移除战斗单位
    /// </summary>
    /// <param name="battleUnit"></param>
    public void RemoveBattleUnit(BattleUnit battleUnit)
    {
        if (battleUnit.BattleTeam == null || !battleUnit.BattleTeam.Equals(this))
        {
            UtilityHelper.LogError("移除战斗单位失败");
            return;
        }
        BattleUnits.Remove(battleUnit);
        battleUnitsDic.Remove(battleUnit.Id);
        
        battleUnit.QuitBattleTeam();
    }

    public override bool Equals(object obj)
    {
        if (obj is BattleTeam)
        {
            return Id == ((BattleTeam) obj).Id;
        }
        return false;
    }

    public string Desc()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("Team id = {0}\n", Id);
        for (int i = 0; i < BattleUnits.Count; i++)
        {
            sb.AppendFormat(" {0}\n", BattleUnits[i].Desc());
        }

        return sb.ToString();
    }
    
    //    //进入战场
//    public void EnterBattleField(BattleField battleField, GridUnit[] bornGrids)
//    {
//        if (battleField == null)
//        {
//            UtilityHelper.LogError("进入战场失败，战场为空");
//            return;
//        }
//
//        if (bornGrids == null || bornGrids.Length == 0)
//        {
//            UtilityHelper.LogError("进入战场失败，出生格为空");
//            return;
//        }
//
//        BattleField = battleField;
//
//        for (int i = 0; i < BattleUnits.Count; i++)
//        {
//            if (i >= bornGrids.Length)
//            {
//                UtilityHelper.LogError("进入战场失败，出生格不够");
//                continue;
//            }
//            BattleUnits[i].EnterBattleField(battleField, bornGrids[i]);
//        }
//    }
}
