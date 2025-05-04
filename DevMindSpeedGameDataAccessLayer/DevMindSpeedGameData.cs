using Microsoft.Data.SqlClient;

namespace DevMindSpeedGameDataAccessLayer
{
    
    static public class DevMindSpeedGameData
    {


        public static void AddGameSession(Guid sessionId, string playerName, int difficulty)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                conn.Open();
                string query = @"INSERT INTO GameSessions (GameSessionID, PlayerName, Difficulty, CurrentScore, TotalQuestions, TotalTimeSpent, TimeStarted)
                             VALUES (@GameSessionID, @PlayerName, @Difficulty, 0, 0, 0, @TimeStarted)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@GameSessionID", sessionId);
                    cmd.Parameters.AddWithValue("@PlayerName", playerName);
                    cmd.Parameters.AddWithValue("@Difficulty", difficulty);
                    cmd.Parameters.AddWithValue("@TimeStarted", DateTime.UtcNow);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        static public void AddGameHistory(Guid gameSessionId, string question, float answer, bool correct, float timeTaken)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
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

        static public List<clsGameHistory> GetGameHistoryBySessionId(Guid gameSessionId)
        {
            List<clsGameHistory> historyList = new List<clsGameHistory>();
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
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

        public static clsGameSession GetGameSessionById(Guid gameSessionId)
        {
            clsGameSession session = null;
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                conn.Open();
                string query = @"SELECT GameSessionID, PlayerName, Difficulty, CurrentScore, TotalQuestions, TotalTimeSpent, TimeStarted
                         FROM GameSessions
                         WHERE GameSessionID = @GameSessionID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@GameSessionID", gameSessionId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            session = new clsGameSession
                            {
                                GameSessionID = reader.GetGuid(0),
                                PlayerName = reader.GetString(1),
                                Difficulty = reader.GetInt32(2),
                                CurrentScore = reader.GetInt32(3),
                                TotalQuestions = reader.GetInt32(4),
                                TotalTimeSpent = (float)reader.GetDouble(5),
                                TimeStarted = reader.GetDateTime(6)
                            };
                        }
                    }
                }
            }
            return session;
        }

    }
}
 