using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json; // You can switch to System.Text.Json if preferred
using UserAgentApi.Dtos;
using UserAgentApi.Models;
using UserAgentApi.Repositories;

namespace UserAgentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache cache, ILogger<UsersController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            const string cacheKey = "all_users";
            var cachedUsers = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedUsers))
            {
                var users = JsonConvert.DeserializeObject<List<UserDto>>(cachedUsers);
                _logger.LogInformation("Fetched users from cache.");
                return Ok(users);
            }

            var allUsers = await _unitOfWork.Users.GetAllWithAgentsAsync();
            var usersList = _mapper.Map<List<UserDto>>(allUsers);
            var serializedUsers = JsonConvert.SerializeObject(usersList);

            await _cache.SetStringAsync(cacheKey, serializedUsers, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10) 
            });

            _logger.LogInformation("Fetched users from database.");
            return Ok(usersList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _unitOfWork.Users.GetByIdWithAgentAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User with ID {Id} not found.", id);
                return NotFound();
            }

            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto userCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _mapper.Map<User>(userCreateDto);
            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            var createdUserDto = _mapper.Map<UserCreateDto>(user);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUserDto.Id }, createdUserDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserCreateDto userCreateDto)
        {
            if (id != userCreateDto.Id || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var existingUser = await _unitOfWork.Users.GetByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            _mapper.Map(userCreateDto, existingUser);
            _unitOfWork.Users.Update(existingUser);
            await _unitOfWork.CompleteAsync();

            var updatedUserDto = _mapper.Map<UserDto>(existingUser);
            return Ok(updatedUserDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {Id} not found.", id);
                return NotFound();
            }

            _unitOfWork.Users.Delete(user);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}