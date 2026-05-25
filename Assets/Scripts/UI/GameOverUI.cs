using TMPro;
using UnityEngine;
public class GameOverUI : Singleton<GameOverUI>
{
    [SerializeField] TextMeshProUGUI gameresultText;

    void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        Hide();
    }

    void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsGameOver())
        {
            if (FadeTransition.Instance != null)
            {
                FadeTransition.Instance.FadeInToOutOnGameOver();
            }
            else
            {
                HandleGameOver();
            }
        }
    }

    public void HandleGameOver()
    {
        if (!GameManager.Instance.IsGameOver()) { return; }
        SetGameResultText();
        Show();
    }

    void SetGameResultText()
    {
        int numCapsulesAlive = LifeSupportHub.Instance.GetNumCapsulesAlive();

        if (numCapsulesAlive == 0)
        {
            gameresultText.text = "You Lost! All crew members died";
            return;
        }
        gameresultText.text = $"You kept {numCapsulesAlive} / {LifeSupportHub.Instance.GetTotalCapsuleAmt()} crew members alive";
    }

    void Show()
    {
        gameObject.SetActive(true);
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }

}
