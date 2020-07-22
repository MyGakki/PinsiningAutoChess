using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BattleUnitCreator:
    CounterMap<BattleUnitCreator, BattleUnit>, IGameBase
{
    public void Init(params object[] args)
    {
        UtilityHelper.Log("BattleUnitCreator初始化");
    }

    public string Desc()
    {
        return string.Empty;
    }

    public BattleUnit Create(string name, GridUnit bornGrid)
    {
        BattleUnit battleUnit = Create();
        
        battleUnit.AssetAddress = name;
        string chessName = name.Replace("棋子/", "").Replace(".prefab", "");

        Addressables.LoadAssetAsync<SO_BattleUnitAttribute>("配置属性/" + chessName + ".asset").Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                battleUnit.BattleUnitAttribute = new BattleUnitAttribute(handle.Result);
            }
        };

        battleUnit.EnterGrid(bornGrid);
        return battleUnit;
    }
}
