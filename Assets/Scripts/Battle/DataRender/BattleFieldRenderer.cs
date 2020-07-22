using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BattleFieldRenderer : MonoBehaviourSingleton<BattleFieldRenderer>,
    IVisualRenderer<BattleField, BattleFieldRenderer>
{
    public BattleField BattleField;
    private int currentActionIndex = 0;
    public Camera BattleCamera;
    public UIViewDamageFigure uiViewDamageFigure;
    
    [SerializeField] private GridUnitRenderer gridUnitModel;
    [SerializeField] private Transform gridUnitRoot;
    [SerializeField] private Transform battleUnitRoot;
    
    private List<GridUnitRenderer> gridRenderersPool = new List<GridUnitRenderer>();
    public void Init(System.Action initedCallback)
    {
        if (gridUnitModel == null ||
            gridUnitRoot == null)
        {
            UtilityHelper.LogError("初始化BattleFieldRenderer失败");
            return;
        }
        
        UtilityHelper.Log("BattleFieldRenderer初始化");
        
        InitGridUnitRenderer(100);
        
        UtilityHelper.Log("BattleFieldRenderer初始化成功");

        if (initedCallback != null)
        {
            initedCallback();
        }
    }

    public void CreateDamageFigure(int damage, DamageType damageType, bool isCritical, Vector3 position)
    {
        StartCoroutine(CreateDamageFigureCoroutine(damage, damageType, isCritical, position));
    }

    private IEnumerator CreateDamageFigureCoroutine(int damage, DamageType damageType, bool isCritical, Vector3 position)
    {
        yield return new WaitForSeconds(0.5f);
        uiViewDamageFigure.CreateFigure((int)damage, DamageType.Physical, isCritical, position);
    }

    private void Update()
    {
        if (BattleField != null)
        {
            switch (BattleField.BattleState)
            {
                case BattleState.Ready:
                    OnlyUpdateBar();
                    break;
                case BattleState.Fighting:
                    BattleUnitAction();
                    break;
                default:
                    Debug.Log("还没想好");
                    break;
            }
        }
    }

    private void BattleUnitAction()
    {
        foreach (BattleUnit battleUnit in BattleField.ActionUnitList)
        {
            if (battleUnit.CanAction)
            {
                battleUnit.AutoAction();
            }
        }
    }
    
    private void OnlyUpdateBar()
    {
        foreach (BattleUnit battleUnit in BattleField.ActionUnitList)
        {
            if (battleUnit.CanAction)
            {
                battleUnit.BarUpdate();
            }
        }
    }

    private void InitGridUnitRenderer(int count)
    {
        for (int i = 0; i < count; i++)
        {
            CreateGridUnitRenderer();
        }
    }

    private GridUnitRenderer CreateGridUnitRenderer()
    {
        var clone = Instantiate(gridUnitModel, gridUnitRoot);
        clone.transform.SetUnused(false, GameConst.STR_Unused);
        gridRenderersPool.Add(clone);
        return clone;
    }

    private GridUnitRenderer GetUnusedGridUnitRenderer()
    {
        for (int i = 0; i < gridRenderersPool.Count; i++)
        {
            if (!gridRenderersPool[i].gameObject.activeSelf)
            {
                return gridRenderersPool[i];
            }
        }

        return CreateGridUnitRenderer();
    }

    private void RefreshBattleMapGrids()
    {
        if (BattleField == null)
        {
            UtilityHelper.LogError("准备战场失败。没有战场数据");
            return;
        }

        for (int row = 0; row < BattleField.BattleMap.MapHeight; row++)
        {
            for (int column = 0; column < BattleField.BattleMap.MapWidth; column++)
            {
                GridUnit gridUnitData = BattleField.BattleMap.MapGrids[column, row];
                if (gridUnitData != null)
                {
                    GridUnitRenderer gridUnitRenderer = GetUnusedGridUnitRenderer();
                    if (gridUnitRenderer != null)
                    {
                        gridUnitData.ConnectRenderer(gridUnitRenderer);
                    }
                }
            }
        }
    }

    private void RefreshBattleUnits()
    {
        if (BattleField == null)
        {
            UtilityHelper.LogError("准备战斗单位失败，没有战场");
            return;
        }

        int allCount = BattleField.Teams[0].BattleUnits.Count + BattleField.Teams[1].BattleUnits.Count;
        int hasInstantiate = 0;
        for (int i = 0; i < BattleField.Teams.Count; i++)
        {
            BattleTeam team = BattleField.Teams[i];
            if (team.BattleUnits != null)
            {
                for (var index = 0; index < team.BattleUnits.Count; index++)
                {
                    var battleUnitData = team.BattleUnits[index];
                    Addressables.InstantiateAsync(battleUnitData.AssetAddress, battleUnitRoot).Completed += (handle) =>
                    {
                        if (handle.Status == AsyncOperationStatus.Succeeded)
                        {
                            BattleUnitRenderer battleUnitRenderer = handle.Result.GetComponent<BattleUnitRenderer>();
                            if (battleUnitRenderer != null)
                            {
                                battleUnitData.ConnectRenderer(battleUnitRenderer);
                                battleUnitRenderer.UpdatePositionByGrid(battleUnitData.mapGrid);
                            }
                            else
                            {
                                UtilityHelper.LogError("BattleUnitRender为空，无法连接");
                            }
                        }
                        hasInstantiate++;
                        if (hasInstantiate == allCount)
                        {
                            BattleField.Run();
                        }
                    };
                }
            }
        }

    }

    public void OnConnect(BattleField data)
    {
        BattleField = data;
        //加载战场
        RefreshBattleMapGrids();
        //加载战斗单位
        RefreshBattleUnits();
    }

    public void OnDisconnect()
    {
        if (BattleField != null)
        {
            BattleField = null;
        }
    }
    
    
    //    /// <summary>
//    /// 播放战场动作
//    /// </summary>
//    /// <param name="callback"></param>
//    /// <returns></returns>
//    public IEnumerator PlayerBattleByCoroutine(Action callback)
//    {
//        if (BattleField == null ||
//            BattleField.BattleActions == null ||
//            BattleField.BattleActions.Count == 0)
//        {
//            UtilityHelper.LogError($"播放战场行动失败");
//            yield break;
//        }
//        
//        
//        
//        //遍历所有战场动作
//        while (currentActionIndex < BattleField.BattleActions.Count)
//        {
//            BattleUnitAction heroAction = null;
//
//            if (BattleField.BattleActions[currentActionIndex] is BattleUnitAction)
//            {
//                heroAction = (BattleUnitAction) BattleField.BattleActions[currentActionIndex];
//
//                if (heroAction.ActionUnit != null && heroAction.ActionUnit.BattleUnitRenderer != null)
//                {
//                    yield return heroAction.ActionUnit.BattleUnitRenderer.RunHeroAction(heroAction);
//                }
//            }
//
//            ++currentActionIndex;
//        }
//        
//        callback?.Invoke();
//    }

//    /// <summary>
//    /// 播放战场动作
//    /// </summary>
//    /// <param name="callback"></param>
//    public void PlayBattle(Action callback)
//    {
//        StartCoroutine(PlayerBattleByCoroutine(callback));
//    }
}