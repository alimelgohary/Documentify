namespace Documentify.Domain.Entities.Common
{
    public partial class ServiceBase : EntityBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsAvailableOnline { get; set; }
        public string? Description { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public string? Notes { get; set; }

        /* TODO: Add Trigger to calculate when adding|removing|updating a step */
        public int EstimatedTime { get; set; } 
        public decimal EstimatedCost { get; set; } 
        /* TODO: Add Trigger to calculate it when adding|removing|updating a step */
        public string? WriterId { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public ICollection<OfficeBase>? Offices { get; set; }
        public ICollection<Step>? Steps { get; set; }
    }
    public partial class ServiceBase
    {
        public static class ValidationConstants
        {
            public const int NameMaxLength = 200;
            public const int NameMinLength = 5;
            public const int DescriptionMaxLength = 500;
            public const int NotesMaxLength = 500;
        }
    }
}
