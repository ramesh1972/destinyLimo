using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DestinyLimoServer.Repositories;
using Microsoft.AspNetCore.Mvc;
using AutoMapper; // Add this line to import the IMapper interface
using DestinyLimoServer.DTOs.RequestDTOs;
using DestinyLimoServer.DTOs.ResponseDTOs;
using DestinyLimoServer.Models;
using Org.BouncyCastle.Crypto.Prng;

namespace DestinyLimoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IRepositoryManager repository, IMapper mapper, ILogger logger) : ControllerBase
    {
        private readonly IRepositoryManager _repository = repository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger _logger = logger;

        [HttpGet("{includeInActive?}/{includeDeleted?}")]
        public async Task<IActionResult> GetUsersAsync(bool? includeInActive = true, bool? includeDeleted = false)
        {
            var users = await _repository.User.GetUsers(includeInActive ?? true, includeDeleted ?? false);
            System.Console.WriteLine("Users: " + users);

            var userDto = _mapper.Map<IEnumerable<UserDTO>>(users);
            return Ok(userDto);
        }

        [HttpGet("profile/{includeInActive?}/{includeDeleted?}")]
        public async Task<IActionResult> GetUsersProfilesAsync(bool? includeInActive = true, bool? includeDeleted = false)
        {
            var users = await _repository.UserProfile.GetUsers(includeInActive ?? true, includeDeleted ?? false);
            System.Console.WriteLine("Users: " + users);

            var userDto = _mapper.Map<IEnumerable<UserProfileDTO>>(users);
            return Ok(userDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserAsync(int id)
        {
            var user = await _repository.User.GetUserById(id);
            if (user == null)
            {
                _logger.LogInformation("User with id: {id} doesn't exist in the database.", id);
                return NotFound();
            }
            else
            {
                var userDto = _mapper.Map<UserDTO>(user);
                return Ok(userDto);
            }
        }


        [HttpGet("profile/{id}")]
        public async Task<IActionResult> GetUserProfileAsync(int id)
        {
            var user = await _repository.User.GetUserById(id);
            if (user == null)
            {
                _logger.LogInformation("User with id: {id} doesn't exist in the database.", id);
                return NotFound();
            }
            else
            {
                var userDto = _mapper.Map<UserProfileDTO>(user);
                return Ok(userDto);
            }
        }

        [HttpPost]
        async public Task<IActionResult> CreateUserAsync([FromBody] UserDTO user)
        {
            if (user == null)
            {
                _logger.LogError("UserForCreationDto object sent from client is null.");
                return BadRequest("UserForCreationDto object is null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the UserForCreationDTO object");
                return UnprocessableEntity(ModelState);
            }

            var userEntity = _mapper.Map<User>(user);
            int newUserId = await _repository.User.AddAsync(userEntity);

            var userProfile = user.UserProfile;
            userProfile!.UserId = newUserId;

            var userProfileEntity = _mapper.Map<UserProfile>(userProfile);
            int newProfileId = await _repository.UserProfile.AddAsync(userProfileEntity);

            return CreatedAtRoute("UserById", new { id = userEntity.UserId }, userEntity);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            var user = _repository.User.GetUserById(id);
            if (user == null)
            {
                _logger.LogInformation("User with id: {id} doesn't exist in the database.", id);
                return NotFound();
            }

            await _repository.User.DeleteAsync(id);
            await _repository.UserProfile.DeleteAsync(id);

            return CreatedAtRoute("UserById", new { id = id }, user);
        }

        [HttpPut("{id}")]
        async public Task<IActionResult> UpdateUserAsync(int id, [FromBody] UserDTO user)
        {
            if (user == null)
            {
                _logger.LogError("UserForUpdateDto object sent from client is null.");
                return BadRequest("UserForUpdateDto object is null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the UserForUpdateDto object");
                return UnprocessableEntity(ModelState);
            }

            User userEntity = _mapper.Map<User>(user);
            await _repository.User.UpdateAsync(userEntity, id);

            var userProfile = user.UserProfile;
            
            if (userProfile != null)
            {
                userProfile.UserId = id;
                var userProfileEntity = _mapper.Map<UserProfile>(userProfile);
                await _repository.UserProfile.UpdateAsync(userProfileEntity, id);
            }

            return CreatedAtRoute("UserById", new { id = userEntity.UserId }, userEntity);
        }
    }
}


