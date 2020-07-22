using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum DamageType
{
    Physical,
    Magical,
    Actual,
}

public class UIViewDamageFigure: UIViewBase
{
    [SerializeField] private GameObject figure;
    
    public void CreateFigure(int damage, DamageType type, bool isCritical, Vector3 position)
    {
        GameObject figureObj = Instantiate(figure, transform);
        figureObj.SetActive(true);
        Vector2 pos = UIViewManager.Instance.ConvertWorldPositionToRootCanvasPosition(position);
        Vector2 createPos = new Vector2(pos.x, pos.y + 50f);
        RectTransform figureRect = figureObj.GetComponent<RectTransform>();
        figureRect.anchoredPosition = createPos;
        TextMeshProUGUI damageText = figureRect.GetComponentInChildren<TextMeshProUGUI>();
        damageText.text = damage.ToString();
        switch (type)
        {
            case DamageType.Physical:
                damageText.color = new Color(1.0f, 0.5f, 0f);    
                break;
            case DamageType.Magical:
                damageText.color = Color.cyan;
                break;
            case DamageType.Actual:
                damageText.color = Color.grey;
                break;
        }

        if (isCritical)
        {
            figureRect.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }
        else
        {
            figureRect.GetComponentInChildren<Image>().gameObject.SetActive(false);
        }

        float randomAngle = Random.Range(0f, Mathf.PI);
        figureRect.DOAnchorPos(
                new Vector2(createPos.x + Mathf.Cos(randomAngle) * 50, createPos.y + Mathf.Sin(randomAngle) * 50), 1.5f)
            .OnComplete(()=>Destroy(figureRect.gameObject));
    }
}
