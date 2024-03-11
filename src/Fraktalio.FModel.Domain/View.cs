namespace Fraktalio.FModel.Domain;

public interface IView<TState, in TEvent>
    where TState : class?
    where TEvent : class
{
    TState Evolve(TState s, TEvent e);

    TState InitialState { get; }
}

public record View<TState, TEvent>(
    Func<TState, TEvent, TState> Evolve,
    TState InitialState
) : IView<TState, TEvent>
    where TState : class?
    where TEvent : class
{
    TState IView<TState, TEvent>.Evolve(TState s, TEvent e) => Evolve.Invoke(s, e);

    TState IView<TState, TEvent>.InitialState => InitialState;
}
