# Test Version-based Concurrency
$baseUrl = "http://localhost:8080/api/account"

Write-Host "Creating initial account..."
$account = @{
    name = "Test Account"
    balance = 100.00
} | ConvertTo-Json

$newAccount = Invoke-RestMethod -Uri $baseUrl -Method Post -Body $account -ContentType "application/json"
$accountId = $newAccount.id
$initialVersion = $newAccount.version

Write-Host "Created account:"
Write-Host "ID: $accountId"
Write-Host "Initial Version: $initialVersion"
Write-Host "Initial Balance: $($newAccount.balance)"
Write-Host ""

Start-Sleep -Seconds 1

Write-Host "Scenario 1: Update with correct version (should succeed)"
Write-Host "------------------------------------------------"
$update1 = @{
    id = $accountId
    name = "Updated with Version"
    balance = 150.00
    version = $initialVersion
} | ConvertTo-Json

try {
    $result1 = Invoke-RestMethod -Uri "$baseUrl/$accountId" -Method Put -Body $update1 -ContentType "application/json"
    Write-Host "Update with version succeeded!"
    Write-Host "New version: $($result1.version)"
    Write-Host "New balance: $($result1.balance)"
    $currentVersion = $result1.version
} catch {
    Write-Host "Update with version failed: $_"
}

Write-Host ""
Start-Sleep -Seconds 1

Write-Host "Scenario 2: Update with current version (should succeed)"
Write-Host "------------------------------------------------"
$update2 = @{
    id = $accountId
    name = "Updated with Current Version"
    balance = 200.00
    version = $currentVersion
} | ConvertTo-Json

try {
    $result2 = Invoke-RestMethod -Uri "$baseUrl/$accountId" -Method Put -Body $update2 -ContentType "application/json"
    Write-Host "Update with current version succeeded!"
    Write-Host "New version: $($result2.version)"
    Write-Host "New balance: $($result2.balance)"
    $previousVersion = $currentVersion
    $currentVersion = $result2.version
} catch {
    Write-Host "Update with current version failed: $_"
}

Write-Host ""
Start-Sleep -Seconds 1 

Write-Host "Scenario 3: Update without version (should succeed)"
Write-Host "------------------------------------------------"
$update3 = @{
    id = $accountId
    name = "Updated without Version"
    balance = 225.00
} | ConvertTo-Json

try {
    $result3 = Invoke-RestMethod -Uri "$baseUrl/$accountId" -Method Put -Body $update3 -ContentType "application/json"
    Write-Host "Update without version succeeded!"
    Write-Host "New version: $($result3.version)"
    Write-Host "New balance: $($result3.balance)"
} catch {
    Write-Host "Update without version failed: $_"
}

Write-Host ""
Start-Sleep -Seconds 1 

Write-Host "Scenario 4: Update with wrong version (should fail)"
Write-Host "------------------------------------------------"
$update4 = @{
    id = $accountId
    name = "Updated with Wrong Version"
    balance = 250.00
    version = $previousVersion  # Using outdated version
} | ConvertTo-Json

try {
    $result4 = Invoke-RestMethod -Uri "$baseUrl/$accountId" -Method Put -Body $update4 -ContentType "application/json"
    Write-Host "Update with wrong version succeeded (THIS SHOULDN'T HAPPEN!)"
    Write-Host "New version: $($result4.version)"
    Write-Host "New balance: $($result4.balance)"
} catch {
    Write-Host "Update with wrong version failed (Expected):"
    Write-Host "Status code: $($_.Exception.Response.StatusCode)"
    try {
        $rawError = $_.ErrorDetails.Message
        $errorObj = $rawError | ConvertFrom-Json
        Write-Host "Error details: $($errorObj.message)"
        Write-Host "Current version: $($errorObj.currentVersion)"
        Write-Host "Provided version: $($errorObj.providedVersion)"
    } catch {
        Write-Host "Raw error: $_"
    }
}

Write-Host ""
Write-Host "Test completed!"
