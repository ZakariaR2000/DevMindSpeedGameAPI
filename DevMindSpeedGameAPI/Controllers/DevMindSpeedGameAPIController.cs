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
        public IActionResult StartGame([FromBody] StartGameRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.PlayerName) || request.Difficulty < 1 || request.Difficulty > 4)
            {
                return BadRequest("Invalid request. PlayerName must not be empty, and Difficulty must be between 1 and 4.");
            }

            // إنشاء جلسة جديدة
            var gameSession = _gameLogic.StartNewGame(request.PlayerName, request.Difficulty);

            // توليد معادلة رياضية عشوائية
            var (question, answer) = _gameLogic.GenerateMathQuestion(request.Difficulty);

            gameSession.CurrentQuestion = question;
            gameSession.CurrentAnswer = answer;

            return Ok(new
            {
                Message = $"Hello {request.PlayerName}, find your submit API URL below.",
                SubmitUrl = $"/api/game/{gameSession.GameSessionID}/submit",
                Question = question,
                TimeStarted = gameSession.TimeStarted
            });
        }


        [HttpPost("{game_id}/submit")]
        public IActionResult SubmitAnswer(int game_id, [FromBody] SubmitAnswerRequest request)
        {
            if (request == null || float.IsNaN(request.Answer))
            {
                return BadRequest("Invalid answer. Please provide a valid numeric answer.");
            }

            var gameSession = new GameSession();
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

            var (newQuestion, newAnswer) = _gameLogic.GenerateMathQuestion(gameSession.Difficulty);
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
        public IActionResult GetGameStatus(int game_id)
        {
            var gameSession = new GameSession(); 
            if (gameSession == null)
            {
                return NotFound($"Game session with ID {game_id} not found.");
            }

            var history = _gameLogic.GetGameHistory(game_id);

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

    public class StartGameRequest
    {
        public string PlayerName { get; set; }
        public int Difficulty { get; set; }
    }
}


