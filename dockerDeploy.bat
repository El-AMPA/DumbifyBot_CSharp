MSBuild.exe .\Dumbify_CSharp\Dumbify_CSharp.csproj /t:ContainerBuild /p:Configuration=Release
docker image tag dumbifycsharp telegrambotsregistry.azurecr.io/dumbifycsharp:latest
docker push telegrambotsregistry.azurecr.io/dumbifycsharp:latest