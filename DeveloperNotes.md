# Developing nuget package with unreleased native plugin
This list of instruction should be applied if you want to set up development environment in which development of `PrivmxEndpointCsharpExtra` depends on not yet released version of `PrivmxEndpointCsharp`.

Steps:
1. Build and pack `PrivmxEndpointCsharp`, see [package developer notes](https://github.com/simplito/privmx-endpoint-csharp/blob/main/DeveloperNotes.md#creating-nuget-release-with-native-packages) to get information on how to build native wrapper
2. Clear nuget cache with `nuget locals all -clear`
3. Copy nuget package from step `1.` to `./LocalPackages` 
4. Restore nuget packages in project/reload project

# Creating nuget release
In `./PrivmxEndpointCsharpExtra` directory run:
```bash
dotnet pack -c Release
```
nuget package will have dependency on `PrivmxEndpointCsharp` and contain include `InernalTools` dll as its private dependency. 