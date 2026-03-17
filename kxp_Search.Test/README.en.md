# KitX Aggregate Search Plugin Test Project

[English](./README.en.md) | [中文](./README.md)

## Overview

This project is the standalone test program for the KitX Aggregate Search Plugin (kxp-Search). It allows direct testing of plugin functionality during development without requiring KitX Dashboard.

This is an interactive command-line test tool that can:

- Check availability of search tools
- Test Ripgrep text search functionality
- Test Everything(ES) file name search functionality
- Test SearXNG web search functionality
- View and modify plugin configuration

## Function Menu

```
========== Main Menu ==========
1. Check Tool Availability (ToolChecker)   - Check tool availability
2. Ripgrep Search Test                     - Ripgrep text search test
3. Everything(ES) Search Test              - Everything file name search test
4. SearXNG Search Test                     - SearXNG web search test
5. View Current Config                     - View current configuration
6. Update Config                           - Update configuration
0. Exit                                    - Exit
==================================
```

## Quick Start

### 1. Build Test Project

```bash
cd kxp_Search.Test
dotnet build -c Release
```

### 2. Prepare Configuration File

Ensure `config.json` exists in the test program directory. Reference the main plugin configuration:

```json
{
    "ripgrep": {
        "executablePath": "rg",
        "defaultPath": ".",
        "defaultFilePattern": "*",
        "isEnabled": true,
        "timeoutSeconds": 10
    },
    "everything": {
        "executablePath": "C:\\Program Files\\Everything\\es.exe",
        "isEnabled": true,
        "timeoutSeconds": 10
    },
    "searxng": {
        "instanceUrl": "http://localhost:4567/",
        "apiKey": "",
        "defaultCategory": "general",
        "isEnabled": true,
        "timeoutSeconds": 10
    },
    "general": {
        "maxResultsPerSource": 100,
        "defaultMaxResults": 20
    }
}
```

### 3. Run Test Program

```bash
dotnet run
# or
./bin/Release/net8.0/kxp_Search.Test.exe
```

## User Guide

### Check Tool Availability

Select menu `1` to check if the following tools are installed and configured:

- **Ripgrep**: Check if `rg` command is available
- **Everything**: Check if `es.exe` is available
- **SearXNG**: Check if configured instance is reachable

Each tool returns:
- `Available`: Whether available
- `Version`: Version number (if available)
- `Message`: Status message

### Ripgrep Search Test

Select menu `2` to test Ripgrep text search.

**Input Parameters:**
- Keyword: Search keyword
- Path: Search path (empty uses default config)
- IsRegex: Use regular expression
- IsCaseSensitive: Case sensitive search
- FilePattern: File name matching pattern (e.g., `*.cs`)
- MaxCount: Maximum number of results
- ContextLines: Context lines to show

### Everything Search Test

Select menu `3` to test Everything file name search.

**Input Parameters:**
- Keyword: Search keyword
- Path: Limit search path (empty searches all)
- IsRegex: Use regular expression
- IsCaseSensitive: Case sensitive search
- MaxResults: Maximum number of results
- FileType: File type filter (e.g., `md`, `txt`)

### SearXNG Search Test

Select menu `4` to test SearXNG web search.

**Input Parameters:**
- Keyword: Search keyword
- Category: Search category (general/academic/music/video)
- MaxResults: Maximum number of results
- Language: Search language (zh-CN/en-US)
- SafeSearch: Enable safe search

### View Current Configuration

Select menu `5` to view the complete current configuration (JSON format).

### Update Configuration

Select menu `6` to dynamically modify configuration items:

1. Modify Ripgrep executable path
2. Modify Everything(ES) executable path
3. Modify SearXNG instance URL
4. Enable/Disable Ripgrep
5. Enable/Disable Everything(ES)
6. Enable/Disable SearXNG

Modified configuration will be saved to `config.json`.

## Project Structure

```
kxp_Search.Test/
├── Program.cs              # Test program entry point
├── config.json             # Test configuration file
└── kxp_Search.Test.csproj # Project file
```

## Tech Stack

- **.NET**: 8.0
- **Project Reference**: kxp_Search (main plugin project)

## Notes

1. **External Tool Dependencies**: Ensure the following search tools are installed before testing:
   - Ripgrep (rg.exe)
   - Everything (es.exe)
   - SearXNG instance (optional)

2. **Working Directory**: Test program reads `config.json` from its directory

3. **Configuration Persistence**: Configuration modified via menu is saved to file but does not affect loaded configuration (requires restart or reload)
