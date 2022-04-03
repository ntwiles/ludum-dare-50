using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{
    // Dependencies
    [SerializeField] private GameOverScreen gameOverScreen;

    // Events
    public UnityEvent GameEnded;

    // State
    private float startTime;
    private int points;

    private void Awake() {
        startTime = Time.time;
        Goal.GoalSunk.AddListener(RegisterPoints);
    }

    public void RegisterPoints(int _points) 
    {
        points += _points;
    }

    public void EndGame()
    {
        GameEnded.Invoke();
        gameOverScreen.RecordStats(Time.time - startTime, points);
        gameOverScreen.gameObject.SetActive(true);
    }
}
