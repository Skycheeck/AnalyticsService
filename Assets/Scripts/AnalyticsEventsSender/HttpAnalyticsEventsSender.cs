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

    public async UniTask<(AnalyticsSendResult, string)> SendEvents(string events)
    {
        UnityWebRequest webRequest = UnityWebRequest.Post(_url, events);
        await webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log(webRequest.downloadHandler.text);
            return (AnalyticsSendResult.Success, events);
        }
        
        Debug.LogError(webRequest.error);
        return (AnalyticsSendResult.Failure, events);
    }
}