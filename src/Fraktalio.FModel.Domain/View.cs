namespace Fraktalio.FModel.Domain;

/// <summary>
/// An Interface for the View
/// Represents the event handling algorithm, responsible for translating the events into denormalized state, which is more adequate for querying.
/// </summary>
/// <typeparam name="TState">State</typeparam>
/// <typeparam name="TEvent">Event</typeparam>
public interface IView<TState, in TEvent>
    where TState : class?
    where TEvent : class
{
    /// <summary>
    /// A function/lambda that takes input state of type S and input event of type E as parameters, and returns the output/new state S
    /// </summary>
    TState Evolve(TState s, TEvent e);

    /// <summary>
    /// A starting point / An initial state of type S
    /// </summary>
    TState InitialState { get; }
}

public record View<TState, TEvent>(
    Func<TState, TEvent, TState> Evolve,
    Func<TState> InitialState
) : IView<TState, TEvent>
    where TState : class?
    where TEvent : class
{
    TState IView<TState, TEvent>.Evolve(TState s, TEvent e) => Evolve.Invoke(s, e);

    TState IView<TState, TEvent>.InitialState => InitialState();

    // public View<(TState, TState2), TEventSuper> Combine<TState2, TEvent1, TEvent2, TEventSuper>(
    //     View<TState2, TEvent> other)
    //     where TState2 : class?
    //     where TEvent1 : class, TEventSuper
    //     where TEvent2 : class, TEventSuper
    //     where TEventSuper : class
    // {
    //     var internalView1 = new InternalView<TState, TState, TEvent>(Evolve, InitialState);
    //     var internalView2 = new InternalView<TState2, TState2, TEvent>(other.Evolve, other.InitialState);
    //
    //     var combinedInternalView =
    //         InternalView<(TState, TState2), (TState, TState2), TEventSuper>
    //             .Combine<TState, TState2, TState, TState2, TEvent1, TEvent2, TEventSuper>(
    //                 internalView1, internalView2);
    //
    //     return combinedInternalView.AsView<(TState, TState2), TEventSuper>();
    // }
}

internal record InternalView<TStateIn, TStateOut, TEvent>(
    Func<TStateIn, TEvent, TStateOut> Evolve,
    Func<TStateOut> InitialState)
{
    InternalView<TStateIn, TStateOut, TEventNew> MapContraOnEvent<TEventNew>(Func<TEventNew, TEvent> f)
        where TEventNew : class => new(
        (si, en) => Evolve(si, f(en)),
        InitialState
    );

    InternalView<TStateInNew, TStateOutNew, TEvent> DimapOnState<TStateInNew, TStateOutNew>(
        Func<TStateInNew, TStateIn> fl,
        Func<TStateOut, TStateOutNew> fr) => new(
        (s, e) => fr(Evolve(fl(s), e)),
        () => fr(InitialState())
    );

    /// <summary>
    /// Contra (Left) map on S/State parameter - Contravariant
    /// 
    /// This method transforms the input state of the `InternalView` class.
    /// It takes a function `f` as a parameter, which is a function that transforms the input state from `TStateInNew` to `TStateIn`.
    /// The method returns a new `InternalView` instance with the `Evolve` function modified to use this transformation.
    /// The `DimapOnState` method is used here, with the second parameter being an identity function, meaning it doesn't change the output state.
    /// 
    /// </summary>
    /// <typeparam name="TStateInNew">New input State</typeparam>
    /// <param name="f">A function that transforms the input state from `TStateInNew` to `TStateIn`</param>
    /// <returns>A new `InternalView` instance with the `Evolve` function modified to use the transformation function `f`</returns>
    InternalView<TStateInNew, TStateOut, TEvent> MapContraOnState<TStateInNew>(
        Func<TStateInNew, TStateIn> f) => DimapOnState(f, x => x);

    /// <summary>
    /// (Right) map on S/State parameter - Covariant
    /// 
    /// This method transforms the output state of the `InternalView` class.
    /// It takes a function `f` as a parameter, which is a function that transforms the output state from `TStateOut` to `TStateOutNew`.
    /// The method returns a new `InternalView` instance with the `Evolve` function modified to use this transformation.
    /// The `DimapOnState` method is used here, with the first parameter being an identity function, meaning it doesn't change the input state.
    /// 
    /// </summary>
    /// <typeparam name="TStateOutNew">New output State</typeparam>
    /// <param name="f">A function that transforms the output state from `TStateOut` to `TStateOutNew`</param>
    /// <returns>A new `InternalView` instance with the `Evolve` function modified to use the transformation function `f`</returns>
    InternalView<TStateIn, TStateOutNew, TEvent> MapOnState<TStateOutNew>(
        Func<TStateOut, TStateOutNew> f) =>
        DimapOnState<TStateIn, TStateOutNew>(x => x, f);

    InternalView<TStateIn, TStateOutNew, TEvent> ApplyOnState<TStateOutNew>(
        InternalView<TStateIn, Func<TStateOut, TStateOutNew>, TEvent> ff) =>
        new(
            (s, e) => ff.Evolve(s, e)(Evolve(s, e)),
            () => ff.InitialState()(InitialState())
        );

    InternalView<TStateIn, (TStateOut, TStateOutNew), TEvent>
        ProductOnState<TStateOutNew>(InternalView<TStateIn, TStateOutNew, TEvent> fb)
    {
        var mappedState = fb.MapOnState(b => new Func<TStateOut, (TStateOut, TStateOutNew)>(a => (a, b)));
        return ApplyOnState(mappedState);
    }

    internal static InternalView<(TStateIn1, TStateIn2), (TStateOut1, TStateOut2), TEventSuper> Combine<TStateIn1,
        TStateIn2, TStateOut1, TStateOut2, TEvent1, TEvent2, TEventSuper>(
        InternalView<TStateIn1, TStateOut1, TEvent1> x,
        InternalView<TStateIn2, TStateOut2, TEvent2> y)
        where TEvent1 : class, TEventSuper
        where TEvent2 : class, TEventSuper
        where TEventSuper : class
    {
        var viewX = x.MapContraOnEvent<TEventSuper>(it => (it as TEvent1)!)
            .DimapOnState<(TStateIn1, TStateIn2), TStateOut1>(pair => pair.Item1, it => it);

        var viewY = y.MapContraOnEvent<TEventSuper>(it => (it as TEvent2)!)
            .DimapOnState<(TStateIn1, TStateIn2), TStateOut2>(pair => pair.Item2, it => it);

        return viewX.ProductOnState(viewY);
    }
}
