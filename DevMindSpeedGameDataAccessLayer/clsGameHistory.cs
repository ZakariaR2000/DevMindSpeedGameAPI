using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevMindSpeedGameDataAccessLayer
{
    public class clsGameHistory
    {
        public int GameHistoryID { get; set; }
        public Guid GameSessionID { get; set; }
        public string Question { get; set; }
        public float Answer { get; set; }
        public bool Correct { get; set; }
        public float TimeTaken { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
