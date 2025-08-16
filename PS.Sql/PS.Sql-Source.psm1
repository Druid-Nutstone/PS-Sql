#region Self-loading SqlClient for .NET 8 and PS.Sql module

$script:BasePath = $PSScriptRoot
$script:PlatformDllPath = Join-Path $BasePath 'runtimes\win\lib\net8.0'
$script:NativePath = Join-Path $BasePath 'runtimes\win-x64\native'  # location of native libraries
$script:ModuleDllPath = $BasePath  # location of PS.Sql.dll

Write-Host "Loading required modules .."    

$env:COREHOST_TRACE = "1"
$env:COREHOST_TRACE_VERBOSITY = "4"

function Import-NativeLibrary {
    param(
        [string]$dllName,
        [string]$folder = $script:NativePath
    )
    $fullPath = Join-Path $folder $dllName
    if (Test-Path $fullPath) {
        try {
            [System.Runtime.InteropServices.NativeLibrary]::Load($fullPath) | Out-Null
            Write-Host "✅ Loaded native library: $dllName"
        } catch {
            Write-Host "❌ Failed to load native library '$dllName': $_"
        }
    } else {
        Write-Host "⚠️ Native library not found: $fullPath"
    }
}

function Import-ManagedAssembly {
    param(
        [string]$dllName,
        [string]$folder = $script:BasePath
    )
    $fullPath = Join-Path $folder $dllName
    if (Test-Path $fullPath) {
        try {
            Add-Type -Path $fullPath -ErrorAction Stop
            Write-Host "Imported managed assembly: $dllName"
        } catch {
            Write-Host "Failed to import managed assembly '$dllName': $_"
        }
    } else {
        Write-Host "Managed assembly not found: $fullPath"
    }
}
Import-NativeLibrary 'Microsoft.Data.SqlClient.SNI.dll' -folder $script:NativePath
# Microsoft.Identity.Client.dll 
Import-ManagedAssembly 'Microsoft.Identity.Client.dll' -folder $script:ModuleDllPath
Import-ManagedAssembly 'Microsoft.SqlServer.Server.dll' -folder $script:ModuleDllPath
# Load platform-specific Microsoft.Data.SqlClient.dll for .NET 8
Import-ManagedAssembly 'Microsoft.Data.SqlClient.dll' -folder $script:PlatformDllPath

# Import your custom cmdlet module PS.Sql.dll
$psSqlPath = Join-Path $script:ModuleDllPath 'PS.Sql.dll'
if (Test-Path $psSqlPath) {
    try {
        Import-Module $psSqlPath -ErrorAction Stop -Verbose
    } catch {
        Write-Warning "Failed to import PS.Sql.dll: $_"
    }
} else {
    Write-Warning "PS.Sql.dll not found at: $psSqlPath"
}

#endregion
