using Attribution.Api.Dtos;
using FluentValidation;

namespace Attribution.Api.Validators
{
    public class ChannelWithIdDtoValidator : AbstractValidator<ChannelWithIdDto>
    {
        public ChannelWithIdDtoValidator()
        {
            RuleFor(channelDto => channelDto.Id)
                .GreaterThan(0);
            
            RuleFor(channelDto => channelDto.Owner)
                .NotEmpty();
        }
    }
}