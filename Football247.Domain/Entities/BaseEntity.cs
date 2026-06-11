using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Football247.Domain.Entities
{
    public class BaseEntity
    {
        [Key]
        [Column(Order = 0)]
        public virtual Guid Id { get; set; }

        [Column(Order = 101)]
        public virtual Guid CreatedUserId { get; set; }

        [Column(Order = 102)]
        public virtual Guid? UpdatedUserId { get; set; }

        [Column(Order = 103)]
        public virtual Guid? DeletedUserId { get; set; }

        [Column(Order = 104)]
        [MaxLength(100)]
        public virtual string CreatedFullName { get; set; }

        [Column(Order = 105)]
        public virtual string? UpdatedFullName { get; set; }

        [Column(Order = 106)]
        [MaxLength(100)]
        public virtual string? DeletedFullName { get; set; }

        [Column(Order = 107)]
        [MaxLength(100)]
        public virtual DateTime CreatedDate { get; set; }

        [Column(Order = 108)]
        public virtual DateTime? UpdatedDate { get; set; }

        [Column(Order = 109)]
        public virtual DateTime? DeletedDate { get; set; }

        [Column(Order = 110)]
        [DefaultValue("false")]
        public virtual bool IsDeleted { get; set; }

        protected BaseEntity()
        {
            CreatedDate = DateTime.UtcNow;
            CreatedUserId = Guid.Empty;
            CreatedFullName = string.Empty;
            Id = Guid.Empty;
        }


        public virtual bool IsTransient()
        {
            return Id == Guid.Empty;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is BaseEntity))
            {
                return false;
            }

            if (this == obj)
            {
                return true;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            BaseEntity entity = (BaseEntity)obj;
            if (entity.IsTransient() || IsTransient())
            {
                return false;
            }

            return entity.Id == Id;
        }
    }
}
