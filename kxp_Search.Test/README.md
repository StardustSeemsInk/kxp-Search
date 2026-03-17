# KitX 聚合搜索插件测试项目

[English](./README.en.md) | [中文](./README.md)

## 概述

本项目是 KitX 聚合搜索插件 (kxp-Search) 的独立测试程序，用于在开发过程中直接测试插件的各项功能，而无需通过 KitX Dashboard。

这是一个交互式命令行测试工具，可以：

- 检测各搜索工具的可用性
- 测试 Ripgrep 文本搜索功能
- 测试 Everything(ES) 文件名搜索功能
- 测试 SearXNG 网络搜索功能
- 查看和修改插件配置

## 功能菜单

```
========== Main Menu ==========
1. Check Tool Availability (ToolChecker)   - 检查工具可用性
2. Ripgrep Search Test                     - Ripgrep 文本搜索测试
3. Everything(ES) Search Test              - Everything 文件名搜索测试
4. SearXNG Search Test                     - SearXNG 网络搜索测试
5. View Current Config                     - 查看当前配置
6. Update Config                           - 更新配置
0. Exit                                    - 退出
==================================
```

## 快速开始

### 1. 构建测试项目

```bash
cd kxp_Search.Test
dotnet build -c Release
```

### 2. 准备配置文件

确保测试程序目录下存在 `config.json` 文件，内容参考主插件配置：

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

### 3. 运行测试程序

```bash
dotnet run
# 或
./bin/Release/net8.0/kxp_Search.Test.exe
```

## 使用说明

### 检查工具可用性

选择菜单 `1` 可以检测系统中是否已安装并配置好了以下工具：

- **Ripgrep**: 检查 `rg` 命令是否可用
- **Everything**: 检查 `es.exe` 是否可用
- **SearXNG**: 检查配置的实例是否可连接

每个工具会返回：
- `Available`: 是否可用
- `Version`: 版本号（如可获取）
- `Message`: 状态信息

### Ripgrep 搜索测试

选择菜单 `2` 进行 Ripgrep 文本搜索测试。

**输入参数：**
- Keyword: 搜索关键词
- Path: 搜索路径（留空使用默认配置）
- IsRegex: 是否使用正则表达式
- IsCaseSensitive: 是否区分大小写
- FilePattern: 文件名匹配模式（如 `*.cs`）
- MaxCount: 最大返回结果数
- ContextLines: 上下文行数

### Everything 搜索测试

选择菜单 `3` 进行 Everything 文件名搜索测试。

**输入参数：**
- Keyword: 搜索关键词
- Path: 限定搜索路径（留空搜索全部）
- IsRegex: 是否使用正则表达式
- IsCaseSensitive: 是否区分大小写
- MaxResults: 最大返回结果数
- FileType: 文件类型筛选（如 `md`, `txt`）

### SearXNG 搜索测试

选择菜单 `4` 进行 SearXNG 网络搜索测试。

**输入参数：**
- Keyword: 搜索关键词
- Category: 搜索分类 (general/academic/music/video)
- MaxResults: 最大返回结果数
- Language: 搜索语言 (zh-CN/en-US)
- SafeSearch: 是否启用安全搜索

### 查看当前配置

选择菜单 `5` 可以查看当前的完整配置（JSON 格式）。

### 更新配置

选择菜单 `6` 可以动态修改配置项：

1. 修改 Ripgrep 可执行文件路径
2. 修改 Everything(ES) 可执行文件路径
3. 修改 SearXNG 实例 URL
4. 启用/禁用 Ripgrep
5. 启用/禁用 Everything(ES)
6. 启用/禁用 SearXNG

修改后的配置会保存到 `config.json` 文件中。

## 项目结构

```
kxp_Search.Test/
├── Program.cs              # 测试程序入口
├── config.json             # 测试用配置文件
└── kxp_Search.Test.csproj # 项目文件
```

## 技术栈

- **.NET**: 8.0
- **项目引用**: kxp_Search (主插件项目)

## 注意事项

1. **外部工具依赖**：测试前请确保已安装相应的搜索工具：
   - Ripgrep (rg.exe)
   - Everything (es.exe)
   - SearXNG 实例（可选）

2. **工作目录**：测试程序会从其所在目录读取 `config.json`

3. **配置持久化**：通过菜单修改的配置会保存到文件，但不会影响已加载的配置（需要重启或重新加载）
