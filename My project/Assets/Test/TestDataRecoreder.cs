using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDataRecoreder : MonoBehaviour
{
    void Start()
    {
        var test = new DataRecorder();

        for (int i = 0; i < 10000; i++)
        {
            test.BeginDataSaver(i, Time.realtimeSinceStartup);
            test.RecordReward(RewardType.Direction, -0.5f, Time.realtimeSinceStartup);
            test.RecordReward(RewardType.Checkpoint, 1f, Time.realtimeSinceStartup);
            test.RecordReward(RewardType.Speed, 0.03f, Time.realtimeSinceStartup);
            test.EndDataSaver(i, 0.53f);
        }
        
    }
}
