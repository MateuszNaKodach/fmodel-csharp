using Fraktalio.FModel.Domain;

namespace Fraktalio.FModel.Tests;

public static class SimpleNumberTestFixture
{
    public interface INumberCommand
    {
        string StreamId { get; init; }
    }

    public sealed record AddNumberCommand(string StreamId, int Number) : INumberCommand
    {
        public AddNumberCommand(int number) : this(Guid.NewGuid().ToString(), number)
        {
        }
    }

    public sealed record MultiplyNumberCommand(string StreamId, int Multiplier) : INumberCommand
    {
        public MultiplyNumberCommand(int multiplier) : this(Guid.NewGuid().ToString(), multiplier)
        {
        }
    }

    public interface INumberEvent
    {
        public string StreamId { get; init; }
    }

    public sealed record NumberAddedEvent(string StreamId, int Number) : INumberEvent;

    public sealed record NumberMultipliedEvent(string StreamId, int Multiplier) : INumberEvent;

    public sealed record NumberState(int Value);

    public static readonly IDecider<INumberCommand, NumberState?, INumberEvent> NumberDecider =
        new Decider<INumberCommand, NumberState?, INumberEvent>(
            (c, s) => c switch
            {
                AddNumberCommand cmd => new[] { new NumberAddedEvent(cmd.StreamId, cmd.Number) },
                MultiplyNumberCommand cmd => new[] { new NumberMultipliedEvent(cmd.StreamId, cmd.Multiplier) },
                _ => throw new ArgumentOutOfRangeException(nameof(c)),
            },
            (s, e) => e switch
            {
                NumberAddedEvent ev => new NumberState((s?.Value ?? 0) + ev.Number),
                NumberMultipliedEvent ev => new NumberState((s?.Value ?? 0) * ev.Multiplier),
                _ => throw new ArgumentOutOfRangeException(nameof(e)),
            },
            new NumberState(0)
        );
}
