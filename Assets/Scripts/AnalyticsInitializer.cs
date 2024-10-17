using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using UnityEngine;

public class AnalyticsInitializer : MonoBehaviour
{
    async void Start()
    {
        try
        {
            // Initialize Unity Services
            await UnityServices.InitializeAsync();

            // Check if Analytics is initialized
            if (UnityServices.State == ServicesInitializationState.Initialized)
            {
                Debug.Log("Unity Services initialized successfully.");
            }
        }
        catch (ServicesInitializationException e)
        {
            Debug.LogError($"Failed to initialize Unity Services: {e.Message}");
        }
    }
}
