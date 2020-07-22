using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BattleManager: BaseManager<BattleManager>
{
    [Tooltip("地图尺寸")][SerializeField] private Vector2Int mapSize;

    [SerializeField] private List<string> teamA;
    [SerializeField] private List<string> teamB;
    
    [SerializeField] private Camera battleCamera;
    
    private Dictionary<BattleUnitName, SO_BattleUnitAttribute> battleUnitConfigDic = new Dictionary<BattleUnitName, SO_BattleUnitAttribute>();
    
    private BattleField singleBattle;
    private bool battleFiledRendererIsReady = false;

    public override string MgrName => "BattleManager";

    public override void InitManager()
    {
        base.InitManager();
        //初始化战场显示器,加载了100个隐藏的Grid
        BattleFieldRenderer.Instance.Init(OnBattleFieldReady);

        InitBattleUnitConfig();
        
        UtilityHelper.Log($"-->{MgrName}<--初始化完");
    }

    private void OnBattleFieldReady()
    {
        battleFiledRendererIsReady = true;
        UtilityHelper.Log("战场渲染完成");
    }

    private void ResetBattleCamera()
    {
        if (battleCamera)
        {
            battleCamera.orthographic = false;
            
            battleCamera.transform.position = new Vector3(
                mapSize.x * GameConst.Map_GridWith * 0.5f,
                15,
                -23);
            
        }
    }

    public void Run()
    {
        //将战场的格子密铺（并未连接渲染器），然后创建了两个Team
        singleBattle = BattleFiledCreator.Instance.Create(
            mapSize.x, mapSize.y,
            teamA, teamB);
        
        //重置摄像头
        ResetBattleCamera();
        
        //Gird连接渲染器，加载棋子模型,并连接渲染器
        singleBattle.ConnectRenderer(BattleFieldRenderer.Instance);
    }

    private async void InitBattleUnitConfig()
    {
        var handle = Addressables.LoadAssetsAsync<SO_BattleUnitAttribute>("BattleUnitConfig", null);
        await handle.Task;
        
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (SO_BattleUnitAttribute config in handle.Result)
            {
                Debug.Log($"加载{config.AssetName}配置文件");
                battleUnitConfigDic.Add(config.BattleUnitName, config);
            }
        }
    }
}
