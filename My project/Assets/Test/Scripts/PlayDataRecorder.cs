 using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.MLAgents.Policies;
using UnityEngine;

public class PlayDataRecorder : MonoBehaviour
{
    [SerializeField] private string folderName;
    
    private StreamWriter writer;
    private StringBuilder builder;

    private CustomTimeManager timeManager;
    private string divider = ";";

    private void Start()
    {
        timeManager = FindObjectOfType<CustomTimeManager>();
        string folderPath = "PlayThroughData/" + folderName;
        string filePath = folderPath + "/" + "1" + ".csv";
        FileInfo file = new FileInfo(filePath);
        file.Directory.Create();
        
        int fCount = Directory.GetFiles(folderPath, "*", SearchOption.TopDirectoryOnly).Length +1;
        filePath = folderPath + "/" + fCount + ".csv";

        writer = new StreamWriter(filePath);
        builder = new StringBuilder();
    }

    public void CheckpointReached(int checkpointNumber, bool isTrainig = false)
    {
        float time = timeManager.currentTime;

        if (isTrainig)
        {
            builder.Append(checkpointNumber + divider + time + divider);
        }
        else
        {
            writer.WriteLine(checkpointNumber + divider + time + divider);
        }
    }

    public void EndTrainigEpisode()
    {
        if (builder != null)
        {
            writer.WriteLine(builder.ToString());
            builder.Clear();
        }
    }

    private void OnApplicationQuit()
    {
        writer.Flush();
        writer.Close();
    }
}
