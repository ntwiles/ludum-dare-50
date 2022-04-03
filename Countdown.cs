using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class Countdown : MonoBehaviour
{
    // Dependencies
    [SerializeField] private GameController game;

    // Components
    private TMP_Text display;

    // Settings
    [SerializeField] private float startingSeconds;
    [SerializeField] private float goalBonus;
    
    // State
    private float timeRemaining;
    private bool finished = false;
    private bool begun = false;

    private void Awake()
    {
        timeRemaining = startingSeconds;
        display = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        Goal.GoalSunk.AddListener(OnGoalSunk);
    }

    public void Begin()
    {
        begun = true;
    }

    private void Update()
    {
        if (begun && !finished)
        {

            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0) {
                timeRemaining = 0;
                finished = true;

                game.EndGame();
            }
        }
    }

    private void FixedUpdate()
    {
        int secondsRemaining = (int)Mathf.Floor(timeRemaining);
        float thirdsRemaining =  (timeRemaining - (float)secondsRemaining) * 0.6f;
        display.text = $"{secondsRemaining.ToString("00")}:{thirdsRemaining.ToString("#.00").Replace(".", "")}";
    }

    public void OnGoalSunk(int _points)
    {
        timeRemaining += goalBonus;
    }
}
