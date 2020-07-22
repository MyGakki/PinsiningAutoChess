using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpMpBar : MonoBehaviour
{
    public Slider HpBar;
    public Slider MpBar;

    public void SetHpBar(float value)
    {
        HpBar.value = value;
    }

    public void SetMpBar(float value)
    {
        MpBar.value = value;
    }
}
