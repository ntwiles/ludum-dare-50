using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class Score : MonoBehaviour
{
    // Component
    private TMP_Text display;

    // State
    private int score = 0;

    private void Awake()
    {
        display = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        Goal.GoalSunk.AddListener(registerGoal);
    }

    private void registerGoal(int points)
    {
        score += points;
        display.text = score.ToString("00");
    }
}
