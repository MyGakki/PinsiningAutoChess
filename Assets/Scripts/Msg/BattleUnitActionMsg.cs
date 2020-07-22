using System.Collections.Generic;
using System.Text;

public class BattleUnitAction: BattleAction
{
    protected BattleUnitAction(BattleUnit actionUnit) : base(MsgBattleHeroActionType.BattleUnitAction)
    {
        ActionUnit = actionUnit;
    }

    public BattleUnit ActionUnit;
    public BattleUnitEnterBattleFieldAction EnterBattleFieldAction;
    public BattleUnitChangeTargetAction ChangeTargetAction;
    public BattleUnitMotionAction MotionAction;

    public static BattleUnitAction Create(BattleUnit actionUnit)
    {
        return new BattleUnitAction(actionUnit);
    }

    public string Desc()
    {
        StringBuilder desc = new StringBuilder();
        desc.AppendFormat("BattleUnit action:{0}  ", ActionUnit.ChessName);
        if (EnterBattleFieldAction != null)
        {
            desc.AppendFormat("进入战场:{0}, 出生格:{1}  ", EnterBattleFieldAction.BattleField,
                EnterBattleFieldAction.BornGrid);
        }
        if (ChangeTargetAction != null)
        {
            desc.AppendFormat("改变目标:from {0} to {1}  ", ChangeTargetAction.LastTargetUnit == null ? "None" : ChangeTargetAction.LastTargetUnit.ChessName, ChangeTargetAction.NewTargetUnit == null ? "None" : ChangeTargetAction.NewTargetUnit.ChessName);
        }
        if (MotionAction != null)
        {
            desc.AppendFormat("移动: 目标 is {0}, 从 {1}  ", MotionAction.TargetUnit.ChessName, MotionAction.FromGrid);
            if (MotionAction.GridPath != null)
            {
                for (int i = 0; i < MotionAction.GridPath.Count; ++i)
                {
                    desc.AppendFormat("Passed: {0}  ", MotionAction.GridPath[i]);
                }
            }
            desc.AppendFormat("Move range: {0}  ", MotionAction.MoveRange);
        }
        return desc.ToString();
    }
}

//进入战场
public class BattleUnitEnterBattleFieldAction
{
    public BattleField BattleField;
    public GridUnit BornGrid;
    public BattleUnitSyncAttribute Attribute;
    
    private BattleUnitEnterBattleFieldAction() {}

    public static BattleUnitEnterBattleFieldAction Get()
    {
        return new BattleUnitEnterBattleFieldAction();
    }
}

//切换目标
public class BattleUnitChangeTargetAction
{
    public BattleUnit LastTargetUnit;
    public BattleUnit NewTargetUnit;
    
    private BattleUnitChangeTargetAction() {}

    public static BattleUnitChangeTargetAction Get()
    {
        return new BattleUnitChangeTargetAction();
    }
}

//移动
public class BattleUnitMotionAction
{
    public BattleUnit TargetUnit;        //目标战斗单位
    public GridUnit FromGrid;            //从哪个格子开始出发
    public List<GridUnit> GridPath;          //移动路径
    public int MoveRange;                
    
    private BattleUnitMotionAction() {}

    public static BattleUnitMotionAction Get()
    {
        return new BattleUnitMotionAction();
    }
}

public class BattleUnitAttributeUpdate
{
    public BattleUnitSyncAttribute Attribute;
    
    private BattleUnitAttributeUpdate() {}

    public static BattleUnitAttributeUpdate Get()
    {
        return new BattleUnitAttributeUpdate();
    }
}

public class BattleUnitSyncAttribute
{
    public int HpChanged;
    public int CurrentHp;
    public int EnergyChanged;
    public int CurrentEnergy;
}
