namespace RealWorldConduit.Infrastructure.EntityConfigurations
{
    internal class BlogConfiguration : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.ToTable<Blog>(nameof(Blog), MainDbContext.BlogSchema);

            builder.Property(x => x.Title).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(250);
            builder.Property(x => x.Content).IsRequired();


            builder.HasIndex(x => x.Title).IsUnique();


            builder.HasOne(x => x.Author)
                   .WithMany(x => x.Blogs)
                   .HasForeignKey(x => x.AuthorId);
        }
    }
}
