Library is created in layered way and user may at any point opt out of using features of the library.

## Layer 1 - default extension methods
This layer is the base layer of the library. 
It provides basic support for asynchronous operations for classes such as:
- `Connection`
- `ThreadApi`
- `StoreApi`
- `InoboxApi`
- etc.

All extension methods that add asynchronous support are in [DefaultExtension](../PrivmxEndpointCsharpExtra/DefaultExtensions).

## Layer 2 - managed wrappers

This layer provides wrappers for existing api that exposes base library functionality in more 'dotnet' way.
Wrappers are in `PrivmxEndpointCsharpExtra.api` and are made of:
- `AsyncConnection`
- `AsyncEventQueue`
- `AsyncStoreApi`
- `AsyncThreadApi`
- `AsyncInboxApi`

This layer is meant to manage various api related functionalities as objects implementing `IDisposable` and `IAsyncDisposable` interfaces.
It's common to see events exposed as `IObservable` streams.
Async wrappers on this layer are made with testability in mind and can have their dependencies injected easily for test mocking purposes.

## Layer 3 - connection container

This layer provides opinionated way of managing user connection in the form of single container with all APIs grouped together.
The layer is made of single class named `ConnectionSession` that is meant to be used as a single point of entry for all api operations.

## Utility
Aside from extending main functionality of the library, there are also utility classes that are responsible for:
- controlling where and how asynchronous calls are dispatched onto the thread pool or custom task scheduler
- providing way to listen to library logs
- providing way to track unobserved exceptions in asynchronous calls

For further information please refer to [03-Customization](03-Customization.md) article.