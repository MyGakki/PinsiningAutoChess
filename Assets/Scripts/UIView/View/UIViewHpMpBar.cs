using UnityEngine;

public class UIViewHpMpBar: UIViewBase
{
    [SerializeField] private GameObject barBackUp;

    protected override void InitUIObjects()
    {
        base.InitUIObjects();
        
        barBackUp.SetActive(false);
    }

    public HpMpBar CreateHpMpBar()
    {
        GameObject hpMpBarRect = Instantiate(barBackUp, transform);
        hpMpBarRect.gameObject.SetActive(true);
        return hpMpBarRect.GetComponent<HpMpBar>();
    }
}
