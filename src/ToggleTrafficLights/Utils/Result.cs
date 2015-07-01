using System;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils
{
// ReSharper disable once InconsistentNaming
    public interface Result<T>
    {
         
    }

    public sealed class Failure<T> : Result<T>
    {
        public Failure(string reason)
        {
            Reason = reason;
        }
        public string Reason { get; private set; }
    }
    public sealed class Success<T> : Result<T>
    {
        public Success(T value)
        {
            Value = value;
        }

        public T Value { get; private set; }
    }

    public static class Result
    {
        public static Success<T> Success<T>(T value)
        {
            return new Success<T>(value);
        }

        public static Failure<T> Failure<T>(string reason)
        {
            return new Failure<T>(reason);
        }

        public static bool IsSuccess<T>(this Result<T> result)
        {
            return result is Success<T>;
        }
        public static bool IsFailure<T>(this Result<T> result)
        {
            return result is Failure<T>;
        }
        public static T GetSuccessValue<T>(this Result<T> result)
        {
            var success = result as Success<T>;
            if (success != null)
            {
                return success.Value;
            }
            else
            {
                throw new ArgumentException("Result is not Success", "result");
            }
        }
        public static string GetFailureReason<T>(this Result<T> result)
        {
            var failure = result as Failure<T>;
            if (failure != null)
            {
                return failure.Reason;
            }
            else
            {
                throw new ArgumentException("Result is not Failure", "result");
            }
        }

    }
}