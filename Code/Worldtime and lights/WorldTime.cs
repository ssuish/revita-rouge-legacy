using System;
using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class WorldTime : MonoBehaviour
{
    public event EventHandler<TimeSpan> WorldTimeChanged;
    public float dayLengthInSeconds = 60; // Length of day in seconds
    public bool isOfflineMode { get; private set; } = false; // To track offline mode

    private DateTime _realWorldTime;
    private TimeSpan _currentTime;
    private DateTime _startTime;
    private DateTime _lastUpdatedTime;
    private bool _fetchSuccess = false; // Flag to track success of real-world time fetch

    private void Start()
    {
        _realWorldTime = DateTime.Now; // Initialize real-world time
        _startTime = DateTime.Now; // Initialize the start time for offline fallback
        _currentTime = DateTime.Now.TimeOfDay; // Initialize the current time
        _lastUpdatedTime = DateTime.Now; // Initialize the last updated time
        StartCoroutine(UpdateTime());
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator UpdateTime()
    {
        while (true)
        {
            // Attempt to fetch real-world time
            yield return FetchRealWorldTime();

            if (!isOfflineMode)
            {
                // Successfully fetched real-world time
                yield return UpdateRealWorldTime();
            }
            else
            {
                // Fallback to offline time system if fetching real-world time fails
                yield return UpdateOfflineTime();
            }

            // Update every minute to avoid too many API calls
            yield return new WaitForSeconds(300);
        }
    }

    private async Task FetchRealWorldTime()
    {
        const string url = "https://worldtimeapi.org/api/timezone/Asia/Manila";
        const int maxAttempts = 3; // Max retry attempts
        int attempt = 0;
    
        using var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };
    
        using var client = new HttpClient(handler);
    
        while (attempt < maxAttempts)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var timeData = JObject.Parse(jsonResponse);
                    var dateTimeString = timeData["utc_datetime"]?.ToString();

                    if (!string.IsNullOrEmpty(dateTimeString))
                    {
                        _realWorldTime = DateTime.Parse(dateTimeString);
                        isOfflineMode = false;
                        //Debug.Log("Successfully fetched real-world time.");
                        return;
                    }
                    
                }
                
            }
            catch (HttpRequestException httpEx)
            {
                //Debug.LogError($"HTTP Request error: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                //Debug.LogError($"General error: {ex.Message}");
            }

            // Increment the attempt counter and set offline mode if max attempts are reached
            attempt++;
            if (attempt >= maxAttempts)
            {
                //Debug.LogWarning("Switching to offline mode due to repeated fetch failures.");
                isOfflineMode = true;
                return;
            }
        }
    }

    private IEnumerator UpdateRealWorldTime()
    {
        while (!isOfflineMode)
        {
            // Increment real-world time every second
            _realWorldTime = _realWorldTime.AddSeconds(1);
            _lastUpdatedTime = _realWorldTime; // Update last updated time
            WorldTimeChanged?.Invoke(this, _realWorldTime.TimeOfDay); // Fire event with updated time

            yield return new WaitForSeconds(1); // Update every second
        }
    }

    public IEnumerator UpdateOfflineTime()
    {
        while (isOfflineMode)
        {
            // Calculate the elapsed time since the game started
            TimeSpan elapsedTime = DateTime.Now - _startTime;
            // Convert the elapsed time to in-game time
            _currentTime = TimeSpan.FromSeconds(elapsedTime.TotalSeconds % dayLengthInSeconds);
            // Convert the in-game time to DateTime format
            _lastUpdatedTime = DateTime.Today.Add(_currentTime);
            //_lastUpdatedTime = DateTime.Now;
            WorldTimeChanged?.Invoke(this, _currentTime);
            //Debug.Log("Offline in-game time: " + _currentTime);


            yield return new WaitForSeconds(1); // Update time every second
        }
    }

    public TimeSpan GetCurrentTime()
    {
        return isOfflineMode ? _currentTime : _realWorldTime.TimeOfDay;
    }

    public DateTime GetLastUpdateTime()
    {
        return _lastUpdatedTime;
    }

    public void SetLastUpdateTime(DateTime newUpdateTime)
    {
        _lastUpdatedTime = newUpdateTime;
    }
}