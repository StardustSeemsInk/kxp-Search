using KitX.Contract.CSharp;
using KitX.Shared.CSharp.Plugin;

namespace kxp_Search.Plugin;

public class IdentityInterface : IIdentityInterface
{
    public IController GetController() => new Controller();

    public IMarketPluginContract GetMarketPluginContract() => null!;

    public PluginInfo GetPluginInfo() => new()
    {
        Name = "kxp_Search",
        Version = "v1.0.0",
        DisplayName = new()
        {
            { "zh-cn", "KitX 聚合搜索插件" },
            { "zh-tw", "KitX 聚合搜索插件" },
            { "en-us", "KitX Aggregate Search Plugin" },
        },
        AuthorName = "StarInk",
        AuthorLink = "https://github.com/StardustSeemsInk",
        PublisherName = "Crequency",
        PublisherLink = "https://catrol.cn",
        SimpleDescription = new()
        {
            { "zh-cn", "统一的搜索接口，支持 Ripgrep、Everything、SearXNG" },
            { "zh-tw", "統一的搜索接口，支持 Ripgrep、Everything、SearXNG" },
            { "en-us", "Unified search interface for Ripgrep, Everything, SearXNG" },
        },
        ComplexDescription = new()
        {
            { "zh-cn", "KitX 聚合搜索插件，提供统一的搜索接口。支持 Ripgrep 文本搜索、Everything(ES) 文件名搜索、SearXNG 网络搜索。" },
            { "zh-tw", "KitX 聚合搜索插件，提供統一的搜索接口。支持 Ripgrep 文本搜索、Everything(ES) 文件名搜索、SearXNG 網絡搜索。" },
            { "en-us", "KitX Aggregate Search Plugin - Unified search interface supporting Ripgrep text search, Everything(ES) file search, and SearXNG web search." },
        },
        TotalDescriptionInMarkdown = new()
        {
            {
                "zh-cn",
                """
                # KitX 聚合搜索插件

                统一的搜索接口插件，支持以下搜索工具：

                ## 功能

                - Ripgrep 文本搜索
                - Everything(ES) 文件名搜索
                - SearXNG 网络搜索

                ## 使用方法

                通过 KitX Workflow 调用相应的 Function。
                """
            },
            {
                "en-us",
                """
                # KitX Aggregate Search Plugin

                Unified search interface plugin supporting:

                ## Features

                - Ripgrep text search
                - Everything(ES) file name search
                - SearXNG web search

                ## Usage

                Call the corresponding Function via KitX Workflow.
                """
            },
        },
        IconInBase64 = "",
        PublishDate = DateTime.Now,
        LastUpdateDate = DateTime.Now,
        IsMarketVersion = false,
        RootStartupFileName = "kxp_Search.dll",
        Tags = [],
        Functions = Functions.GetFunctions()
    };
}
