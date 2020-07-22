using System;

public class Main: MonoBehaviourSingleton<Main>
{
    //初始化各个管理器
    private void PrepareManager()
    {
        BattleManager.Instance.InitManager();
        
        UIViewManager.Instance.InitManager();
    }
    
    private void Start()
    {
        UtilityHelper.Log("Main Start");
        PrepareManager();
    }

    private void OnDestroy()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.ClearProgressBar();
#endif
    }
}
