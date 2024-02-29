using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Analytics;
using Unity.Services.Core;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        await UnityServices.InitializeAsync();
            // Send custom event
        Dictionary<string, object> parameters = new Dictionary<string, object>()
        {
            { "fabulousString", "COF is in production" },
            { "sparklingInt", 420 },
            { "spectacularFloat", 0.69f },
            { "peculiarBool", true },
        };
        // The ‘myEvent’ event will get queued up and sent every minute
        Events.CustomData("rathTestEvent", parameters); 

        // Optional - You can call Events.Flush() to send the event immediately
        Events.Flush();  
    }
              
}