namespace Attribution.UserActionService.Models
{
    public class TaskEntityDto
    {
        public int Id { get; set; }
        public int CreatorId { get; set; }
        public TaskAttributesDto Attributes { get; set; }
    }
}