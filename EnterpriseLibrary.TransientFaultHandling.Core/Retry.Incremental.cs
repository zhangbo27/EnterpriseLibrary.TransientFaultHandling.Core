﻿namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling
{
    using System;
    using System.Threading.Tasks;

    public static partial class Retry
    {
        /// <summary>
        /// Repetitively executes the specified action while it satisfies the specified retry strategy.
        /// </summary>
        /// <typeparam name="TResult">he type of result expected from the executable action.</typeparam>
        /// <param name="func">A delegate that represents the executable action that returns the result of type <typeparamref name="TResult" />.</param>
        /// <param name="retryCount">The number of retry attempts.</param>
        /// <param name="isTransient">The predicate function to detect whether the specified exception is transient.</param>
        /// <param name="retryingHandler">The callback function that will be invoked whenever a retry condition is encountered.</param>
        /// <param name="initialInterval">The initial interval that will apply for the first retry.</param>
        /// <param name="increment">The incremental time value that will be used to calculate the progressive delay between retries.</param>
        /// <param name="firstFastRetry">true to immediately retry in the first attempt; otherwise, false. The subsequent retries will remain subject to the configured retry interval.</param>
        /// <returns>The result from the action.</returns>
        /// <exception cref="ArgumentNullException">func</exception>
        public static TResult Incremental<TResult>(
            Func<TResult> func,
            int? retryCount = null,
            Func<Exception, bool> isTransient = null,
            EventHandler<RetryingEventArgs> retryingHandler = null,
            TimeSpan? initialInterval = null,
            TimeSpan? increment = null,
            bool? firstFastRetry = null)
        {
            Guard.ArgumentNotNull(func, nameof(func));

            return Execute(
                func,
                CreateIncremental(retryCount, initialInterval, increment, firstFastRetry),
                isTransient,
                retryingHandler);
        }

        /// <summary>
        /// Repetitively executes the specified action while it satisfies the specified retry strategy.
        /// </summary>
        /// <param name="action">A delegate that represents the executable action that doesn't return any results.</param>
        /// <param name="retryCount">The number of retry attempts.</param>
        /// <param name="isTransient">The predicate function to detect whether the specified exception is transient.</param>
        /// <param name="retryingHandler">The callback function that will be invoked whenever a retry condition is encountered.</param>
        /// <param name="initialInterval">The initial interval that will apply for the first retry.</param>
        /// <param name="increment">The incremental time value that will be used to calculate the progressive delay between retries.</param>
        /// <param name="firstFastRetry">true to immediately retry in the first attempt; otherwise, false. The subsequent retries will remain subject to the configured retry interval.</param>
        /// <exception cref="ArgumentNullException">action</exception>
        public static void Incremental(
            Action action,
            int? retryCount = null,
            Func<Exception, bool> isTransient = null,
            EventHandler<RetryingEventArgs> retryingHandler = null,
            TimeSpan? initialInterval = null,
            TimeSpan? increment = null,
            bool? firstFastRetry = null)
        {
            Guard.ArgumentNotNull(action, nameof(action));

            Execute(
                action,
                CreateIncremental(retryCount, initialInterval, increment, firstFastRetry),
                isTransient,
                retryingHandler);
        }

        /// <summary>
        /// Repeatedly executes the specified asynchronous function while it satisfies the current retry policy.
        /// </summary>
        /// <typeparam name="TResult">he type of result expected from the executable asynchronous function.</typeparam>
        /// <param name="func">A asynchronous function that returns a started task (also known as "hot" task).</param>
        /// <param name="retryCount">The number of retry attempts.</param>
        /// <param name="isTransient">The predicate function to detect whether the specified exception is transient.</param>
        /// <param name="retryingHandler">The callback function that will be invoked whenever a retry condition is encountered.</param>
        /// <param name="initialInterval">The initial interval that will apply for the first retry.</param>
        /// <param name="increment">The incremental time value that will be used to calculate the progressive delay between retries.</param>
        /// <param name="firstFastRetry">true to immediately retry in the first attempt; otherwise, false. The subsequent retries will remain subject to the configured retry interval.</param>
        /// <returns>Returns a task that will run to completion if the original task completes successfully (either the first time or after retrying transient failures). If the task fails with a non-transient error or the retry limit is reached, the returned task will transition to a faulted state and the exception must be observed.</returns>
        /// <exception cref="ArgumentNullException">func</exception>
        public static Task<TResult> IncrementalAsync<TResult>(
            Func<Task<TResult>> func,
            int? retryCount = null,
            Func<Exception, bool> isTransient = null,
            EventHandler<RetryingEventArgs> retryingHandler = null,
            TimeSpan? initialInterval = null,
            TimeSpan? increment = null,
            bool? firstFastRetry = null)
        {
            Guard.ArgumentNotNull(func, nameof(func));

            return ExecuteAsync(
                func,
                CreateIncremental(retryCount, initialInterval, increment, firstFastRetry),
                isTransient,
                retryingHandler);
        }

        /// <summary>
        /// Repeatedly executes the specified asynchronous function while it satisfies the current retry policy.
        /// </summary>
        /// <param name="func">A asynchronous function that returns a started task (also known as "hot" task).</param>
        /// <param name="retryCount">The number of retry attempts.</param>
        /// <param name="isTransient">The predicate function to detect whether the specified exception is transient.</param>
        /// <param name="retryingHandler">The callback function that will be invoked whenever a retry condition is encountered.</param>
        /// <param name="initialInterval">The initial interval that will apply for the first retry.</param>
        /// <param name="increment">The incremental time value that will be used to calculate the progressive delay between retries.</param>
        /// <param name="firstFastRetry">true to immediately retry in the first attempt; otherwise, false. The subsequent retries will remain subject to the configured retry interval.</param>
        /// <returns>Returns a task that will run to completion if the original task completes successfully (either the first time or after retrying transient failures). If the task fails with a non-transient error or the retry limit is reached, the returned task will transition to a faulted state and the exception must be observed.</returns>
        /// <exception cref="ArgumentNullException">func</exception>
        public static Task IncrementalAsync(
            Func<Task> func,
            int? retryCount = null,
            Func<Exception, bool> isTransient = null,
            EventHandler<RetryingEventArgs> retryingHandler = null,
            TimeSpan? initialInterval = null,
            TimeSpan? increment = null,
            bool? firstFastRetry = null)
        {
            Guard.ArgumentNotNull(func, nameof(func));

            return ExecuteAsync(
                func,
                CreateIncremental(retryCount, initialInterval, increment, firstFastRetry),
                isTransient,
                retryingHandler);
        }

        private static Incremental CreateIncremental(
            int? retryCount = null,
            TimeSpan? initialInterval =null, 
            TimeSpan? increment = null,
            bool? firstFastRetry = null,
            string name = null) => new Incremental(
                name,
                retryCount ?? RetryStrategy.DefaultClientRetryCount,
                initialInterval ?? RetryStrategy.DefaultRetryInterval,
                increment ?? RetryStrategy.DefaultRetryIncrement,
                firstFastRetry ?? RetryStrategy.DefaultFirstFastRetry);
    }
}
