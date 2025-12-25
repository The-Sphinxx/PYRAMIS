$baseUrl = "http://localhost:5137"

try {
    Write-Host "1. fetching trips to find a target..."
    $trips = Invoke-RestMethod -Method Get -Uri "$baseUrl/api/trips"
    $idToDelete = $null

    if ($trips.data.Count -gt 0) {
        $idToDelete = $trips.data[0].id
        Write-Host "Found trip ID: $idToDelete"
    } else {
        Write-Host "No trips found to delete."
        exit
    }

    Write-Host "2. Attempting DELETE on trip $idToDelete..."
    try {
        Invoke-RestMethod -Method Delete -Uri "$baseUrl/api/trips/$idToDelete"
        Write-Host "DELETE SUCCESSFUL"
    } catch {
        Write-Host "DELETE FAILED"
        Write-Host $_.Exception.Message
        if ($_.Exception.Response) {
             $stream = $_.Exception.Response.GetResponseStream()
             $reader = New-Object System.IO.StreamReader($stream)
             Write-Host "Response Body: " $reader.ReadToEnd()
        }
    }

} catch {
    Write-Host "General Error: " $_.Exception.Message
}
