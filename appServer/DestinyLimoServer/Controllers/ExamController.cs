using DestinyLimoServer.Repositories;
using Microsoft.AspNetCore.Mvc;
using AutoMapper; // Add this line to import the IMapper interface
using DestinyLimoServer.DTOs.ResponseDTOs;
using DestinyLimoServer.Models;

namespace DestinyLimoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController(IRepositoryManager repository, IMapper mapper, ILogger logger) : ControllerBase
    {
        private readonly IRepositoryManager _repository = repository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger _logger = logger;

        private readonly int numQuestions = 15;

        [HttpGet("{onlyExamsNotStarted}")]
        public async Task<IActionResult> GetUserExamsAsync(bool onlyExamsNotStarted = false)
        {
            var userExams = await _repository.Exam.GetAllUserExams(onlyExamsNotStarted);
            System.Console.WriteLine("UserExams: " + userExams);

            var userExamsDto = _mapper.Map<IEnumerable<UserExamDTO>>(userExams);
            return Ok(userExamsDto);
        }

        [HttpGet("user/{userId}")]
        async public Task<IActionResult> GetUserExamsForUser(int userId)
        {
            var userExams = await _repository.Exam.GetUserExamsByUserId(userId);
            if (userExams == null)
            {
                _logger.LogInformation("UserExam with id: {id} doesn't exist in the database.", userId);
                return NotFound();
            }
            else
            {
                var userExamsDto = _mapper.Map<IEnumerable<UserExamDTO>>(userExams);
                return Ok(userExamsDto);
            }
        }


        [HttpGet("questions/{examId}")]
        async public Task<IActionResult> GetExam(int examId)
        {
            var userExam = await _repository.Exam.GetByIdAsync(examId);
            if (userExam == null)
            {
                _logger.LogInformation("UserExam with id: {id} doesn't exist in the database.", examId);
                return NotFound();
            }
            else
            {
                var userExamDto = _mapper.Map<UserExamDTO>(userExam);

                // create the questions for the exam
                var mcqs = await _repository.Material<MaterialMCQ>().GetRandomQuestions(numQuestions);
                double min_correct_answers_for_pass = Math.Round((double)(numQuestions / 2), 0);

                // return the exam
                userExamDto.DateStarted = DateTime.Now;
                userExamDto.min_correct_answers_for_pass = Convert.ToInt32(min_correct_answers_for_pass);
                userExamDto.ExamQuestions = _mapper.Map<IEnumerable<MaterialMCQDTO>>(mcqs);

                return Ok(userExamDto);
            }
        }

        [HttpGet("answers/{examId}")]
        async public Task<IActionResult> GetUserExamQuestionsAnswers(int examId)
        {
            var userExamAnswers = await _repository.Exam.GetUserExamAnswersByExamId(examId);
            if (userExamAnswers == null)
            {
                _logger.LogInformation("Exam with id: {id} doesn't exist in the database.", examId);
                return NotFound();
            }
            else
            {
                var userExamAnswersDto = _mapper.Map<IEnumerable<UserExamAnswerDTO>>(userExamAnswers);
                return Ok(userExamAnswersDto);
            }
        }

        [HttpPost("admin/{userId}")]
        async public Task<IActionResult> AddNewExamByAdminAsync(int userId)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state");
                return UnprocessableEntity(ModelState);
            }

            // create a record of the new exam
            int newExamId = await _repository.Exam.AddNewExamAsync(userId);

            return Ok(newExamId);
        }

        [HttpPost("{userId}")]
        async public Task<IActionResult> AddNewExamAsync(int userId)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state");
                return UnprocessableEntity(ModelState);
            }

            // create a record of the new exam
            int newExamId = await _repository.Exam.AddNewExamAsync(userId);

            // create the questions for the exam
            var mcqs = await _repository.Material<MaterialMCQ>().GetRandomQuestions(numQuestions);

            double min_correct_answers_for_pass = Math.Round((double)(numQuestions / 2), 0);

            // return the exam
            var userExam = new UserExamDTO
            {
                ExamId = newExamId,
                UserId = userId,
                DateStarted = DateTime.Now,
                min_correct_answers_for_pass = Convert.ToInt32(min_correct_answers_for_pass),
                ExamQuestions = _mapper.Map<IEnumerable<MaterialMCQDTO>>(mcqs),
            };

            return Ok(userExam);
        }

        [HttpPut()]
        async public Task<IActionResult> UpdateUserExamAsyn(UserExamDTO newExam)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the UserExamForCreationDTO object");
                return UnprocessableEntity(ModelState);
            }

            var userExams = _mapper.Map<IEnumerable<UserExamAnswer>>(newExam.UserExamAnswers);
            var examAnswerIds = await _repository.Exam.SumbitUserExamAnswersAsync(userExams);

            // update score
            int score = newExam.UserExamAnswers!.Where(x => x.is_correct == true).Count();
            newExam.Score = score;

            // update result
            newExam.num_correct = score;
            newExam.num_attempted = newExam.UserExamAnswers!.Where(x => x.attempted == true).Count();
            newExam.num_wrong = newExam.num_questions - newExam.num_correct;

            newExam.Result = score >= newExam.min_correct_answers_for_pass ? 1 : 0;

            // update date completed
            newExam.DateCompleted = DateTime.Now;

            var userExam = _mapper.Map<UserExam>(newExam);
            await _repository.Exam.UpdateAsync(userExam, userExam.exam_id);

            return Ok(true);
        }
    }
}


