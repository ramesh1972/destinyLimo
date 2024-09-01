using DestinyLimoServer.Repositories;
using Microsoft.AspNetCore.Mvc;
using AutoMapper; // Add this line to import the IMapper interface
using DestinyLimoServer.DTOs.ResponseDTOs;
using DestinyLimoServer.Models;

namespace DestinyLimoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentController(IRepositoryManager repository, IMapper mapper, ILogger logger) : ControllerBase
    {
        private readonly IRepositoryManager _repository = repository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger _logger = logger;

        [HttpGet("{includeInActive?}/{includeDeleted?}")]
        public async Task<IActionResult> GetContentAsync(bool? includeInActive = true, bool? includeDeleted = false)
        {
            var Contents = await _repository.Content.GetAllContents(includeInActive ?? true, includeDeleted ?? false);
            System.Console.WriteLine("Contents: " + Contents);

            var ContentDto = _mapper.Map<IEnumerable<ContentDTO>>(Contents);
            return Ok(ContentDto);
        }

        [HttpGet("{id}")]
        async public Task<IActionResult> GetContent(int id)
        {
            var Content = await _repository.Content.GetContentById(id);
            if (Content == null)
            {
                _logger.LogInformation("Content with id: {id} doesn't exist in the database.", id);
                return NotFound();
            }
            else
            {
                var ContentDto = _mapper.Map<ContentDTO>(Content);
                return Ok(ContentDto);
            }
        }

        [HttpPost]
        async public Task<IActionResult> CreateContentAsync([FromBody] ContentDTO contentDTO)
        {
            if (contentDTO == null)
            {
                _logger.LogError("ContentForCreationDto object sent from client is null.");
                return BadRequest("ContentForCreationDto object is null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the ContentForCreationDTO object");
                return UnprocessableEntity(ModelState);
            }

            var content = _mapper.Map<Content>(contentDTO);
            await _repository.Content.AddAsync(content);

            return Ok(content.content_id);

        }

        [HttpDelete("{id}")]
        async public Task<IActionResult> DeleteContentAsync(int id)
        {
            var content = await _repository.Content.GetContentById(id);
            if (content == null)
            {
                _logger.LogInformation("Content with id: {id} doesn't exist in the database.", id);
                return NotFound();
            }

            await _repository.Content.DeleteAsync(id);

            return Ok(true);
        }

        [HttpPost("{id}")]
        async public Task<IActionResult> UpdateContentAsync(int id, [FromBody] ContentDTO contentDTO)
        {
            if (contentDTO == null)
            {
                _logger.LogError("ContentForUpdateDto object sent from client is null.");
                return BadRequest("ContentForUpdateDto object is null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the ContentForUpdateDto object");
                return UnprocessableEntity(ModelState);
            }

            Content content = _mapper.Map<Content>(contentDTO);

            await _repository.Content.UpdateAsync(content, id);

            return Ok(true);
        }
    }
}


