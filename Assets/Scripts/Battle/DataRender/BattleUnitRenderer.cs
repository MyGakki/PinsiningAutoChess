using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;

public class BattleUnitRenderer: 
    BaseBehaviour,
    IVisualRenderer<BattleUnit, BattleUnitRenderer>
{

    public BattleUnit BattleUnit;
    private Animator animator;
    private float attackAnimationTime;
    

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        attackAnimationTime = animator.GetClipLength("Attack");
    }

    public void MoveToTargetGrid(GridUnit gridUnit, bool stopMoveAnim)
    {
        if (gridUnit == null)
        {
            animator.SetBool("Run", false);
            return;
        }

        foreach (Transform child in transform)
        {
            child.LookAt(gridUnit.LocalPosition);
        }
        if (animator.GetBool("Run") == false)
        {
            animator.SetBool("Run", true);
        }
        BattleUnit.EnterGrid(gridUnit);
        transform.DOMove(gridUnit.LocalPosition, 0.5f).OnComplete(()=>
        {
            BattleUnit.BattleUnitState = BattleUnitState.Wait;
            if (stopMoveAnim)
            {
                animator.SetBool("Run", false);
            }
        });
    }

    public void AttackToBattleUnit(BattleUnit targetUnit, float coldTime)
    {
        foreach (Transform child in transform)
        {
            child.LookAt(targetUnit.BattleUnitRenderer.transform);
        }
        animator.SetBool("Run", false);
        //在平A冷却时间内可以把动画播放完
        if (attackAnimationTime < coldTime)
        {
            animator.SetFloat("AttackPlaySpeed", 1.0f);
        }
        else
        {
            animator.SetFloat("AttackPlaySpeed", attackAnimationTime / coldTime);
        }
        
        animator.SetTrigger("Attack");
        StartCoroutine(WaitColdTime(coldTime));
    }

    private IEnumerator WaitColdTime(float coldTime)
    {
        yield return new WaitForSeconds(coldTime);
        BattleUnit.BattleUnitState = BattleUnitState.Wait;
    }

    public void UpdatePositionByGrid(GridUnit gridUnit)
    {
        if (BattleUnit != null)
        {
            transform.localPosition = gridUnit.LocalPosition;
        }
    }

    public void OnConnect(BattleUnit data)
    {
        BattleUnit = data;
        if (BattleUnit != null)
        {
            transform.ShiftOut();
            gameObject.SetActive(true);
        }
    }

    public void OnDisconnect()
    {
        BattleUnit = null;
        transform.SetUnused(false, transform.name);
    }
    
    

//    public IEnumerator RunHeroAction(BattleUnitAction heroAction)
//    {
//        if (heroAction == null)
//            yield break;
//        
//        
//        if (heroAction.EnterBattleFieldAction != null)
//        {
//            yield return PlayEnterBattleFieldAction(heroAction.EnterBattleFieldAction);
//        }
//
//        if (heroAction.MotionAction != null)
//        {
//            yield return PlayMotionAction(heroAction.MotionAction);
//        }
//
//        yield return null;
//    }
//
//    private IEnumerator PlayEnterBattleFieldAction(BattleUnitEnterBattleFieldAction heroAction)
//    {
//        if (heroAction == null)
//        {
//            UtilityHelper.LogError("播放进入战场过程失败，记录为空");
//            yield break;
//        }
//        
//        UpdatePositionByGrid(heroAction.BornGrid);
//        RefreshAttribute(heroAction.Attribute);
//    }
//
//    private IEnumerator PlayMotionAction(BattleUnitMotionAction heroAction)
//    {
//        if (heroAction == null)
//        {
//            UtilityHelper.LogError("错误的移动指令");
//            yield break;
//        }
//
//        for (int i = 0; i < heroAction.GridPath.Count; i++)
//        {
//            UpdatePositionByGrid(heroAction.GridPath[i]);
//            yield return GameConst.WaitForDotOneSecond;
//        }
//    }
//
//    /// <summary>
//    /// 刷新值显示
//    /// </summary>
//    /// <param name="heroActionAttribute"></param>
//    private void RefreshAttribute(BattleUnitSyncAttribute heroActionAttribute)
//    {
//        if (BattleUnit == null || heroActionAttribute == null)
//        {
//            return;
//        }
//        
//    }
}
