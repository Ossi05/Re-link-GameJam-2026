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
    [SerializeField] float flashSpeed = 3f;

    Color32 defaultColor;
    bool isFlashingDanger = false;

    void Awake()
    {
        defaultColor = oxygenFillImage.color;
    }
    void Start()
    {
        oxygen.OnOxygenChanged += Oxygen_OnOxygenChanged;
        UpdateUI();
    }

    void Update()
    {
        if (isFlashingDanger)
        {
            float t = Mathf.PingPong(Time.time * flashSpeed, 1f);
            oxygenFillImage.color = Color.Lerp(dangerLevelColor, warningLevelColor, t);
        }
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
            isFlashingDanger = true;
        }
        else if (oxygenLevel <= warningLevelThreshold)
        {
            oxygenFillImage.color = warningLevelColor;
            isFlashingDanger = false;
        }
        else
        {
            oxygenFillImage.color = defaultColor;
            isFlashingDanger = false;
        }
    }

}
