using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using INFOPluseTest.Models;
using NLog;

namespace INFOPluseTest.Service;

public class MatchBetService
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public List<MatchRecord> MatchBetDataList { get; private set; }
    
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

    public List<(string,decimal)> GetTop3TotalBetAmount()
    {
        //TODO:題目二移植
        return null;
    }
}
