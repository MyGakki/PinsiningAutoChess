using System.Collections.Generic;

public class BattleFiledCreator: 
    CounterMap<BattleFiledCreator, BattleField>, IGameBase
{
    public void Init(params object[] args)
    {
        UtilityHelper.Log("BattleFieldCreator初始化");
    }

    public string Desc()
    {
        return string.Empty;
    }

    public BattleField Create(int mapWidth, int mapHeight, List<string> teamA, List<string> teamB)
    {
        BattleField battleField = Create();
        battleField.Init(mapWidth, mapHeight, teamA, teamB);
        return battleField;
    }
}
