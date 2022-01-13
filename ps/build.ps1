###
# build, create and start windows background service.
# remove any pre-existing service and publish artifacts.
###

Write-Output "Must be run as Administrator!"

$serviceName = "rainway_background_service"


sc.exe query $serviceName
Write-Output "LastExitCode $LastExitCode"
if ($LastExitCode -eq 0) {
  Write-Output "Stop..."
  sc.exe stop $serviceName 
  Write-Output "Sleep..."
  Start-Sleep -Seconds 20
  Write-Output "Delete..."
  sc.exe delete $serviceName
  Write-Output "deleted $serviceName"
}
else {
  Write-Output "No need to delete the service" 
}

Remove-Item -Force -Recurse .\publish

dotnet publish --output "publish"
$location = Split-Path -Parent ($PSCommandPath)
$publishDir = Resolve-Path(Join-Path -Path $location -ChildPath "..\publish" )

Write-Output "publish dir: $publishDir"
$binPath = "$publishDir\rainway-background-service.exe" 
Write-Output "bin path: $binPath"
$createCmd = "sc.exe create '$serviceName' binPath=$binPath"
Write-Output "cmd: $createCmd"
Invoke-Expression $createCmd

Write-Output "Start service"

sc.exe start $serviceName
