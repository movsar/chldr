using chldr_data.Resources.Localizations;
using chldr_tools;
using chldr_utils.Services;
using Microsoft.Extensions.Localization;

namespace chldr_api.GraphQL.MutationServices
{
    public class MutationService
    {
        protected readonly SqlContext dbContext;
        protected readonly IConfiguration _configuration;
        protected readonly IStringLocalizer<AppLocalizations> _localizer;
        protected readonly EmailService _emailService;

        public MutationService(IConfiguration configuration, IStringLocalizer<AppLocalizations> localizer, EmailService emailService)
        {
            dbContext = new SqlContext();
            _configuration = configuration;
            _localizer = localizer;
            _emailService = emailService;
        }
    }
}
