using System;
using System.Collections.Generic;

[Serializable]
public struct AnalyticsServiceState
{
    public List<string> SerializedEvents;

    public AnalyticsServiceState(int eventsCapacity)
    {
        SerializedEvents = new List<string>(eventsCapacity);
    }
}