[System.Serializable]
public struct RangeFloat
{
    public float min;
    public float max;
    public RangeFloat(float min, float max)
    {
        this.min = min;
        this.max = max;
    }

    public bool IsInRange(float value)
    {
        if (min <= value && value <= max)
            return true;
        return false;
    }

    public float GetMedium()
    {
        return (min + max) / 2;
    }
}