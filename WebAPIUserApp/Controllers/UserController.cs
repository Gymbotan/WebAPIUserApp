using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Data;
using WebAPIUserApp.Domain.Repositories.Interfaces;
using WebAPIUserApp.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebAPIUserApp.Controllers
{
    /// <summary>
    /// User controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        public UserController(IUserRepository repository, IRoleRepository roleRepository)
        {
            _userRepository = repository;
            _roleRepository = roleRepository;
        }

        /// <summary>
        /// Get all users from DB
        /// </summary>
        /// <returns>Data of users</returns>
        [HttpGet("GetAllUsers")]
        [ProducesResponseType(200, Type = typeof(IQueryable<User>))]
        [ProducesResponseType(400, Type = typeof(void))]
        public IActionResult GetAllUsers()
        {
            var data = _userRepository.GetAllUsers();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(data);
        }

        /// <summary>
        /// Get specific users using filtering and sorting
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="nameFilter">Data to filter Name field</param>
        /// <param name="emailFilter">Data to filter Email field</param>
        /// <param name="ageLimit1">One side age limitation</param>
        /// <param name="ageLimit2">Another side age limitation</param>
        /// <param name="roleFilter">Data to filter Roles field</param>
        /// <param name="propertyForOrdering">Property (field) to order result</param>
        /// <param name="direction">Ascending or descending direction of ordering</param>
        /// <returns>Data of choosen users</returns>
        [HttpGet("GetSpecificUsers")]
        [ProducesResponseType(200, Type = typeof(IQueryable<User>))]
        [ProducesResponseType(400, Type = typeof(void))]
        [ProducesResponseType(425, Type = typeof(void))]
        public IActionResult GetSpecificUsers(int page = 1, int pageSize = 10, string nameFilter = "", string emailFilter = "", int ageLimit1 = 0, int ageLimit2 = 100, string roleFilter = "", string propertyForOrdering = "id", Direction direction = Direction.asc)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!new List<string> { ""}.Contains(propertyForOrdering.ToLower()))
            {
                ModelState.AddModelError("", "There is no such field to order");
            }

            var totalCount = _userRepository.GetCount();
            var totalPages = (int)Math.Ceiling((decimal)totalCount/pageSize);

            if (page > totalPages)
            {
                ModelState.AddModelError("", "There are no so many pages in the selected dataset");
                return StatusCode(425, ModelState);
            }

            var data = _userRepository.GetUsers(page, pageSize, nameFilter, emailFilter, Math.Min(ageLimit1, ageLimit2), Math.Max(ageLimit1, ageLimit2), roleFilter, propertyForOrdering, direction);
            
            return Ok(data);
        }

        /// <summary>
        /// Get user's information with selected Id
        /// </summary>
        /// <param name="id">User's Id</param>
        /// <returns>User's information</returns>
        [HttpGet("GetUserById")]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(400, Type = typeof(void))]
        [ProducesResponseType(404, Type = typeof(void))]
        [ProducesErrorResponseType(typeof(void))]
        public IActionResult GetUser(Guid id)
        {
            if (!_userRepository.IsUserExistById(id))
            {
                ModelState.AddModelError("", "This user doesn't exist");
                Log.Error("Getting user by Id error. Selected user doesn't exist");
                return NotFound(ModelState);
            }

            User result = _userRepository.GetUserById(id);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get roles of selected user
        /// </summary>
        /// <param name="id">User's Id</param>
        /// <returns>User's roles</returns>
        [HttpGet("GetUserRolesById")]
        [ProducesResponseType(200, Type = typeof(IQueryable<RoleDto>))]
        [ProducesResponseType(400, Type = typeof(void))]
        [ProducesResponseType(404, Type = typeof(void))]
        [ProducesErrorResponseType(typeof(void))]
        public IActionResult GetUserRoles(Guid id)
        {
            if (!_userRepository.IsUserExistById(id))
            {
                ModelState.AddModelError("", "This user doesn't exist");
                Log.Error("Getting user's roles error. Selected user doesn't exist");
                return NotFound(ModelState);
            }

            var result = _userRepository.GetRolesByUserId(id).
                Select(r => new RoleDto { RoleName = r.RoleName }).AsQueryable();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(result);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="data">User's information</param>
        /// <returns>Status code</returns>
        [HttpPost("CreateUser")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400, Type = typeof(void))]
        [ProducesResponseType(422, Type = typeof(void))]
        [ProducesResponseType(424, Type = typeof(void))]
        [ProducesErrorResponseType(typeof(void))]
        public IActionResult CreateUser([FromBody] UserDto data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (data is null)
                return BadRequest(ModelState);

            if (string.IsNullOrEmpty(data.Name))
            {
                ModelState.AddModelError("", "User's name is null or empty");
                Log.Error("User creation error. User's name is null or empty");
                return StatusCode(424, ModelState);
            }
            if (string.IsNullOrEmpty(data.Email))
            {
                ModelState.AddModelError("", "User's email is null or empty");
                Log.Error("User creation error. User's email is null or empty"); 
                return StatusCode(424, ModelState);
            }
            if (data.Age < 0)
            {
                ModelState.AddModelError("", "User's age is a negative number");
                Log.Error("User creation error. User's age is a negative number");
                return StatusCode(424, ModelState);
            }

            if (_userRepository.IsUserExistByEmail(data.Email))
            {
                ModelState.AddModelError("", "This email already exists");
                Log.Error("User creation error. Inputed email already exists"); 
                return StatusCode(422, ModelState);
            }

            var user = new User
            {
                Id = new Guid(),
                Name = data.Name,
                Email = data.Email,
                Age = data.Age,
                Roles = new List<Role>()
            };

            if (!_userRepository.AddUser(user))
            {
                ModelState.AddModelError("", "Something went wrong adding a user");
                Log.Error("User creation error. Something went wrong adding a user");
            }
            return StatusCode(204, "Successfully created");
        }

        /// <summary>
        /// Update user's information
        /// </summary>
        /// <param name="userId">Id of a user we should update</param>
        /// <param name="updatedData">New user's information</param>
        /// <returns>Status code</returns>
        [HttpPut("UpdateUser")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400, Type = typeof(void))]
        [ProducesResponseType(404, Type = typeof(void))]
        [ProducesErrorResponseType(typeof(void))]
        public IActionResult UpdateUser(Guid userId, [FromBody] UserDto updatedData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (updatedData is null || updatedData.Name is null || updatedData.Email is null || updatedData.Age < 0)
            {
                return BadRequest(ModelState);
            }

            if (!_userRepository.IsUserExistById(userId))
            {
                Log.Error("User updating error. Selected user doesn't exist");
                return NotFound();
            }
            
            var updatedUser = new User
            {
                Id = userId,
                Name = updatedData.Name,
                Email = updatedData.Email,
                Age = updatedData.Age,
                Roles = _userRepository.GetRolesByUserId(userId).ToList()
            };

            if (!_userRepository.UpdateUser(updatedUser))
            {
                ModelState.AddModelError("", "Something went wrong updating a user");
                Log.Error("User updating error. Something went wrong updating a user");
            }
            return StatusCode(204, "Successfully updated");
        }

        /// <summary>
        /// Add a new role for a user
        /// </summary>
        /// <param name="userId">User's id</param>
        /// <param name="roleName">Role's name we should to add</param>
        /// <returns>Status code</returns>
        [HttpPut("AddRoleToUser")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400, Type = typeof(void))]
        [ProducesResponseType(404, Type = typeof(void))]
        [ProducesResponseType(423, Type = typeof(string))]
        [ProducesErrorResponseType(typeof(void))]
        public IActionResult AddRoleToUser(Guid userId, string roleName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                ModelState.AddModelError("", "RoleName can not be a null or empty string");
                return BadRequest(ModelState);
            }

            if (!_userRepository.IsUserExistById(userId))
            {
                ModelState.AddModelError("", "This user doesn't exist");
                Log.Error("Adding new role error. Selected user doesn't exist");
                return NotFound(ModelState);
            }

            if (!_roleRepository.IsRoleExist(roleName))
            {
                ModelState.AddModelError("", "This role doesn't exist");
                Log.Error("Adding new role error. Selected role doesn't exist");
                return NotFound(ModelState);
            }

            var updatedUser = _userRepository.GetUserById(userId);
            var newRole = _roleRepository.GetRoleByName(roleName);

            updatedUser.Roles ??= new List<Role>();

            if (updatedUser.Roles.Any(r => r.RoleName == newRole.RoleName))
            {
                Log.Error("Adding new role error. The user already contains this role");
                return StatusCode(423, "The user already contains this role");
            }

            updatedUser.Roles = new List<Role>() { newRole };

            if (!_userRepository.UpdateUser(updatedUser))
            {
                ModelState.AddModelError("", "Something went wrong updating a user");
                Log.Error("Adding new role error. Something went wrong updating a user");
            }

            return StatusCode(204, "Successfully updated");
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="userId">Id of a user we should delete</param>
        /// <returns>Status code</returns>
        [HttpDelete("DeleteUser")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400, Type = typeof(void))]
        [ProducesResponseType(404, Type = typeof(void))]
        [ProducesErrorResponseType(typeof(void))]
        public IActionResult DeleteUser(Guid userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_userRepository.IsUserExistById(userId))
            {
                return NotFound();
            }

            var userToDelete = _userRepository.GetUserById(userId);
            
            if (userToDelete is null)
            {
                ModelState.AddModelError("", "This user doesn't exist");
                Log.Error("User deleting error. Selected user doesn't exist");
                return NotFound(ModelState);
            }

            if (!_userRepository.DeleteUser(userToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting a user");
                Log.Error("User deleting error. Something went wrong deleting a user");
            }
            return StatusCode(204, "Successfully deleted");
        }
    }
}
