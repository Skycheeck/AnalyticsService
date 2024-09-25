using Cysharp.Threading.Tasks;

public interface IAnalyticsEventsSender
{
    UniTask<(AnalyticsSendResult, string)> SendEvents(string events);
}