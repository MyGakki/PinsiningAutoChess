using System;

public class BattleMapCreator
    : CounterMap<BattleMapCreator, BattleMap>, IGameBase
{
    public void Init(params object[] args)
    {
        UtilityHelper.Log("BattleMapCreator初始化");
    }

    public string Desc()
    {
        return String.Empty;
    }

    public BattleMap Create(int width, int height)
    {
        BattleMap battleMap = Create();
        battleMap.Init(width, height);
        return battleMap;
    }
}
