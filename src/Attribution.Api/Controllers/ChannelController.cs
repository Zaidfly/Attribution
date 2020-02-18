using System.Collections.Generic;
using System.Threading.Tasks;
using Attribution.Api.Dtos;
using Attribution.Api.Validators;
using Attribution.Domain.Dal;
using Attribution.Domain.Managers;
using Attribution.Domain.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Attribution.Api.Controllers
{
    /// <summary>
    /// API для управления справочника каналов
    /// </summary>
    [ApiController]
    [Route("api/v{api-version:apiVersion}/channels")]
    public class ChannelController : ControllerBase
    {
        private readonly IChannelRepository _channelRepository;
        private readonly IMapper _mapper;

        /// <inheritdoc />
        public ChannelController(IChannelRepository channelRepository, IMapper mapper)
        {
            _channelRepository = channelRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Возвращает все имеющиеся каналы
        /// </summary>
        [HttpGet]
        public async Task<List<ChannelWithIdDto>> GetAll()
        {
            var channels = await _channelRepository.GetAllAsync();
            return _mapper.Map<List<ChannelWithIdDto>>(channels);
        }

        /// <summary>
        /// Возвращает канал по указанному идентификатору 
        /// </summary>
        /// <param name="id">идентификатор канала</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ChannelDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ChannelWithIdDto>> Get([FromRoute] int id)
        {
            var channel = await _channelRepository.GetByIdAsync(id);
            if (channel == null)
            {
                return NotFound(ApiError.NotFound($"Channel with id {id} does not exists"));
            }

            var channelDto = _mapper.Map<ChannelWithIdDto>(channel);

            return Ok(channelDto);
        }

        /// <summary>
        /// Добавляет новый канал
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ChannelDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ChannelWithIdDto>> Add([FromBody] ChannelDto channelDto)
        {
            var channel = _mapper.Map<Channel>(channelDto);

            var addedChannel = await _channelRepository.AddAsync(channel);
            
            return Ok(_mapper.Map<ChannelWithIdDto>(addedChannel));
        }

        /// <summary>
        /// Обновляет данные по каналу
        /// </summary>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Update([FromBody] ChannelWithIdDto channelDto)
        {
            var channel = _mapper.Map<Channel>(channelDto);

            if (await _channelRepository.UpdateAsync(channel))
            {
                return Ok();
            }

            return NotFound(ApiError.NotFound($"Channel with id {channelDto.Id} does not exists"));
        }
    }
}