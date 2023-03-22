using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataRecorder
{
    public DataRecorder()
    {

    }

    public void BeginDataSaver(int episodeNumber, float time)
    {
        Debug.Log("New episode №" + episodeNumber + ". Time since game start:" + time);

    }

    public void RecordReward(RewardType type, float amount, float time)
    {

    }

    public void EndDataSaver(int step, float reward)
    {

    }
}

[System.Serializable]
public class Rewards
{
    public float hitPunishment = -1f;
    public float checkpointReward = 1f;
    public float rightDirectionReward;
    public float speedReward;
    public float accelerationReward;
}

public enum RewardType
{
    Checkpoint,
    Hit,
    Direction,
    Speed,
    Acceleration
}

