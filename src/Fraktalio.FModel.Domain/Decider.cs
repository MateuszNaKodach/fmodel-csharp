namespace Fraktalio.FModel.Domain;

/**
*  [Decider] is a datatype that represents the main decision-making algorithm.
 * It has three generic parameters `C`, `S`, `E` , representing the type of the values that [Decider] may contain or use.
 * [Decider] can be specialized for any type `C` or `S` or `E` because these types does not affect its behavior.
 * [Decider] behaves the same for `C`=[Int] or `C`=`OddNumberCommand`.
 *
 * If you want to know more about this pattern please check out following resources:
 * - https://event-driven.io/en/how_to_effectively_compose_your_business_logic/ (How to effectively compose your business logic)
 * - https://github.com/fraktalio/fmodel (implementation with examples on whole domains in Kotlin)
 * - https://bettersoftwaredesign.pl/episodes/50 (Better Software Design - 50. O implementacji logiki biznesowej z Decider Pattern z Oskarem Dudyczem)
 *
 * @param C Command
 * @param S State
 * @param E Event
 * @property decide A function/lambda that takes command of type [C] and input state of type [S] as parameters, and returns/emits the flow of output events [Flow]<[E]>
 * @property evolve A function/lambda that takes input state of type [S] and input event of type [E] as parameters, and returns the output/new state [S]
 * @property initialState A starting point / An initial state of type [S]
 */
public interface IDecider<in TCommand, TState, TEvent>
    where TCommand : class
    where TState : class?
    where TEvent : class
{
    IEnumerable<TEvent> Decide(TCommand c, TState? s);

    TState? Evolve(TState? s, TEvent e);

    TState? InitialState { get; }
}

public record Decider<TCommand, TState, TEvent>(
    Func<TCommand, TState?, IEnumerable<TEvent>> Decide,
    Func<TState?, TEvent, TState?> Evolve,
    TState? InitialState
) : IDecider<TCommand, TState, TEvent>
    where TCommand : class
    where TState : class?
    where TEvent : class
{
    IEnumerable<TEvent> IDecider<TCommand, TState, TEvent>.Decide(TCommand c, TState? s) => Decide.Invoke(c, s);

    TState? IDecider<TCommand, TState, TEvent>.Evolve(TState? s, TEvent e) => Evolve.Invoke(s, e);
}
