using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayingTimerUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeLeftText;
    [SerializeField] Image timerImage;

    void Update()
    {
        timerImage.fillAmount = GameManager.Instance.GetGamePlayingTimerNormalized();
        timeLeftText.text = GetTimeLeftText();
    }

    string GetTimeLeftText()
    {
        float timeLeft = GameManager.Instance.GetTimeLeft();
        int minutes = Mathf.FloorToInt(timeLeft / 60f);
        int seconds = Mathf.FloorToInt(timeLeft % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
}
