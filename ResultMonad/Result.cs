namespace ResultMonad
{
    /// <summary>
    /// Internal value has reference semantics, relevant for structs
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TError">Potential error</typeparam>
    public ref struct RefResult<TValue, TError>
        where TValue : notnull
        where TError : struct, IResultError<TError>
    {
        public delegate U MatchOk<T, U>(ref T item) where T : TValue;
        public delegate U MatchError<T, U>(T item) where T : struct, IResultError<TError>;

        public delegate void MatchOkNoRet<T>(ref T item) where T : TValue;
        public delegate void MatchErrorNoRet<T>(T item) where T : struct, IResultError<TError>;

        ref TValue Value;
        TError Error { get; }
        public bool Faulted { get; }

        /// <summary>
        /// A value being given means result is successfull
        /// </summary>
        /// <param name="value"></param>
        public RefResult(ref TValue value)
        {
            Faulted = false;
            Error = default;
            Value = ref value;
        }

        /// <summary>
        /// Parameterless constructor is used to show an error (TError) has occured
        /// </summary>
        public RefResult()
        {
            Faulted = true;
            Error = TError.ErrorInstance;
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

        public override string ToString()
        {
            return Faulted ? TError.ErrorInstance.Message : Value.ToString()!;
        }

        public static RefResult<TValue, TError> Ok(ref TValue value)
        {
            return new RefResult<TValue, TError>(ref value);
        }
        public static RefResult<TValue, TError> Err()
        {
            return new RefResult<TValue, TError>();
        }
    }

    /// <summary>
    /// Internal value has copy semantics, relevant for structs
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TError">Potential error</typeparam>
    public readonly struct Result<TValue, TError>
        where TValue : notnull
        where TError : struct, IResultError<TError>
    {
        public delegate U MatchOk<T, U>(T item) where T : TValue;
        public delegate U MatchError<T, U>(T item) where T : struct, IResultError<TError>;

        public delegate void MatchOkNoRet<T>(T item) where T : TValue;
        public delegate void MatchErrorNoRet<T>(T item) where T : struct, IResultError<TError>;

        TValue Value { get; }
        TError Error { get; }
        public bool Faulted { get; }

        /// <summary>
        /// A value being given means result is successfull
        /// </summary>
        /// <param name="value"></param>
        public Result(TValue value)
        {
            Faulted = false;
            Error = default;
            Value = value;
        }

        /// <summary>
        /// Parameterless constructor is used to show an error (TError) has occured
        /// </summary>
        public Result()
        {
            Faulted = true;
            Error = TError.ErrorInstance;
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

        public override string ToString()
        {
            return Faulted ? TError.ErrorInstance.Message : Value.ToString()!;
        }

        public static Result<TValue, TError> Ok(TValue value)
        {
            return new Result<TValue, TError>(value);
        }
        public static Result<TValue, TError> Err()
        {
            return new Result<TValue, TError>();
        }
    }

    public interface IResultError<T> where T : struct
    {
        public abstract string Message { get; }
        public static abstract T ErrorInstance { get; }

    }
}