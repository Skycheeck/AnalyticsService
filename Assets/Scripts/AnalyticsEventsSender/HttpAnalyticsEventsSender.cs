using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class HttpAnalyticsEventsSender : IAnalyticsEventsSender
{
    private readonly string _url;
    public HttpAnalyticsEventsSender(string url)
    {
        _url = url;
    }

    public async UniTask<AnalyticsSendResult> SendEvents(string events)
    {
        UnityWebRequest webRequest = UnityWebRequest.Post(_url, events);
        await webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
            return AnalyticsSendResult.Success;
        
        Debug.Log(webRequest.error);
        return AnalyticsSendResult.Failure;
    }
}