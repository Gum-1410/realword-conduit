﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using RealWorldConduit.Infrastructure;

#nullable disable

namespace RealworldConduit.Infrastructure.Migrations
{
    [DbContext(typeof(MainDbContext))]
    [Migration("20231208065211_add-slug-into-tables")]
    partial class addslugintotables
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "uuid-ossp");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("RealWorldConduit.Domain.Entities.Blog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<DateTime>("LastUpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("Slug")
                        .IsUnique();

                    b.HasIndex("Title")
                        .IsUnique();

                    b.ToTable("Blog", "blog");
                });

            modelBuilder.Entity("RealWorldConduit.Domain.Entities.BlogTag", b =>
                {
                    b.Property<Guid>("BlogId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("TagId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("LastUpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("BlogId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("BlogTag", "blog");
                });

            modelBuilder.Entity("RealWorldConduit.Domain.Entities.FavoriteBlog", b =>
                {
                    b.Property<Guid>("BlogId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("FavoritedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("LastUpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("BlogId", "FavoritedById");

                    b.HasIndex("FavoritedById");

                    b.ToTable("FavoriteBlog", "blog");
                });

            modelBuilder.Entity("RealWorldConduit.Domain.Entities.RefreshToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AccessToken")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ExpiredDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("LastUpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("AccessToken");

                    b.HasIndex("ExpiredDate");

                    b.HasIndex("UserId");

                    b.ToTable("RefreshToken", "user");
                });

            modelBuilder.Entity("RealWorldConduit.Domain.Entities.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("LastUpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Tag", "blog");
                });

            modelBuilder.Entity("RealWorldConduit.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Bio")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<DateTime>("LastUpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("ProfileImage")
                        .HasColumnType("text");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Slug")
                        .IsUnique();

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("User", "user");
                });

            modelBuilder.Entity("RealWorldConduit.Domain.Entities.UserFollower", b =>
                {
                    b.Property<Guid>("FollowedUserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("FollowerId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("LastUpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("FollowedUserId", "FollowerId");

                    b.HasIndex("FollowerId");

                    b.ToTable("UserFollower", "user");
                });

            modelBuilder.Entity("RealWorldConduit.Domain.Entities.Blog", b =>
                {
                    b.HasOne("RealWorldConduit.Domain.Entities.User", "Author")
                        .WithMany("Blogs")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("RealWorldConduit.Domain.Entities.BlogTag", b =>
                {
                    b.HasOne("RealWorldConduit.Domain.Entities.Blog", "Blog")
                        .WithMany("BlogTags")
                        .HasForeignKey("BlogId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RealWorldConduit.Domain.Entities.Tag", "Tag")
                        .WithMany("BlogTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Blog");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("RealWorldConduit.Domain.Entities.FavoriteBlog", b =>
                {
                    b.HasOne("RealWorldConduit.Domain.Entities.Blog", "Blog")
                        .WithMany("FavoriteBlogs")
                        .HasForeignKey("BlogId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RealWorldConduit.Domain.Entities.User", "FavoritedBy")
                        .WithMany("FavoriteBlogs")
                        .HasForeignKey("FavoritedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Blog");

                    b.Navigation("FavoritedBy");
                });

            modelBuilder.Entity("RealWorldConduit.Domain.Entities.RefreshToken", b =>
                {
                    b.HasOne("RealWorldConduit.Domain.Entities.User", "User")
                        .WithMany("RefreshToken")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("RealWorldConduit.Domain.Entities.UserFollower", b =>
                {
                    b.HasOne("RealWorldConduit.Domain.Entities.User", "FollowedUser")
                        .WithMany("FollowedUsers")
                        .HasForeignKey("FollowedUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RealWorldConduit.Domain.Entities.User", "Follower")
                        .WithMany("Followers")
                        .HasForeignKey("FollowerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FollowedUser");

                    b.Navigation("Follower");
                });

            modelBuilder.Entity("RealWorldConduit.Domain.Entities.Blog", b =>
                {
                    b.Navigation("BlogTags");

                    b.Navigation("FavoriteBlogs");
                });

            modelBuilder.Entity("RealWorldConduit.Domain.Entities.Tag", b =>
                {
                    b.Navigation("BlogTags");
                });

            modelBuilder.Entity("RealWorldConduit.Domain.Entities.User", b =>
                {
                    b.Navigation("Blogs");

                    b.Navigation("FavoriteBlogs");

                    b.Navigation("FollowedUsers");

                    b.Navigation("Followers");

                    b.Navigation("RefreshToken");
                });
#pragma warning restore 612, 618
        }
    }
}
