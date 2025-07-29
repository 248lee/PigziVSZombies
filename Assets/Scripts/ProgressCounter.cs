using System.Collections;
using System.Collections.Generic;

public class ProgressCounter
{
    public int fullCount { get; private set; }
    public int currentCount {get; private set;}
    public string name { get; private set; }
    ProgressCounter parentCounter;

    public ProgressCounter(int _fullCount, string _name)
    {
        this.fullCount = _fullCount;
        this.name = _name;
        this.currentCount = 0;
        this.parentCounter = null;
    }
    public ProgressCounter (List<ProgressCounter> childrenCounters, int additionalCount, string _name)
    {
        this.name = _name;
        // Calculate the full count
        int sum = 0;
        foreach (ProgressCounter counter in childrenCounters)
        {
            sum += counter.fullCount;
        }
        this.fullCount = sum + additionalCount;

        // Calculate the current count
        sum = 0;
        foreach (ProgressCounter counter in childrenCounters)
        {
            sum += counter.currentCount;
        }
        this.currentCount = sum;

        // Assign this to be the parent of children
        foreach (ProgressCounter counter in childrenCounters)
        {
            counter.parentCounter = this;
        }

        this.parentCounter = null;
    }
    public bool CountUp()
    {
        if (this.currentCount < this.fullCount)
        {   this.currentCount += 1;
            if (this.parentCounter != null)
            {
                this.parentCounter.currentCount += 1;
            }
            return true; // Successfully counted up
        }
        return false; // Already at full count, cannot count up
    }
}
