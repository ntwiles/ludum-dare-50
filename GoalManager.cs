using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    // Dependencies
    [SerializeField] private List<Goal> goals;
    [SerializeField] private AudioClip goalSound;

    // Components
    private AudioSource audio;

    // Settings
    [SerializeField] private float spawnDelay;

    // State
    int currentGoal;
    int nextGoal;

    void Awake()
    {
        currentGoal = -1;

        audio = GetComponent<AudioSource>();
    }

    void Start()
    {
        Goal.GoalSunk.AddListener(OnGoalSunk);

        currentGoal = chooseRandomGoal();
        StartCoroutine(waitThenActivateCurrent());
    }

    public void OnGoalSunk(int _points)
    {
        audio.PlayOneShot(goalSound, 1f);

        currentGoal = nextGoal;
        StartCoroutine(waitThenActivateCurrent());
    }

    private IEnumerator waitThenHintNext()
    {
        yield return new WaitForSeconds(spawnDelay);
        goals[nextGoal].Hint();
    }

    private IEnumerator waitThenActivateCurrent()
    {
        yield return new WaitForSeconds(spawnDelay);
        goals[currentGoal].Activate();
    
        nextGoal = chooseRandomGoal();
        goals[nextGoal].Hint();
    }

    private int chooseRandomGoal()
    {
        int next;

        do { next = Random.Range(0, goals.Count); } 
        while (next == currentGoal);

        return next;
    }
}
