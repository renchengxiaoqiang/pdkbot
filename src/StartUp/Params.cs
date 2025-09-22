using PdkBot.BotLib.Common;
using PdkBotLib.Db;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace PdkBot
{
    public class Params
    {
        public const int Version = 10000;
        public static string VersionStr;
        public const string CreateDateStr = "2025.10.01";
        public static string HelpRoot;
        public const int KeepInstalledVersionsCount = 3;
        public const string AppName = "拼多客AI";
        public const int MaxAddQaCountForQuestionAndAnswersCiteTableManager = 30000;
        public const int MaxSynableQuestionTimeoutDays = 10;
        public static int BottomPannelAnswerCount;
        public static bool RulePatternMatchStrict;
        private static string _pcGuid;
        private static string _instanceGuid;
        public static readonly DateTime AppStartTime;
        public static bool IsAppClosing;


        static Params()
        {
            VersionStr = ShareUtil.ConvertVersionToString(Version);
            HelpRoot = "https://github.com/renchengxiaofeixia";
            AppStartTime = DateTime.Now;
            IsAppClosing = false;
        }


        public class Other
        {
            public const string DefaultApplicationThemeName = "Dark";
            public const int DefaultAssiatPanelWidth = 260;
            public const bool DefaultAssitPanelHidden = true;

            public const int AssiatPanelMaxWidth = 400;
            public const int AssiatPanelMinWidth = 180;
            public static string GetApplicationThemeName()
            {
                return PersistentParams.GetParam2Key("ApplicationThemeName", string.Empty, DefaultApplicationThemeName);
            }
            public static void SetApplicationThemeName(string themeName)
            {
                PersistentParams.TrySaveParam2Key("ApplicationThemeName", string.Empty, themeName);
            }
            public static int GetAssistPanelWidth()
            {
                return PersistentParams.GetParam2Key("AssiatPanelWidth", string.Empty, DefaultAssiatPanelWidth);
            }
            public static void SetAssistPanelWidth(int width)
            {
                PersistentParams.TrySaveParam2Key("AssiatPanelWidth", string.Empty, width);
            }
            public static bool GetIsAssitPanelHidden()
            {
                return PersistentParams.GetParam2Key("AssitPanelHidden", string.Empty, DefaultAssitPanelHidden);
            }
            public static void SetIsAssitPanelHidden(bool isHidden)
            {
                PersistentParams.TrySaveParam2Key("AssitPanelHidden", string.Empty, isHidden);
            }
        }


        public class PlatformShop
        {
            [JsonPropertyName("platform")]
            public string Platform { get; set; }
            [JsonPropertyName("userId")]
            public string UserId { get; set; }
            [JsonPropertyName("userName")]
            public string UserName { get; set; }
            [JsonPropertyName("mallId")]
            public string MallId { get; set; }
            [JsonPropertyName("mallName")]
            public string MallName { get; set; }
            [JsonPropertyName("avatar")]
            public string Avatar { get; set; }
            [JsonPropertyName("webViewId")]
            public string WebViewId { get; set; }
        }

        public class Shop
        {
            public enum ShopTypeEnum
            {
                None,
                Pinduodduo,
                Doudian,
                Kuaishou
            }

            public static List<PlatformShop> GetPlatformShops(ShopTypeEnum shopType)
            {
                return PersistentParams.GetParam2Key("PlatformShops", shopType.ToString(), new List<PlatformShop>());
            }
            public static void AddNewShop(ShopTypeEnum shopType, PlatformShop shop)
            {
                var shops = GetPlatformShops(shopType);
                shops.Add(shop);
                PersistentParams.TrySaveParam2Key("PlatformShops", shopType.ToString(), shops);
            }
            public static void RemoveShop(ShopTypeEnum shopType, PlatformShop shop)
            {
                var shops = GetPlatformShops(shopType);
                var rmvShop = shops.FirstOrDefault(s => s.UserId == shop.UserId && s.Platform == shop.Platform);
                if (rmvShop == null) return;
                shops.Remove(rmvShop);
                PersistentParams.TrySaveParam2Key("AssitPanelHidden", shopType.ToString(), shops);
            }
        }


        public class Panel
        {
            public const string RightPanelCompOrderCsvDefault = "机器人";
            public const bool ShortcutIsVisibleDefault = true;
            public const bool GoodsKnowledgeIsVisibleDefault = true;
            public const bool RobotIsVisibleDefault = true;
            public const bool OrderIsVisibleDefault = true;
            public const bool LogisIsVisibleDefault = true;
            public const bool CouponIsVisibleDefault = true;
            public static string GetRightPanelCompOrderCsv(string seller)
            {
                return PersistentParams.GetParam2Key("RightPanelCompOrderCsv", seller, RightPanelCompOrderCsvDefault);
            }
            public static void SetRightPanelCompOrderCsv(string seller, string tabs)
            {
                PersistentParams.TrySaveParam2Key("RightPanelCompOrderCsv", seller, tabs);
            }
            public static bool GetShortcutIsVisible(string seller)
            {
                return PersistentParams.GetParam2Key("ShortcutIsVisible", seller, true);
            }
            public static void SetShortcutIsVisible(string seller, bool visible)
            {
                PersistentParams.TrySaveParam2Key("ShortcutIsVisible", seller, visible);
            }
            public static bool GetGoodsKnowledgeIsVisible(string seller)
            {
                return PersistentParams.GetParam2Key("GoodsKnowledgeIsVisible", seller, true);
            }
            public static void SetGoodsKnowledgeIsVisible(string seller, bool visible)
            {
                PersistentParams.TrySaveParam2Key("GoodsKnowledgeIsVisible", seller, visible);
            }
            public static bool GetRobotIsVisible(string seller)
            {
                return PersistentParams.GetParam2Key("RobotIsVisible", seller, true);
            }
            public static void SetRobotIsVisible(string seller, bool visible)
            {
                PersistentParams.TrySaveParam2Key("RobotIsVisible", seller, visible);
            }
            public static bool GetOrderIsVisible(string seller)
            {
                return PersistentParams.GetParam2Key("OrderIsVisible", seller, true);
            }
            public static void SetOrderIsVisible(string seller, bool visible)
            {
                PersistentParams.TrySaveParam2Key("OrderIsVisible", seller, visible);
            }
            public static bool GetCouponIsVisible(string seller)
            {
                return PersistentParams.GetParam2Key("CouponIsVisible", seller, true);
            }
            public static void SetCouponIsVisible(string seller, bool visible)
            {
                PersistentParams.TrySaveParam2Key("CouponIsVisible", seller, visible);
            }

            public static bool GetPanelOptionVisible(string seller, string tabName)
            {
                if (tabName == "话术")
                {
                    return GetShortcutIsVisible(seller);
                }
                if (tabName == "订单")
                {
                    return GetOrderIsVisible(seller);
                }
                if (tabName == "机器人")
                {
                    return GetRobotIsVisible(seller);
                }
                if (tabName == "商品")
                {
                    return GetGoodsKnowledgeIsVisible(seller);
                }
                if (tabName == "优惠券")
                {
                    return GetCouponIsVisible(seller);
                }
                return false;
            }
        }

        public class Robot
        {
            public static bool CanUseRobot;
            public const int AutoModeBringForegroundIntervalSecond = 5;
            public const int AutoModeCloseUnAnsweredBuyerIntervalSecond = 10;
            public const bool RuleIncludeExceptDefault = false;
            public const int AutoModeReplyDelaySecDefault = 0;
            public const int SendModeReplyDelaySecDefault = 0;
            public const bool QuoteModeSendAnswerWhenFullMatchDefault = false;
            public const double AutoModeAnswerMiniScore = 0.5;
            public const double QuoteOrSendModeAnswerMiniScore = 0.5;
            public const bool CancelAutoOnResetDefault = true;
            public const string AutoModeNoAnswerTipDefault = "亲,目前是机器人值班.这个问题机器人无法回答,等人工客服回来后再回复您.";

            static Robot()
            {
                CanUseRobot = true;
            }
            public enum OperationEnum
            {
                None,
                Auto,
                Send,
                Quote
            }
            public static bool CanUseRobotReal
            {
                get
                {
                    return CanUseRobot;
                }
            }
            public static string GetBaseUrl()
            {
                return PersistentParams.GetParam2Key("BaseUrl", "ai", string.Empty);
            }
            public static void SetBaseUrl(string baseUrl)
            {
                PersistentParams.TrySaveParam2Key("BaseUrl", "ai", baseUrl);
            }
            public static string GetApiKey()
            {
                return PersistentParams.GetParam2Key("ApiKey", "ai", string.Empty);
            }
            public static void SetApiKey(string apiKey)
            {
                PersistentParams.TrySaveParam2Key("ApiKey", "ai", apiKey);
            }
            public static string GetModelName()
            {
                return PersistentParams.GetParam2Key("ModelName", "ai", string.Empty);
            }
            public static void SetModelName( string modelName)
            {
                PersistentParams.TrySaveParam2Key("ModelName", "ai", modelName);
            }
            public static string GetSystemPrompt()
            {
                return PersistentParams.GetParam2Key("SystemPrompt", "ai", string.Empty);
            }
            public static void SetSystemPrompt(string systemPrompt)
            {
                PersistentParams.TrySaveParam2Key("SystemPrompt", "ai", systemPrompt);
            }
            public static OperationEnum GetOperation()
            {
                return PersistentParams.GetParam2Key("Robot.Operation", "ai", OperationEnum.None);

            }
            public static void SetOperation(string nick, OperationEnum operation)
            {
                PersistentParams.TrySaveParam2Key("Robot.Operation", "ai", operation);
            }


            public static bool GetIsAutoReply()
            {
                return PersistentParams.GetParam<bool>("IsAutoReply", true);
            }

            public static void SetIsAutoReply(bool isAutoReply)
            {
                PersistentParams.TrySaveParam<bool>("IsAutoReply", isAutoReply);
            }

            public static string GetAutoModeNoAnswerTip(string nick)
            {
                return PersistentParams.GetParam2Key("Robot.AutoModeNoAnswerTip", nick, AutoModeNoAnswerTipDefault);
            }
            public static void SetAutoModeNoAnswerTip(string nick, string autoModeNoAnswerTip)
            {
                PersistentParams.TrySaveParam2Key("Robot.AutoModeNoAnswerTip", nick, autoModeNoAnswerTip);
            }

        }

    }

}
