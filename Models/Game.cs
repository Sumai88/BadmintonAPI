using System;

namespace BadmintonSvc.Models
{
    public class Game
    {
        public int GameID { get; set; }
        public int player1 { get; set; }
        public int player2 { get; set; }
        public int player3 { get; set; }
        public int player4 { get; set; }
        public int ScoreA { get; set; }
        public int ScoreB { get; set; }
        public DateTime StartTime { get; set; }
        private DateTime _endTime = DateTime.Now;
        public DateTime EndTime
        {
            get { return _endTime; }
            set { _endTime = value; }
        }
    }
}