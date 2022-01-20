using System;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public float time;

    enum DayPeriod { Day, Night }
    DayPeriod dayPeriod = DayPeriod.Day;

    public event Action dayStart;
    public event Action dayEnd;

    void Start()
    {
        time = 9.0f;
    }

    void Update()
    {
        time += Time.deltaTime;

        if (time > 12.0f)
        {
            time = 0.0f;

            if (dayPeriod == DayPeriod.Night)
            {
                dayPeriod = DayPeriod.Day;
                dayStart?.Invoke();
            }
            else
            {
                dayPeriod = DayPeriod.Night;
                dayEnd?.Invoke();
            }
        }
    }
}
