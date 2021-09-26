using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FirLib.Core.Patterns.ObjectPooling;

namespace FirLib.Core
{
    /// <summary>
    /// Some helper methods.
    /// </summary>
    public static class FirLibExtensions
    {
        public static async void FireAndForget(this Task task)
        {
            await task;
        }

        public static string ToCommaSeparatedString(this IEnumerable enumerable, string separator = ", ")
        {
            var strBuilder = PooledStringBuilders.Current.TakeStringBuilder();
            try
            {
                switch (enumerable)
                {
                    case string?[] stringArray:
                    {
                        for(var loop=0; loop<stringArray.Length; loop++)
                        {
                            var actString = stringArray[loop];
                            if (actString == null) { actString = "<null>"; }

                            if (strBuilder.Length > 0)
                            {
                                strBuilder.Append(separator);
                            }
                            strBuilder.Append(actString);
                        }
                        break;
                    }

                    case IEnumerable<string> stringEnumerable:
                        foreach (var actStringLoop in stringEnumerable)
                        {
                            var actString = actStringLoop;
                            if (actString == null) { actString = "<null>"; }

                            if (strBuilder.Length > 0)
                            {
                                strBuilder.Append(separator);
                            }
                            strBuilder.Append(actString);
                        }
                        break;

                    default:
                        foreach(var actObj in enumerable)
                        {
                            if(actObj == null){ continue; }

                            var actString = actObj.ToString();
                            if (actString == null) { actString = "<null>"; }

                            if (strBuilder.Length > 0)
                            {
                                strBuilder.Append(separator);
                            }
                            strBuilder.Append(actString);
                        }
                        break;
                }

                return strBuilder.ToString();
            }
            finally
            {
                PooledStringBuilders.Current.ReRegisterStringBuilder(strBuilder);
            }
        }

        /// <summary>
        /// Posts the given action to the given SynchronizationContext also if it is null.
        /// If it is null, then a new task will be started.
        /// </summary>
        /// <param name="syncContext">The context to send the action to.</param>
        /// <param name="actionToPost">The action to be executed on the target thread.</param>
        /// <param name="actionIfNull">What should we do if weg get no SyncContext?</param>
        public static void PostAlsoIfNull(
            this SynchronizationContext? syncContext, 
            Action actionToPost, 
            ActionIfSyncContextIsNull actionIfNull = ActionIfSyncContextIsNull.InvokeUsingNewTask)
        {
            if (syncContext != null) { syncContext.Post((_) => actionToPost(), null); }
            else
            {
                switch (actionIfNull)
                {
                    case ActionIfSyncContextIsNull.InvokeSynchronous:
                        actionToPost();
                        break;

                    case ActionIfSyncContextIsNull.InvokeUsingNewTask:
                        Task.Factory.StartNew(actionToPost);
                        break;

                    case ActionIfSyncContextIsNull.DontInvoke:
                        break;

                    default:
                        throw new ArgumentException("actionIfNull", "Action " + actionIfNull + " unknown!");
                }
            }
        }

        /// <summary>
        /// Post the given action in an async manner to the given SynchronizationContext.
        /// </summary>
        /// <param name="syncContext">The target SynchronizationContext.</param>
        /// <param name="actionToPost">The action to be executed on the target thread.</param>
        /// <param name="actionIfNull">What should we do if we get no SyncContext?</param>
        public static Task PostAlsoIfNullAsync(
            this SynchronizationContext? syncContext, 
            Action actionToPost, 
            ActionIfSyncContextIsNull actionIfNull = ActionIfSyncContextIsNull.InvokeUsingNewTask)
        {
            TaskCompletionSource<object?> completionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);
            syncContext.PostAlsoIfNull(() =>
                {
                    try
                    {
                        actionToPost();
                        completionSource.SetResult(null);
                    }
                    catch (Exception ex)
                    {
                        completionSource.SetException(ex);
                    }
                }, 
                actionIfNull);
            return completionSource.Task;
        }
    }
}
