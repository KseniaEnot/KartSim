using KartGame.KartSystems;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;

[RequireComponent(typeof(ArcadeKart))]
public class DrivingAgent : Agent, IInput
{
    [SerializeField] private int maxStep = 5000;
    [SerializeField] private bool isTrainig = true;
    [SerializeField] private Collider[] checkpointColliders;
    [SerializeField] private GameObject defaultSensor;
    [Tooltip("What objects should the raycasts hit and detect?")]
    [SerializeField] private LayerMask mask;
    [SerializeField] private float sensorRayDistance;
    [SerializeField] private float hitDistance;

    [Header("Rewards")]
    [SerializeField] Rewards rewards;
    [SerializeField] string outputFile = "0";

    private ArcadeKart kart;
    private InputData inputData;
    private int currentCheckpoint = 0;
    private float currentReward;
    private bool isEpisodeEnd = false;
    private bool maxStepReached = false;

    private DataRecorder recorder;

    private void Awake()
    {
        kart = GetComponent<ArcadeKart>();
        recorder = new DataRecorder(outputFile);
    }

    private void Start() => OnEpisodeBegin();

    void Update()
    {
        if (isEpisodeEnd)
        {
            isEpisodeEnd = false;
            AddReward(currentReward);
            recorder.EndDataSaver(StepCount, GetCumulativeReward());
            EndEpisode();
            OnEpisodeBegin();
        }
        if (maxStepReached)
        {
            maxStepReached = false;
            AddReward(currentReward); 
            recorder.EndDataSaver(StepCount, GetCumulativeReward());
            EpisodeInterrupted();
            OnEpisodeBegin();
        }
    }

    public override void OnEpisodeBegin()
    {
        recorder.BeginDataSaver(CompletedEpisodes + 1, Time.realtimeSinceStartup);
        if (isTrainig)
        {
            GetComponent<PlayDataRecorder>().EndTrainigEpisode();
            currentCheckpoint = Random.Range(0, checkpointColliders.Length - 1);
            Collider start = checkpointColliders[currentCheckpoint];

            transform.localRotation = start.transform.rotation;
            transform.position = start.transform.position;
            kart.Rigidbody.velocity = default;

            inputData = new InputData();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(kart.LocalSpeed());
        sensor.AddObservation(Vector3.Dot(kart.Rigidbody.velocity.normalized, NextColliderDirection()));
        sensor.AddObservation(inputData.Accelerate);

        isEpisodeEnd = false;
        currentReward = 0f;

        int sensorLength = GetComponent<BehaviorParameters>().BrainParameters.VectorObservationSize - 3;
        float stepRotation = 180 / sensorLength;
        var cTransform = defaultSensor.GetComponent<Transform>();
        cTransform.localRotation = Quaternion.Euler(0, -90, 0);
        for (var i = 0; i < sensorLength; i++)
        {
            //Debug.Log("Sensor rotation " + cTransform.localRotation);
            var hit = Physics.Raycast(transform.position, cTransform.forward, out var hitInfo,
                sensorRayDistance, mask, QueryTriggerInteraction.Ignore);

            if (hit)
            {
                if (hitInfo.distance < hitDistance)
                {
                    Debug.Log("Hit: " + hitInfo.collider.gameObject.name);
                    GetComponent<PlayDataRecorder>().WasHit(isTrainig);
                    recorder.RecordReward(RewardType.Hit, rewards.hitPunishment, Time.realtimeSinceStartup);
                    currentReward += rewards.hitPunishment;
                    isEpisodeEnd = true;
                }
            }

            sensor.AddObservation(hit ? hitInfo.distance : sensorRayDistance);
            cTransform.Rotate(Vector3.up, stepRotation);
        }

        if (StepCount >= MaxStep) maxStepReached = true;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);
        //get input
        inputData.TurnInput = actions.DiscreteActions[0] - 1f;
        inputData.Accelerate = actions.DiscreteActions[1] >= 1.0f;
        //inputData.Accelerate = true;
       /*if (actions.ContinuousActions[0] >= 0.33f) inputData.TurnInput = 1f;
        else if (actions.ContinuousActions[0] <= 0.33f) inputData.TurnInput = -1f;
        else inputData.TurnInput = 0f;
        if (actions.ContinuousActions[0] >= 0) inputData.Accelerate = true;
        else inputData.Accelerate = false;*/
            
        
       inputData.Brake = !inputData.Accelerate;

        //scalar multiplier - negative if we're moving in wrong sirection
        float reward = Vector3.Dot(kart.Rigidbody.velocity.normalized, NextColliderDirection());

        //if (ShowRaycasts) Debug.DrawRay(AgentSensorTransform.position, m_Kart.Rigidbody.velocity, Color.blue);

        //rewards
        AddReward(reward * rewards.rightDirectionReward);
        recorder.RecordReward(RewardType.Direction, reward * rewards.rightDirectionReward, Time.realtimeSinceStartup);
        //AddReward((inputData.Accelerate ? 1.0f : 0.0f) * accelerationReward);
        AddReward(kart.LocalSpeed() * rewards.speedReward);
        recorder.RecordReward(RewardType.Speed, kart.LocalSpeed() * rewards.speedReward, Time.realtimeSinceStartup);
    }

    private Vector3 NextColliderDirection()
    {
        Collider nextCollider = checkpointColliders[(currentCheckpoint + 1) % checkpointColliders.Length];
        Vector3 colliderDirection = (nextCollider.transform.position - kart.transform.position).normalized;

        return colliderDirection;
    }

    private void OnTriggerEnter(Collider other)
    {
        int index = -1;
        for (int i = 0; i < checkpointColliders.Length; i++)
            if (checkpointColliders[i].GetInstanceID() == other.GetInstanceID())
                index = i;

        Debug.Log("Index current " + currentCheckpoint + "index triggered " + index);

        if (index == currentCheckpoint + 1 || index == 0 && currentCheckpoint == checkpointColliders.Length - 1)
        {
            AddReward(rewards.checkpointReward);
            recorder.RecordReward(RewardType.Checkpoint, rewards.checkpointReward, Time.realtimeSinceStartup);
            currentCheckpoint = index;
            GetComponent<PlayDataRecorder>().CheckpointReached(currentCheckpoint, isTrainig);
        }
    }

    public InputData GenerateInput() => inputData;
    private void OnApplicationQuit() => recorder.EndGame();
}
