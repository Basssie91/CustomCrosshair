# Bouwt CustomCrosshair.exe uit src/CustomCrosshair.cs (met icoon).
# Draait in Windows PowerShell 5.1 (zit in elke Windows en in GitHub Actions).
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$src  = [IO.File]::ReadAllText((Join-Path $root 'src\CustomCrosshair.cs'))
$out  = Join-Path $root 'CustomCrosshair.exe'
$ico  = Join-Path $env:TEMP 'CustomCrosshair.ico'

if (Test-Path -LiteralPath $out) { Remove-Item -LiteralPath $out -Force }

# Eerst in het geheugen laden om het icoon te kunnen maken
Add-Type -TypeDefinition $src -ReferencedAssemblies System.Windows.Forms, System.Drawing, System.Security, System.Core -IgnoreWarnings
[CustomCrosshair.AppIcon]::WriteIco($ico)

$cp = New-Object System.CodeDom.Compiler.CompilerParameters
$cp.GenerateExecutable = $true
$cp.OutputAssembly = $out
$cp.CompilerOptions = '/target:winexe /optimize+ /win32icon:"' + $ico + '"'
[void]$cp.ReferencedAssemblies.Add('System.dll')
[void]$cp.ReferencedAssemblies.Add('System.Drawing.dll')
[void]$cp.ReferencedAssemblies.Add('System.Windows.Forms.dll')
[void]$cp.ReferencedAssemblies.Add('System.Security.dll')
[void]$cp.ReferencedAssemblies.Add('System.Core.dll')

$res = (New-Object Microsoft.CSharp.CSharpCodeProvider).CompileAssemblyFromSource($cp, $src)
$errs = @($res.Errors | Where-Object { -not $_.IsWarning })
if ($errs.Count -gt 0) {
    $errs | ForEach-Object { Write-Host $_.ToString() }
    throw "Bouwen mislukt ($($errs.Count) fouten)"
}
Write-Host "Klaar: $out"
