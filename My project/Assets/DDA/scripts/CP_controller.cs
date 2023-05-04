using System.Collections.Generic;
using UnityEngine;

public class CP_controller : MonoBehaviour
{
    private float currentLapStartTime;
    private int dot;
    private DDA_maneger dda;
    [SerializeField]
    private List<GameObject> placeToSpawn;
    private List<GameObject> baf;
    private List<GameObject> baf_spawned;

    void Start()
    {
        dot = -1;
        currentLapStartTime = 0;
        dda = gameObject.GetComponent<DDA_maneger>();
        baf_spawned = new List<GameObject>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            float time;
            if ((dot == -1)) //������ ��������
            {
                dot++;
                currentLapStartTime = Time.time;
                time = Time.time - currentLapStartTime;
                return;
            }
            time = Time.time - currentLapStartTime;
            currentLapStartTime = Time.time; //���������� ���������
            foreach (GameObject item in baf_spawned)  //����������� ���������� ��������
            {
                Destroy(item);
            }
            baf_spawned.Clear();
            baf = dda.balanceDifficult(dot, time);  //��������� �������� ��� ���������
            dot++;
            for (int i = 0; i < baf.Count; i++)  //��������� ��������
            {
                baf_spawned.Add(Instantiate(baf[i], placeToSpawn[i].transform));
            }
        }
    }
}
