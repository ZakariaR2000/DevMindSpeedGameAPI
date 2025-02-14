using DevMindSpeedGameDataAccessLayer;
using System;

namespace DevMindSpeedGameAPIBusinessLayer
{
    public class GameSession
    {
        public Guid GameSessionID { get; set; } 
        public string PlayerName { get; set; }
        public int Difficulty { get; set; }
        public int CurrentScore { get; set; }
        public int TotalQuestions { get; set; }
        public double TotalTimeSpent { get; set; }
        public string CurrentQuestion { get; set; }
        public double CurrentAnswer { get; set; }
        public DateTime TimeStarted { get; set; }
    }


    public class DevMindSpeedGame
    {
        private DevMindSpeedGameData _historyDataAccess = new DevMindSpeedGameData();

        public void SaveGameHistory(Guid gameSessionId, string question, float answer, bool correct, float timeTaken)
        {
            try
            {
                _historyDataAccess.AddGameHistory(gameSessionId, question, answer, correct, timeTaken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SaveGameHistory: {ex.Message}");
                throw;
            }
        }

        public List<GameHistory> GetGameHistory(Guid gameSessionId)
        {
            try
            {
                return _historyDataAccess.GetGameHistoryBySessionId(gameSessionId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetGameHistory: {ex.Message}");
                throw;
            }
        }
       


        public (string Question, float Answer) GenerateMathQuestion(int difficulty)
        {
            var random = new Random();
            var operations = new[] { "+", "-", "*", "/" };

            int operandCount = difficulty + 1;
            double maxDigit = Math.Pow(10, difficulty);

            var numbers = Enumerable.Range(0, operandCount)
                                    .Select(_ => random.Next(1, (int)maxDigit))
                                    .Select(num => (double)num)
                                    .ToArray();
            var operation = operations[random.Next(operations.Length)];

            string question = string.Join($" {operation} ", numbers);
            var result = new System.Data.DataTable().Compute(question, null);
            float answer = Convert.ToSingle(result);

            return (question, answer);
        }

        public GameSession StartNewGame(string playerName, int difficulty)
        {
            return new GameSession
            {
                GameSessionID = Guid.NewGuid(), 
                PlayerName = playerName,
                Difficulty = difficulty,
                TimeStarted = DateTime.UtcNow,
                CurrentScore = 0,
                TotalQuestions = 0
            };
        }



    }
}
