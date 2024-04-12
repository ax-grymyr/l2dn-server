Param(
    [String] $InputPath,
    [String] $OutputPath
)

$json = @{
    BaseUrl = "https://l2dn.pages.dev/latest/geodata/"
    Files = Get-FileHash (Join-Path $InputPath -ChildPath "*.*") | ForEach-Object {
        @{
            Name = Split-Path -Leaf $_.Path
            Hash = $_.Hash
        }
    }
} | ConvertTo-Json | Out-File $OutputPath
