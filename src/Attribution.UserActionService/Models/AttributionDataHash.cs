namespace Attribution.UserActionService.Models
{
    public struct AttributionDataHashes
    {
        public AttributionDataHashes(long? actualHash, long? lingeringHash)
        {
            ActualHash = actualHash;
            LingeringHash = lingeringHash;
        }
        
        public long? ActualHash { get; set; } 
        public long? LingeringHash { get; set; } 
    }
}