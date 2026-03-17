# KitX 聚合搜索插件 (kxp-Search)

[English](./README.en.md) | [中文](./README.md)

## 概述

KitX 聚合搜索插件是一个纯命令行的搜索接口插件，为 KitX 提供统一的搜索功能入口。该插件集成了三种主流搜索工具：

- **Ripgrep** - 本地文本内容搜索
- **Everything (ES)** - 本地文件名快速搜索
- **SearXNG** - 网络元搜索

## 功能

### 核心搜索功能

| Function | 功能描述 | 返回类型 |
|----------|----------|----------|
| `RipgrepSearch` | Ripgrep 文本搜索，支持正则表达式 | `List<SearchResultItem>` |
| `EverythingSearch` | Everything(ES) 文件名搜索 | `List<SearchResultItem>` |
| `SearxngSearch` | SearXNG 网络搜索 | `List<SearchResultItem>` |

### 辅助功能

| Function | 功能描述 | 返回类型 |
|----------|----------|----------|
| `CheckToolAvailability` | 检查各搜索工具是否可用 | `Dictionary<string, bool>` |
| `GetConfig` | 获取当前插件配置 | `PluginConfig` |
| `SetConfig` | 更新插件配置 | `void` |

## 支持的搜索源

### 1. Ripgrep 文本搜索

在本地文件系统中进行文本内容搜索。

**参数：**
- `keyword` (必填): 搜索关键词
- `path` (可选): 搜索路径
- `isRegex` (可选): 是否使用正则表达式
- `isCaseSensitive` (可选): 是否区分大小写
- `filePattern` (可选): 文件名匹配模式
- `maxCount` (可选): 单文件最大匹配行数
- `contextLines` (可选): 上下文行数

### 2. Everything 文件名搜索

基于文件名的快速搜索（需要安装 Voidtools Everything）。

**参数：**
- `keyword` (必填): 搜索关键词
- `path` (可选): 限定搜索路径
- `isRegex` (可选): 是否使用正则表达式
- `isCaseSensitive` (可选): 是否区分大小写
- `maxResults` (可选): 最大结果数
- `fileType` (可选): 文件类型筛选

### 3. SearXNG 网络搜索

通过自托管的 SearXNG 实例进行网络搜索。

**参数：**
- `keyword` (必填): 搜索关键词
- `category` (可选): 搜索分类 (general/academic/music/video)
- `maxResults` (可选): 最大结果数
- `language` (可选): 搜索语言
- `safeSearch` (可选): 安全搜索

## 统一返回结构

所有搜索功能返回统一的 `SearchResultItem` 列表：

```json
{
  "source": "ripgrep",
  "title": "Program.cs",
  "subtitle": "第 42 行",
  "content": "    public static void Main(string[] args)",
  "extra": {
    "filePath": "C:/Dev/App/Program.cs",
    "lineNumber": 42
  },
  "url": "file:///C:/Dev/App/Program.cs"
}
```

## 配置文件

插件使用 `config.json` 进行配置：

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

## 项目结构

```
kxp-Search/
├── Plugin/                    # 插件核心代码
│   ├── IdentityInterface.cs   # 插件入口
│   ├── Functions.cs           # 功能定义
│   └── Controller.cs          # 控制器
├── Services/                  # 业务服务
│   ├── ISearchService.cs      # 搜索服务接口
│   ├── AggregateSearchService.cs
│   ├── ConfigService.cs
│   ├── ToolCheckerService.cs
│   ├── RipgrepSearchService.cs
│   ├── EverythingSearchService.cs
│   └── SearxngSearchService.cs
├── Models/                     # 数据模型
│   ├── SearchResultItem.cs
│   ├── PluginConfig.cs
│   ├── ToolStatus.cs
│   └── SearchRequests.cs
├── kxp_Search.csproj          # 项目文件
├── config.json                # 配置文件
├── PluginStruct.json          # 插件结构定义
└── LoaderStruct.json          # 加载器结构定义
```

## 技术栈

- **.NET**: 8.0
- **C#**: 12 (ImplicitUsings, Nullable)
- **框架**: System.ComponentModel.Composition 8.0.0
- **依赖**: KitX.Contract.CSharp, KitX.Shared.CSharp

## 外部依赖

使用此插件前，请确保已安装以下工具：

1. **Ripgrep (rg)** - Windows 版本 (rg.exe)
   - 下载地址: https://github.com/BurntSushi/ripgrep

2. **Everything (es.exe)** - Voidtools Everything 命令行版本
   - 下载地址: https://www.voidtools.com/zh-cn/downloads/

3. **SearXNG** (可选) - 自托管元搜索引擎
   - 官方地址: https://searxng.org/

## 构建与打包

### 构建项目

```bash
dotnet build kxp_Search.csproj -c Release
```

### 打包为 KXP 插件

使用 KXPBuilder 工具进行打包：

```bash
cd bin/Release/net8.0
kxpbuilder -n kxp-Search --plugin-version 1.0.0
```

## 使用方法

1. 将打包好的 `.kxp` 文件通过 KitX Dashboard 安装
2. 启动插件
3. 通过 KitX Workflow 调用相应的 Function 进行搜索

## 许可证

MIT License

## 作者

- 作者: StarInk
- GitHub: https://github.com/StardustSeemsInk

---

*此插件是 KitX 生态系统的一部分*
