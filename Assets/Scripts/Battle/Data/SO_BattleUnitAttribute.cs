using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public enum BattleUnitName
{
    LuciaCrowFeatherHaze,
    QiShiPulseMeteor,
    Qu,
    RossettaHumanlike,
}

#if UNITY_EDITOR
[CustomEditor(typeof(SO_BattleUnitAttribute))]
[CanEditMultipleObjects]
public class SO_BattleUnitAttributeCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Reset name."))
        {
            SO_BattleUnitAttribute instance = (SO_BattleUnitAttribute) target;
            instance.BattleUnitChineseName = instance.name;
        }
    }
}
#endif

[CreateAssetMenu(menuName = "ScriptableObject/Battle unit attribute")]
public class SO_BattleUnitAttribute: ScriptableObject
{
    [Tooltip("中文名")]public string BattleUnitChineseName;
    
    [Tooltip("最大HP")]public int MaxHp;
    
    [Tooltip("初始MP")]public int InitialMp;
    [Tooltip("最大MP")]public int MaxMp;

    [Tooltip("移动力")]public int Mobility;
    [Tooltip("攻击范围")]public int AttackRange;

    [Tooltip("AD")]public int BaseAttackDamage;
    [Tooltip("AP")]public float BaseAbilityPower;
    [Tooltip("攻速")]public float BaseAttackSpeed;
    [Tooltip("暴击率")]public float BaseCriticalChance;
    [Tooltip("暴击伤害倍率")]public float BaseCriticalDamageRate;

    [Tooltip("护甲")]public int BaseArmor;
    [Tooltip("魔抗")]public int BaseSpellResistance;

    [Tooltip("战斗单位名")] public BattleUnitName BattleUnitName;
    [Tooltip("资源地址")] public string AssetName;
}