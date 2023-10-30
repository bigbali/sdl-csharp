namespace sdl_csharp.Utility
{
    public class Math
    {
        public static float Percent(int maxIndex, int currentIndex, float value)
        {
            float members = System.Math.Max(maxIndex, 1.0f);
            float index = System.Math.Max(currentIndex, 1.0f);

            float maxPercentPerMember = 1.0f / members * 100.0f;
            float basePercent = maxPercentPerMember * index - maxPercentPerMember;

            float memberPercent = value / 100.0f * maxPercentPerMember;

            float totalPercent = basePercent + memberPercent;

            return totalPercent;
        }
    }
}
