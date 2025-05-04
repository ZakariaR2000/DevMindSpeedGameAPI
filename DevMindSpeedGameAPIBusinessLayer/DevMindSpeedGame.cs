// DevMindSpeedGameAPIBusinessLayer/DevMindSpeedGame.cs
using DevMindSpeedGameDataAccessLayer;

namespace DevMindSpeedGameAPIBusinessLayer
{
    public class DevMindSpeedGame
    {
        public void SaveGameHistory(Guid gameSessionId, string question, float answer, bool correct, float timeTaken)
        {
            try
            {
                DevMindSpeedGameData.AddGameHistory(gameSessionId, question, answer, correct, timeTaken);
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
                return DevMindSpeedGameData.GetGameHistoryBySessionId(gameSessionId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetGameHistory: {ex.Message}");
                throw;
            }
        }

        public clsGameSession StartNewGame(string playerName, int difficulty)
        {
            var sessionId = Guid.NewGuid();
            DevMindSpeedGameData.AddGameSession(sessionId, playerName, difficulty);
            return new clsGameSession
            {
                GameSessionID = sessionId,
                PlayerName = playerName,
                Difficulty = difficulty,
                TimeStarted = DateTime.UtcNow,
                CurrentScore = 0,
                TotalQuestions = 0
            };
        }

        public clsGameSession GetGameSession(Guid sessionId)
        {
            var dalSession = DevMindSpeedGameData.GetGameSessionById(sessionId);
            return ConvertToBLLSession(dalSession);
        }

        private clsGameSession ConvertToBLLSession(DevMindSpeedGameDataAccessLayer.clsGameSession dalSession)
        {
            if (dalSession == null) return null;

            return new clsGameSession
            {
                GameSessionID = dalSession.GameSessionID,
                PlayerName = dalSession.PlayerName,
                Difficulty = dalSession.Difficulty,
                CurrentScore = dalSession.CurrentScore,
                TotalQuestions = dalSession.TotalQuestions,
                TotalTimeSpent = dalSession.TotalTimeSpent,
                TimeStarted = dalSession.TimeStarted
            };
        }
    }
}
