using INFOPluseTest.Service;
using NLog;

namespace INFOPluseTest;

internal static class Program
{
    private const string DefaultDataFilename = "Folder/Data.csv";

    private static int Main(string[] args)
    {
        var configPath = Path.Combine(AppContext.BaseDirectory, "NLog.config");
        var logger = LogManager
            .Setup()
            .LoadConfigurationFromFile(configPath)
            .GetCurrentClassLogger();

        try
        {
            logger.Info("應用程式啟動");

            var dataPath = args.Length > 0
                ? Path.GetFullPath(args[0])
                : Path.Combine(AppContext.BaseDirectory, DefaultDataFilename);

            if (!File.Exists(dataPath))
            {
                logger.Error("找不到資料檔案：{DataPath}", dataPath);
                return 1;
            }
            //題目一：
            var matchBetService = new MatchBetService();
            matchBetService.LoadData(dataPath);

            var CurrentData = matchBetService.MatchBetDataList;
            logger.Info(
                "成功從 {DataPath} 載入 {RecordCount} 筆比賽資料",
                dataPath,
                matchBetService.MatchBetDataList.Count);
            if (CurrentData.Count() == 0)
                return 0;
            //題目二：
            var result2 = CurrentData.GroupBy(x => x.League)
                .Select(g => new
                {
                    League = g.Key,
                    TotalBetAmount = g.Sum(x => x.BetAmount)
                }).OrderByDescending(x=>x.TotalBetAmount)
                .Take(3)
                .ToList();
            



            return 0;
        }
        catch (Exception exception)
        {
            logger.Error(exception, "應用程式執行失敗");
            return 1;
        }
        finally
        {
            LogManager.Shutdown();
        }
    }
}
