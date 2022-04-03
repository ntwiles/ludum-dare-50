using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class Goal : MonoBehaviour
{
    // Components
    private SpriteRenderer ring;

    // Settings
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color hintColor;
    [SerializeField] private Color sunkColor; 
    [SerializeField] private Color transparentColor;
    [SerializeField] private float flashDuration;
    [SerializeField] private float spinSpeedNormal;
    [SerializeField] private float spinSpeedFast;
    [SerializeField] private float hintScale;


    // State
    private bool active = false;
    private float startScale;
    private float currentSpinSpeed;

    private Queue<Action> effectQueue;
    private bool effectsLocked;

    // Events
    public static UnityEvent<int> GoalSunk;

    void Awake() 
    {
        if (GoalSunk == null)
        {
            GoalSunk = new UnityEvent<int>();
        }

        ring = GetComponent<SpriteRenderer>();

        startScale = transform.localScale.x;
        effectQueue = new Queue<Action>();
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, currentSpinSpeed * Time.deltaTime * -1));

        if (!effectsLocked && effectQueue.Count > 0) 
        {
            effectsLocked = true;
            effectQueue.Dequeue()();
        }
    }

    public void Hint()
    {        
        currentSpinSpeed = spinSpeedNormal;
        transform.localScale = new Vector3(hintScale, hintScale, hintScale);

        effectQueue.Enqueue(effectHint);
    }

    public void Activate()
    {
        active = true;

        currentSpinSpeed = spinSpeedNormal;
        transform.localScale = new Vector3(startScale, startScale, startScale);

        effectQueue.Enqueue(effectFadeIn);
    }

    public bool SinkGoal(int bounces)
    {
        if (active) 
        {
            int points = bounces < 1 ? 3 : 1;
            GoalSunk.Invoke(points);

            effectQueue.Enqueue(effectFlash);
            active = false;

            return true;
        } else {
            return false;
        }
    }

    private void effectHint()
    {
        ring.color = transparentColor;

        LeanTween
            .value( gameObject, updateGoalColor, transparentColor, hintColor, flashDuration)
            .setOnComplete(unlockEffects);
    }

    private void effectFadeIn()
    {
        LeanTween
            .value( gameObject, updateGoalColor, hintColor, defaultColor, flashDuration)
            .setEase(LeanTweenType.easeInCirc)
            .setOnComplete(unlockEffects);
    }

    private void effectFlash()
    {
        LeanTween
            .value( gameObject, updateGoalColor, defaultColor, sunkColor, flashDuration)
            .setEase(LeanTweenType.easeInCirc);

        // This is at present the longest tween, so it sets unlockEffects on complete.
        LeanTween
            .value( gameObject, updateScale, startScale, 0f, 0.35f)
            .setEase(LeanTweenType.easeInCirc)
            .setOnComplete(unlockEffects);

        LeanTween
            .value( gameObject, updateSpeed, spinSpeedNormal, spinSpeedFast, .001f)
            .setEase(LeanTweenType.easeInCirc);
    }

    void updateSpeed(float speed)
    {
        currentSpinSpeed = speed;
    }

    void updateScale(float scale) 
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }

    void updateGoalColor( Color val ){
        ring.color = val;
    }   

    void unlockEffects()
    {
        effectsLocked = false;
    }
}
