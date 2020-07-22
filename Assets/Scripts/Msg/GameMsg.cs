using System.Collections.Generic;
using System.Text;

public enum MsgBattleHeroActionType
{
    None,
    BattleUnitAction,
    BattleStart,
    BattleEnd
}

public class BattleAction
{
    public MsgBattleHeroActionType ActionType;
    public int sn = 0;

    protected BattleAction(MsgBattleHeroActionType ActionType)
    {
        ActionType = ActionType;
    }
}