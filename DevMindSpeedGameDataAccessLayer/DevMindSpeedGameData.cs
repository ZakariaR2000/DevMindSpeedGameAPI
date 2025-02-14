using Microsoft.Data.SqlClient;

namespace DevMindSpeedGameDataAccessLayer
{
    
    public class DevMindSpeedGameData
    {
        private string _connectionString = clsDataAccessSettings.ConnectionString;


        public Guid AddGameSession(string playerName, int difficulty)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO GameSessions (GameSessionID, PlayerName, Difficulty, CurrentScore, TotalQuestions, TotalTimeSpent, TimeStarted)
                         VALUES (@GameSessionID, @PlayerName, @Difficulty, 0, 0, 0, @TimeStarted)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    Guid newSessionId = Guid.NewGuid(); 
                    cmd.Parameters.AddWithValue("@GameSessionID", newSessionId);
                    cmd.Parameters.AddWithValue("@PlayerName", playerName);
                    cmd.Parameters.AddWithValue("@Difficulty", difficulty);
                    cmd.Parameters.AddWithValue("@TimeStarted", DateTime.UtcNow);
                    cmd.ExecuteNonQuery();

                    return newSessionId; 
                }
            }
        }

        public void AddGameHistory(Guid gameSessionId, string question, float answer, bool correct, float timeTaken)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO GameHistory (GameSessionID, Question, Answer, Correct, TimeTaken, CreatedAt)
                         VALUES (@GameSessionID, @Question, @Answer, @Correct, @TimeTaken, @CreatedAt)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@GameSessionID", gameSessionId);
                    cmd.Parameters.AddWithValue("@Question", question ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Answer", answer);
                    cmd.Parameters.AddWithValue("@Correct", correct);
                    cmd.Parameters.AddWithValue("@TimeTaken", timeTaken);
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<clsGameHistory> GetGameHistoryBySessionId(Guid gameSessionId)
        {
            List<clsGameHistory> historyList = new List<clsGameHistory>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT GameHistoryID, GameSessionID, Question, Answer, Correct, TimeTaken, CreatedAt FROM GameHistory WHERE GameSessionID = @GameSessionID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@GameSessionID", gameSessionId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            historyList.Add(new clsGameHistory
                            {
                                GameHistoryID = reader.GetInt32(0),
                                GameSessionID = reader.GetGuid(1),
                                Question = reader.GetString(2),
                                Answer = (float)reader.GetDouble(3),
                                Correct = reader.GetBoolean(4),
                                TimeTaken = (float)reader.GetDouble(5),
                                CreatedAt = reader.GetDateTime(6)
                            });
                        }
                    }
                }
            }
            return historyList;
        }
    }
}
