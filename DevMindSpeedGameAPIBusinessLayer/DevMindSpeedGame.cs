using DevMindSpeedGameDataAccessLayer;

namespace DevMindSpeedGameAPIBusinessLayer
{
    
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

        public List<clsGameHistory> GetGameHistory(Guid gameSessionId)
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
       

        public clsGameSession StartNewGame(string playerName, int difficulty)
        {
            return new clsGameSession
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
