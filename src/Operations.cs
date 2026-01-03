using JetBrains.Annotations;
using Steamworks;

namespace ShockerKnight;

public static class Operations
{
    public abstract class BaseOperation(int op, double duration, int intensity)
    {
        public int Op { get; set; } = op;
        public double Duration { get; set; } = duration;
        public int Intensity { get; set; } = intensity;
    }

    public class ShockOperation(double duration, int intensity) : BaseOperation(0, duration, intensity);

    public class VibrationOperation(double duration, int intensity) : BaseOperation(1, duration, intensity);

    public class BeepOperation(double duration) : BaseOperation(2, duration, 0);
}