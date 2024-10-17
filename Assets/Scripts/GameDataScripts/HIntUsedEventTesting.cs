using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.Analytics;


public class HIntUsedEventTesting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnHintUsed()
    {
        var parameters = new Dictionary<string, object>
        {
            { "hint_used_time", Time.timeSinceLevelLoad }
        };

        var customEvent = new CustomEvent("OnHintUsed");

        foreach (var param in parameters)
        {
            customEvent.Add(param.Key, param.Value);
        }

        // Record the custom event
        AnalyticsService.Instance.RecordEvent(customEvent);
    }
}
