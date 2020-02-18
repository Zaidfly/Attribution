namespace Attribution.Api.Dtos
{
    /// <summary>
    /// Коды ошибок
    /// </summary>
    public enum ApiErrorCode
    {
        /// <summary>
        /// Не указан
        /// </summary>
        None = 0,
        
        /// <summary>
        /// Ошибка валидации входящих данных
        /// </summary>
        BadRequest = 400,
        
        /// <summary>
        /// Фича не найдена
        /// </summary>
        NotFound = 404,
        
        /// <summary>
        /// Фича с указанным ключом уже существует
        /// </summary>
        AlreadyExists = 409
    }
}