using System;

namespace StatusRateHud;

// Values are percentage points; time is unscaled real seconds.
public sealed class RateMeter
{
    double startTime;
    double startValue;
    double lastTime;
    bool initialized;
    public double Rate { get; private set; }
    public bool Ready { get; private set; }
    public void Reset() { initialized = false; Ready = false; Rate = 0; }
    public void Sample(double seconds, double value)
    {
        if (!double.IsFinite(seconds) || !double.IsFinite(value)) { Reset(); return; }
        if (!initialized || seconds < lastTime || seconds - lastTime > 2)
        {
            startTime = lastTime = seconds; startValue = value;
            initialized = true; Ready = false; return;
        }
        lastTime = seconds;
        double elapsed = seconds - startTime;
        if (elapsed < 0.5) return;
        Rate = (value - startValue) / elapsed;
        Ready = true;
        startTime = seconds; startValue = value;
    }
}

