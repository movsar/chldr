namespace chldr_data.Models
{
    public class Change
    {
        public string Property { get; set; }
        public dynamic OldValue { get; set; }
        public dynamic NewValue { get; set; }
    }
}
