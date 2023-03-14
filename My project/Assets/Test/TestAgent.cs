using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class TestAgent : Agent
{
    [SerializeField] private Transform targetTransform;

    private Vector3 startPosition = Vector3.zero;
    private float movementSpeed = 6f;
    private float rotationSpeed = 20f;

    private void Awake()
    {
        startPosition = transform.position;
    }

    public override void OnEpisodeBegin()
    {
        transform.position = startPosition;
        transform.rotation = Quaternion.Euler(0, Random.Range(-180f, 180f), 0);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(transform.rotation);
        sensor.AddObservation(targetTransform.position);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveZ = actions.ContinuousActions[0];
        float rotate = actions.ContinuousActions[1];
        Debug.Log(moveZ);

        transform.localPosition += transform.forward * moveZ * movementSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, rotationSpeed * rotate * Time.deltaTime);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Vertical");
        continuousActions[1] = Input.GetAxisRaw("Horizontal");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Goal")
        {
            SetReward(1f);
            EndEpisode();
        }
        if (other.tag == "Wall")
        {
            SetReward(-1f);
            EndEpisode();
        }
    }
}
