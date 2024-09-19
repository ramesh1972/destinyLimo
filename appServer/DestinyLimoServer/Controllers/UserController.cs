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
using DestinyLimoServer.Common.Uploader;
using Newtonsoft.Json;

namespace DestinyLimoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IRepositoryManager repository, IMapper mapper, ILogger logger, IConfiguration configuration) : ControllerBase
    {
        private readonly IRepositoryManager _repository = repository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger _logger = logger;
        private readonly IConfiguration _configuration = configuration;

        public class ApiResponse<T>
        {
            public bool? Success { get; set; }
            public string? Message { get; set; }
            public T? Data { get; set; }
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> authenicateUser([FromBody] UserLoginDTO loginDTO)
        {
            User user = await _repository.User.AuthenticateUser(loginDTO.Username, loginDTO.Password);
            if (user == null)
            {
                _logger.LogInformation("User with username: {username} is not found", loginDTO.Username);
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid username or password",
                    Data = null
                });
            }
            else
            {
                var userDTO = _mapper.Map<DestinyLimoServer.DTOs.ResponseDTOs.UserDTO>(user);

                var userRoles = await _repository.Role.GetUserRoles(user.user_id ?? -1);
                var userRolesDTO = _mapper.Map<IEnumerable<DTOs.ResponseDTOs.UserRoleDTO>>(userRoles);
                userDTO.Roles = userRolesDTO;

                var userProfile = await _repository.UserProfile.GetUserById(user.user_id ?? -1);
                userDTO.UserProfile = _mapper.Map<DestinyLimoServer.DTOs.ResponseDTOs.UserProfileDTO>(userProfile);

                if (userDTO.IsLocked)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Account is locked",
                        Data = userDTO
                    });
                }
                else if (!userDTO.IsApproved)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Account not approved",
                        Data = userDTO
                    });
                }

                return Ok(userDTO);
            }
        }

        [HttpPost("approveReject")]
        public async Task<IActionResult> ApproveUser([FromBody] ApproveRejectDTO approveRejectDTO)
        {
            User user = await _repository.User.GetUserById(approveRejectDTO.UserId);
            if (user == null)
            {
                _logger.LogInformation("User with id: {id} is not found", approveRejectDTO.UserId);
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User not found",
                    Data = null
                });
            }

            await _repository.User.ApproveRejectUser(approveRejectDTO.UserId, approveRejectDTO.IsApproved, approveRejectDTO.ApproveRejectReason, approveRejectDTO.ApprovedRejectedBy);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "User approved successfully",
                Data = null
            });
        }

        [HttpPost("lockUser")]
        public async Task<IActionResult> LockUser([FromBody] LockUnlockUserDTO userDTO)
        {
            User user = await _repository.User.GetUserById(userDTO.UserId);
            if (user == null)
            {
                _logger.LogInformation("User with id: {id} is not found", userDTO.UserId);
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User not found",
                    Data = null
                });
            }

            await _repository.User.LockUser(userDTO.UserId, userDTO.IsLocked);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "User locked successfully",
                Data = null
            });
        }


        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPassword)
        {
            User user = await _repository.User.GetUserByUsername(resetPassword.Username);
            if (user == null)
            {
                _logger.LogInformation("User with username: {username} is not found", resetPassword.Username);
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid username",
                    Data = null
                });
            }
            if (user.is_locked)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Account is locked",
                    Data = null
                });
            }
            else if (!user.is_approved)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Account not approved",
                    Data = null
                });
            }

            await _repository.User.ResetPassword(user.user_id ?? -1, resetPassword.NewPassword);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Password reset successfully. Login with your new password",
                Data = null
            });
        }

        [HttpGet("{includeInActive?}/{includeDeleted?}")]
        public async Task<IActionResult> GetUsersAsync(bool? includeInActive = true, bool? includeDeleted = false)
        {
            var users = await _repository.User.GetUsers(includeInActive ?? true, includeDeleted ?? false);
            System.Console.WriteLine("Users: " + users);

            var usersDto = _mapper.Map<IEnumerable<DestinyLimoServer.DTOs.ResponseDTOs.UserDTO>>(users);

            // get all the profiles
            var userProfiles = await _repository.UserProfile.GetUsers(includeInActive ?? true, includeDeleted ?? false);

            // get all the roles
            var rolesList = await _repository.Role.GetRoles();
            var userRolesList = await _repository.Role.GetAllUsersRoles();

            // map the userDTO to the userProfiles, roles and userRoles
            foreach (var userDto in usersDto)
            {
                var userProfile = userProfiles.FirstOrDefault(x => x.user_id == userDto.UserId);
                if (userProfile != null)
                {
                    userDto.UserProfile = _mapper.Map<DestinyLimoServer.DTOs.ResponseDTOs.UserProfileDTO>(userProfile);
                }

                var userRoles = userRolesList.Where(x => x.user_id == userDto.UserId);
                if (userRoles != null)
                {
                    var rolesDTO = _mapper.Map<IEnumerable<DTOs.ResponseDTOs.UserRoleDTO>>(userRoles);
                    userDto.Roles = rolesDTO;
                }
            }

            // get only the drivers 
            // TODO: make it more generic for any role
            int driverRoleId = 2;
            usersDto = usersDto.Where(x => x.Roles!.Any(y => y.role_id == driverRoleId));
            return Ok(usersDto);
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
        async public Task<IActionResult> CreateUserAsync([FromForm] DestinyLimoServer.DTOs.RequestDTOs.UserDTOJsonString userJSON)
        {
            DTOs.RequestDTOs.UserDTO user = JsonConvert.DeserializeObject<DTOs.RequestDTOs.UserDTO>(userJSON.user)!;

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

            // inputs to register
            var userEntity = _mapper.Map<User>(user);

            var userProfile = user.UserProfile;
            var userProfileEntity = _mapper.Map<UserProfile>(userProfile);

            // Handle avatar file
            var avatar = HttpContext.Request.Form.Files[0];
            if (avatar != null && avatar.Length > 0)
            {
                FileUploader fileUploader = new FileUploader(_logger, _configuration);
                await fileUploader.UploadFileAsync(avatar, UploadFilePaths.ProfilePicture);
            }

            // extract the avatar file name
            userProfileEntity.avatar = Path.GetFileName(userProfileEntity.avatar) ?? "default-avatar.png";

            // register
            User? newUser = await _repository.User.RegisterUser(userEntity, userProfileEntity);
            if (newUser == null)
            {
                _logger.LogError("Unable to register user");
                return Ok(false);
            }

            // set output
            UserProfile newUserProfile = await _repository.UserProfile.GetUserById(newUser.user_id ?? -1);

            IEnumerable<UserRole> userRoles = await _repository.Role.GetUserRoles(newUser.user_id ?? -1);

            DestinyLimoServer.DTOs.ResponseDTOs.UserDTO userDTO = _mapper.Map<DestinyLimoServer.DTOs.ResponseDTOs.UserDTO>(newUser);
            userDTO.UserProfile = _mapper.Map<DestinyLimoServer.DTOs.ResponseDTOs.UserProfileDTO>(newUserProfile);
            userDTO.Roles = _mapper.Map<IEnumerable<DestinyLimoServer.DTOs.ResponseDTOs.UserRoleDTO>>(userRoles);

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

        [HttpPut()]
        async public Task<IActionResult> UpdateUserAsync([FromForm] DestinyLimoServer.DTOs.RequestDTOs.UserDTOJsonString userJSON)
        {
            DTOs.RequestDTOs.UserDTO user = JsonConvert.DeserializeObject<DTOs.RequestDTOs.UserDTO>(userJSON.user)!;

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
            await _repository.User.UpdateUser(userEntity);

            var userProfile = user.UserProfile;

            if (userProfile != null)
            {
                if (HttpContext.Request.Form.Files.Count > 0)
                {
                    var avatar = HttpContext.Request.Form.Files[0];
                    if (avatar != null && avatar.Length > 0)
                    {
                        FileUploader fileUploader = new FileUploader(_logger, _configuration);
                        await fileUploader.UploadFileAsync(avatar, UploadFilePaths.ProfilePicture);
                    }
                }

                userProfile.UserId = userEntity.user_id;
                var userProfileEntity = _mapper.Map<UserProfile>(userProfile);
                await _repository.UserProfile.UpdateAsync(userProfileEntity, userEntity.user_id ?? -1);

                var userResponseDTO = _mapper.Map<DestinyLimoServer.DTOs.ResponseDTOs.UserDTO>(userEntity);

                var userProfileResponseDTO = _mapper.Map<DestinyLimoServer.DTOs.ResponseDTOs.UserProfileDTO>(userProfileEntity);
                userResponseDTO.UserProfile = userProfileResponseDTO;

                // get the roles
                var userRoles = await _repository.Role.GetUserRoles(userEntity.user_id ?? -1);
                var userRolesDTO = _mapper.Map<IEnumerable<DTOs.ResponseDTOs.UserRoleDTO>>(userRoles);
                userResponseDTO.Roles = userRolesDTO;

                return Ok(userResponseDTO);
            }

            return BadRequest("User profile is required");
        }
    }
}


