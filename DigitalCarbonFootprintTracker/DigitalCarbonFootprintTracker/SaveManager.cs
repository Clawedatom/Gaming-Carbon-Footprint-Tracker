using DigitalCarbonFootprintTracker;

internal class SaveManager
{
    private readonly string filePath;

    public SaveManager(string filePath)
    {
        this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }

    public void SaveNewSummary(SessionSummary summary)
    {
        if (summary == null)
            throw new ArgumentNullException(nameof(summary));
        Console.WriteLine($"Saving sessions to: {Path.GetFullPath("SessionData.txt")}");
        try
        {
            File.AppendAllText(filePath, summary.SaveData() + Environment.NewLine);
            
        }
        catch (IOException ex)
        {
            Console.WriteLine($"File error while saving session: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Access denied while saving session: {ex.Message}");
        }
    }
    public List<SessionSummary> LoadSummaries(string filePath)
    {
        List<SessionSummary> summaries = new List<SessionSummary>();

        if (!File.Exists(filePath))
            return summaries;

        foreach (string line in File.ReadLines(filePath))
        {
            if (!TryParseSessionSummary(line, out SessionSummary summary))
                continue;

            summaries.Add(summary);
        }

        return summaries;
    }


    public string GetRecentSession()
    {
        string result = "";

        if (!File.Exists(filePath))
            return result;

        foreach (string line in File.ReadLines(filePath).Reverse())
        {
            if (!TryParseSessionSummary(line, out SessionSummary summary))
                continue;

           
            result = summary.ToString();
            break;
        }

        return result;
    }

    private bool TryParseSessionSummary(string line, out SessionSummary summary)
    {
        summary = null!;

        if (string.IsNullOrWhiteSpace(line))
            return false;

        string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 4)
            return false;

        string gameName = parts[0];

        if (!float.TryParse(parts[1], out float targetHours))
            return false;

        if (!float.TryParse(parts[2], out float sessionHours))
            return false;

        if (!Enum.TryParse(parts[3], true, out DeviceType device))
            device = DeviceType.Desktop; // fallback

        summary = new SessionSummary(gameName, targetHours, device);
        summary.OnSessionEnd(sessionHours);

        return true;
    }

    public List<SessionSummary> GetAllSessions()
    {
        List<SessionSummary> sessions = new List<SessionSummary>();
      
        if (!File.Exists(filePath))
            return sessions;

        foreach (string line in File.ReadLines(filePath))
        {
            if (!TryParseSessionSummary(line, out SessionSummary summary)) // convert back to summary so i can just do summary.tostring for the same summaries everywhere
                continue;


            sessions.Add(summary);
        }



        return sessions;
    }
}
