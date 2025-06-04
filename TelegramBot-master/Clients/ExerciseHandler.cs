using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using VshghCoachBot;
using VshghCoachBot.Models;

namespace APIprot.Clients
{
    public class ExerciseHandler
    {
        private readonly Client _client;

        public ExerciseHandler(Client client)
        {
            _client = client;
        }

        public async Task Handle(ITelegramBotClient bot, Update update)
        {
            var message = update.Message;
            if (message?.Text == null) return;

            string bodyPart = "";
            string botMessage = "";

            switch (message.Text.ToLower())
            {
                case "/start":
                    await bot.SendTextMessageAsync(message.Chat.Id, VshghCoachBot.BotMessages.CommandsResponse.start);
                    return;
                case "/back": bodyPart = "back"; break;
                case "/chest": bodyPart = "chest"; break;
                case "/shoulders": bodyPart = "shoulders"; break;
                case "/arms": bodyPart = "upper arms"; break;
                case "/legs": bodyPart = "upper legs"; break;
                case "/cardio": bodyPart = "cardio"; break;
                case "/neck": bodyPart = "neck"; break;
                case "/waist": bodyPart = "waist"; break;
                case string text when text.StartsWith("/caloriesetup"):
                    var parts = text.Split(' ');
                    if (parts.Length != 4 && parts.Length != 6)
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "Формат: /caloriesetup [стать: m/f] [вік] [рівень активності: 1.2/1.375/1.55/1.725/1.9] [зріст, см] [вага, кг] (останні два параметри необов'язкові, якщо вже задані)");
                        return;
                    }
                    string gender = parts[1].ToLower();
                    if (gender != "m" && gender != "male" && gender != "f" && gender != "female")
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "Такої статі не існує. Формат: /caloriesetup [стать: m/f] [вік] [рівень активності: 1.2/1.375/1.55/1.725/1.9]");
                        return;
                    }
                    if (!int.TryParse(parts[2], out int age) || age < 1 || age > 100)
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "Не правильно вказаний вік");
                        return;
                    }
                    if (!double.TryParse(parts[3], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double activity) ||
                        (activity != 1.2 && activity != 1.375 && activity != 1.55 && activity != 1.725 && activity != 1.9))
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "Вказаний рівень активності не коректний. Оберіть доступний з переліку вище");
                        return;
                    }
                    double caloriesHeight, caloriesWeight;
                    if (parts.Length == 6)
                    {
                        if (!double.TryParse(parts[4], out caloriesHeight) || !double.TryParse(parts[5], out caloriesWeight))
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id, "Неправильний формат. Зріст і вага мають бути числами.");
                            return;
                        }
                    }
                    else
                    {
                        var userParams = await _client.GetUserParameters(message.Chat.Id);
                        if (userParams == null || userParams.Value.height == 0 || userParams.Value.weight == 0)
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id, "Введіть зріст і вагу у команді: /caloriesetup [стать] [вік] [активність] [зріст] [вага]");
                            return;
                        }
                        caloriesHeight = userParams.Value.height;
                        caloriesWeight = userParams.Value.weight;
                    }
                    double bmr = gender == "m" || gender == "male"
                        ? 88.362 + (13.397 * caloriesWeight) + (4.799 * caloriesHeight) - (5.677 * age)
                        : 447.593 + (9.247 * caloriesWeight) + (3.098 * caloriesHeight) - (4.330 * age);
                    double calories = bmr * activity;
                    await bot.SendTextMessageAsync(message.Chat.Id, $"Ваша добова норма калорій: {calories:F0} ккал");
                    return;
                case string text when text.StartsWith("/addfavorite"):
                    var addFavParts = text.Split(' ');
                    if (addFavParts.Length != 2)
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "Неправильний формат. Правильний: /addfavorite [id_вправи]");
                        return;
                    }
                    string addId = addFavParts[1];
                    string name = await _client.GetExerciseNameById(addId);
                    if (string.IsNullOrEmpty(name) || name == "Невідома вправа")
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "Вправу не знайдено. Перевірте id або скористайтесь /exercises для перегляду доступних вправ.");
                        return;
                    }
                    await _client.AddFavoriteExercise(message.Chat.Id, addId, name);
                    botMessage = "Вправа додана до улюблених.";
                    break;

                case string text when text.StartsWith("/removefavorite"):
                    var remFavParts = text.Split(' ');
                    if (remFavParts.Length != 2)
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "Неправильний формат. Правильний: /removefavorite [id_вправи]");
                        return;
                    }
                    string removeId = remFavParts[1];
                    bool removed = await _client.RemoveFavoriteExercise(message.Chat.Id, removeId);
                    botMessage = removed ? "Вправу видалено" : "Цієї вправи немає в списку улюблених вправ";
                    break;
                case "/favorites":
                    var favs = await _client.GetFavoriteExercises(message.Chat.Id);
                    botMessage = favs.Count > 0
                        ? string.Join("\n", favs.Select(f => $"[{f.ExerciseId}] {f.ExerciseName}"))
                        : "У вас немає улюблених вправ.";
                    break;
                case string text when text.StartsWith("/createplan"):
                    var createPlanParts = text.Split(' ');
                    if (createPlanParts.Length != 2)
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "Неправильний формат. Правильний: /createplan [назва]");
                        return;
                    }
                    await _client.CreateTrainingPlan(message.Chat.Id, createPlanParts[1]);
                    botMessage = $"План \"{createPlanParts[1]}\" створено.";
                    break;
                case string text when text.StartsWith("/addtoplan"):
                    var addToPlanParts = text.Split(' ');
                    if (addToPlanParts.Length != 3)
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "Неправильний формат. Правильний: /addtoplan [назва_плану] [id_вправи]");
                        return;
                    }
                    string planNameAdd = addToPlanParts[1];
                    string exIdAdd = addToPlanParts[2];
                    string exNameAdd = await _client.GetExerciseNameById(exIdAdd);
                    if (string.IsNullOrEmpty(exNameAdd) || exNameAdd == "Невідома вправа")
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "Вправу не знайдено. Перевірте id або скористайтесь /exercises для перегляду доступних вправ.");
                        return;
                    }
                    await _client.AddExerciseToPlan(message.Chat.Id, planNameAdd, exIdAdd, exNameAdd);
                    botMessage = $"Вправу [{exIdAdd}] {exNameAdd} додано до плану '{planNameAdd}'.";
                    break;
                case string text when text.StartsWith("/removefromplan"):
                    var remFromPlanParts = text.Split(' ');
                    if (remFromPlanParts.Length != 3)
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "Неправильний формат. Правильний: /removefromplan [назва_плану] [id_вправи]");
                        return;
                    }
                    string planNameRem = remFromPlanParts[1];
                    string exIdRem = remFromPlanParts[2].Trim();
                    bool removedFromPlan = await _client.RemoveExerciseFromPlan(message.Chat.Id, planNameRem, exIdRem);
                    botMessage = removedFromPlan
                        ? $"Вправу [{exIdRem}] видалено з плану '{planNameRem}'."
                        : "Цієї вправи немає в плані.";
                    break;
                case string text when text.StartsWith("/editplanexercise"):
                    var editPlanParts = text.Split(' ');
                    if (editPlanParts.Length != 4)
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "Неправильний формат. Правильний: /editplanexercise [назва_плану] [старий_id] [новий_id]");
                        return;
                    }
                    string planNameEdit = editPlanParts[1];
                    string oldExId = editPlanParts[2];
                    string newExId = editPlanParts[3];
                    string newExName = await _client.GetExerciseNameById(newExId);
                    if (string.IsNullOrEmpty(newExName) || newExName == "Невідома вправа")
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "Нову вправу не знайдено. Перевірте id або скористайтесь /exercises для перегляду доступних вправ.");
                        return;
                    }
                    await _client.EditExerciseInPlan(message.Chat.Id, planNameEdit, oldExId, newExId, newExName);
                    botMessage = $"Вправу [{oldExId}] замінено на [{newExId}] {newExName} у плані '{planNameEdit}'.";
                    break;
                case "/myplans":
                    var plans = await _client.GetUserPlans(message.Chat.Id);
                    if (plans.Count == 0)
                    {
                        botMessage = "У вас немає жодного плану.";
                    }
                    else
                    {
                        botMessage = string.Join("\n\n", plans.Select(p => $"План: {p.PlanName}\n" + (p.Exercises.Count > 0 ? string.Join("\n", p.Exercises.Select(e => $"[{e.ExerciseId}] {e.ExerciseName}")) : "(немає вправ)")));
                    }
                    break;
                case string text when text.StartsWith("/ai"):
                    var aiParts = text.Split(' ');
                    if (aiParts.Length != 2)
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "Неправильний формат. Правильний: /ai [назва_плану]");
                        return;
                    }
                    string aiPlanName = aiParts[1];
                    var aiResult = await _client.AiRecommend(message.Chat.Id, aiPlanName);
                    if (!string.IsNullOrEmpty(aiResult?.added))
                    {
                        botMessage = $"AI додав вправу: {aiResult.added} (група: {aiResult.bodyPart})\n\n{aiResult.aiResponse}";
                    }
                    else if (!string.IsNullOrEmpty(aiResult?.aiResponse) && aiResult.aiResponse.StartsWith("[AI Error]"))
                    {
                        botMessage = aiResult.aiResponse;
                    }
                    else
                    {
                        botMessage = $"AI не зміг запропонувати вправу.\n\n{aiResult?.aiResponse}";
                    }
                    break;
                case "/help":
                    await bot.SendTextMessageAsync(message.Chat.Id, VshghCoachBot.BotMessages.CommandsResponse.help);
                    return;
                case "/exercises":
                    await bot.SendTextMessageAsync(message.Chat.Id, VshghCoachBot.BotMessages.CommandsResponse.exercises);
                    return;
                case string text when text.StartsWith("/set"):
                    var setParts = text.Split(' ');
                    double setHeight, setWeight;
                    if (setParts.Length != 3 || !double.TryParse(setParts[1], out setHeight) || !double.TryParse(setParts[2], out setWeight))
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "Неправильний формат. Правильний: /set [зріст] [вага]");
                        return;
                    }
                    await _client.SetUserParameters(message.Chat.Id, setHeight, setWeight);
                    await bot.SendTextMessageAsync(message.Chat.Id, "Ваші параметри збережено.");
                    return;
                case string text when text.StartsWith("/bmi"):
                    if (text.Trim() != "/bmi")
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "Неправильний формат. Просто: /bmi");
                        return;
                    }
                    var parameters = await _client.GetUserParameters(message.Chat.Id);
                    if (parameters == null)
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "Параметри не знайдено. Введіть їх через команду /set.");
                        return;
                    }
                    double bmiHeight = parameters.Value.height;
                    double bmiWeight = parameters.Value.weight;
                    double bmi = bmiWeight / ((bmiHeight / 100) * (bmiHeight / 100));
                    string category = bmi < 18.5 ? "Надмірна худорлявість" :
                                     bmi < 25 ? "Норма" :
                                     bmi < 30 ? "Надмірна вага" : "Ожиріння";
                    await bot.SendTextMessageAsync(message.Chat.Id, $"Індекс маси тіла (BMI): {bmi:F2} — {category}");
                    return;
                case string text when text.StartsWith("/sbdset"):
                    var sbdParts = text.Split(' ');
                    if (sbdParts.Length != 4 || !double.TryParse(sbdParts[1], out double squat) || !double.TryParse(sbdParts[2], out double bench) || !double.TryParse(sbdParts[3], out double deadlift))
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "Неправильний формат. Правильний: /sbdset [присідання] [жим лежачи] [станова тяга]");
                        return;
                    }
                    await _client.SetSBD(message.Chat.Id, squat, bench, deadlift);
                    await bot.SendTextMessageAsync(message.Chat.Id, "Ваші SBD-результати збережено.");
                    return;
                case string text when text.StartsWith("/deletesbd"):
                    if (text.Trim() != "/deletesbd")
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "Неправильний формат. Просто: /deletesbd");
                        return;
                    }
                    await _client.DeleteSBD(message.Chat.Id);
                    await bot.SendTextMessageAsync(message.Chat.Id, "Ваші SBD-результати видалено.");
                    return;
                case "/sbd":
                    var sbdResult = await _client.GetSBD(message.Chat.Id);
                    if (sbdResult == null)
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "Дані SBD не знайдено.");
                    }
                    else
                    {
                        var (sbdSquat, sbdBench, sbdDeadlift) = sbdResult.Value;
                        double total = sbdSquat + sbdBench + sbdDeadlift;
                        await bot.SendTextMessageAsync(message.Chat.Id, $"Ваші SBD результати:\nПрисідання: {sbdSquat} кг\nЖим: {sbdBench} кг\nТяга: {sbdDeadlift} кг\nЗагалом: {total} кг");
                    }
                    return;
                case string text when text.StartsWith("/killplan"):
                    var killParts = text.Split(' ');
                    if (killParts.Length != 2)
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "Неправильний формат. Правильний: /killplan [назва_плану]");
                        return;
                    }
                    string planNameToDelete = killParts[1];
                    bool deleted = await _client.DeleteTrainingPlan(message.Chat.Id, planNameToDelete);
                    if (deleted)
                        botMessage = $"План \"{planNameToDelete}\" успішно видалено.";
                    else
                        botMessage = "Такого плану не створено.";
                    break;
                case "/deleteparameters":
                    await _client.DeleteUserParameters(message.Chat.Id);
                    await bot.SendTextMessageAsync(message.Chat.Id, "Ваші параметри були видалені.");
                    return;
                default:
                    await bot.SendTextMessageAsync(message.Chat.Id, "Такої команди не існує. Оберіть бажану функцію з переліку:\n" + VshghCoachBot.BotMessages.CommandsResponse.help);
                    return;
            }
            if (!string.IsNullOrEmpty(bodyPart))
            {
                try
                {
                    var exercises = await _client.GetExercises(bodyPart);
                    if (exercises == null || exercises.Count == 0)
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, $"Немає вправ для {bodyPart}.");
                        return;
                    }
                    var rnd = new Random();
                    var ex = exercises[rnd.Next(exercises.Count)];
                    string response = $"ID: {ex.Id}\n{ex.Name}\nОбладнання: {ex.Equipment}\nЦіль: {ex.Target}\nВторинні м'язи: {string.Join(", ", ex.SecondaryMuscles)}\nІнструкція:\n{string.Join("\n", ex.Instructions)}\n{ex.GifUrl}";
                    await bot.SendTextMessageAsync(message.Chat.Id, response);
                }
                catch (Exception ex)
                {
                    await bot.SendTextMessageAsync(message.Chat.Id, $" Виникла технічна помилка при отриманні вправ. Спробуйте пізніше.\n{ex.Message}");
                }
            }
            else if (!string.IsNullOrEmpty(botMessage))
            {
                await bot.SendTextMessageAsync(message.Chat.Id, botMessage);
            }
        }
    }
}





























