using System.Globalization;
using CsvHelper;
using INFOPluseTest.Models;

namespace INFOPluseTest.Service;

public class MatchBetService
{
    public List<MatchRecord> MatchBetDataList { get; set; } 
    
    public MatchBetService()
    {
        MatchBetDataList = new List<MatchRecord>();
    }
    
    public void LoadData(string path)
    {
        if (!File.Exists(path))
        {
            return;
        }

        using (var reader = new StreamReader(path))
        {
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                MatchBetDataList = csv.GetRecords<MatchRecord>().ToList();
            }
        }

    }

}
