using Fraktalio.FModel.Domain;

namespace Fraktalio.FModel.Tests;

public class ViewTest
{
    [Fact]
    public void OddView_Evolve_By_OddNumberAdded()
    {
        var view = new OddView();

        var state = view.Evolve(new OddViewState(1), new OddNumberAdded(1));

        state.Should().Be(new OddViewState(2));
    }
    
    [Fact]
    public void OddView_Evolve_By_OddNumberMultiplied()
    {
        var view = new OddView();

        var state = view.Evolve(new OddViewState(2), new OddNumberMultiplied(5));

        state.Should().Be(new OddViewState(10));
    }
    
    [Fact]
    public void CombinedViewEvolve()
    {
        var view = View

        var state = view.Evolve(new OddViewState(2), new OddNumberMultiplied(5));

        state.Should().Be(new OddViewState(10));
    }
    /**
     *  t.deepEqual(
    oddView
      .combine(evenView)
      .evolve(
        { oddState: 0, evenState: 0 },
        { kind: 'OddNumberAddedEvt', value: 1 }
      ),
    { oddState: 1, evenState: 0 }
  );
     */
}

interface IOddNumberEvent;

record OddNumberAdded(int Value) : IOddNumberEvent;

record OddNumberMultiplied(int Multiplier) : IOddNumberEvent;

record OddViewState(int OddState);

record OddView() : View<OddViewState, IOddNumberEvent>(
    Evolve: (s, e) => e switch
    {
        OddNumberAdded oddNumberAdded => new OddViewState(s.OddState + oddNumberAdded.Value),
        OddNumberMultiplied oddNumberMultiplied => new OddViewState(s.OddState * oddNumberMultiplied.Multiplier),
        _ => s
    },
    InitialState: () => new OddViewState(0)
);

interface IEvenNumberEvent;

record EvenNumberAdded(int Value) : IEvenNumberEvent;

record EvenNumberMultiplied(int Multiplier) : IEvenNumberEvent;

record EvenViewState(int EvenState);

record EvenView() : View<EvenViewState, IEvenNumberEvent>(
    Evolve: (s, e) => e switch
    {
        EvenNumberAdded oddNumberAdded => new EvenViewState(s.EvenState + oddNumberAdded.Value),
        EvenNumberMultiplied oddNumberMultiplied => new EvenViewState(s.EvenState * oddNumberMultiplied.Multiplier),
        _ => s
    },
    InitialState: () => new EvenViewState(0)
);
