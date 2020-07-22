using UnityEngine;

public static class UtilityHelper
{
    public static void Log(object msg)
    {
        Debug.Log(msg);
    }
    
    public static void LogError(object msg)
    {
        Debug.LogError(msg);
    }
    
    public static void LogWarning(object msg)
    {
        Debug.LogWarning(msg);
    }

    public static void ShiftOut(this Transform transform)
    {
        transform.localPosition = new Vector3(0f, 1000f, 0f);
    }

    public static void Normalize(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        transform.localRotation =Quaternion.identity;
    }
    
    public static void Normalize(this RectTransform rectTransform)
    {
        rectTransform.localPosition = Vector3.zero;
        rectTransform.localScale = Vector3.one;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;
    }

    public static void SetUnused(this Transform transform, bool active, string originName)
    {
        transform.Normalize();
        transform.gameObject.SetActive(active);
#if UNITY_EDITOR
        transform.name = active ? originName : $"{originName}{GameConst.STR_Unused}";
#else
            transform.name = originName;
#endif
    }
    
    public static void CleanName(this Transform transform)
    {
        transform.name = transform.name.Replace("(Clone)", string.Empty);
    }

    public static float GetClipLength(this Animator animator, string clipName) 
    {
        if (null== animator || 
            string.IsNullOrEmpty(clipName) || 
            null == animator.runtimeAnimatorController) 
            return 0;
        // 获取所有的clips	
        var clips = animator.runtimeAnimatorController.animationClips;
        if( null == clips || clips.Length <= 0) return 0;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name.Equals(clipName))
            {
                return clip.length;
            }
        }
        return 0f;
    }
}
