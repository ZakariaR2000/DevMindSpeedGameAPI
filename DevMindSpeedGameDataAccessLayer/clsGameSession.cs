using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevMindSpeedGameDataAccessLayer
{
    public class clsGameSession
    {
        public Guid GameSessionID { get; set; }
        public string PlayerName { get; set; }
        public int Difficulty { get; set; }
        public int CurrentScore { get; set; }
        public int TotalQuestions { get; set; }
        public double TotalTimeSpent { get; set; }
       
        public DateTime TimeStarted { get; set; }
    }
}
