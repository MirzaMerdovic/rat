using System;
using System.Threading.Tasks;

namespace Rat.Providers.Resiliency
{
    public interface IRetryProvider
    {
        Task<TResult> RetryOn<TException, TResult>(
            Func<TException, bool> exceptionPredicate,
            Func<TResult, bool> resultPredicate,
            Func<Task<TResult>> execute)
            where TException : Exception;
    }
}