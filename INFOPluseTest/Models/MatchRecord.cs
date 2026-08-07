namespace INFOPluseTest.Models;

public class MatchRecord
{
    /// <summary>
    /// 比賽識別碼，例如 M001。
    /// </summary>
    public required string MatchId { get; init; }

    /// <summary>
    /// 聯盟名稱，例如 NBA、MLB、EPL。
    /// </summary>
    public required string League { get; init; }

    /// <summary>
    /// 主場隊伍名稱。
    /// </summary>
    public required string HomeTeam { get; init; }

    /// <summary>
    /// 客場隊伍名稱。
    /// </summary>
    public required string AwayTeam { get; init; }

    /// <summary>
    /// 主場隊伍分數。
    /// </summary>
    public int HomeScore { get; init; }

    /// <summary>
    /// 客場隊伍分數。
    /// </summary>
    public int AwayScore { get; init; }

    /// <summary>
    /// 目前累積投注金額。
    /// </summary>
    public decimal BetAmount { get; init; }

    /// <summary>
    /// 目前累積投注筆數。
    /// </summary>
    public int BetCount { get; init; }

    /// <summary>
    /// 資料更新時間。
    /// </summary>
    public DateTime UpdateTime { get; init; }
}