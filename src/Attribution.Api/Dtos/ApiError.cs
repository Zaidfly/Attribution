using System.Runtime.Serialization;

namespace Attribution.Api.Dtos
{
    /// <summary>
    /// Информация об ошибке
    /// </summary>
    [DataContract]
    public class ApiError
    {
        [DataMember(Name = "code")]
        public ApiErrorCode Code { get; set; }
        
        [DataMember(Name = "message")]
        public string Message { get; set; }
        
        public ApiError(ApiErrorCode errorCode) : this(errorCode, null)
        {
        }
        
        public ApiError(ApiErrorCode errorCode, string message)
        {
            Code = errorCode;
            Message = message;
        }
        
        public static ApiError NotFound(string message = null) 
            => new ApiError(ApiErrorCode.NotFound, message);
        
        public static ApiError AlreadyExists(string message = null) 
            => new ApiError(ApiErrorCode.AlreadyExists, message);
        
        public static ApiError BadRequest(string message = null) 
            => new ApiError(ApiErrorCode.BadRequest, message);
    }

    /// <summary>
    /// Информация об ошибке
    /// </summary>
    public class ApiError<TData> : ApiError
    {
        /// <summary>
        /// Дополнительная информация об ошибке
        /// </summary>
        public TData Data { get; set; }
        
        public ApiError(TData data, ApiErrorCode errorCode, string message = null) 
            : base(errorCode, message)
        {
            Data = data;
        }
        
        public static ApiError<TData> NotFound(TData data, string message = null) 
            => new ApiError<TData>(data, ApiErrorCode.NotFound, message);
        
        public static ApiError<TData> AlreadyExists(TData data, string message = null) 
            => new ApiError<TData>(data, ApiErrorCode.AlreadyExists, message);
        
        public static ApiError<TData> BadRequest(TData data, string message = null) 
            => new ApiError<TData>(data, ApiErrorCode.BadRequest, message);
    }
}