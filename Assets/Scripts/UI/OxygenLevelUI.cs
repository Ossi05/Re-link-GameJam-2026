using System;
using UnityEngine;
using UnityEngine.UI;

public class OxygenLevelUI : MonoBehaviour
{
    [SerializeField] Oxygen oxygen;
    [SerializeField] Image oxygenFillImage;

    [SerializeField] Color32 dangerLevelColor = Color.red;
    [SerializeField] Color32 warningLevelColor = Color.orange;
    [SerializeField] float warningLevelThreshold = 0.5f;
    [SerializeField] float dangerLevelThreshold = 0.25f;

    Color32 defaultColor;
    void Awake()
    {
        defaultColor = oxygenFillImage.color;
    }
    void Start()
    {
        oxygen.OnOxygenChanged += Oxygen_OnOxygenChanged;
        UpdateUI();
    }

    void Oxygen_OnOxygenChanged(object sender, EventArgs e)
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        float oxygenLevel = oxygen.GetOxygenLevel();
        oxygenFillImage.fillAmount = oxygenLevel;
        if (oxygenLevel <= dangerLevelThreshold)
        {
            oxygenFillImage.color = dangerLevelColor;
        }
        else if (oxygenLevel <= warningLevelThreshold)
        {
            oxygenFillImage.color = warningLevelColor;
        }
        else
        {
            oxygenFillImage.color = defaultColor;
        }
    }

}
