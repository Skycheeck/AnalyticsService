using Cysharp.Threading.Tasks;
using UnityEngine;

public class MockAnalyticsEventsSender : IAnalyticsEventsSender
{
    public UniTask<AnalyticsSendResult> SendEvents(string events)
    {
        Debug.Log(events);
        return new UniTask<AnalyticsSendResult>(AnalyticsSendResult.Success);
    }
}