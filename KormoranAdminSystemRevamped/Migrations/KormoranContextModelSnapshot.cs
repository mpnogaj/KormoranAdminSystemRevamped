﻿// <auto-generated />
using System;
using KormoranAdminSystemRevamped.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KormoranAdminSystemRevamped.Migrations
{
	[DbContext(typeof(KormoranContext))]
	partial class KormoranContextModelSnapshot : ModelSnapshot
	{
		protected override void BuildModel(ModelBuilder modelBuilder)
		{
#pragma warning disable 612, 618
			modelBuilder
				.HasAnnotation("Relational:MaxIdentifierLength", 64)
				.HasAnnotation("ProductVersion", "5.0.12");

			modelBuilder.Entity("KormoranAdminSystemRevamped.Models.Discipline", b =>
				{
					b.Property<int>("Id")
						.ValueGeneratedOnAdd()
						.HasColumnType("int(11)")
						.HasColumnName("id");

					b.Property<string>("Name")
						.IsRequired()
						.HasColumnType("longtext")
						.HasColumnName("name");

					b.HasKey("Id");

					b.ToTable("disciplines");
				});

			modelBuilder.Entity("KormoranAdminSystemRevamped.Models.State", b =>
				{
					b.Property<int>("Id")
						.ValueGeneratedOnAdd()
						.HasColumnType("int(11)")
						.HasColumnName("id");

					b.Property<string>("Name")
						.IsRequired()
						.HasColumnType("longtext")
						.HasColumnName("name");

					b.HasKey("Id");

					b.ToTable("states");
				});

			modelBuilder.Entity("KormoranAdminSystemRevamped.Models.Tournament", b =>
				{
					b.Property<int>("Id")
						.ValueGeneratedOnAdd()
						.HasColumnType("int(11)")
						.HasColumnName("id");

					b.Property<int?>("DisciplineId")
						.HasColumnType("int(11)");

					b.Property<string>("Name")
						.IsRequired()
						.HasColumnType("longtext")
						.HasColumnName("name");

					b.Property<int?>("StateId")
						.HasColumnType("int(11)");

					b.Property<string>("TournamentType")
						.IsRequired()
						.HasColumnType("longtext")
						.HasColumnName("tournament_type");

					b.Property<string>("TournamentTypeShort")
						.IsRequired()
						.HasColumnType("longtext")
						.HasColumnName("tournament_type_short");

					b.HasKey("Id");

					b.HasIndex("DisciplineId");

					b.HasIndex("StateId");

					b.ToTable("tournaments");
				});

			modelBuilder.Entity("KormoranAdminSystemRevamped.Models.User", b =>
				{
					b.Property<int>("Id")
						.ValueGeneratedOnAdd()
						.HasColumnType("int(11)")
						.HasColumnName("id");

					b.Property<string>("Fullname")
						.IsRequired()
						.HasColumnType("longtext")
						.HasColumnName("fullname");

					b.Property<string>("Login")
						.IsRequired()
						.HasColumnType("longtext")
						.HasColumnName("user");

					b.Property<string>("PasswordHash")
						.IsRequired()
						.HasColumnType("longtext")
						.HasColumnName("pass");

					b.Property<string>("Permissions")
						.HasColumnType("json")
						.HasColumnName("permissions");

					b.HasKey("Id");

					b.ToTable("users");
				});

			modelBuilder.Entity("KormoranAdminSystemRevamped.Models.Tournament", b =>
				{
					b.HasOne("KormoranAdminSystemRevamped.Models.Discipline", "Discipline")
						.WithMany()
						.HasForeignKey("DisciplineId");

					b.HasOne("KormoranAdminSystemRevamped.Models.State", "State")
						.WithMany()
						.HasForeignKey("StateId");

					b.Navigation("Discipline");

					b.Navigation("State");
				});
#pragma warning restore 612, 618
		}
	}
}
