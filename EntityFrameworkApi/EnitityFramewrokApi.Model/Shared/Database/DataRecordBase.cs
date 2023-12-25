
namespace EntityFrameworkApi.Model.Shared.Database
{
    public class DataRecordBase
    {
        public long Id { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public long CreatorUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long ModifierUserId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
