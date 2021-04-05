using DevCars.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevCars.API.Persistence.Configurations
{
    public class ExtraOderDbConfiguration : IEntityTypeConfiguration<ExtraOderItem>
    {
        public void Configure(EntityTypeBuilder<ExtraOderItem> builder)
        {
            builder
                .HasKey(e => e.Id);
        }
    }
}
