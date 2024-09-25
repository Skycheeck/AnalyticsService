using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DTO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Assertions;

public class AnalyticsService : MonoBehaviour
{
    private readonly List<AnalyticsEventDTO> _events = new() {new AnalyticsEventDTO("test1", "1"), new AnalyticsEventDTO("test2", "2")};
    private readonly List<string> _serializedEvents = new();
    private IAnalyticsEventsSender _analyticsEventsSender;
    private bool _sendInProgress = false;

    private const float SEND_COOLDOWN = 3f;
    private float _sendTimer = SEND_COOLDOWN;

    private void Update()
    {
        _sendTimer -= Time.deltaTime;
        if (!(_sendTimer <= 0f)) return;
            
        SerializeEvents();
        SendNextEventBatch();
        ResetTimer();
    }

    public void TrackEvent(string type, string data)
    {
        _events.Add(new AnalyticsEventDTO(type, data));
        ResetTimer();
    }

    private void SerializeEvents()
    {
        string result = JsonConvert.SerializeObject(new {events = _events});
        _serializedEvents.Add(result);
        _events.Clear();
    }

    private async UniTask SendNextEventBatch()
    {
        Assert.IsTrue(_serializedEvents.Count > 0);
        
        while (_sendInProgress)
        {
            await UniTask.NextFrame();
        }

        _sendInProgress = true;

        (string s, UniTask<AnalyticsSendResult> task)[] pairs = _serializedEvents
            .Select(batch => (s: batch, task: _analyticsEventsSender.SendEvents(batch)))
            .ToArray();

        UniTask.WhenAll(pairs.Select(x => x.task));

        foreach ((string s, UniTask<AnalyticsSendResult> task) pair in pairs)
        {
            AnalyticsSendResult result = await pair.task;
            if (result != AnalyticsSendResult.Success) continue;
            _serializedEvents.Remove(pair.s);
        }
            
        _sendInProgress = false;
    }

    private void ResetTimer()
    {
        _sendTimer = SEND_COOLDOWN;
    }
}