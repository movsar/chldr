namespace chldr_data.Interfaces
{
    public interface IFileService
    {
        static string AppDirectory { get; set; }
        static string AppDataDirectory { get; set; }
        static string DatabasePath { get; set; }
        void PrepareDatabaseFile();

    }
}
