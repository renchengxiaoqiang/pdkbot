using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PdkBot.DbEntity
{
    public class PlatformResponse
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("response")]
        public string Response { get; set; }
    }

    public class NewMessage
    {
        [JsonPropertyName("hasNewMessage")]
        public bool HasNewMessage { get; set; }
        [JsonPropertyName("newMessageCount")]
        public int NewMessageCount { get; set; }
    }


    public enum PlatformTypeEnum
    {
        Qianiu,
        Jingdong,
        Pinduoduo,
        Doudian,
        Kuaishou,
        Xiaohongshu
    }


    public class Platform
    {
        public string UserProfileId { get; set; }
        public string WorkbenchUrl { get; set; }
        public string ChatUrl { get; set; }
        public string LoginUrl { get; set; }
        public string InjectJsPath { get; set; }
        public string PlatformImage { get; set; }
        public string PlatformValue { get; set; }
    }

    public static class PlatformConfig
    {
        public static string UserProfileConfigPath { get; set; } = System.IO.Path.Combine(AppContext.BaseDirectory, "user_profile.json");

        public static Dictionary<string, PlatformTypeEnum> PlatformTypes { get; set; } =
            new Dictionary<string, PlatformTypeEnum>() {
                { "千牛",PlatformTypeEnum.Qianiu },
                { "京东",PlatformTypeEnum.Jingdong },
                { "拼多多",PlatformTypeEnum.Pinduoduo },
                { "抖店",PlatformTypeEnum.Doudian },
                { "快手",PlatformTypeEnum.Kuaishou },
                { "小红书",PlatformTypeEnum.Xiaohongshu }
            };

        public static Platform GetPlatform(PlatformTypeEnum platformType)
        {
            Platform p = null;
            switch(platformType)
            {
                case PlatformTypeEnum.Qianiu:
                    p = new Platform
                    {
                        ChatUrl = "",
                        LoginUrl = "",
                        PlatformImage = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "qn.png"),
                        PlatformValue = "千牛",
                    };
                    break;
                case PlatformTypeEnum.Jingdong:
                    p = new Platform
                    {
                        WorkbenchUrl = "https://www.jd.com/",
                        ChatUrl = "https://dongdong.jd.com/",
                        LoginUrl = "https://passport.jd.com/uc/login",
                        InjectJsPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Platform", "jd.js"),
                        PlatformImage = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "jd.png"),
                        PlatformValue = "京东",
                    };
                    break;
                case PlatformTypeEnum.Pinduoduo:
                    p = new Platform
                    {
                        WorkbenchUrl = "https://mms.pinduoduo.com/home/",
                        ChatUrl = "https://mms.pinduoduo.com/chat-merchant/index.html#/",
                        LoginUrl = "https://mms.pinduoduo.com/login/",
                        InjectJsPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Platform", "pdd.js"),
                        PlatformImage = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "pdd.png"),
                        PlatformValue = "拼多多",
                    };
                    break;
                case PlatformTypeEnum.Doudian:
                    p = new Platform
                    {
                        WorkbenchUrl = "https://fxg.jinritemai.com/ffa/mshop/homepage/index",
                        ChatUrl = "https://im.jinritemai.com/pc_seller_v2/main/workspace",
                        LoginUrl = "https://fxg.jinritemai.com/login",
                        InjectJsPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Platform", "doudian.js"),
                        PlatformImage = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "dd.png"),
                        PlatformValue = "抖店",
                    };
                    break;
                case PlatformTypeEnum.Kuaishou: 
                        p = new Platform();
                    break;  
                case PlatformTypeEnum.Xiaohongshu:  
                        p = new Platform();
                    break;  
            }
            return p;
        }

        public static Dictionary<PlatformTypeEnum, Platform> Platforms { get; set; } =
            new Dictionary<PlatformTypeEnum, Platform>() {
            {
                PlatformTypeEnum.Qianiu,
                new Platform
                {
                    ChatUrl ="",
                    LoginUrl = "",
                    PlatformImage = System.IO.Path.Combine(AppContext.BaseDirectory,"Assets","qn.png"),
                }
            },
            {
                PlatformTypeEnum.Jingdong,
                new Platform
                {
                    ChatUrl ="https://dongdong.jd.com/",
                    LoginUrl = "https://passport.jd.com/uc/login",
                    InjectJsPath = System.IO.Path.Combine(AppContext.BaseDirectory,"Platform","jd.js"),
                    PlatformImage = System.IO.Path.Combine(AppContext.BaseDirectory,"Assets","jd.png"),
                }
            },
            {
                PlatformTypeEnum.Pinduoduo,
                new Platform
                {
                    WorkbenchUrl="https://mms.pinduoduo.com/home/",
                    ChatUrl ="https://mms.pinduoduo.com/chat-merchant/index.html#/",
                    LoginUrl = "https://mms.pinduoduo.com/login/",
                    InjectJsPath = System.IO.Path.Combine(AppContext.BaseDirectory,"Platform","pdd.js"),
                    PlatformImage = System.IO.Path.Combine(AppContext.BaseDirectory,"Assets","pdd.png"),
                }
            },
            {
                PlatformTypeEnum.Doudian,
                new Platform
                {
                    WorkbenchUrl="https://fxg.jinritemai.com/ffa/mshop/homepage/index",
                    ChatUrl ="https://im.jinritemai.com/pc_seller_v2/main/workspace",
                    LoginUrl = "https://fxg.jinritemai.com/login",
                    InjectJsPath = System.IO.Path.Combine(AppContext.BaseDirectory,"Platform","doudian.js"),
                    PlatformImage = System.IO.Path.Combine(AppContext.BaseDirectory,"Assets","dd.png"),
                }
            }
        };
    }
}
