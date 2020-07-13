using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AidPacket : MonoBehaviour
{
    [Serializable]
    public struct TimeFormat
    {
        public int hours;
        public int mins;
        public int seconds;

        public TimeFormat(int h, int m, int s)
        {
            hours = h; mins = m; seconds = s;
        }
        public TimeFormat(DateTime date)
        {
            hours = date.Hour; mins = date.Minute; seconds = date.Second;
        }
        public TimeFormat(TimeSpan span)
        {
            hours = span.Hours; mins = span.Minutes; seconds = span.Seconds;
        }
        public override string ToString()
        {
            return hours.ToString() + ":" + mins.ToString() + ":" + seconds.ToString();
        }
        public static explicit operator TimeSpan(TimeFormat time)
        {
            return new TimeSpan(time.hours, time.mins, time.seconds);
        }
    }
    public TimeFormat dateInterval;
    private TimeSpan timeInterval;
    private DateTime lastDateUsed;
    public TMPro.TextMeshProUGUI timerText;

    public float refreshTime = 1f;
    private float currentTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        timeInterval = (TimeSpan)dateInterval;   
    }

    // Update is called once per frame
    void Update()
    {
        // update timer
        currentTime += Time.unscaledDeltaTime;
        if (currentTime >= refreshTime)
        {
            currentTime = 0f;
            DateTime now = DateTime.Now;
            TimeSpan dateDif = now.Subtract(lastDateUsed);
            if (TimeSpan.Compare(timeInterval, dateDif) < 0)
            {
                // time passed
                TimeFormat zeroTime = new TimeFormat(0, 0, 0);
                timerText.text = zeroTime.ToString();
                lastDateUsed = now;
                return;
            }
            TimeSpan timeLeft = timeInterval.Subtract(dateDif);
            TimeFormat timeLeftFormat = new TimeFormat(timeLeft);
            timerText.text = timeLeftFormat.ToString();
        }
    }

    string FormatTime()
    {
        //int minutes = (int)secondsToReset / 60;
        //int hours = minutes / 60;
        //minutes = minutes % 60;
        //int seconds = (int)secondsToReset % 60;
        //return hours.ToString() + ":" + minutes.ToString() + ":" + seconds.ToString();
        return dateInterval.ToString();
    }
}
