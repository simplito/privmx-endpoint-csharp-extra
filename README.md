privmx-endpoint-csharp-extra is a library that is build on top of
the [privmx-endpoint-csharp](https://github.com/simplito/privmx-endpoint-csharp) library.
It provides additional functionality that is not available in the base library.
The library is designed to be used in conjunction with the base library and is not intended to be used on its own.

## Project structure

- `PrivmxEndpointCsharpExtra` - main library of this project
- `InternalTools` - internal library used by other lib
- `UnionSourceGenerator` - source generator that generates implementation for structs implementing `Union<T1, T2>` and `Union<T1, T2, T3>` interfaces
