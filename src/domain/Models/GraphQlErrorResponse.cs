namespace domain.Models
{
    public class GraphQlErrorResponse
    {
        public class Location
        {
            public int Line { get; set; }
            public int Column { get; set; }
        }

        public class Error
        {
            public string Message { get; set; }
            public List<Location> Locations { get; set; }
            public List<string> Path { get; set; }
            public Dictionary<string, string> Extensions { get; set; }
        }
        public List<Error> Errors { get; set; }
    }
}
