using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class AnalyticsServiceBehaviour : MonoBehaviour
{
    [SerializeField] private Button _addEventButton;
    [SerializeField] private Button _saveButton;
        
    private SaveManager _saveManager;
    private AnalyticsService _analyticsService;

    private void Awake()
    {
        _saveManager = new SaveManager();
        _analyticsService = new AnalyticsService(new HttpAnalyticsEventsSender("https://httpbin.org/post"), _saveManager.LoadState());
        _addEventButton.onClick.AddListener(() => _analyticsService.TrackEvent("test", Time.time.ToString(CultureInfo.InvariantCulture)));
        _saveButton.onClick.AddListener(() =>
        {
            _analyticsService.SerializeEvents();
            _saveManager.SaveState(_analyticsService.State);
        });
    }

    private void Update()
    {
        _analyticsService.Tick(Time.deltaTime);
    }
}