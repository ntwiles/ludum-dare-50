using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]

public class Ball : MonoBehaviour
{
    // Dependencies
    [SerializeField] private List<AudioClip> bounceSounds;

    // Components
    private AudioSource audio;
    private Rigidbody2D rigidbody;

    // Settings
    [SerializeField] private float bounceSoundThreshold;

    // State
    int bounces = 0;

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public void ResetBounces()
    {
        bounces = 0;
    }

    void OnCollisionEnter2D(Collision2D other) 
    {
        bounces++;

        if (rigidbody.velocity.magnitude > bounceSoundThreshold)
        {
            var clip = bounceSounds[Random.Range(0, bounceSounds.Count)];
            audio.PlayOneShot(clip, 1f);
        }
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        var goal = other.gameObject.GetComponent<Goal>();

        if (goal != null) 
        {
            goal.SinkGoal(bounces);
        }
    }
}
