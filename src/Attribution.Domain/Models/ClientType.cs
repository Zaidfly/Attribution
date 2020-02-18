using System.ComponentModel;

namespace Attribution.Domain.Models
{
    /// <summary>
    /// Тип источника
    /// </summary>
    public enum ClientType
    {
        /// <summary>
        /// Полная версия сайта в браузере
        /// </summary>
        [Description("Полная версия в браузере")]
        DesktopBrowser = 1,

        /// <summary>
        /// Мобильная версия сайта
        /// </summary>
        [Description("Мобильная версия")]
        MobileBrowser = 2,

        /// <summary>
        /// Приложение в Iphone
        /// </summary>
        [Description("Iphone-приложение")]
        iPhoneApplication = 3,

        /// <summary>
        /// Приложение в Android
        /// </summary>
        [Description("Android-приложение")]
        AndroidApplication = 4,

        [Description("Система")]
        System = 5,

        /// <summary>
        /// Браузер iOs
        /// </summary>
        [Description("iOs-браузер")]
        iOsBrowser = 6,

        /// <summary>
        /// Браузер в Android
        /// </summary>
        [Description("Android-браузер")]
        AndroidBrowser = 7
    }
}