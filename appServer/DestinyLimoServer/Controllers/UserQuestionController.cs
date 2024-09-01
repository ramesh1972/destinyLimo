using DestinyLimoServer.Repositories;
using Microsoft.AspNetCore.Mvc;
using AutoMapper; // Add this line to import the IMapper interface
using DestinyLimoServer.DTOs.ResponseDTOs;
using DestinyLimoServer.Models;

namespace DestinyLimoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserQuestionController(IRepositoryManager repository, IMapper mapper, ILogger logger) : ControllerBase
    {
        private readonly IRepositoryManager _repository = repository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger _logger = logger;

        [HttpGet("{includeOnlyAnswered?}/{includeInActive?}/{includeDeleted?}")]
        public async Task<IActionResult> GetUserQuestionsAsync(bool? includeOnlyAnswered = false, bool? includeInActive = true, bool? includeDeleted = false)
        {   
            var userQuestions = await _repository.UserQuestion.GetAllAsync(includeOnlyAnswered ?? false, includeInActive ?? true, includeDeleted ?? false);
            System.Console.WriteLine("UserQuestions: " + userQuestions);

            var userQuestionDto = _mapper.Map<IEnumerable<UserAskedQuestionDTO>>(userQuestions);
            return Ok(userQuestionDto);
        }

        [HttpGet("user/{userId}")]
        async public Task<IActionResult> GetUserQuestions(int userId)
        {
            var userQuestions = await _repository.UserQuestion.GetUserQuestions(userId);
            if (userQuestions == null)
            {
                _logger.LogInformation("UserQuestions with userId: {userId} doesn't exist in the database.", userId);
                return NotFound();
            }
            else
            {
                var userQuestionDto = _mapper.Map<IEnumerable<UserAskedQuestionDTO>>(userQuestions);
                return Ok(userQuestionDto);
            }
        }

        [HttpGet("question/{userQuestionId}")]
        async public Task<IActionResult> GetUserQuestionById(int userQuestionId)
        {
            var userQuestion = await _repository.UserQuestion.GetUserQuestionById(userQuestionId);
            if (userQuestion == null)
            {
                _logger.LogInformation("UserQuestion with id: {userQuestionId} doesn't exist in the database.", userQuestionId);
                return NotFound();
            }
            else
            {
                var userQuestionDto = _mapper.Map<UserAskedQuestionDTO>(userQuestion);
                return Ok(userQuestionDto);
            }
        }

        [HttpPost("answer/{userQuestionId}")]
        async public Task<IActionResult> AnswerQuestionAsync(int userQuestionId, [FromQuery] int admin_user_id, string answer)
        {
            if (String.IsNullOrEmpty(answer))
            {
                _logger.LogError("Answer string sent from client is null.");
                return BadRequest("Answer string is null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the UserQuestionId object");
                return UnprocessableEntity(ModelState);
            }

            var userAskedQuestion = _mapper.Map<UserAskedQuestion>(userQuestionId);
            bool success = await _repository.UserQuestion.AnswerQuestion(userAskedQuestion.user_question_id, userAskedQuestion.admin_answer!, userAskedQuestion.admin_user_id);

            if (!success)
            {
                _logger.LogError("Failed to answer the question with id: {userQuestionId}", userQuestionId);
                return StatusCode(500, "Failed to answer the question");
            }

            return Ok("Question answered successfully");
        }

        [HttpGet("public")]
        async public Task<IActionResult> GetPublicUserQuestions()
        {
            var userQuestions = await _repository.UserQuestion.GetPublicUserQuestions();
            if (userQuestions == null)
            {
                _logger.LogInformation("UserQuestions that are public doesn't exist in the database.");
                return NotFound();
            }
            else
            {
                var userQuestionDto = _mapper.Map<UserAskedQuestionDTO>(userQuestions);
                return Ok(userQuestionDto);
            }
        }

        [HttpPost("public/{userQuestionId}")]
        async public Task<IActionResult> MakePublic(int userQuestionId, bool makePublic = true)
        {
            bool success = makePublic ? await _repository.UserQuestion.MakePublic(userQuestionId) : await _repository.UserQuestion.MakePrivate(userQuestionId);

            if (!success)
            {
                _logger.LogError("Failed to make the question with id: {userQuestionId} public", userQuestionId);
                return StatusCode(500, "Failed to make the question public");
            }

            return Ok("Question made public successfully");
        }

        [HttpPost]
        async public Task<IActionResult> AddUserQuestion([FromBody] UserAskedQuestionDTO userQuestionDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the UserQuestionDTO object");
                return UnprocessableEntity(ModelState);
            }

            System.Console.WriteLine("UserQuestionDTO: " + userQuestionDto);

            var userQuestion = _mapper.Map<UserAskedQuestion>(userQuestionDto);
            int userQuestionId = await _repository.UserQuestion.AddAsync(userQuestion);

            if (userQuestionId == 0)
            {
                _logger.LogError("Failed to add the user question to the database");
                return StatusCode(500, "Failed to add the user question");
            }

            return Ok(true);
        }

        [HttpPut("{userQuestionId}")]
        async public Task<IActionResult> UpdateUserQuestion(int userQuestionId, [FromBody] UserAskedQuestionDTO userQuestionDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the UserQuestionDTO object");
                return UnprocessableEntity(ModelState);
            }

            var userQuestion = _mapper.Map<UserAskedQuestion>(userQuestionDto);
            bool success = await _repository.UserQuestion.UpdateAsync(userQuestion, userQuestionId);

            if (!success)
            {
                _logger.LogError("Failed to update the user question with id: {userQuestionId}", userQuestionId);
                return StatusCode(500, "Failed to update the user question");
            }

            return Ok("User question updated successfully");
        }

        [HttpDelete("{userQuestionId}")]
        async public Task<IActionResult> DeleteUserQuestion(int userQuestionId)
        {
            bool success = await _repository.UserQuestion.DeleteAsync(userQuestionId);

            if (!success)
            {
                _logger.LogError("Failed to delete the user question with id: {userQuestionId}", userQuestionId);
                return StatusCode(500, "Failed to delete the user question");
            }

            return Ok(true);
        }
    }
}
