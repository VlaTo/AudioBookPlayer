using System.Threading.Tasks;

namespace LibraProgramming.Xamarin.Interaction.Extensions
{
    public static class InteractionRequestExtensions
    {
        public static Task RaiseAsync<TContext>(this InteractionRequest<TContext> request, TContext context)
            where TContext : InteractionRequestContext
        {
            var tcs = new TaskCompletionSource<bool>();

            request.Raise(context, () => tcs.SetResult(true));

            return tcs.Task;
        }
    }
}
