using DigitalCarbonFootprintTracker;

internal class AnalyticsManager
{
    private readonly List<SessionSummary> sessions;

    public AnalyticsManager(List<SessionSummary> sessions)
    {
        this.sessions = sessions;
    }

    public float GetTotalCarbon()
    {
        return sessions.Sum(s => s.GetCarbon());
    }

    public float GetTotalPlayTime()
    {
        return sessions.Sum(s => s.GetSessionHours());
    }

    public float GetAverageSessionLength()
    {
        if (sessions.Count == 0)
            return 0;

        return GetTotalPlayTime() / sessions.Count;
    }

    public Dictionary<DeviceType, float> GetCarbonByDevice()
    {
        return sessions.GroupBy(s => s.DeviceUsed).ToDictionary(g => g.Key,g => g.Sum(s => s.GetCarbon()));
    }
}
