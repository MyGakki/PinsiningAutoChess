using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIViewTopologicalMap: UIViewBase
{
    [SerializeField] private RectTransform mapRect;
    [SerializeField] private Toggle switchToggle;
    [SerializeField] private RectTransform switchImageRect;

    protected override void InitUIObjects()
    {
        base.InitUIObjects();

        switchToggle.onValueChanged.AddListener(OnSwitchChanged);
    }

    private void OnSwitchChanged(bool isOn)
    {
        if (isOn)
        {
            mapRect.DOAnchorPos(new Vector2(0, mapRect.anchoredPosition.y - mapRect.sizeDelta.y), 1.0f);
        }
        else
        {
            mapRect.DOAnchorPos(new Vector2(0, mapRect.anchoredPosition.y + mapRect.sizeDelta.y), 1.0f);
        }
        
        switchImageRect.localScale = new Vector3(1, -switchImageRect.localScale.y, 1);
    }
}
