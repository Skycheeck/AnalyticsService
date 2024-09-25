using Cysharp.Threading.Tasks;

public interface IAnalyticsEventsSender
{
    UniTask<AnalyticsSendResult> SendEvents(string events);
}