$ErrorActionPreference = 'Stop'

Invoke-PSLoader -Name "PS.Sql" -Verbose

class PSTable {
    [string]$FirstName
    [string]$LastName

    PSTable(){}
    
    PSTable([string]$first, [string]$last) {
        $this.FirstName = $first
        $this.LastName = $last
    }

    [string] GetFullName() {
        return "$($this.FirstName) $($this.LastName)"
    }
}


$databaseName = "PowerShellTests"
$tableName = "Table1"

$connection = New-MsSqlConnection -Database "master"

Write-Host "Ended showing modules"

New-MsSqlDatabase -Connection $connection -DatabaseName $databaseName

New-MsSqlTable -Connection $connection -DatabaseName $databaseName -TableType PSTable -TableName $tableName

$data = @()

$data += [PSTable]::new("A new first", "A new last")

Add-MsSqlObjectToTable -Connection $connection -DatabaseName $databaseName -TableName $tableName -Data $data

Get-MsSqlObjectFromTable -Connection $connection -DatabaseName $databaseName -TableName $tableName -TableType PSTable