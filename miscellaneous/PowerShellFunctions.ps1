Function Publish-Chldr_Server {
    dotnet publish chldr_server.csproj -c Release -o ./bin/published
    Compress-Archive -Path ./bin/published/* -DestinationPath ./published.zip
    az webapp deploy --resource-group Dosham_group --name Dosham --src-path ./published.zip
    Remove-Item -r -force ./published.zip
    Remove-Item -r -force ./bin/published
}