using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using TMPro;

public class GameOverScreen : MonoBehaviour
{
    // Dependencies
    [SerializeField] private TMP_Text scoreOutput;
    [SerializeField] private TMP_Text timeOutput;
    [SerializeField] private TMP_Text bestScoreOutput;
    [SerializeField] private TMP_Text bestSecondsPlayedOutput;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("Game");
        }
    }

    public void RecordStats(float secondsPlayed, int finalScore)
    {
        int minutes = (int)(secondsPlayed / 60);
        float remainder = secondsPlayed % 60;
        int seconds = (int)remainder;
        int thirds = (int)(remainder % 60);

        scoreOutput.text = finalScore.ToString();
        timeOutput.text = $"{minutes.ToString("00")}:{seconds.ToString("00")}:{thirds.ToString("00")}";

        SaveStats(secondsPlayed, finalScore);
    }

    public void SaveStats(float secondsPlayed, int finalScore)
    {
        int bestScore = PlayerPrefs.GetInt("bestScore", 0);

        if (finalScore > bestScore) {
            PlayerPrefs.SetInt("bestScore", finalScore);
            bestScore = finalScore;
        }

        float bestSecondsPlayed = PlayerPrefs.GetFloat("bestSecondsPlayed", 0);
        
        if (secondsPlayed > bestSecondsPlayed) {
            PlayerPrefs.SetFloat("bestSecondsPlayed", secondsPlayed);
            bestSecondsPlayed = secondsPlayed;
        }

        int minutes = (int)(bestSecondsPlayed / 60);
        float remainder = bestSecondsPlayed % 60;
        int seconds = (int)remainder;
        int thirds = (int)(remainder % 60);

        bestScoreOutput.text = bestScore.ToString();
        bestSecondsPlayedOutput.text = $"{minutes.ToString("00")}:{seconds.ToString("00")}:{thirds.ToString("00")}";
    }
}
