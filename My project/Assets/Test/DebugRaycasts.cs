using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Policies;
using UnityEngine;

public class DebugRaycasts : MonoBehaviour
{
    [SerializeField] private GameObject defaultSensor;
    [SerializeField] private float sensorRayDistance;
    [SerializeField] private float hitDistance;
    //[SerializeField] private int rayNums = 9;

    private void Update()
    {
        int sensorLength = GetComponent<BehaviorParameters>().BrainParameters.VectorObservationSize - 3;
        float stepRotation = 180 / sensorLength;
        var cTransform = defaultSensor.GetComponent<Transform>();
        cTransform.localRotation = Quaternion.Euler(0, -90, 0);
        for (var i = 0; i <= sensorLength; i++)
        {
            Debug.DrawRay(cTransform.position, cTransform.forward * sensorRayDistance, Color.blue, 1);
            Debug.DrawRay(cTransform.position, cTransform.forward * hitDistance, Color.red, 1);
            cTransform.Rotate(Vector3.up, stepRotation);
        }

    }
}
