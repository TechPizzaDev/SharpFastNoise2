namespace SharpFastNoise2.Generators
{
    public interface INoiseGeneratorAbstract : INoiseGenerator
    {
        static new abstract int UnitSize { get; }
    }
}
