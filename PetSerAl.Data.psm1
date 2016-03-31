Set-StrictMode -Version Latest
&{
    $Version=
        if($PSVersionTable.PSVersion-ge'5.0') {
            'v5'
        } elseif ($PSVersionTable.PSVersion-ge'4.0') {
            'v4'
        } else {
            $NET35TestPath='Registry::HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5'
            if([Environment]::Version-ge'4.0'-or((Test-Path -LiteralPath $NET35TestPath)-and(Get-Item -LiteralPath $NET35TestPath).GetValue('Install',0))) {
                'v2-v35'
            } else {
                'v2-v2'
            }
        }
    Import-Module -Name (Join-Path -Path (Join-Path -Path $PSScriptRoot -ChildPath $Version) -ChildPath PetSerAl.PowerShell.Data.dll)
}
New-Alias -Name idbcm -Value Invoke-DbCommand
New-Alias -Name ndbcm -Value New-DbCommand
Export-ModuleMember -Alias * -Cmdlet * -Function @() -Variable @()
