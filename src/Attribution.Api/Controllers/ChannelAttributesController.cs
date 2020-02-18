using System.Threading.Tasks;

using Attribution.Api.Dtos;
using Attribution.Domain.Managers;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

namespace Attribution.Api.Controllers
{
    /// <summary>
    /// API для управления справочника каналов
    /// </summary>
    [ApiController]
    [Route("api/v{api-version:apiVersion}/channelattributes")]
    public class ChannelAttributesController : ControllerBase
    {
        private readonly IChannelAttributesManager _channelAttributesManager;
        private readonly IMapper _mapper;
        
        /// <inheritdoc />
        public ChannelAttributesController(IChannelAttributesManager channelAttributesManager, IMapper mapper)
        {
            _channelAttributesManager = channelAttributesManager;
            _mapper = mapper;
        }

        /// <summary>
        /// Возвращает заголовок набора данных аттрибуции
        /// </summary>
        [HttpGet("title")]
        public async Task<ActionResult<ChannelAttributesTitleDto>> GetChannelAttributesTitle([FromQuery] long hash)
        {
            var title = await _channelAttributesManager.GetTitleAsync(hash);

            var titleDto = _mapper.Map<ChannelAttributesTitleDto>(title);
            
            return Ok(titleDto);
        }
    }
}