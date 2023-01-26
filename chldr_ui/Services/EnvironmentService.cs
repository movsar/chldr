using chldr_ui.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_ui.Services
{
    public class EnvironmentService
    {
        public Platforms CurrentPlatform { get; set; }
        public EnvironmentService(Platforms platform)
        {
            CurrentPlatform = platform;
        }
    }
}
