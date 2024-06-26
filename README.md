# ResultMonad

A way to show that a function may error without using exceptions:
```csharp
public enum FooError
{
	PotentialError,
	AnotherPotentialError
}

public Result<Foo, FooError> SomethingThatCouldError()
{
	if(foo.bar != desiredValue) return Result<Foo,FooError>.Err(FooError.PotentialError);
	if(foo.name != name) return Result<Foo,FooError>.Err(FooError.AnotherPotentialError);

	return Result<Foo,FooError>.Ok(foo);
}
```

You can then match on the result:
```csharp
var result = SomethingThatCouldError();
var myValue = result.Match(
		value => value,
		err => Foo.Empty);
```

You could also execute arbitrary code with no return:
```csharp
var result = SomethingThatCouldError();
result.Match(
	(value) => {
		DoSomething();
	},
	(err) => {
		DoSomethingElse();
	});
```

If you know that the result contains a value or you're just lazy, you can unwrap or expect the result:
```csharp
var result = SomethingThatCouldError();
var myValue = result.UnWrap(); //Will throw a generic error if no value
///// or
var myValue = result.Expect("foo should have a value here"); //Will throw an error with message if no value
```
