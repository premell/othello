﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using othello_api.Contexts;

#nullable disable

namespace othelloapi.Migrations
{
    [DbContext(typeof(OthelloContext))]
    [Migration("20230312134054_things")]
    partial class things
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("othello_api.Models.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BlackTimeSeconds")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("TimeIncrementSeconds")
                        .HasColumnType("int");

                    b.Property<int>("TimeLimitSeconds")
                        .HasColumnType("int");

                    b.Property<int>("WhiteTimeSeconds")
                        .HasColumnType("int");

                    b.Property<int>("Winner")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("othello_api.Models.Game", b =>
                {
                    b.OwnsOne("othello_api.Models.GameState", "State", b1 =>
                        {
                            b1.Property<int>("GameId")
                                .HasColumnType("int");

                            b1.Property<string>("State")
                                .IsRequired()
                                .HasMaxLength(150)
                                .HasColumnType("nvarchar(150)");

                            b1.HasKey("GameId");

                            b1.ToTable("Games");

                            b1.WithOwner()
                                .HasForeignKey("GameId");
                        });

                    b.Navigation("State")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
