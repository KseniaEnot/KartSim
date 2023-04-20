using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timer;

    private CustomTimeManager _timeManager;

    private void Start()
    {
        _timeManager = FindObjectOfType<CustomTimeManager>();
        
    }

    private void Update()
    {
        timer.text = secondsToString(_timeManager.currentTime);
    }

    private string secondsToString(float seconds)
    {
        int time = Mathf.CeilToInt(seconds);
        return string.Format("{0:00}:{1:00}", time / 60, time % 60);
    }
}
