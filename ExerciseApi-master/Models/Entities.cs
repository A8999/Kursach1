using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExerciseApi.Models
{
    public class FavoriteExercise
    {
        [Key]
        public int Id { get; set; }
        public long UserId { get; set; }
        public string ExerciseId { get; set; }
        public string ExerciseName { get; set; }
    }

    public class PlanExercise
    {
        [Key]
        public int Id { get; set; }
        public long UserId { get; set; }
        public string PlanName { get; set; }
        public string ExerciseId { get; set; }
        public string ExerciseName { get; set; }
    }

    public class TrainingHistory
    {
        [Key]
        public int Id { get; set; }
        public long UserId { get; set; }
        public string PlanName { get; set; }
        public DateTime Date { get; set; }
        public ICollection<TrainingHistoryExercise> Exercises { get; set; }
    }

    public class TrainingHistoryExercise
    {
        [Key]
        public int Id { get; set; }
        public int TrainingHistoryId { get; set; }
        [ForeignKey("TrainingHistoryId")]
        public TrainingHistory TrainingHistory { get; set; }
        public string ExerciseId { get; set; }
        public string ExerciseName { get; set; }
    }

    public class UserParameter
    {
        [Key]
        public long UserId { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
    }

    public class SBDRecord
    {
        [Key]
        public long UserId { get; set; }
        public double Squat { get; set; }
        public double Bench { get; set; }
        public double Deadlift { get; set; }
    }
}
