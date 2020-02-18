namespace Attribution.UserActionService.Models.YouDoEvents
{
    public class WebContext
    {
        /// <summary>
        /// Hash набора данных для аттрибуции
        /// </summary>
        public long? AttributionDataHash { get; set; }
        
        /// <summary>
        /// Долгоживущий Hash набора данных для аттрибуции (до 90 дней)
        /// </summary>
        public long? AttributionDataLingeringHash { get; set; }
    }
}