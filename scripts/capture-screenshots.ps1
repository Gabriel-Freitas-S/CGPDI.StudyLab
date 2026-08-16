<#
.SYNOPSIS
    Captura screenshots automaticas de cada aba do CGPDI StudyLab e gera GIF animado.

.DESCRIPTION
    1. Compila o app em modo Release (self-contained, sem instalar)
    2. Inicia o executavel e aguarda a janela aparecer
    3. Captura cada aba principal via PrintWindow (GDI32 P/Invoke - sem dependencias externas)
    4. Chama FFmpeg para montar GIF animado a partir dos frames estaticos
    5. Salva tudo em docs/public/screenshots/ e docs/public/gifs/

.NOTES
    Requer: .NET 10 SDK, FFmpeg no PATH (ou instale: winget install Gyan.FFmpeg)
    Funciona localmente e no GitHub Actions (windows-latest).
#>

param (
    [string]$OutputDir       = "$PSScriptRoot\..\docs\public",
    [string]$FfmpegPath      = "ffmpeg",
    [int]   $StartupWaitSec  = 10,
    [int]   $TabWaitMs       = 2500,
    [switch]$SkipBuild
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$RepoRoot       = (Resolve-Path "$PSScriptRoot\..").Path
$ProjectFile    = Join-Path $RepoRoot "CGPDI.StudyLab\CGPDI.StudyLab.csproj"
$PublishDir     = Join-Path $RepoRoot "publish\screenshots-capture"
$ExePath        = Join-Path $PublishDir "CGPDI.StudyLab.exe"
$ScreenshotsDir = Join-Path $OutputDir "screenshots"
$GifsDir        = Join-Path $OutputDir "gifs"

New-Item -ItemType Directory -Force -Path $ScreenshotsDir | Out-Null
New-Item -ItemType Directory -Force -Path $GifsDir        | Out-Null

Add-Type -TypeDefinition @"
using System;
using System.IO;
using System.Runtime.InteropServices;

public static class WinCapture {
    [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr h, out RECT r);
    [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr h);
    [DllImport("user32.dll")] public static extern bool ShowWindow(IntPtr h, int n);
    [DllImport("user32.dll")] public static extern bool PrintWindow(IntPtr h, IntPtr dc, uint f);
    [DllImport("user32.dll")] public static extern IntPtr GetDC(IntPtr hWnd);
    [DllImport("user32.dll")] public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    [DllImport("gdi32.dll")] public static extern IntPtr CreateCompatibleDC(IntPtr hdc);
    [DllImport("gdi32.dll")] public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
    [DllImport("gdi32.dll")] public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
    [DllImport("gdi32.dll")] public static extern bool DeleteDC(IntPtr hdc);
    [DllImport("gdi32.dll")] public static extern bool DeleteObject(IntPtr hObject);

    [DllImport("gdiplus.dll")] public static extern int GdiplusStartup(out IntPtr token, ref StartupInput input, out StartupOutput output);
    [DllImport("gdiplus.dll")] public static extern int GdiplusShutdown(IntPtr token);
    [DllImport("gdiplus.dll")] public static extern int GdipCreateBitmapFromHBITMAP(IntPtr hbm, IntPtr hpal, out IntPtr bitmap);
    [DllImport("gdiplus.dll")] public static extern int GdipSaveImageToFile(IntPtr image, [MarshalAs(UnmanagedType.LPWStr)] string filename, ref Guid clsidEncoder, IntPtr encoderParams);
    [DllImport("gdiplus.dll")] public static extern int GdipDisposeImage(IntPtr image);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT { public int L, T, R, B; public int W { get { return R - L; } } public int H { get { return B - T; } } }

    [StructLayout(LayoutKind.Sequential)]
    public struct StartupInput {
        public uint GdiplusVersion;
        public IntPtr DebugEventCallback;
        public bool SuppressBackgroundThread;
        public bool SuppressExternalCodecs;
        public static StartupInput Default() { return new StartupInput { GdiplusVersion = 1 }; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct StartupOutput {
        public IntPtr NotificationHook;
        public IntPtr NotificationUnhook;
    }

    private static readonly Guid PngClsid = new Guid("557cf406-1a04-11d3-9a73-0000f81ef32e");

    public static void Capture(IntPtr hWnd, string path) {
        RECT rc;
        GetWindowRect(hWnd, out rc);
        int w = Math.Max(1, rc.W);
        int h = Math.Max(1, rc.H);

        IntPtr hdcScreen = GetDC(IntPtr.Zero);
        IntPtr hdcMem = CreateCompatibleDC(hdcScreen);
        IntPtr hBitmap = CreateCompatibleBitmap(hdcScreen, w, h);
        IntPtr hOld = SelectObject(hdcMem, hBitmap);

        // PW_RENDERFULLCONTENT = 2
        PrintWindow(hWnd, hdcMem, 2);

        SelectObject(hdcMem, hOld);
        DeleteDC(hdcMem);
        ReleaseDC(IntPtr.Zero, hdcScreen);

        IntPtr token;
        StartupInput input = StartupInput.Default();
        StartupOutput output;
        GdiplusStartup(out token, ref input, out output);

        IntPtr gpBmp;
        GdipCreateBitmapFromHBITMAP(hBitmap, IntPtr.Zero, out gpBmp);
        Guid png = PngClsid;
        GdipSaveImageToFile(gpBmp, path, ref png, IntPtr.Zero);
        GdipDisposeImage(gpBmp);
        GdiplusShutdown(token);

        DeleteObject(hBitmap);
    }
}
"@

function Select-Tab([IntPtr]$hWnd, [int]$idx) {
    if (-not ('System.Windows.Automation.AutomationElement' -as [type])) {
        try {
            Add-Type -AssemblyName UIAutomationClient -ErrorAction Stop
            Add-Type -AssemblyName UIAutomationTypes  -ErrorAction Stop
        } catch {
            $wpfDir = "$env:windir\Microsoft.NET\Framework64\v4.0.30319\WPF"
            if (-not (Test-Path $wpfDir)) { $wpfDir = "$env:windir\Microsoft.NET\Framework\v4.0.30319\WPF" }
            if (Test-Path "$wpfDir\UIAutomationClient.dll") {
                Add-Type -Path "$wpfDir\UIAutomationClient.dll" -ErrorAction SilentlyContinue
                Add-Type -Path "$wpfDir\UIAutomationTypes.dll"  -ErrorAction SilentlyContinue
            }
        }
    }
    try {
        $root = [System.Windows.Automation.AutomationElement]::FromHandle($hWnd)
        if (-not $root) { return $false }
        $cond = New-Object System.Windows.Automation.PropertyCondition(
            [System.Windows.Automation.AutomationElement]::ControlTypeProperty,
            [System.Windows.Automation.ControlType]::Tab)
        $tc = $root.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $cond)
        if ($tc) {
            $items = $tc.FindAll([System.Windows.Automation.TreeScope]::Children,
                                 [System.Windows.Automation.Condition]::TrueCondition)
            if ($idx -lt $items.Count) {
                $pat = $items[$idx].GetCurrentPattern(
                    [System.Windows.Automation.SelectionItemPattern]::Pattern)
                $pat.Select()
                Start-Sleep -Milliseconds $TabWaitMs
                return $true
            }
        }
    } catch {
        Write-Warning "Falha ao selecionar aba via UIAutomation: $_"
    }
    return $false
}

function Find-Window([int]$procId) {
    for ($i=0; $i -lt 40; $i++) {
        $p = Get-Process -Id $procId -ErrorAction SilentlyContinue
        if ($p -and $p.MainWindowHandle -ne [IntPtr]::Zero) { return $p.MainWindowHandle }
        Start-Sleep -Milliseconds 500
    }
    throw "Janela nao apareceu."
}

$Tabs = @(
    @{i=0;n="hero";          l="PDI - Visao Geral"},
    @{i=0;n="pdi";           l="Laboratorio PDI"},
    @{i=1;n="cg2d";          l="Computacao Grafica 2D"},
    @{i=2;n="cg3d";          l="Computacao Grafica 3D"},
    @{i=3;n="ray-tracing";   l="Ray Tracing"},
    @{i=4;n="central-estudos";l="Central de Estudos"},
    @{i=5;n="laboratorio";   l="Laboratorio de Codigo"},
    @{i=6;n="estudio";       l="Estudio de Projetos"}
)

if (-not $SkipBuild) {
    Write-Host "BUILD..." -ForegroundColor Cyan
    dotnet publish $ProjectFile -c Release -r win-x64 --self-contained true `
        -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true `
        -p:EnableCompressionInSingleFile=true -p:MinVerSkip=true -o $PublishDir `
        --nologo -v quiet
    if ($LASTEXITCODE -ne 0) { throw "Build falhou" }
}

if (-not (Test-Path $ExePath)) { throw "Exe nao encontrado: $ExePath" }

Write-Host "INICIANDO APP..." -ForegroundColor Cyan
$proc = Start-Process -FilePath $ExePath -PassThru
Start-Sleep -Seconds $StartupWaitSec
$hWnd = Find-Window -procId $proc.Id
[WinCapture]::ShowWindow($hWnd, 3) | Out-Null
Start-Sleep -Milliseconds 800

$captured = @()
foreach ($tab in $Tabs) {
    Write-Host "  Capturando: $($tab.l)" -ForegroundColor Yellow
    Select-Tab -hWnd $hWnd -idx $tab.i | Out-Null
    $path = Join-Path $ScreenshotsDir "$($tab.n).png"
    [WinCapture]::Capture($hWnd, $path)
    $captured += $path
    Write-Host "    OK: $path" -ForegroundColor Green
}

try { $proc.Kill() } catch {}

Write-Host "GERANDO GIF..." -ForegroundColor Cyan
$gifOut   = Join-Path $GifsDir "demo.gif"
$listFile = Join-Path $env:TEMP "cgpdi_frames.txt"
$palette  = Join-Path $env:TEMP "cgpdi_palette.png"

$lines = @()
foreach ($f in $captured) { $lines += "file '$f'"; $lines += "duration 2" }
$lines += "file '$($captured[-1])'"
$lines | Set-Content $listFile -Encoding utf8

$eap = $ErrorActionPreference
$ErrorActionPreference = 'Continue'

& $FfmpegPath -y -loglevel error `
    -f concat -safe 0 -i $listFile `
    -vf "scale=1280:-2:flags=lanczos,palettegen=stats_mode=diff" $palette

& $FfmpegPath -y -loglevel error `
    -f concat -safe 0 -i $listFile -i $palette `
    -lavfi "scale=1280:-2:flags=lanczos [x]; [x][1:v] paletteuse=dither=bayer:bayer_scale=5:diff_mode=rectangle" `
    $gifOut

$ErrorActionPreference = $eap

if (Test-Path $gifOut) {
    $sz = [math]::Round((Get-Item $gifOut).Length / 1MB, 2)
    Write-Host "GIF gerado: $gifOut ($sz MB)" -ForegroundColor Green
} else {
    Write-Warning "GIF nao foi gerado - verifique o FFmpeg."
}

Remove-Item $listFile,$palette -ErrorAction SilentlyContinue

Write-Host "CONCLUIDO!" -ForegroundColor Green
Write-Host "  Screenshots : $ScreenshotsDir"
Write-Host "  GIF         : $gifOut"
