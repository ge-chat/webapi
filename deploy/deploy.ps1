$projectPath = "..\src\Geofy.WebAPi"
$outputFolder = "output"
$publishPath = "bin"
$arhiveName = "$outputFolder.zip"

cd $projectPath

dotnet restore
dotnet build
dotnet publish -o "$publishPath\$outputFolder" 
Compress-Archive -Path $publishPath -DestinationPath "$publishPath\$arhiveName"

cd C:\geofy\Geofy\deploy
pwd