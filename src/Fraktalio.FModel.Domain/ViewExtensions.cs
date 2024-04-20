namespace Fraktalio.FModel.Domain
{
    internal static class ViewExtensions
    {
        internal static View<TS, TE> AsView<TS, TE>(this InternalView<TS, TS, TE> internalView)
            where TS : class?
            where TE : class =>
            new(
                internalView.Evolve,
                internalView.InitialState
            );
    }
}
