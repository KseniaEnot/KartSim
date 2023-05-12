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
    float speedRev = 0f;
    float dirRew = 0f;

    public DataRecorder(string fileName = "INeedAName")
    {
        writer = new StreamWriter(fileName + ".csv");
        writer.AutoFlush = true;
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
        if (episodeNumber % 500 == 0) writer.Flush();
        if (speedRev != 0)
        {
            EndDataSaver(5000, 0);
        }
        builder.Clear();
        builder.Append(episodeNumber + divider + time + divider);
        foreach (RewardType type in (RewardType[])Enum.GetValues(typeof(RewardType)))
        {
            rewardRecords[type].StartBuild();
        }
    }

    public void RecordReward(RewardType type, float amount, float time)
    {
        if (type == RewardType.Speed) speedRev += amount;
        else if (type == RewardType.Direction) dirRew += amount;
        else rewardRecords[type].Record(amount.ToString() + " + ");
        //builder.Append(amount + " ");
    }

    public void EndDataSaver(int step, float reward)
    {
        foreach (RewardType type in (RewardType[])Enum.GetValues(typeof(RewardType)))
        {
            if (type == RewardType.Speed) builder.Append(speedRev + divider);
            else if (type == RewardType.Direction) builder.Append(dirRew + divider);
            else builder.Append(rewardRecords[type].StrBuilder + divider);
        }
        builder.Append(step + divider + reward);
        writer.WriteLine(builder);
        writer.Flush();
        builder.Clear();
        speedRev = 0;
        dirRew = 0;
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

