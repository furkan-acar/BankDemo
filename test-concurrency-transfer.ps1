# Test Concurrent Transfers
$baseUrl = "http://localhost:8080/api/account"

Write-Host "Creating source account..."
$sourceAccount = @{
    name = "Source Account"
    balance = 1000.00
} | ConvertTo-Json

$source = Invoke-RestMethod -Uri $baseUrl -Method Post -Body $sourceAccount -ContentType "application/json"
$sourceId = $source.id

Write-Host "Creating destination account..."
$destAccount = @{
    name = "Destination Account"
    balance = 0.00
} | ConvertTo-Json

$dest = Invoke-RestMethod -Uri $baseUrl -Method Post -Body $destAccount -ContentType "application/json"
$destId = $dest.id

Write-Host "Created accounts:"
Write-Host "Source ID: $sourceId (Balance: $($source.balance))"
Write-Host "Destination ID: $destId (Balance: $($dest.balance))"
Write-Host ""

Write-Host "Starting concurrent transfers..."

# Create an array to store jobs
$jobs = @()

# Start 5 concurrent transfer requests
for ($i = 0; $i -lt 5; $i++) {
    $jobs += Start-Job -ScriptBlock {
        param($baseUrl, $sourceId, $destId)
        
        $transfer = @{
            sourceAccountId = $sourceId
            destinationAccountId = $destId
            amount = 100.00
        } | ConvertTo-Json

        try {
            $result = Invoke-RestMethod -Uri "$baseUrl/transactions/transfer" -Method Post -Body $transfer -ContentType "application/json"
            "Transfer succeeded. New source balance: $($result.balance)"
        } catch {
            "Transfer failed: $($_.Exception.Message)"
        }
    } -ArgumentList $baseUrl, $sourceId, $destId
}

Write-Host "Waiting for all transfers to complete..."
$jobs | Wait-Job | Receive-Job

Write-Host "`nFinal account states:"
$finalSource = Invoke-RestMethod -Uri "$baseUrl/$sourceId" -Method Get
$finalDest = Invoke-RestMethod -Uri "$baseUrl/$destId" -Method Get

Write-Host "Source Account Balance: $($finalSource.balance)"
Write-Host "Destination Account Balance: $($finalDest.balance)"

# Cleanup jobs
$jobs | Remove-Job
