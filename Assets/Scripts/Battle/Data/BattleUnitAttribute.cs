using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleUnitAttribute
{
    public string BattleUnitChineseName;

    private int currentHp;
    public int CurrentHp
    {
        get => currentHp;
        set => currentHp = Mathf.Max(value, 0);
    }
    public int BaseMaxHp;
    public int AddMaxHp;
    public int MaxHp => BaseMaxHp + AddMaxHp;

    private int currentMp;
    public int CurrentMp
    {
        get => currentMp;
        set => currentMp = Mathf.Min(value, MaxHp);
    }
    public int BaseInitialMp;
    public int AddInitialMp;
    public int InitialMp => BaseInitialMp + AddInitialMp;
    public int MaxMp;

    public int Mobility;
    public int BaseAttackRange;
    public int AddAttackRange;
    public int AttackRange => BaseAttackRange + AddAttackRange;

    public int BaseAttackDamage;
    public int AddAttackDamage;
    public int AttackDamage => BaseAttackDamage + AddAttackDamage;
    
    public float BaseAbilityPower;
    public float AddAbilityPower;
    public float AbilityPower => BaseAbilityPower + AddAbilityPower;
    
    public float BaseAttackSpeed;
    public float AddAttackSpeed;
    public float AttackSpeed => BaseAttackSpeed + AddAttackSpeed;
    
    public float BaseCriticalChance;
    public float AddCriticalChance;
    public float CriticalChance => BaseCriticalChance + AddCriticalChance;
    
    public float BaseCriticalDamageRate;
    public float AddCriticalDamageRate;
    public float CriticalDamageRate => BaseCriticalDamageRate + AddCriticalDamageRate;

    public int BaseArmor;
    public int AddArmor;
    public int Armor => BaseArmor + AddArmor;
    public float ArmorPercent => 100 / (100f + Armor);
    
    public int BaseSpellResistance;
    public int AddSpellResistance;
    public int SpellResistance => BaseSpellResistance + AddSpellResistance;
    public float SpellResistancePercent => 100 / (100f + SpellResistance);

    public BattleUnitAttribute(SO_BattleUnitAttribute so)
    {
        BattleUnitChineseName = so.BattleUnitChineseName;
        
        BaseMaxHp = so.MaxHp;
        AddMaxHp = 0;

        BaseInitialMp = so.InitialMp;
        AddInitialMp = 0;
        MaxMp = so.MaxMp;

        Mobility = so.Mobility;
        BaseAttackRange = so.AttackRange;
        AddAttackRange = 0;

        BaseAttackDamage = so.BaseAttackDamage;
        AddAttackDamage = 0;
        
        BaseAbilityPower = so.BaseAbilityPower;
        AddAbilityPower = 0;
        
        BaseAttackSpeed = so.BaseAttackSpeed;
        AddAttackSpeed = 0;
        
        BaseCriticalChance = so.BaseCriticalChance;
        AddCriticalChance = 0;
        
        BaseCriticalDamageRate = so.BaseCriticalDamageRate;
        AddCriticalDamageRate = 0;
        
        BaseArmor = so.BaseArmor;
        AddArmor = 0;
        
        BaseSpellResistance = so.BaseSpellResistance;
        AddSpellResistance = 0;
        
        ResetHpAndMp();
    }

    public void ResetHpAndMp()
    {
        CurrentHp = MaxHp;
        CurrentMp = InitialMp;
    }

    public void CalculateAttack(BattleUnit victimUnit)
    {
        BattleUnitAttribute victim = victimUnit.BattleUnitAttribute;
        float damage = AttackDamage * victim.ArmorPercent;
        bool isCritical = Random.Range(0f, 1f) < CriticalChance;
        if (isCritical)
        {
            damage *= CriticalDamageRate;
        }
        victim.CurrentHp -= (int) damage;
        CurrentMp += 10;
        victim.CurrentMp += (int)(damage * 100 / victim.MaxHp);
        
        BattleFieldRenderer.Instance.CreateDamageFigure((int) damage, DamageType.Physical, isCritical, victimUnit.BattleUnitRenderer.transform.position);

        if (victim.CurrentHp <= 0)
            victimUnit.BattleUnitRenderer.StartCoroutine(victimUnit.Die());
    }
}
