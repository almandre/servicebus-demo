# Azure Service Bus Demo - Execution Script
# This script makes it easier to run the demo

param(
    [Parameter(Position=0)]
    [ValidateSet("sender", "receiver", "menu", "build", "clean")]
    [string]$Action = "menu"
)

Write-Host "=== Azure Service Bus Demo ===" -ForegroundColor Cyan
Write-Host ""

switch ($Action.ToLower()) {
    "build" {
        Write-Host "Building the project..." -ForegroundColor Yellow
        dotnet build
    }
    
    "clean" {
        Write-Host "Cleaning the project..." -ForegroundColor Yellow
        dotnet clean
        dotnet restore
        dotnet build
    }
    
    "sender" {
        Write-Host "Running Sender..." -ForegroundColor Green
        Write-Host "IMPORTANT: Configure the connection string in appsettings.json before running!" -ForegroundColor Red
        Write-Host ""
        
        # Temporarily rename Program.cs and use RunSender.cs as entry point
        if (Test-Path "Program.cs") {
            Rename-Item "Program.cs" "Program.cs.bak"
        }
        if (Test-Path "RunSender.cs") {
            Rename-Item "RunSender.cs" "Program.cs"
        }
        
        try {
            dotnet run
        }
        finally {
            # Restore original files
            if (Test-Path "Program.cs") {
                Rename-Item "Program.cs" "RunSender.cs"
            }
            if (Test-Path "Program.cs.bak") {
                Rename-Item "Program.cs.bak" "Program.cs"
            }
        }
    }
    
    "receiver" {
        Write-Host "Running Receiver..." -ForegroundColor Green
        Write-Host "IMPORTANT: Configure the connection string in appsettings.json before running!" -ForegroundColor Red
        Write-Host "Press 'q' to stop the receiver." -ForegroundColor Yellow
        Write-Host ""
        
        # Temporarily rename Program.cs and use RunReceiver.cs as entry point
        if (Test-Path "Program.cs") {
            Rename-Item "Program.cs" "Program.cs.bak"
        }
        if (Test-Path "RunReceiver.cs") {
            Rename-Item "RunReceiver.cs" "Program.cs"
        }
        
        try {
            dotnet run
        }
        finally {
            # Restore original files
            if (Test-Path "Program.cs") {
                Rename-Item "Program.cs" "RunReceiver.cs"
            }
            if (Test-Path "Program.cs.bak") {
                Rename-Item "Program.cs.bak" "Program.cs"
            }
        }
    }
    
    default {
        Write-Host "Running main menu..." -ForegroundColor Green
        Write-Host "IMPORTANT: Configure the connection string in appsettings.json before running!" -ForegroundColor Red
        Write-Host ""
        dotnet run
    }
}

Write-Host ""
Write-Host "Available commands:" -ForegroundColor Cyan
Write-Host "  .\run.ps1 menu     - Run main menu (default)" -ForegroundColor Gray
Write-Host "  .\run.ps1 sender   - Run only the Sender" -ForegroundColor Gray
Write-Host "  .\run.ps1 receiver - Run only the Receiver" -ForegroundColor Gray
Write-Host "  .\run.ps1 build    - Build the project" -ForegroundColor Gray
Write-Host "  .\run.ps1 clean    - Clean and rebuild the project" -ForegroundColor Gray
