using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(SO_UIViewConfig))]
public class SO_UIViewConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        if (GUI.changed)
        {
            var pUnique = serializedObject.FindProperty("unique");
            var pCacheScheme = serializedObject.FindProperty("cacheScheme");
            if (pCacheScheme.enumValueIndex == 2 && pUnique.boolValue == false)
            {
                EditorUtility.DisplayDialog("Warning", "常驻内存必须唯一!", "OK");
                pUnique.boolValue = true;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif

public enum UIViewName
{
    Main,
    HpMpBar,
    DamageFigure,
    StartGame,
    TopologicalMap,
}

public enum UIViewLayer
{
    None,
    
    Background,
    Base,
    HUD,
    Popup,
    Top,
    Debug,
}

public enum UIViewCacheScheme
{
    AutoRemove,
    TempCache,
    Cache,
}

[CreateAssetMenu(menuName = "ScriptableObject/UI View Config")]
public class SO_UIViewConfig : ScriptableObject
{
    [Tooltip("是否唯一打开")] public bool unique = true;
    [Tooltip("界面名称")] public UIViewName viewName;
    [Tooltip("所在层")] public UIViewLayer viewLayer;
    [Tooltip("缓存策略")] public UIViewCacheScheme cacheScheme;
    [Tooltip("资源地址")] public string assetName;
    [Tooltip("是否遮挡整个屏幕")] public bool coverScreen;
    [Tooltip("被遮挡后是否还更新")] public bool alwaysUpdate;
    [Tooltip("点击背景后是否关闭")] public bool bgTriggerClose;

    public string Address
    {
        get { return $"界面/{assetName}.prefab";  }
    }
}