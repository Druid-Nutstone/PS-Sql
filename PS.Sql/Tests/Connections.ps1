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

# create connection
$connection = New-MsSqlConnection -Database "master"

# create database if it does not exist 
Write-Host "Connecting to DB .."
New-MsSqlDatabase -Connection $connection -DatabaseName $databaseName

# alter connection to point to new database 
$connection = Set-MsSqlConnection -Connection $connection -DatabaseName $databaseName

# remove table if it exists 
Remove-MsSqlTable -Connection $connection -TableName $tableName

# create table 
New-MsSqlTable -Connection $connection -TableType PSTable -TableName $tableName

# create an array of $tableName data 
$data = @()

$data += [PSTable]::new("A new first", "A new last")

# add it to the table 
Add-MsSqlObjectToTable -Connection $connection -TableName $tableName -Data $data


# insert intotable with sql
$recordsUpdated = Set-MsSql -Connection $connection -Sql "Insert into $($tableName) (FirstName) Values('hello missus')"  

Write-Host "Inserted records $($recordsUpdated)"



# send an sql selact return as a dataset , find the table in the dataset , populate a list<PSTable> object from the table    

$result = Get-MsSql -Connection $connection -Sql "select * from $($tableName)" -Output DataSet `
          | Find-Table -TableName $tableName `
          | Set-MsSqlObject -OutputType PSTable  

Write-Host "Results from dataset table into list<PSTable>"          
foreach ($obj in $result) {
    Write-Host $obj.FirstName
}

Write-Host "Results using dynamic object" 
$result = Get-MsSql -Connection $connection -Sql "select FirstName as ColumnForName, LastName as ColumnForLastName from $($tableName)" -Output Dynamic

foreach ($objdyn in $result) {
    Write-Host "FirstName in dynamic $($objdyn.ColumnForName)"
}

# as object list List<PSTable>
$asListObject = Get-MsSql -Connection $connection -Sql "Select * from $($tableName)" -SqlParameters $sqlParameters -Output Object -Type PSTable
Write-Host "As list<PSTable>"
foreach ($obj in $asListObject) {
    Write-Host "FirstName $($obj.FirstName) LastName $($obj.LastName)"
}

Write-Host "Using Stored procedure"

# and now using a storedproc 
$sqlParameters = Add-MsSqlParameter -Name "Name" -Value "hello missus"

# as dataset 
$asDataset = Get-MsSql -Connection $connection -Sql "TestStoredProc" -SqlParameters $sqlParameters -Input StoredProcedure -Output DataSet
Write-Host "Table count from ds $($asDataset.Tables.Count)"

# as dynamic 
$asDataset = Get-MsSql -Connection $connection -Sql "TestStoredProc" -SqlParameters $sqlParameters -Input StoredProcedure -Output Dynamic
Write-Host "Dynamic first is $($asDataset[0].FirstName)"

# as object list 
$asobjectList = Get-MsSql -Connection $connection -Sql "TestStoredProc" -SqlParameters $sqlParameters -Input StoredProcedure -Output Object -Type PSTable
foreach ($obj in $asobjectList) {
    Write-Host "Obj list from Stored proc - firstname $($obj.FirstName) lastname $($obj.LastName)"
}