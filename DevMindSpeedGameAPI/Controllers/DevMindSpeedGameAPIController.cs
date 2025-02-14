using DevMindSpeedGameAPI.General;
using DevMindSpeedGameAPI.Models;
using DevMindSpeedGameAPIBusinessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DevMindSpeedGameAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevMindSpeedGameAPIController : ControllerBase
    {
        private readonly DevMindSpeedGame _gameLogic;

        public DevMindSpeedGameAPIController()
        {
            _gameLogic = new DevMindSpeedGame();
        }

        [HttpPost("start")]
        public IActionResult StartGame(string playerName, int difficulty)
        {
            // استدعاء BLL لإنشاء جلسة جديدة
            var sessionId = _gameLogic.StartNewGame(playerName, difficulty).GameSessionID; // استخدم GameSessionID مباشرةً

            // توليد السؤال والإجابة
            var (question, answer) = clsGenerateMath.GenerateMathQuestion(difficulty);

            return Ok(new
            {
                message = $"Hello {playerName}, find your submit API URL below.",
                submitUrl = $"/api/game/{sessionId}/submit", // استخدم sessionId هنا
                question = question,
                timeStarted = DateTime.UtcNow
            });
        }

        [HttpPost("{game_id}/submit")]
        public IActionResult SubmitAnswer(Guid game_id, [FromBody] SubmitAnswerRequest request)
        {
            if (request == null || float.IsNaN(request.Answer))
            {
                return BadRequest("Invalid answer. Please provide a valid numeric answer.");
            }

            var gameSession = new clsGameSession();
            if (gameSession == null)
            {
                return NotFound($"Game session with ID {game_id} not found.");
            }

            var timeTaken = (DateTime.UtcNow - gameSession.TimeStarted).TotalSeconds;

            var isCorrect = Math.Abs(gameSession.CurrentAnswer - request.Answer) < 0.01;

            gameSession.TotalQuestions++;
            if (isCorrect)
            {
                gameSession.CurrentScore++;
            }

            _gameLogic.SaveGameHistory(game_id, gameSession.CurrentQuestion, request.Answer, isCorrect, (float)timeTaken);

            var (newQuestion, newAnswer) = clsGenerateMath.GenerateMathQuestion(gameSession.Difficulty);
            gameSession.CurrentQuestion = newQuestion;
            gameSession.CurrentAnswer = newAnswer;

            return Ok(new
            {
                Result = isCorrect ? "Good job! Your answer is correct." : "Sorry, your answer is incorrect.",
                TimeTaken = Math.Round(timeTaken, 2),
                CurrentScore = $"{gameSession.CurrentScore} / {gameSession.TotalQuestions}",
                NextQuestion = newQuestion
            });
        }

        [HttpGet("{game_id}/status")]
        public IActionResult GetGameStatus(Guid game_id)
        {
            // جلب الجلسة (استخدام DataAccessLayer)
            var gameSession = new clsGameSession(); // استبدل هذا بجلب البيانات من قاعدة البيانات
            if (gameSession == null)
            {
                return NotFound($"Game session with ID {game_id} not found.");
            }

            // جلب تاريخ الإجابات
            var history = _gameLogic.GetGameHistory(game_id);

            // استجابة
            return Ok(new
            {
                Name = gameSession.PlayerName,
                Difficulty = gameSession.Difficulty,
                CurrentScore = $"{gameSession.CurrentScore} / {gameSession.TotalQuestions}",
                TotalTimeSpent = gameSession.TotalTimeSpent,
                History = history.Select(h => new
                {
                    h.Question,
                    h.Answer,
                    h.Correct,
                    h.TimeTaken,
                    h.CreatedAt
                })
            });
        }


    }

    
}

