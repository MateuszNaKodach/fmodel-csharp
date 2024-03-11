using static Fraktalio.FModel.Tests.SimpleNumberTestFixture;

namespace Fraktalio.FModel.Tests;

public sealed class DeciderTest
{
    [Theory]
    [MemberData(nameof(DecideTestCases))]
    public void Decider_Decide((INumberCommand c, NumberState s, INumberEvent e) data)
    {
        // given
        var (command, state, @event) = data;

        // when
        var events = NumberDecider.Decide(command, state);

        // then
        events.Should().ContainInOrder(@event);
    }

    public static TheoryData<(INumberCommand c, NumberState s, INumberEvent e)> DecideTestCases = new()
    {
        (new AddNumberCommand("test-id-1", 2), new NumberState(3), new NumberAddedEvent("test-id-1", 2)),
        (new MultiplyNumberCommand("test-id-2", 2), new NumberState(3), new NumberMultipliedEvent("test-id-2", 2)),
    };

    [Theory]
    [MemberData(nameof(EvolveTestCases))]
    public void Decider_Evolve((NumberState state, INumberEvent e, NumberState newState) data)
    {
        // given
        var (state, @event, newState) = data;

        // when
        var result = NumberDecider.Evolve(state, @event);

        // then
        result.Should().Be(newState);
    }

    public static TheoryData<(NumberState state, INumberEvent e, NumberState newState)> EvolveTestCases = new()
    {
        (new NumberState(3), new NumberAddedEvent("test-id-1", -1), new NumberState(2)),
        (new NumberState(3), new NumberMultipliedEvent("test-id-2", 2), new NumberState(6)),
    };
}
