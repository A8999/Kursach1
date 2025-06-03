namespace ExerciseApi.Body
{
    public class TrainingPlanDto
    {
        public string PlanName { get; set; }
        public List<PlanExerciseDto> Exercises { get; set; } = new();
    }
    public class PlanExerciseDto
    {
        public string ExerciseId { get; set; }
        public string ExerciseName { get; set; }
    }
    public class TrainingHistoryDto
    {
        public string PlanName { get; set; }
        public List<PlanExerciseDto> Exercises { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
    public class DonePlanDto
    {
        public long TelegramId { get; set; }
        public string PlanName { get; set; }
    }
    public class PlanDto
    {
        public long TelegramId { get; set; }
        public string PlanName { get; set; }
    }
    public class AddExerciseToPlanDto
    {
        public long TelegramId { get; set; }
        public string PlanName { get; set; }
        public string ExerciseId { get; set; }
        public string ExerciseName { get; set; }
    }
    public class RemoveExerciseFromPlanDto
    {
        public long TelegramId { get; set; }
        public string PlanName { get; set; }
        public string ExerciseId { get; set; }
    }
    public class EditExerciseInPlanDto
    {
        public long TelegramId { get; set; }
        public string PlanName { get; set; }
        public string OldExerciseId { get; set; }
        public string NewExerciseId { get; set; }
        public string NewExerciseName { get; set; }
    }
    public class AiRecommendDto
    {
        public long TelegramId { get; set; }
        public string PlanName { get; set; }
    }
}
