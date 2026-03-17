# KitX Aggregate Search Plugin (kxp-Search)

[English](./README.en.md) | [中文](./README.md)

## Overview

KitX Aggregate Search Plugin is a command-line search interface plugin for KitX, providing a unified entry point for search functionality. This plugin integrates three popular search tools:

- **Ripgrep** - Local text content search
- **Everything (ES)** - Fast local file name search
- **SearXNG** - Web meta-search

## Features

### Core Search Functions

| Function | Description | Return Type |
|----------|-------------|-------------|
| `RipgrepSearch` | Ripgrep text search with regex support | `List<SearchResultItem>` |
| `EverythingSearch` | Everything(ES) file name search | `List<SearchResultItem>` |
| `SearxngSearch` | SearXNG web search | `List<SearchResultItem>` |

### Auxiliary Functions

| Function | Description | Return Type |
|----------|-------------|-------------|
| `CheckToolAvailability` | Check if search tools are available | `Dictionary<string, bool>` |
| `GetConfig` | Get current plugin configuration | `PluginConfig` |
| `SetConfig` | Update plugin configuration | `void` |

## Supported Search Sources

### 1. Ripgrep Text Search

Search for text content in local file system.

**Parameters:**
- `keyword` (required): Search keyword
- `path` (optional): Search path
- `isRegex` (optional): Use regular expression
- `isCaseSensitive` (optional): Case sensitive search
- `filePattern` (optional): File name matching pattern
- `maxCount` (optional): Maximum number of results
- `contextLines` (optional): Context lines to show

### 2. Everything File Name Search

Fast file name search (requires Voidtools Everything installation).

**Parameters:**
- `keyword` (required): Search keyword
- `path` (optional): Limit search path
- `isRegex` (optional): Use regular expression
- `isCaseSensitive` (optional): Case sensitive search
- `maxResults` (optional): Maximum number of results
- `fileType` (optional): File type filter

### 3. SearXNG Web Search

Web search via self-hosted SearXNG instance.

**Parameters:**
- `keyword` (required): Search keyword
- `category` (optional): Search category (general/academic/music/video)
- `maxResults` (optional): Maximum number of results
- `language` (optional): Search language
- `safeSearch` (optional): Safe search enabled

## Unified Return Structure

All search functions return a unified `SearchResultItem` list:

```json
{
  "source": "ripgrep",
  "title": "Program.cs",
  "subtitle": "Line 42",
  "content": "    public static void Main(string[] args)",
  "extra": {
    "filePath": "C:/Dev/App/Program.cs",
    "lineNumber": 42
  },
  "url": "file:///C:/Dev/App/Program.cs"
}
```

## Configuration

The plugin uses `config.json` for configuration:

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

## Project Structure

```
kxp-Search/
├── Plugin/                    # Plugin core code
│   ├── IdentityInterface.cs   # Plugin entry point
│   ├── Functions.cs           # Function definitions
│   └── Controller.cs          # Controller
├── Services/                  # Business services
│   ├── ISearchService.cs      # Search service interface
│   ├── AggregateSearchService.cs
│   ├── ConfigService.cs
│   ├── ToolCheckerService.cs
│   ├── RipgrepSearchService.cs
│   ├── EverythingSearchService.cs
│   └── SearxngSearchService.cs
├── Models/                     # Data models
│   ├── SearchResultItem.cs
│   ├── PluginConfig.cs
│   ├── ToolStatus.cs
│   └── SearchRequests.cs
├── kxp_Search.csproj          # Project file
├── config.json                # Configuration file
├── PluginStruct.json          # Plugin structure definition
└── LoaderStruct.json          # Loader structure definition
```

## Tech Stack

- **.NET**: 8.0
- **C#**: 12 (ImplicitUsings, Nullable)
- **Framework**: System.ComponentModel.Composition 8.0.0
- **Dependencies**: KitX.Contract.CSharp, KitX.Shared.CSharp

## External Dependencies

Before using this plugin, ensure the following tools are installed:

1. **Ripgrep (rg)** - Windows version (rg.exe)
   - Download: https://github.com/BurntSushi/ripgrep

2. **Everything (es.exe)** - Voidtools Everything command line version
   - Download: https://www.voidtools.com/downloads/

3. **SearXNG** (optional) - Self-hosted meta search engine
   - Official site: https://searxng.org/

## Build and Package

### Build Project

```bash
dotnet build kxp_Search.csproj -c Release
```

### Package as KXP Plugin

Use KXPBuilder tool for packaging:

```bash
cd bin/Release/net8.0
kxpbuilder -n kxp-Search --plugin-version 1.0.0
```

## Usage

1. Install the packaged `.kxp` file via KitX Dashboard
2. Start the plugin
3. Use KitX Workflow to call the corresponding Function for search

## License

MIT License

## Author

- Author: StarInk
- GitHub: https://github.com/StardustSeemsInk

---

*This plugin is part of the KitX ecosystem*
