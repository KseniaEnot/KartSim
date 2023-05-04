using System.Collections.Generic;
using UnityEngine;

public class DDA_maneger : MonoBehaviour
{
    [SerializeField]
    int levl;
    [SerializeField]
    public List<float> BafCof; //коэффициенты бафов
    [SerializeField]
    public List<GameObject> BafObj; //объекты бафов
    [SerializeField]
    public List<int> CountPlace;  //количество мест для бафа на данном чекпоинте

    public List<float> desiredValue;  //ожидаемые значения на чекпоинтах
    private SavingStatistics savingStatistics;

    private void Start()
    {
        savingStatistics = new SavingStatistics("Statistics.csv");
    }

    public List<GameObject> balanceDifficult(int dotNamber, float realValue)
    {
        List<GameObject> returnObj = new List<GameObject>();
        List<int> returnObj_int = findItems(BafCof.Count, CountPlace[dotNamber], (realValue - desiredValue[dotNamber]));
        float sum = 0;

        foreach (var item in returnObj_int)
        {
            returnObj.Add(BafObj[item]);
            sum += BafCof[item];
        }
        savingStatistics.AddStatistics(dotNamber, realValue, desiredValue[dotNamber], sum, levl); //запись статистики
        savingStatistics.SaveAll();  //сохранение статистики
        return returnObj;
    }

    private List<int> findItems(int countCoef, int countDot, float differ)
    {
        countCoef = countCoef + 1;
        countDot = countDot + 1;
        float[,] A = new float[countCoef, countDot];
        for (int i = 0; i < countDot; i++)  //задание начальных значений
            A[0, i] = 0;
        for (int i = 0; i < countCoef; i++)
            A[i, 0] = 0;
        bool flag = false;
        int x = -1;
        int[,] paths = new int[countCoef, countDot];
        for (int i = 0; i < countCoef; i++)
            for (int j = 0; j < countDot; j++)
                paths[i, j] = -1;


        for (int coef = 1; (coef < countCoef) && !flag; coef++)//колво коэф. k=n
        {
            for (int place = 1; (place < countDot) && !flag; place++)//колво мест
            {
                A[coef, place] = A[coef - 1, place];  //берём предыдущий вес коэф.
                paths[coef, place] = paths[coef - 1, place]; //запись пути
                float difer1 = differ - A[coef, place];  //оставшееся расстояние до одного варианта пути
                int p = (coef - 1) % countCoef;  //вычисление номера коэф.
                float difer2 = differ - (A[coef, place - 1] + BafCof[p]); //оставшееся расстояние до другого варианта пути
                if (Mathf.Abs(difer1) > Mathf.Abs(difer2)) //новый коэф приближает к сумме
                {
                    A[coef, place] = A[coef, place - 1] + BafCof[p];
                    paths[coef, place] = p;
                }
                if (A[coef, place] == differ) //получили нужную сумму
                {
                    flag = true;
                    x = coef;
                }
            }
        }
        if (x == -1) x = countCoef - 1;
        List<int> usedCoef = new List<int>();
        for (int i = 0; i < countDot; i++)
            if (paths[x, i] != -1)
                usedCoef.Add(paths[x, i]);
        return usedCoef;
    }

}