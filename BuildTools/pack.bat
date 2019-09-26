msbuild ..\ReactiveProperty.sln /p:Configuration=Release /t:Clean;Build
nuget pack ReactiveProperty.nuspec
