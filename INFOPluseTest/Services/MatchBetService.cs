using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using INFOPluseTest.Models;
using NLog;

namespace INFOPluseTest.Service;

public class MatchBetService
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public List<MatchRecord> MatchBetDataList { get;  set; }
    
    public MatchBetService()
    {
        MatchBetDataList = new List<MatchRecord>();
    }
    
    public void LoadData(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            Logger.Warn("指定路徑為空：{DataPath}", path);
            return;
        }

        if (!File.Exists(path))
        {
            Logger.Warn("指定的比賽資料檔案不存在：{DataPath}", path);
            return;
        }

        Logger.Debug("開始讀取比賽資料：{DataPath}", path);

        var skippedRecordCount = 0;
        var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            ReadingExceptionOccurred = args =>
            {
                skippedRecordCount++;

                var parser = args.Exception.Context?.Parser;
                Logger.Error(
                    args.Exception,
                    "略過格式錯誤的資料。CSV 列號：{RowNumber}，原始資料：{RawRecord}",
                    parser?.RawRow,
                    parser?.RawRecord?.TrimEnd() ?? string.Empty);

                return false;
            }
        };

        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, csvConfiguration);

        MatchBetDataList = csv
            .GetRecords<MatchRecord>()
            .Where(record => record is not null)
            .ToList();

        Logger.Info(
            "比賽資料讀取完成。成功：{SuccessCount} 筆，略過：{SkippedCount} 筆",
            MatchBetDataList.Count,
            skippedRecordCount);
    }

    public List<(string League, decimal TotalBetAmount)> GetTop3TotalBetAmount()
    {
        return MatchBetDataList
            .GroupBy(record => record.League)
            .Select(group => (
                League: group.Key,
                TotalBetAmount: group.Sum(record => record.BetAmount)))
            .OrderByDescending(result => result.TotalBetAmount)
            .ThenBy(result => result.League, StringComparer.Ordinal)
            .Take(3)
            .ToList();
    }

    public List<(string MatchKey, int Count, decimal TotalBetAmount)> GetduplicateMatches()
    {
        return MatchBetDataList
            .GroupBy(record => new
            {
                record.League,
                record.HomeTeam,
                record.AwayTeam,
                record.HomeScore,
                record.AwayScore
            })
            .Where(group => group.Count() > 1)
            .Select(group => (
                MatchKey:
                    $"{group.Key.League} | " +
                    $"{group.Key.HomeTeam} vs {group.Key.AwayTeam} | " +
                    $"{group.Key.HomeScore}:{group.Key.AwayScore}",
                Count: group.Count(),
                TotalBetAmount: group.Sum(record => record.BetAmount)))
            .OrderByDescending(result => result.Count)
            .ThenByDescending(result => result.TotalBetAmount)
            .ThenBy(result => result.MatchKey, StringComparer.Ordinal)
            .ToList();
    }
}
