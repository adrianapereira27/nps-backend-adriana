using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using nps_backend_adriana.Models.Entities;

namespace nps_backend_adriana.Services.Configuration
{
    public class NpsLogConfiguration : IEntityTypeConfiguration<NpsLog>
    {
        public void Configure(EntityTypeBuilder<NpsLog> builder)
        {
            builder.ToTable("NpsLog");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DateScore)
                .IsRequired();

            builder.Property(x => x.SystemId)
                .IsRequired();

            builder.Property(x => x.Score)
                .IsRequired();

            builder.Property(x => x.UserId)
                .IsRequired();
        }
    }
}
