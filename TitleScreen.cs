using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    // Settings
    [SerializeField] private float hangDuration;

    // State
    private float hangTime = 0;
    void Start()
    {
        
    }

    void Update()
    {
        hangTime += Time.deltaTime;

        if (hangTime > hangDuration)
        {
            SceneManager.LoadScene("Tutorial");
        }
    }
}
