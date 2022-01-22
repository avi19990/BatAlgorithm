using System;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public float time = 9.0f;

    public Light sunLight;

    enum DayPeriod { Day, Night }
    DayPeriod dayPeriod = DayPeriod.Day;

    public event Action dayStart;
    public event Action dayEnd;

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

        Quaternion lightRotation;
        if (dayPeriod == DayPeriod.Day)
            lightRotation = Quaternion.AngleAxis(90.0f, new Vector3(0.0f, 1.0f, 0.0f)) * Quaternion.AngleAxis(time / 12.0f * 230.0f - 50.0f, new Vector3(1.0f, 0.0f, 0.0f));
        else
            lightRotation = Quaternion.AngleAxis(90.0f, new Vector3(0.0f, 1.0f, 0.0f)) * Quaternion.AngleAxis(time / 12.0f * 130.0f + 180.0f, new Vector3(1.0f, 0.0f, 0.0f));

        sunLight.transform.rotation = lightRotation;
    }
}
