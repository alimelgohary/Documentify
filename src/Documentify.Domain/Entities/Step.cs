
using Documentify.Domain.Entities.Common;

namespace Documentify.Domain.Entities
{
    public partial class Step : EntityBase
    {
        public int Order { get; set; }
        public int TimeRequired { get; set; }
        public decimal CostRequired { get; set; }
        public string? Details { get; set; }
        public Guid AssociatedServiceId { get; set; }
        public ServiceBase AssociatedService { get; set; } = null!;
        public Guid? InnerServiceId { get; set; }
        public ServiceBase? InnerService { get; set; }
        public int? InnerServiceCountOriginals { get; set; }
        public int? InnerServiceCountCopies { get; set; }
        
    }
   
    public partial class Step
    {
        public static class ValidationConstants
        {
            public const int DetailsMaxLength = 200;
            public const int DetailsMinLength = 10;
        }
    }
}
