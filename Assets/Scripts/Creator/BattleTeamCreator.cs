using System.Collections.Generic;
using System.Linq;

public class BattleTeamCreator: 
    CounterMap<BattleTeamCreator, BattleTeam>, IGameBase
{
    public void Init(params object[] args)
    {
        UtilityHelper.Log("BattleTeamCreator初始化");
    }

    public string Desc()
    {
        return string.Empty;
    }

    public BattleTeam Create(List<string> members, GridUnit[] borGridUnits)
    {
        BattleTeam battleTeam = Create();
        for (int i = 0; i < members.Count; i++)
        {
            BattleUnit battleUnit = BattleUnitCreator.Instance.Create(members[i], borGridUnits[i]);
            battleTeam.AddBattleUnit(battleUnit);
        }

        return battleTeam;
    }
    
}
