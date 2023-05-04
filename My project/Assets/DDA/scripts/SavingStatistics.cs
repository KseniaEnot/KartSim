using System.IO;
using System.Text;

public class SavingStatistics
{
    private string path;
    private StringBuilder csv;
    public SavingStatistics(string _path)
    {
        path=_path;
        csv = new StringBuilder();
    }

    public void AddStatistics(int CPNumb, float RealValue, float DesiredValue, float CoefficientsSum,int levl=1)
    {
        if (!File.Exists(path))
        {
            var newLine_ = string.Format("levl; CPNumb; RealValue; DesiredValue; CoefficientsSum");
            File.Create(path).Dispose();
            csv.AppendLine(newLine_);
        }
        var newLine = string.Format("{0}; {1}; {2}; {3}; {4}", levl, CPNumb, RealValue, DesiredValue, CoefficientsSum);
        csv.AppendLine(newLine);
    }

    public void SaveAll()
    {
        File.AppendAllText(path, csv.ToString());
        csv.Clear();
    }
}
