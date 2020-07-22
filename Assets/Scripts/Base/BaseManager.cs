using UnityEngine;

public class BaseManager<T>: MonoBehaviourSingleton<T> where T: BaseManager<T>
{
    [SerializeField] public bool DebugMode = false;
    
    public virtual string MgrName => "BaseManager";

    public virtual void InitManager()
    {
        UtilityHelper.Log($"-->{MgrName}<-- 初始化");
    }

    public void MgrLog(string info)
    {
        if (DebugMode)
        {
            UtilityHelper.Log($"{MgrName} :::: {info}");
        }
    }
}
