using JetBrains.Annotations;
using Steamworks;

namespace ShockerKnight;

public static class Operations
{
    public abstract class BaseOperation(int op, int duration, int intensity)
    {
        public int Op { get; set; } = op;
        public int Duration { get; set; } = duration;
        public int Intensity { get; set; } = intensity;
    }

    public class ShockOperation(int duration, int intensity) : BaseOperation(0, duration, intensity);

    public class VibrationOperation(int duration, int intensity) : BaseOperation(1, duration, intensity);

    public class BeepOperation(int duration) : BaseOperation(2, duration, 0);
}