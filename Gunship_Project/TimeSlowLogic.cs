using UnityEngine;

public class TimeSlowLogic : MonoBehaviour
{
    public static TimeSlowLogic Instance;
    public float SlowMotionTime = 2f;
    private float SlowMoAmount = 0.3f;
    private float fdt = 0;
    private float tms = 0;
    /* new variables */
    private float differenceFdt;
    private float differenceTms;
    void Start()
    {
        Instance = this;
        fdt = Time.fixedDeltaTime;
        tms = Time.timeScale;
    }
    void Update()
    {
        if (Time.timeScale < tms)
        {
            float multiplierValue = Time.unscaledDeltaTime * 1.0f / SlowMotionTime;
            Time.timeScale += multiplierValue * differenceTms;
            Time.fixedDeltaTime += multiplierValue * differenceFdt;
        }
        else
        {
            Time.timeScale = tms;
            Time.fixedDeltaTime = fdt;
        }
    }

    public void SlowMoStart()
    {
        Time.timeScale = SlowMoAmount;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

        /* new operations */
        differenceTms = tms - Time.timeScale;
        differenceFdt = fdt - Time.fixedDeltaTime;
    }
}
