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

        [HttpPost("authenticate")]
        public async Task<IActionResult> authenicateUser([FromBody] UserLoginDTO loginDTO)
        {
            User user = await _repository.User.AuthenticateUser(loginDTO.Username, loginDTO.Password);
            if (user == null)
            {
                _logger.LogInformation("User with username: {username} is not authenticated.", loginDTO.Username);
                return NotFound();
            }
            else
            {
                var userDTO = _mapper.Map<DestinyLimoServer.DTOs.ResponseDTOs.UserDTO>(user);

                var userRoles = await _repository.User.GetUserRolesByUserId(user.user_id);
                var userRolesDTO = _mapper.Map<IEnumerable<DTOs.ResponseDTOs.RoleDTO>>(userRoles);
                userDTO.Roles = userRolesDTO;


                var userProfile = await _repository.UserProfile.GetUserById(user.user_id);
                userDTO.UserProfile = _mapper.Map<DestinyLimoServer.DTOs.ResponseDTOs.UserProfileDTO>(userProfile);
                return Ok(userDTO);
            }
        }

        [HttpGet("{includeInActive?}/{includeDeleted?}")]
        public async Task<IActionResult> GetUsersAsync(bool? includeInActive = true, bool? includeDeleted = false)
        {
            var users = await _repository.User.GetUsers(includeInActive ?? true, includeDeleted ?? false);
            System.Console.WriteLine("Users: " + users);

            var userDto = _mapper.Map<IEnumerable<DestinyLimoServer.DTOs.ResponseDTOs.UserDTO>>(users);

            // get all the profiles
            var userProfiles = await _repository.UserProfile.GetUsers(includeInActive ?? true, includeDeleted ?? false);

            // get all the roles
            var rolesList = await _repository.Role.GetRoles();
            var userRolesList = await _repository.Role.GetAllUsersRoles();

            // map the userDTO to the userProfiles, roles and userRoles
            foreach (var user in userDto)
            {
                var userProfile = userProfiles.FirstOrDefault(x => x.user_id == user.UserId);
                if (userProfile != null)
                {
                    user.UserProfile = _mapper.Map<DestinyLimoServer.DTOs.ResponseDTOs.UserProfileDTO>(userProfile);
                }

                var userRoles = userRolesList.Where(x => x.UserId == user.UserId);
                if (userRoles != null)
                {
                    var roles = userRoles.Select(x => new Role { role_id = x.RoleId, role_name = rolesList.FirstOrDefault(y => y.role_id == x.RoleId).role_name });
                    var rolesDTO = _mapper.Map<IEnumerable<DTOs.ResponseDTOs.RoleDTO>>(roles);
                    user.Roles = rolesDTO;
                }
            }

            return Ok(userDto);
        }

        [HttpGet("profile/{includeInActive?}/{includeDeleted?}")]
        public async Task<IActionResult> GetUsersProfilesAsync(bool? includeInActive = true, bool? includeDeleted = false)
        {
            var users = await _repository.UserProfile.GetUsers(includeInActive ?? true, includeDeleted ?? false);
            System.Console.WriteLine("Users: " + users);

            var userDto = _mapper.Map<IEnumerable<DestinyLimoServer.DTOs.ResponseDTOs.UserProfileDTO>>(users);
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
                var userDto = _mapper.Map<DestinyLimoServer.DTOs.ResponseDTOs.UserDTO>(user);
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
                var userDto = _mapper.Map<DestinyLimoServer.DTOs.ResponseDTOs.UserProfileDTO>(user);
                return Ok(userDto);
            }
        }

        [HttpPost]
        async public Task<IActionResult> CreateUserAsync([FromBody] DestinyLimoServer.DTOs.RequestDTOs.UserDTO user)
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

            var userProfile = user.UserProfile;
            var userProfileEntity = _mapper.Map<UserProfile>(userProfile);

            User newUser = await _repository.User.RegisterUser(userEntity, userProfileEntity);
            if (newUser == null)
            {
                _logger.LogError("Unable to register user");
                return Ok(false);
            }

            UserProfile newUserProfile = await _repository.UserProfile.GetUserById(newUser.user_id);
            userEntity.UserProfile = newUserProfile;

            DestinyLimoServer.DTOs.ResponseDTOs.UserDTO userDTO = _mapper.Map<DestinyLimoServer.DTOs.ResponseDTOs.UserDTO>(userEntity);

            return Ok(userDTO);
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

            await _repository.User.DeleteUser(id);
            await _repository.UserProfile.DeleteAsync(id);

            return CreatedAtRoute("UserById", new { id = id }, user);
        }

        [HttpPut("{id}")]
        async public Task<IActionResult> UpdateUserAsync(int id, [FromBody] DestinyLimoServer.DTOs.RequestDTOs.UserDTO user)
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
            userEntity.user_id = id;
            await _repository.User.UpdateUser(userEntity);

            var userProfile = user.UserProfile;

            if (userProfile != null)
            {
                userProfile.UserId = id;
                var userProfileEntity = _mapper.Map<UserProfile>(userProfile);
                userProfileEntity.user_id = id;
                await _repository.UserProfile.UpdateAsync(userProfileEntity, id);
            }

            return CreatedAtRoute("UserById", new { id = userEntity.user_id }, userEntity);
        }
    }
}


