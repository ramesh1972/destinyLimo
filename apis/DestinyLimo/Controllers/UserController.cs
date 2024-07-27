using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DestinyLimo.Repositories;
using Microsoft.AspNetCore.Mvc;
using AutoMapper; // Add this line to import the IMapper interface
using DestinyLimo.DTOs.RequestDTOs;
using DestinyLimo.Models;
using Org.BouncyCastle.Crypto.Prng;

namespace DestinyLimo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IRepositoryManager repository, IMapper mapper, ILogger logger) : ControllerBase
    {
        private readonly IRepositoryManager _repository = repository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger _logger = logger;

        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _repository.User.GetUsers();
            var userDto = _mapper.Map<IEnumerable<UserDTO>>(users);
            return Ok(userDto);
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var user = _repository.User.GetUserById(id);
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

        [HttpPost]
        public IActionResult CreateUser([FromBody] UserDTO user)
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
            _repository.User.AddUser(userEntity);
            //_repository.Save();

            return CreatedAtRoute("UserById", new { id = userEntity.UserId }, userEntity);

        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _repository.User.GetUserById(id);
            if (user == null)
            {
                _logger.LogInformation("User with id: {id} doesn't exist in the database.", id);
                return NotFound();
            }

            _repository.User.DeleteUser(id);
            //_repository.Save();

            return CreatedAtRoute("UserById", new { id = id }, user);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UserDTO user)
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

            _repository.User.UpdateUser(userEntity);
            //_repository.Save();

            return CreatedAtRoute("UserById", new { id = userEntity.UserId }, userEntity);
        }
    }
}


