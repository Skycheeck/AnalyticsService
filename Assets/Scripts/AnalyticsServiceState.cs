using System.Collections.Generic;

public struct AnalyticsServiceState
{
    public readonly List<string> SerializedEvents;
    public float SendTimer;

    public AnalyticsServiceState(float timer = 1f, int eventsCapacity = 10)
    {
        SerializedEvents = new List<string>(eventsCapacity);
        SendTimer = timer;
    }
}