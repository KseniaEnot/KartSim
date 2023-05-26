using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTimeManager : MonoBehaviour
{
    public float currentTime { get; private set; }

    private void Awake()
    {
        currentTime = 0;
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
    }
}
