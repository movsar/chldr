using System.Diagnostics;

namespace chldr_utils.Services
{
    public class ConsoleService
    {
        private string _tag = string.Empty;
        private bool _isIgnored = false;
        private Stopwatch _stopWatch = new Stopwatch();
        public ConsoleService(string tag, bool isIgnored = false)
        {
            _tag = tag;
            _isIgnored = isIgnored;
            if (_isIgnored)
            {
                return;
            }
            Console.WriteLine($"\nLogging {_tag}:");
        }
        public void EchoOff()
        {
            _isIgnored = true;
        }
        public void LogInformation(string message)
        {
            if (_isIgnored)
            {
                return;
            }

            Console.WriteLine($"\t{message}");
        }
        public void StartSpeedTest()
        {
            _stopWatch.Start();
        }

        public void StopSpeedTest(string message)
        {
            LogInformation($"{message} : {_stopWatch.ElapsedMilliseconds}ms");
            _stopWatch.Stop();
            _stopWatch.Reset();
        }
    }
}
