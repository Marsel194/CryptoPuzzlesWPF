using Hairulin_02_01.ViewModels.Base;
using System.Windows;

namespace Hairulin_02_01.ViewModels
{
    public class UserViewModel : ViewModelBase
    {
        // В конструкторе
        public UserViewModel()
        {
            // Тестовые данные, чтобы проверить отображение
            Username = "Тест";
            SolvedCount = 5;
            Score = 100;
            Rank = 10;

            MessageBox.Show("Я ОТКРЫЛСЯ");
        }

        // Свойства
        private string _username;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        private int _solvedCount;
        public int SolvedCount
        {
            get => _solvedCount;
            set { _solvedCount = value; OnPropertyChanged(); }
        }

        private int _score;
        public int Score
        {
            get => _score;
            set { _score = value; OnPropertyChanged(); }
        }

        private int _rank;
        public int Rank
        {
            get => _rank;
            set { _rank = value; OnPropertyChanged(); }
        }
    }
}