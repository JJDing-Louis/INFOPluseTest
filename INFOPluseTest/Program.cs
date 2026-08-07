using INFOPluseTest.Service;

namespace INFOPluseTest;

class Program
{
    public static string CurrentDir = Directory.GetCurrentDirectory();
    public static string Filename = $"Folder/Data.csv";
    
    static void Main(string[] args)
    {
        var path = Path.Combine(CurrentDir, Filename);
        var a = File.Exists(path);


        var matchBetService = new MatchBetService();
        
    }
}
