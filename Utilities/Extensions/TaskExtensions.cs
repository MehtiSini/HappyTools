using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace HappyTools.Utilities.Extensions
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Run task safely ie onError action called on any exception occured
        /// </summary>
        /// <param name="task"></param>
        /// <param name="onError"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task RunSafe(this Task task, Action<Exception> onError = null,
            CancellationToken token = default)
        {
            await task.RunSafe(onError, token);
        }

        /// <summary>
        /// Run task safely ie onError action called on any exception occured
        /// </summary>
        /// <param name="task"></param>
        /// <param name="onError"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<T> RunSafe<T>(this Task<T> task, Action<Exception> onError = null,
            CancellationToken token = default)
        {
            await task.RunSafe(onError, token);
            return task.Result;
        }
        public static Task ContinueWithStandard<T>(this Task<T> task, Action<Task<T>> continuation)
        {

            return task.ContinueWith(continuation, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }
        public static void InvokeAsync<TEventArgs>(this EventHandler<TEventArgs> @event, object sender,
            TEventArgs args, AsyncCallback ar, object userObject = null)
            where TEventArgs : class
        {
            var listeners = @event.GetInvocationList();
            foreach (var t in listeners)
            {
                var handler = (EventHandler<TEventArgs>)t;
                handler.BeginInvoke(sender, args, ar, userObject);
            }
        }
        /// <summary>
        /// This method can be used to ignore the result of a Task without
        /// losing the ability to throw the exception if the task fails.
        /// </summary>
        /// <example>
        /// <code>
        ///     Task.Run(() => ...).IgnoreResult();
        /// </code>
        /// </example>
        /// <param name="task">Task to ignore</param>
        /// <param name="faultHandler">Optional handler for the exception; if null then method throws on UI thread.</param>
        /// <param name="member">Caller name</param>
        /// <param name="lineNumber">Line number.</param>
        public static void IgnoreResult(this Task task, Action<Exception> faultHandler = null, [CallerMemberName] string member = "", [CallerLineNumber] int lineNumber = 0)
        {
            task.ContinueWith(tr =>
                {
                    Debug.WriteLine("Encountered {0} at {1}, line #{2}",
                        task.Exception.GetType(), member, lineNumber);
                    Debug.WriteLine(task.Exception.Flatten());

                    if (faultHandler != null)
                    {
                        faultHandler.Invoke(task.Exception);
                    }
                    else
                    {
                        Debug.WriteLine("WARNING: exception {0} was ignored!", task.Exception.GetType());
                    }

                }, CancellationToken.None,
                TaskContinuationOptions.OnlyOnFaulted,
                TaskScheduler.FromCurrentSynchronizationContext());
        }
        public static void TryStart(this Task task)
        {
            try
            {
                task.Start();
            }
            catch (InvalidOperationException) { }
        }

        [DebuggerStepThrough]
        public static ConfiguredTaskAwaitable<TResult> AnyContext<TResult>(this Task<TResult> task)
        {
            return task.ConfigureAwait(continueOnCapturedContext: false);
        }

        [DebuggerStepThrough]
        public static ConfiguredTaskAwaitable AnyContext(this Task task)
        {
            return task.ConfigureAwait(continueOnCapturedContext: false);
        }

    }
}
