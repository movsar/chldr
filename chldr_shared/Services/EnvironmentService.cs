using chldr_shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_shared.Services
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
