using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Attribution.Api.Dtos;
using Attribution.Domain.Dal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Attribution.Api.Controllers
{
    /// <summary>
    /// API для управления белым списком UrlReferrers
    /// </summary>
    [ApiController]
    [Route("api/v{api-version:apiVersion}/urlreferrers")]
    public class UrlReferrerController : ControllerBase
    {
        private readonly IUrlReferrerRepository _urlReferrerRepository;

        /// <inheritdoc />
        public UrlReferrerController(IUrlReferrerRepository urlReferrerRepository)
        {
            _urlReferrerRepository = urlReferrerRepository;
        }
        
        /// <summary>
        /// Возвращает весь белый список UrlReferrers 
        /// </summary>
        [HttpGet]
        public async Task<List<UrlReferrerDto>> GetAll()
        {
            var urlReferrers = await _urlReferrerRepository.GetAllAsync();
            return urlReferrers.Select(urlReferrer => new UrlReferrerDto(urlReferrer)).ToList();
        }
        
        /// <summary>
        /// Возвращает удаленные из белого списка UrlReferrers
        /// </summary>
        [HttpGet("deleted")]
        public async Task<List<UrlReferrerDto>> GetAllDeleted()
        {
            var urlReferrers = await _urlReferrerRepository.GetAllAsync(true);
            
            return urlReferrers.Select(urlReferrer => new UrlReferrerDto(urlReferrer)).ToList();
        }
        
        /// <summary>
        /// Возвращает информацию по указанному UrlReferrer 
        /// </summary>
        /// <param name="host">Host требуемого UrlReferrer</param>
        [HttpGet("{host}")]
        [ProducesResponseType(typeof(UrlReferrerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UrlReferrerDto>> Get([FromRoute] string host)
        {
            var result = await _urlReferrerRepository.GetByHostAsync(host);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(new UrlReferrerDto(result));
        }
        
        /// <summary>
        /// Добавляет новый UrlReferrer в белый список или обновляет существующий (в том числе снимает пометку удаления)
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(UrlReferrerDto), StatusCodes.Status200OK)]
        public async Task<ActionResult> Add([FromBody] UrlReferrerDto urlReferrerDto)
        {
            await _urlReferrerRepository.AddOrUpdateAsync(urlReferrerDto.ToUrlReferrer());
            
            return Ok();
        }
        
        /// <summary>
        /// Удаляет UrlReferrer из белого списка
        /// </summary>
        /// <param name="host">Host удаляемого UrlReferrer</param>
        [HttpPost("deleted/{host}")]
        [ProducesResponseType(typeof(UrlReferrerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Remove([FromRoute] string host)
        {
            try
            {
                await _urlReferrerRepository.RemoveByHostAsync(host);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            
            return Ok();
        }
    }
}