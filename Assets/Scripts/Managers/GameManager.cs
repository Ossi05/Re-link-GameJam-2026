using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;
    public event EventHandler OnGameStarted;

    [SerializeField] float countDownToStartTimer = 3f;
    [SerializeField] float maxGamePlayingTimeInSeconds = 180f;
    [SerializeField] float gameEndDelayOnVictory = 5f;

    state currentState;

    bool gameWon = false;
    float gamePlayingTimer = 0f;
    bool gamePaused;
    bool startCountdown;

    enum state
    {
        WaitingToStart,
        CountDownToStart,
        Playing,
        GameOver,
    }

    void Awake()
    {
        Time.timeScale = 1f;
        Instance = this;
    }

    void Start()
    {
        PlayerControls.Instance.OnGamePauseAction += PlayerControls_OnGamePausedAction;
        LifeSupportHub.Instance.OnAllCapsulesDied += LifeSupportHub_OnAllCapsulesDied;
    }

    void LifeSupportHub_OnAllCapsulesDied(object sender, EventArgs e)
    {
        HandleGameLose();
    }

    void PlayerControls_OnGamePausedAction(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    void HandleGameLose()
    {
        gameWon = false;
        currentState = state.GameOver;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    IEnumerator GameEndDelay()
    {
        yield return new WaitForSeconds(gameEndDelayOnVictory);
        currentState = state.GameOver;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }


    void Update()
    {
        switch (currentState)
        {
            case state.WaitingToStart:

                currentState = state.Playing;
                OnGameStarted?.Invoke(this, EventArgs.Empty);

                if (startCountdown)
                {
                    currentState = state.CountDownToStart;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case state.CountDownToStart:
                countDownToStartTimer -= Time.deltaTime;
                if (countDownToStartTimer <= 0)
                {
                    currentState = state.Playing;
                    OnGameStarted?.Invoke(this, EventArgs.Empty);
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case state.Playing:
                gamePlayingTimer += Time.deltaTime;
                if (gamePlayingTimer >= maxGamePlayingTimeInSeconds)
                {
                    if (gameWon) return;
                    gameWon = true;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                    StartCoroutine(GameEndDelay());
                }
                break;
            case state.GameOver:
                break;
        }
    }

    public bool IsGameOver()
    {
        return currentState == state.GameOver;
    }

    public bool IsGameWon()
    {
        return gameWon;
    }


    public float GetTimeLeft()
    {
        if (gamePlayingTimer >= maxGamePlayingTimeInSeconds) return 0f;
        return maxGamePlayingTimeInSeconds - gamePlayingTimer;
    }

    public float GetGamePlayingTimerNormalized()
    {
        if (gamePlayingTimer >= maxGamePlayingTimeInSeconds) return 0f;
        return 1 - (gamePlayingTimer / maxGamePlayingTimeInSeconds);
    }

    public bool IsGamePlaying()
    {
        return currentState == state.Playing;
    }

    public bool IsCountDownToStart()
    {
        return currentState == state.CountDownToStart;
    }

    public float GetCountDownToStartTimer()
    {
        return countDownToStartTimer;
    }

    public void StartCountDown()
    {
        startCountdown = true;
    }

    public void TogglePauseGame()
    {
        if (IsGameOver() && !gamePaused)
        {
            return;
        }

        if (currentState == state.WaitingToStart)
        {
            StartCountDown();
            return;
        }

        gamePaused = !gamePaused;

        if (gamePaused)
        {
            Time.timeScale = 0f;
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;
            OnGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    void OnDestroy()
    {
        PlayerControls.Instance.OnGamePauseAction -= PlayerControls_OnGamePausedAction;
    }
}
