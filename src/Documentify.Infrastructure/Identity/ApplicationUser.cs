using Documentify.Domain.Entities;
using Documentify.Domain.Entities.Common;
using Microsoft.AspNetCore.Identity;

namespace Documentify.Infrastructure.Data
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<ServiceBase>? ServicesWritten { get; set; }
        public ICollection<OfficeBase>? OfficesWritten { get; set; }
        public ICollection<ServiceSuggestion>? UpvotedServiceSuggestions { get; set; }
        public ICollection<OfficeSuggestion>? UpvotedOfficeSuggestions { get; set; }
        public ICollection<Office>? RatedOffices { get; set; }
        public ICollection<UserRateOffice>? UserRateOffices { get; set; }
    }
}
