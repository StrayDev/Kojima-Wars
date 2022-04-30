// Define ENABLE_LOG to enable the log
#define ENABLE_LOG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenLog : MonoBehaviour
{
    private class Log
    {
        public string message = "Hello";
        public Color color = Color.cyan;
        public float duration = 2.0f;
        public float timeSubmitted = 0.0f;
    }

    public static ScreenLog Instance = null;

    [SerializeField] private Rect logArea = new Rect(200, 100, 500, 800);

    private static List<Log> logs = new List<Log>();
    private static int maxLogCount = 1024;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Print(string message, Color color, float duration = 2.0f)
    {
#if ENABLE_LOG
        // Clear the log if the number of logs exceeds the maximum count
        if (logs.Count >= maxLogCount) logs.Clear();

        // Setup a log
        Log newLog = new Log();
        newLog.message = message;
        newLog.color = color;
        newLog.duration = duration;
        newLog.timeSubmitted = Time.time;

        // Submit the log
        logs.Add(newLog);
#endif // ENABLE_LOG
    }

    private void Awake()
    {
        // Create a single instance of the ScreenLog class
        Instance = this;

        if (Instance == null)
            Instance = new ScreenLog();

    }

    private void OnGUI()
    {
        GUILayout.BeginArea(logArea);

        // For each log in logs
        int index = 0;
        foreach(Log log in logs)
        {
            // If the log has expired
            if ((Time.time - log.timeSubmitted) >= log.duration)
            {
                log.message = "";
            }
            else
            {
                if(log.message != "")
                {
                    // Log the message if it is not empty
                    GUI.contentColor = log.color;
                    GUILayout.Label(log.message);
                }
            }

            ++index;
        }

        GUILayout.EndArea();
    }
}
