using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DTO;
using Newtonsoft.Json;

public class AnalyticsService
{
    private const float SEND_COOLDOWN = 3f;
    private float _timer = SEND_COOLDOWN;

    private readonly List<AnalyticsEventDTO> _events;
    private readonly IAnalyticsEventsSender _analyticsEventsSender;
    private readonly AnalyticsServiceState _state;
    private bool _sendInProgress = false;

    public AnalyticsServiceState State => _state;
    
    public AnalyticsService(IAnalyticsEventsSender analyticsEventsSender, AnalyticsServiceState state)
    {
        _events = new List<AnalyticsEventDTO>();
        _analyticsEventsSender = analyticsEventsSender;
        _state = state;
    }

    public void Tick(float timeElapsed)
    {
        _timer -= timeElapsed;
        if (!(_timer <= 0f)) return;
            
        SerializeEvents();
        SendNextEventBatch();
        ResetTimer();
    }

    public void TrackEvent(string type, string data)
    {
        _events.Add(new AnalyticsEventDTO(type, data));
        ResetTimer();
    }

    public void SerializeEvents()
    {
        if (_events.Count < 1) return;
        
        string result = JsonConvert.SerializeObject(new {events = _events});
        _state.SerializedEvents.Add(result);
        _events.Clear();
    }

    private async UniTask SendNextEventBatch()
    {
        if (_state.SerializedEvents.Count < 1) return;
        
        while (_sendInProgress)
        {
            await UniTask.NextFrame();
        }

        _sendInProgress = true;

        (string s, UniTask<AnalyticsSendResult> task)[] pairs = _state.SerializedEvents
            .Select(batch => (s: batch, task: _analyticsEventsSender.SendEvents(batch)))
            .ToArray();

        await UniTask.WhenAll(pairs.Select(x => x.task));

        foreach ((string s, UniTask<AnalyticsSendResult> task) pair in pairs)
        {
            AnalyticsSendResult result = await pair.task;
            if (result != AnalyticsSendResult.Success) continue;
            _state.SerializedEvents.Remove(pair.s);
        }
            
        _sendInProgress = false;
    }

    private void ResetTimer()
    {
        _timer = SEND_COOLDOWN;
    }
}