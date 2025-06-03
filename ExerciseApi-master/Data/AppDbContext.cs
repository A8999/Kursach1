using Microsoft.EntityFrameworkCore;
using ExerciseApi.Models;

namespace ExerciseApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<FavoriteExercise> FavoriteExercises { get; set; }
        public DbSet<PlanExercise> PlanExercises { get; set; }
        public DbSet<TrainingHistory> TrainingHistories { get; set; }
        public DbSet<TrainingHistoryExercise> TrainingHistoryExercises { get; set; }
        public DbSet<UserParameter> UserParameters { get; set; }
        public DbSet<SBDRecord> SBDRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TrainingHistory>()
                .HasMany(h => h.Exercises)
                .WithOne(e => e.TrainingHistory)
                .HasForeignKey(e => e.TrainingHistoryId);
        }
    }
}
