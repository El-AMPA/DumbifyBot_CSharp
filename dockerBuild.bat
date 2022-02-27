MSBuild.exe .\Dumbify_CSharp\Dumbify_CSharp.csproj /t:ContainerBuild /p:Configuration=Release
docker run -dt --rm dumbifycsharp