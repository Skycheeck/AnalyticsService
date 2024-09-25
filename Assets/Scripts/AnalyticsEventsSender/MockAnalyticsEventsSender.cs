using Cysharp.Threading.Tasks;
using UnityEngine;

public class MockAnalyticsEventsSender : IAnalyticsEventsSender
{
    public UniTask<(AnalyticsSendResult, string)> SendEvents(string events)
    {
        Debug.Log(events);
        return new UniTask<(AnalyticsSendResult, string)>((AnalyticsSendResult.Success, events));
    }
}