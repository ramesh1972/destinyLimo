using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using Newtonsoft.Json;

using DestinyLimoServer.Repositories;
using DestinyLimoServer.DTOs.RequestDTOs;
using DestinyLimoServer.Models;

namespace DestinyLimoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BulkUpdateController(IBulkUpdateRepository repository, IMapper mapper, ILogger logger) : ControllerBase
    {
        private readonly IBulkUpdateRepository _repository = repository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger _logger = logger;

        [HttpPost("{tableName}")]
        [RequestSizeLimit(100_000_000)] 
        async public Task<IActionResult> BulkUpdate(string tableName, [FromForm] BulkUpdateDTO bulkUpdateDTO)
        {
            // Deserialize actions
            var actions = JsonConvert.DeserializeObject<List<EntityActionDTO>>(bulkUpdateDTO.Actions);

            if (actions == null || actions.Count == 0)
            {
                _logger.LogError("EntityActiondDTO object sent from client is null or empty.");
                return BadRequest("EntityActiondDTOs object is null or empty");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the EntityActionRecordDTO object");
                return UnprocessableEntity(ModelState);
            }

            // Process files
            List<IFormFile> files = HttpContext.Request.Form.Files.ToList();
            Directory.CreateDirectory("uploads");

            foreach (var file in files)
            {
                if (file!.Length > 0)
                {
                    var filePath = Path.Combine("uploads", file.FileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);
                }
            }

            // update the table
            var entityActionsModelObjects = _mapper.Map<List<EntityAction>>(actions);
            var isSuccess = _repository.BulkUpdateEntities(tableName, entityActionsModelObjects);
            return Ok(isSuccess);
        }
    }
}
