using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;

public class DataRecorder
{
    private StreamWriter writer;
    private StringBuilder builder;

    string RewardsTitle = "";
    Dictionary<RewardType, RewardRecorder> rewardRecords;

    string divider = ";";

    public DataRecorder()
    {
        writer = new StreamWriter("INeedAName.csv"); 
        builder = new StringBuilder();
        rewardRecords = new Dictionary<RewardType, RewardRecorder>();
        foreach(RewardType type in (RewardType[]) Enum.GetValues(typeof(RewardType)))
        {
            RewardRecorder recorder = new RewardRecorder();
            rewardRecords.Add(type, recorder);
            RewardsTitle += Enum.GetName(typeof(RewardType), type) + divider;
        }

        writer.WriteLine("Episode number;Time;" + RewardsTitle + "Step;CumulativeRew");
    }

    public void BeginDataSaver(int episodeNumber, float time)
    {
        Debug.Log("New episode №" + episodeNumber + ". Time since game start:" + time);
        //if (episodeNumber % 10 == 0) BackupCopy();
        builder.Clear();
        builder.Append(episodeNumber + divider + time + divider);
        foreach (RewardType type in (RewardType[])Enum.GetValues(typeof(RewardType)))
        {
            rewardRecords[type].StartBuild();
        }
    }

    public void RecordReward(RewardType type, float amount, float time)
    {
        rewardRecords[type].Record(amount.ToString() + " ");
        //builder.Append(amount + " ");
    }

    public void EndDataSaver(int step, float reward)
    {
        foreach (RewardType type in (RewardType[])Enum.GetValues(typeof(RewardType)))
        {
            builder.Append(rewardRecords[type].StrBuilder + divider);
        }
        builder.Append(step + divider + reward);
        writer.WriteLine(builder);
    }

    public void EndGame()
    {
        writer.Flush();
        writer.Close();
    }
}

public class RewardRecorder
{
    StringBuilder builder;

    public string StrBuilder
    {
        get { return builder.ToString(); }
    }

    public RewardRecorder() =>
        builder = new StringBuilder();

    public void StartBuild()
    {
        builder.Clear();
    }

    public void Record(string record)
    {
        builder.Append(record);
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

