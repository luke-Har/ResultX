using System.Reflection.Metadata.Ecma335;

namespace ResultMonad
{
    /// <summary>
    /// A value that can either contain a value or error
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TError">Potential error</typeparam>
    public readonly record struct Result<TValue, TError>
        where TValue : notnull
        where TError : System.Enum
    {
        readonly UnionX.Union<TValue, TError> Value;
        /// <summary>
        /// A value being given means result is successful
        /// </summary>
        /// <param name="value"></param>
        private Result(TValue value)
        {
            Value = new(value);
        }

        /// <summary>
        /// Used to show an error (TError) has occured
        /// </summary>
        private Result(TError error)
        {
            Value = new(error);
        }

        /// <summary>
        /// Used to define two code paths that can be taken, both of which return a value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="OkPath"></param>
        /// <param name="ErrPath"></param>
        /// <returns></returns>
        public T Match<T>(Func<TValue, T> OkPath, Func<TError, T> ErrPath)
        {
            return Value.Match(OkPath, ErrPath);
        }

        /// <summary>
        /// Trusts that the value is valid, will throw if no value
        /// </summary>
        /// <returns></returns>
        public TValue UnWrap()
        {
            return Value.Match(
                value => value,
                err => throw new ResultNoValueException());
        }

        /// <summary>
        /// Functionally the same as UnWrap, but will throw with user defined message
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public TValue Expect(string errorMessage)
        {
            return Value.Match(
                value => value,
                err => throw new ResultNoValueException(errorMessage));
        }

        public override string ToString()
        {
            return Value.Match(value => value.ToString(), err => err.ToString())!;
        }

        public static Result<TValue, TError> Ok(TValue value)
        {
            return new Result<TValue, TError>(value);
        }
        public static Result<TValue, TError> Err(TError error)
        {
            return new Result<TValue, TError>(error);
        }
    }
}