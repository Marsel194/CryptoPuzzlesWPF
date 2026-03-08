using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.Shared;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace CryptoPuzzles
{
    public static class DebugTests
    {
        public static async Task RunAllTests()
        {
            Debug.WriteLine("========== НАЧАЛО ТЕСТОВ ==========");

            // Получаем сервисы из App.Services
            var puzzleApi = App.Services.GetService<PuzzleApiService>();
            var tutorialApi = App.Services.GetService<TutorialApiService>();
            var difficultyApi = App.Services.GetService<DifficultyApiService>();

            if (puzzleApi == null || tutorialApi == null || difficultyApi == null)
            {
                Debug.WriteLine("❌ ОШИБКА: Не удалось получить сервисы API");
                return;
            }

            // ТЕСТ 1: Загрузка всех пазлов
            try
            {
                Debug.WriteLine("\n--- ТЕСТ 1: PuzzleApi.GetAllAsync() ---");
                var allPuzzles = await puzzleApi.GetAllAsync();
                Debug.WriteLine($"Всего пазлов: {allPuzzles.Count}");

                foreach (var p in allPuzzles)
                {
                    Debug.WriteLine($"ID: {p.Id}, Title: {p.Title}, Content: {p.Content}, IsTraining: {p.IsTraining}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Ошибка: {ex.Message}");
            }

            // ТЕСТ 2: Загрузка всех туториалов
            try
            {
                Debug.WriteLine("\n--- ТЕСТ 2: TutorialApi.GetAllAsync() ---");
                var allTutorials = await tutorialApi.GetAllAsync();
                Debug.WriteLine($"Всего туториалов: {allTutorials.Count}");

                foreach (var t in allTutorials)
                {
                    Debug.WriteLine($"ID: {t.Id}, Title: {t.TheoryTitle}, Content: {t.TheoryContent}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Ошибка: {ex.Message}");
            }

            // ТЕСТ 3: Загрузка сложностей
            try
            {
                Debug.WriteLine("\n--- ТЕСТ 3: DifficultyApi.GetAllAsync() ---");
                var difficulties = await difficultyApi.GetAllAsync();
                Debug.WriteLine($"Всего сложностей: {difficulties.Count}");

                foreach (var d in difficulties)
                {
                    Debug.WriteLine($"ID: {d.Id}, Name: {d.DifficultyName}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Ошибка: {ex.Message}");
            }

            // ТЕСТ 4: Фильтрация обучающих пазлов
            try
            {
                Debug.WriteLine("\n--- ТЕСТ 4: Фильтрация IsTraining=true ---");
                var allPuzzles = await puzzleApi.GetAllAsync();
                var trainingPuzzles = allPuzzles.Where(p => p.IsTraining).ToList();
                Debug.WriteLine($"Обучающих пазлов: {trainingPuzzles.Count}");

                foreach (var p in trainingPuzzles)
                {
                    Debug.WriteLine($"ID: {p.Id}, Title: {p.Title}, Order: {p.TutorialOrder}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Ошибка: {ex.Message}");
            }

            // ТЕСТ 5: Фильтрация пазлов по сложности (для практики)
            try
            {
                Debug.WriteLine("\n--- ТЕСТ 5: Фильтрация по DifficultyId=1 (Легкая) ---");
                var allPuzzles = await puzzleApi.GetAllAsync();
                var easyPuzzles = allPuzzles.Where(p => p.DifficultyId == 1 && !p.IsTraining).ToList();
                Debug.WriteLine($"Пазлов для легкой сложности: {easyPuzzles.Count}");

                foreach (var p in easyPuzzles)
                {
                    Debug.WriteLine($"ID: {p.Id}, Title: {p.Title}, Content: {p.Content}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Ошибка: {ex.Message}");
            }

            Debug.WriteLine("\n========== КОНЕЦ ТЕСТОВ ==========");
        }
    }
}