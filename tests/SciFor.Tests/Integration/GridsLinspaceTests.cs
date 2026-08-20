using SciFor.Application.Grids;
using SciFor.Domain.Grids;

namespace SciFor.Tests.Integration;

/// <summary>
/// Exercises the product surface with the real use case behind it. The port is not mocked
/// here: parity and the binding constraints are claimed about this path (ADR-009 §5).
/// </summary>
public sealed class GridsLinspaceTests
{
    /// <summary>A1 - Grids.Linspace(0, 1, 5) returns the FIX-001 sequence.</summary>
    [Fact]
    public void Linspace_UnitInterval_FivePoints_A1()
    {
        var sequence = new Grids().Linspace(0.0, 1.0, 5);

        Assert.Equal(5, sequence.Samples.Count);
        Assert.Equal(new[] { 0.0, 0.25, 0.5, 0.75, 1.0 }, sequence.Samples);
    }

    /// <summary>
    /// Y2 - the default constructor wires the real use case, so the public entry point
    /// reaches the domain formula without a substituted port.
    /// </summary>
    [Fact]
    public void real_use_case_through_adapter_Y2()
    {
        var wiredByDefault = new Grids().Linspace(0.0, 1.0, 5);
        var throughExplicitPort = new Grids(new GenerateLinearSequence()).Linspace(0.0, 1.0, 5);

        Assert.Equal(throughExplicitPort.Samples, wiredByDefault.Samples);
    }

    /// <summary>
    /// The adapter must not restate the formula. If it did, a supplied port would be
    /// ignored and a later HTTP adapter could drift from this one (ADR-002).
    /// </summary>
    [Fact]
    public void Linspace_DelegatesToThePort()
    {
        var spy = new RecordingGenerator();

        new Grids(spy).Linspace(1.5, 3.5, 4);

        Assert.Equal(new LinearSequenceRequest(1.5, 3.5, 4), spy.Received);
    }

    /// <summary>Rejections propagate through the adapter rather than being swallowed.</summary>
    [Fact]
    public void Linspace_PropagatesRejection()
    {
        var exception = Assert.Throws<LinearSequenceRejectedException>(
            () => new Grids().Linspace(0.0, 1.0, -1));

        Assert.Equal(LinearSequenceRejection.NegativeLength, exception.Reason);
    }

    private sealed class RecordingGenerator : IGenerateLinearSequence
    {
        internal LinearSequenceRequest? Received { get; private set; }

        public LinearSequence Generate(LinearSequenceRequest request)
        {
            Received = request;
            return new LinearSequence(new double[request.Length]);
        }
    }
}
