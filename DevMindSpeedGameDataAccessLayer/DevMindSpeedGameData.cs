using Microsoft.Data.SqlClient;

namespace DevMindSpeedGameDataAccessLayer
{
    public class GameHistory
    {
        public int GameHistoryID { get; set; }
        public int GameSessionID { get; set; } 
        public string Question { get; set; }
        public float Answer { get; set; }
        public bool Correct { get; set; }
        public float TimeTaken { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class DevMindSpeedGameData
    {
        private string _connectionString = clsDataAccessSettings.ConnectionString;

        public void AddGameHistory(int gameSessionId, string question, float answer, bool correct, float timeTaken)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO GameHistory (GameSessionId, Question, Answer, Correct, TimeTaken, CreatedAt)
                                     VALUES (@GameSessionId, @Question, @Answer, @Correct, @TimeTaken, @CreatedAt)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@GameSessionId", gameSessionId); 
                        cmd.Parameters.AddWithValue("@Question", question ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Answer", answer);
                        cmd.Parameters.AddWithValue("@Correct", correct);
                        cmd.Parameters.AddWithValue("@TimeTaken", timeTaken);
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public List<GameHistory> GetGameHistoryBySessionId(int gameSessionId)
        {
            List<GameHistory> historyList = new List<GameHistory>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "SELECT GameHistoryID, GameSessionId, Question, Answer, Correct, TimeTaken, CreatedAt FROM GameHistory WHERE GameSessionId = @GameSessionId";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@GameSessionId", gameSessionId); 
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                historyList.Add(new GameHistory
                                {
                                    GameHistoryID = reader.GetInt32(0),
                                    GameSessionID = reader.GetInt32(1), 
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
            return historyList;
        }
    }
}
