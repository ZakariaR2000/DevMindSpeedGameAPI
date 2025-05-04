using DevMindSpeedGameAPI.General;
using DevMindSpeedGameAPI.Models;
using DevMindSpeedGameAPIBusinessLayer;
using Microsoft.AspNetCore.Mvc;

namespace DevMindSpeedGameAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevMindSpeedGameAPIController : ControllerBase
    {
        private readonly DevMindSpeedGame _gameLogic;

        private static Dictionary<Guid, (string Question, float Answer)> _sessionQuestions = new();
        private static Dictionary<Guid, clsGameSession> _sessionMemory = new();


        public DevMindSpeedGameAPIController()
        {
            _gameLogic = new DevMindSpeedGame();
        }

        [HttpPost("start")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult StartGame(string playerName, int difficulty)
        {
            if (string.IsNullOrWhiteSpace(playerName))
                return BadRequest("Player name is required.");

            if (difficulty < 1 || difficulty > 4)
                return BadRequest("Difficulty must be between 1 and 4.");

            var session = _gameLogic.StartNewGame(playerName, difficulty);
            var (question, answer) = clsMathOperation.GenerateMathQuestion(difficulty);

            _sessionQuestions[session.GameSessionID] = (question, answer);
            _sessionMemory[session.GameSessionID] = session; 

            return Ok(new
            {
                message = $"Hello {playerName}, find your submit API URL below.",
                submitUrl = $"/api/DevMindSpeedGameAPI/{session.GameSessionID}/submit",
                question = question,
                timeStarted = session.TimeStarted
            });
        }

        [HttpPost("{game_id}/submit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult SubmitAnswer(Guid game_id, [FromBody] SubmitAnswerRequest request)
        {
            if (request == null || float.IsNaN(request.Answer))
                return BadRequest("Invalid answer. Please provide a valid numeric answer.");

            if (!_sessionMemory.TryGetValue(game_id, out var gameSession))
                return NotFound($"Game session with ID {game_id} not found.");

            if (!_sessionQuestions.TryGetValue(game_id, out var questionData))
                return NotFound("No question found for this session.");

            var (currentQuestion, correctAnswer) = questionData;
            var timeTaken = (DateTime.UtcNow - gameSession.TimeStarted).TotalSeconds;
            var isCorrect = Math.Abs(correctAnswer - request.Answer) < 0.01;

            gameSession.TotalQuestions++;
            if (isCorrect)
                gameSession.CurrentScore++;

            _gameLogic.SaveGameHistory(game_id, currentQuestion, request.Answer, isCorrect, (float)timeTaken);

            var (newQuestion, newAnswer) = clsMathOperation.GenerateMathQuestion(gameSession.Difficulty);
            _sessionQuestions[game_id] = (newQuestion, newAnswer);
            _sessionMemory[game_id] = gameSession; // 

            return Ok(new
            {
                result = isCorrect ? $"Good job {gameSession.PlayerName}, your answer is correct!" :
                                     $"Sorry {gameSession.PlayerName}, your answer is incorrect.",
                timeTaken = Math.Round(timeTaken, 2),
                currentScore = $"{gameSession.CurrentScore} / {gameSession.TotalQuestions}",
                nextQuestion = newQuestion
            });
        }

        [HttpGet("{game_id}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetGameStatus(Guid game_id)
        {
            if (!_sessionMemory.TryGetValue(game_id, out var gameSession))
                return NotFound($"Game session with ID {game_id} not found.");

            var history = _gameLogic.GetGameHistory(game_id);

            return Ok(new
            {
                name = gameSession.PlayerName,
                difficulty = gameSession.Difficulty,
                currentScore = $"{gameSession.CurrentScore} / {gameSession.TotalQuestions}",
                totalTimeSpent = gameSession.TotalTimeSpent,
                history = history.Select(h => new
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
