## Logging
Library has logging mechanism implemented.
They can be used as library debug tool and source of information about library behavior.

By default, library logs are disabled.
To enable log create object that implements `PrivmxEndpointCsharpExtra.Logging.ILibraryLogger` interface and pass it as argument to `PrivmxEndpointCsharpExtra.Internals.Logger.SetLogger` static function.

## Native calls dispatch

User can customize how asynchronous calls are dispatched onto the thread pool or custom task scheduler.
By default, library uses thread pool to execute asynchronous calls, and provides minimal support fot task cancellation.
                        
To customize how native calls are dispatched create class that implements `PrivmxEndpointCsharpExtra.Internals.IWrapperCallsExecutor` and pass object of this class to `PrivmxEndpointCsharpExtra.Internals.WrapperCallsExecutor.SetExecutor` static function.