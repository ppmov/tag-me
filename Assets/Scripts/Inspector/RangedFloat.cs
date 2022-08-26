[System.Serializable]
public struct RangedFloat
{
    public float minValue;
    public float maxValue;

    public float Random => UnityEngine.Random.Range(minValue, maxValue);

    public RangedFloat(float min, float max)
    {
        minValue = min;
        maxValue = max;
    }
}