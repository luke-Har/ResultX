using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultMonad
{
    /// <summary>
    /// Internal value has reference semantics, relevant for structs
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TError">Potential error</typeparam>
    public readonly ref struct RefResult<TValue, TError>
        where TValue : notnull
        where TError : System.Enum
    {
        public delegate U MatchOk<T, U>(ref T item) where T : TValue;
        public delegate U MatchError<T, U>(T item) where T : System.Enum;

        public delegate void MatchOkNoRet<T>(ref T item) where T : TValue;
        public delegate void MatchErrorNoRet<T>(T item) where T : System.Enum;

        readonly ref TValue Value;
        readonly TError Error;
        public readonly bool Faulted;

        /// <summary>
        /// A value being given means result is successfull
        /// </summary>
        /// <param name="value"></param>
        public RefResult(ref TValue value)
        {
            Faulted = false;
            Value = ref value;
        }

        /// <summary>
        /// Parameterless constructor is used to show an error (TError) has occured
        /// </summary>
        public RefResult(TError error)
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
                    return OkPath.Invoke(ref Value);
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
                    OkPath.Invoke(ref Value);
                    break;
            }
        }

        /// <summary>
        /// Trusts that the value is valid
        /// </summary>
        /// <returns></returns>
        public ref TValue UnWrap()
        {
            return ref Value;
        }

        /// <summary>
        /// Functionally the same as UnWrap, but will throw if there is an error.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public ref TValue Expect(string errorMessage)
        {
            if (Faulted) throw new Exception(errorMessage);
            return ref Value;
        }

        public override string ToString()
        {
            return Faulted ? Error.ToString() : Value.ToString()!;
        }

        public static RefResult<TValue, TError> Ok(ref TValue value)
        {
            return new RefResult<TValue, TError>(ref value);
        }
        public static RefResult<TValue, TError> Err(TError error)
        {
            return new RefResult<TValue, TError>(error);
        }
    }
}
