using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using nps_backend_adriana.Models.Entities;

namespace nps_backend_adriana.Services
{
    public class NpsLogConfiguration : IEntityTypeConfiguration<NpsLog>
    {
        public void Configure(EntityTypeBuilder<NpsLog> builder)
        {
            builder.ToTable("NpsLogs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DateScore)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasColumnType("nvarchar(150)");

            builder.Property(x => x.SystemId)
                .IsRequired();

            builder.Property(x => x.Score)
                .IsRequired();

            builder.Property(x => x.UserId)
                .HasColumnType("nvarchar(50)")
                .IsRequired();
        }
    }
}
