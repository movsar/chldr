namespace chldr_data.Dto
{
    public class ChangeDto
    {
        public string ChangeSetId { get; set; }
        public string Property { get; set; }
        public dynamic OldValue { get; set; }
        public dynamic NewValue { get; set; }
    }
}
