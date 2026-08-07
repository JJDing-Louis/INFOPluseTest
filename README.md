# INFOPluseTest

INFOPluseTest 是使用 .NET 10 開發的 Console 應用程式，用來讀取比賽投注 CSV 資料，並執行聯盟投注金額統計與重複賽事分析。

## 功能

- 從 CSV 檔案載入比賽投注資料。
- 遇到單筆格式錯誤時記錄錯誤並略過，不中斷其餘資料的載入。
- 統計總投注金額最高的前三個聯盟。
- 找出聯盟、隊伍與比分皆相同的重複賽事。
- 將執行結果與錯誤資訊輸出至 Console 及每日 log 檔案。

## 環境需求

- [.NET SDK 10.0](https://dotnet.microsoft.com/)

## 使用的 Library

| Library | 版本 | 用途 |
| --- | --- | --- |
| CsvHelper | 33.1.0 | 解析 CSV、將各欄位轉換成 `MatchRecord`，並提供逐筆讀取錯誤處理。 |
| NLog | 6.1.4 | 記錄程式啟動、資料載入結果、警告與錯誤；目前同時輸出至 Console 和檔案。 |
| LINQ | .NET 內建 | 處理分組、加總、排序、篩選及取得前三名等資料分析。 |

## 專案結構

```text
INFOPluseTest/
├── INFOPluseTest.sln
├── README.md
└── INFOPluseTest/
    ├── Folder/
    │   └── Data.csv
    ├── Models/
    │   └── MatchRecord.cs
    ├── Services/
    │   └── MatchBetService.cs
    ├── INFOPluseTest.csproj
    ├── NLog.config
    └── Program.cs
```

## MatchBetService

`MatchBetService` 負責載入與分析比賽投注資料。CSV 成功轉換的資料會儲存在 `MatchBetDataList`。

### `LoadData(string path)`

從指定路徑讀取 CSV，並將有效資料轉換為 `MatchRecord`。

- 輸入：CSV 檔案路徑。
- 輸出：無回傳值；載入結果存入 `MatchBetDataList`。
- 路徑為空或檔案不存在時，寫入 `Warning` log 並停止載入。
- 單筆資料發生型別轉換、缺少欄位或其他 CsvHelper 讀取錯誤時：
  - 略過該筆資料。
  - 記錄 CSV 列號、原始資料及例外資訊。
  - 繼續讀取後續資料。
- 完成後記錄成功與略過的資料筆數。

### `GetTop3TotalBetAmount()`

依 `League` 將資料分組，計算各聯盟的 `BetAmount` 總和，並取得總投注金額最高的前三名。

回傳型別：

```csharp
List<(string League, decimal TotalBetAmount)>
```

排序規則：

1. `TotalBetAmount` 由高至低。
2. 金額相同時，依 `League` 名稱排序。
3. 最多回傳三筆；資料不足三個聯盟時回傳現有資料。

### `GetduplicateMatches()`

找出具有相同比賽特徵且出現超過一次的資料。以下欄位完全相同時視為同一場賽事：

- `League`
- `HomeTeam`
- `AwayTeam`
- `HomeScore`
- `AwayScore`

回傳型別：

```csharp
List<(string MatchKey, int Count, decimal TotalBetAmount)>
```

回傳欄位：

- `MatchKey`：由聯盟、主客隊伍與比分組成的賽事識別文字。
- `Count`：相同比賽資料的筆數。
- `TotalBetAmount`：該組重複資料的投注金額總和。

排序規則：

1. `Count` 由高至低。
2. 次數相同時，依 `TotalBetAmount` 由高至低。
3. 前兩項相同時，依 `MatchKey` 排序。

## CSV 格式

CSV 第一列必須包含以下標題：

```csv
MatchId,League,HomeTeam,AwayTeam,HomeScore,AwayScore,BetAmount,BetCount,UpdateTime
```

| 欄位 | C# 型別 | 說明 |
| --- | --- | --- |
| MatchId | `string` | 比賽識別碼。 |
| League | `string` | 聯盟名稱。 |
| HomeTeam | `string` | 主場隊伍。 |
| AwayTeam | `string` | 客場隊伍。 |
| HomeScore | `int` | 主場比分。 |
| AwayScore | `int` | 客場比分。 |
| BetAmount | `decimal` | 投注金額。 |
| BetCount | `int` | 投注筆數。 |
| UpdateTime | `DateTime` | 資料更新時間。 |

## 執行方式

在 Solution 根目錄還原、建置並執行：

```bash
dotnet restore INFOPluseTest.sln
dotnet build INFOPluseTest.sln
dotnet run --project INFOPluseTest/INFOPluseTest.csproj
```

未提供參數時，程式會讀取建置輸出目錄內的 `Folder/Data.csv`。專案建置時會自動將預設 CSV 複製至輸出目錄。

也可以傳入自訂 CSV 路徑：

```bash
dotnet run --project INFOPluseTest/INFOPluseTest.csproj -- /absolute/path/to/data.csv
```

## Log

NLog 設定存放於 `INFOPluseTest/NLog.config`，目前最低記錄等級為 `Debug`，並輸出至：

- Console。
- `bin/<Configuration>/net10.0/logs/app-yyyy-MM-dd.log`。

程式結束前會呼叫 `LogManager.Shutdown()`，確保非同步 log 完整寫入。
