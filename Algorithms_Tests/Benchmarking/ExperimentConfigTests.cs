using Algorithms_programm.Benchmarking;
using Xunit;

namespace Algorithms_Tests.Benchmarking;

public class ExperimentConfigTests
{
    [Fact]
    public void ComputeConfigHash_OnSameParameters_IsStable()
    {
        var config1 = new ExperimentConfig("Sum", NMax: 1000, Step: 10, RunsPerPoint: 5);
        var config2 = new ExperimentConfig("Sum", NMax: 1000, Step: 10, RunsPerPoint: 5);

        Assert.Equal(config1.ComputeConfigHash(), config2.ComputeConfigHash());
    }

    [Fact]
    public void ComputeConfigHash_OnDifferentParameters_Differs()
    {
        var config1 = new ExperimentConfig("Sum", NMax: 1000, Step: 10, RunsPerPoint: 5);
        var config2 = new ExperimentConfig("Sum", NMax: 2000, Step: 10, RunsPerPoint: 5);

        Assert.NotEqual(config1.ComputeConfigHash(), config2.ComputeConfigHash());
    }

    [Fact]
    public void ComputeConfigHash_IgnoresAlgorithmName_ByDesign()
    {
        // AlgorithmName умышленно не входит в хэш (хранится отдельно как внешний ключ),
        // поэтому два разных алгоритма с одинаковыми числовыми параметрами дадут одинаковый хэш.
        var config1 = new ExperimentConfig("Sum", NMax: 1000, Step: 10, RunsPerPoint: 5);
        var config2 = new ExperimentConfig("Product", NMax: 1000, Step: 10, RunsPerPoint: 5);

        Assert.Equal(config1.ComputeConfigHash(), config2.ComputeConfigHash());
    }

    [Fact]
    public void EnumerateN_ProducesExpectedSequenceIncludingNMax()
    {
        var config = new ExperimentConfig("Sum", NMax: 25, Step: 10, RunsPerPoint: 5);

        var values = config.EnumerateN().ToList();

        Assert.Equal(new[] { 1, 11, 21, 25 }, values);
    }

    [Fact]
    public void EnumerateN_WhenNMaxExactlyOnGrid_DoesNotDuplicate()
    {
        var config = new ExperimentConfig("Sum", NMax: 21, Step: 10, RunsPerPoint: 5);

        var values = config.EnumerateN().ToList();

        Assert.Equal(new[] { 1, 11, 21 }, values);
    }

    [Fact]
    public void EnumerateN_OnZeroStep_Throws()
    {
        var config = new ExperimentConfig("Sum", NMax: 10, Step: 0, RunsPerPoint: 5);

        Assert.Throws<InvalidOperationException>(() => config.EnumerateN().ToList());
    }
}
