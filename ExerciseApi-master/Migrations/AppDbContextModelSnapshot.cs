using System;
using ExerciseApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ExerciseApi.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ExerciseApi.Models.FavoriteExercise", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ExerciseId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ExerciseName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("FavoriteExercises");
                });

            modelBuilder.Entity("ExerciseApi.Models.PlanExercise", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ExerciseId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ExerciseName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PlanName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("PlanExercises");
                });

            modelBuilder.Entity("ExerciseApi.Models.SBDRecord", b =>
                {
                    b.Property<long>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("UserId"));

                    b.Property<double>("Bench")
                        .HasColumnType("double precision");

                    b.Property<double>("Deadlift")
                        .HasColumnType("double precision");

                    b.Property<double>("Squat")
                        .HasColumnType("double precision");

                    b.HasKey("UserId");

                    b.ToTable("SBDRecords");
                });

            modelBuilder.Entity("ExerciseApi.Models.TrainingHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PlanName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("TrainingHistories");
                });

            modelBuilder.Entity("ExerciseApi.Models.TrainingHistoryExercise", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ExerciseId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ExerciseName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("TrainingHistoryId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("TrainingHistoryId");

                    b.ToTable("TrainingHistoryExercises");
                });

            modelBuilder.Entity("ExerciseApi.Models.UserParameter", b =>
                {
                    b.Property<long>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("UserId"));

                    b.Property<double>("Height")
                        .HasColumnType("double precision");

                    b.Property<double>("Weight")
                        .HasColumnType("double precision");

                    b.HasKey("UserId");

                    b.ToTable("UserParameters");
                });

            modelBuilder.Entity("ExerciseApi.Models.TrainingHistoryExercise", b =>
                {
                    b.HasOne("ExerciseApi.Models.TrainingHistory", "TrainingHistory")
                        .WithMany("Exercises")
                        .HasForeignKey("TrainingHistoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TrainingHistory");
                });

            modelBuilder.Entity("ExerciseApi.Models.TrainingHistory", b =>
                {
                    b.Navigation("Exercises");
                });
#pragma warning restore 612, 618
        }
    }
}
