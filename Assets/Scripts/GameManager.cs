using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI mineCounterText;

    private float elapsedTime = 0f;
    private bool gameRunning = false;
    private int totalMines;
    private int flagsPlaced = 0;

    public bool isGameOver = false;
    private int totalTiles;
    private int revealedTiles;

    public TextMeshProUGUI resultText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetTotalTiles(int total)
    {
        totalTiles = total;
        revealedTiles = 0;
    }

    public void RegisterReveal()
    {
        if (isGameOver) return;

        revealedTiles++;
        if (revealedTiles == totalTiles)
        {
            WinGame();
        }
    }

    public void StartGame(int mineCount)
    {
        isGameOver = false;
        gameRunning = true;
        elapsedTime = 0f;
        totalMines = mineCount;
        flagsPlaced = 0;
        UpdateMineCounter();
        resultText.gameObject.SetActive(false);
        StartCoroutine(UpdateTimer());
    }

    public void GameOver()
    {
        isGameOver = true;
        gameRunning = false; // stop timer
        resultText.text = "💥 Game Over!";
        resultText.gameObject.SetActive(true);
    }

    private void WinGame()
    {
        gameRunning = false;
        isGameOver = true;
        resultText.text = "🎉 You Win!";
        resultText.gameObject.SetActive(true);
    }

    private IEnumerator UpdateTimer()
    {
        while (gameRunning)
        {
            elapsedTime += Time.deltaTime;
            int seconds = Mathf.FloorToInt(elapsedTime);
            timerText.text = $"Time: {seconds}s";
            yield return null;
        }
    }

    private void UpdateMineCounter()
    { 
        int remaining = totalMines - flagsPlaced;
        mineCounterText.text = $"Mines: {remaining}";
    }

    public void UpdateFlags(int delta)
    {
        flagsPlaced += delta;
        UpdateMineCounter();
    }
}
