param(
    [string]$version,
    [switch]$rebuild
)

$MsBuild = (Get-ChildItem -Path "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\msbuild.exe").FullName

& ".\build.nuspecs.ps1" $version -NoNewWindow

$SolutionFile = Get-ChildItem ..\src -Recurse | Where-Object {$_.Extension -eq ".sln"}
nuget restore $SolutionFile.FullName

foreach ($NugetSpec in Get-ChildItem ..\src -Recurse | Where-Object {$_.Extension -eq ".nuspec"})
{
	$ProjectFile = Get-ChildItem $NugetSpec.DirectoryName | Where-Object {$_.Extension -eq ".csproj"}

	$BuildArgs = @{
		FilePath = $MsBuild
		ArgumentList = $ProjectFile.FullName,  "/p:Configuration=Release /t:Rebuild"
		Wait = $true
	}
	
	if ($rebuild)
	{
		Start-Process @BuildArgs -NoNewWindow
	}

	#nuget pack $ProjectFile.FullName -BasePath $NugetSpec.DirectoryName -Build -Prop Configuration=Release -Prop FilePath=$MsBuild -NonInteractive -IncludeReferencedProjects
	nuget pack $NugetSpec.FullName -BasePath $NugetSpec.DirectoryName -Prop Configuration=Release -NonInteractive -IncludeReferencedProjects
}