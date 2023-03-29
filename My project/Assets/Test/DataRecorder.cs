using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.Json;
using System.IO;
using System.Text;

public class DataRecorder
{
    private StreamWriter writer;
    StringBuilder builder;
    public DataRecorder()
    {
        writer = new StreamWriter("INeedAName.csv");
        writer.WriteLine("Episode number;Time;Rewards;Step;CumulativeRew");
        builder = new StringBuilder();
    }

    public void BeginDataSaver(int episodeNumber, float time)
    {
        Debug.Log("New episode №" + episodeNumber + ". Time since game start:" + time);
        builder.Clear();
        builder.Append(episodeNumber + ";" + time +";");
    }

    public void RecordReward(RewardType type, float amount, float time)
    {
        builder.Append(amount + " ");
    }

    public void EndDataSaver(int step, float reward)
    {
        builder.Append(";" + step + ";" + reward);
        writer.WriteLine(builder);
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

