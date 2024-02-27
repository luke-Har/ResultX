﻿namespace ResultMonad
{
    /// <summary>
    /// Internal value has copy semantics, relevant for structs
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TError">Potential error</typeparam>
    public readonly struct Result<TValue, TError>
        where TValue : notnull
        where TError : System.Enum
    {
        public delegate U MatchOk<T, U>(T item) where T : TValue;
        public delegate U MatchError<T, U>(T item) where T : System.Enum;

        public delegate void MatchOkNoRet<T>(T item) where T : TValue;
        public delegate void MatchErrorNoRet<T>(T item) where T : System.Enum;

        readonly TValue Value;
        readonly TError Error;
        public readonly bool Faulted;

        /// <summary>
        /// A value being given means result is successfull
        /// </summary>
        /// <param name="value"></param>
        public Result(TValue value)
        {
            Faulted = false;
            Value = value;
        }

        /// <summary>
        /// Parameterless constructor is used to show an error (TError) has occured
        /// </summary>
        public Result(TError error)
        {
            Faulted = true;
            Error = error;
        }


        public T Match<T>(MatchOk<TValue, T> OkPath, MatchError<TError, T> ErrPath)
        {
            switch (Faulted)
            {
                case true:
                    return ErrPath.Invoke(Error);
                case false:
                    return OkPath.Invoke(Value);
            }
        }

        public void Match(MatchOkNoRet<TValue> OkPath, MatchErrorNoRet<TError> ErrPath)
        {
            switch (Faulted)
            {
                case true:
                    ErrPath.Invoke(Error);
                    break;
                case false:
                    OkPath.Invoke(Value);
                    break;
            }
        }

        /// <summary>
        /// Trusts that the value is valid
        /// </summary>
        /// <returns></returns>
        public TValue UnWrap()
        {
            return Value;
        }

        /// <summary>
        /// Functionally the same as UnWrap, but will throw if there is an error.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public TValue Expect(string errorMessage)
        {
            if(Faulted) throw new Exception(errorMessage);
            return Value;
        }

        public override string ToString()
        {
            return Faulted ? Error.ToString() : Value.ToString()!;
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