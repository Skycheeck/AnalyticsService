using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DTO;
using Newtonsoft.Json;
using UnityEngine.Assertions;

public class AnalyticsService
{
    private const float SEND_COOLDOWN = 3f;
    
    private readonly List<AnalyticsEventDTO> _events = new() {new AnalyticsEventDTO("test1", "1"), new AnalyticsEventDTO("test2", "2")};
    private readonly IAnalyticsEventsSender _analyticsEventsSender = new MockAnalyticsEventsSender();
    private bool _sendInProgress = false;

    private State _state = new State(SEND_COOLDOWN);

    private void Tick(float timeElapsed)
    {
        _state.SendTimer -= timeElapsed;
        if (!(_state.SendTimer <= 0f)) return;
            
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
        _state.SerializedEvents.Add(result);
        _events.Clear();
    }

    private async UniTask SendNextEventBatch()
    {
        Assert.IsTrue(_state.SerializedEvents.Count > 0);
        
        while (_sendInProgress)
        {
            await UniTask.NextFrame();
        }

        _sendInProgress = true;

        (string s, UniTask<AnalyticsSendResult> task)[] pairs = _state.SerializedEvents
            .Select(batch => (s: batch, task: _analyticsEventsSender.SendEvents(batch)))
            .ToArray();

        UniTask.WhenAll(pairs.Select(x => x.task));

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
        _state.SendTimer = SEND_COOLDOWN;
    }

    private struct State
    {
        public readonly List<string> SerializedEvents;
        public float SendTimer;

        public State(float timer = 1f, int eventsCapacity = 10)
        {
            SerializedEvents = new List<string>(eventsCapacity);
            SendTimer = timer;
        }
    }
}