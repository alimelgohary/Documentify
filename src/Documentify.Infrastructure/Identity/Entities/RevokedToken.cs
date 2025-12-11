using Documentify.Domain.Entities.Common;

namespace Documentify.Infrastructure.Identity.Entities
{
    public class RevokedRefreshToken : EntityBase
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime NaturalExpireDate { get; set; }
    }
}
