using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using UserAgentApi.Dtos;
using UserAgentApi.Models;
using UserAgentApi.Repositories;

namespace UserAgentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;
        private readonly ILogger<AgentsController> _logger;

        public AgentsController(IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache cache, ILogger<AgentsController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAgents()
        {
            const string cacheKey = "agents";
            var cachedAgents = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedAgents))
            {
                var agents = JsonConvert.DeserializeObject<List<AgentDto>>(cachedAgents);
                _logger.LogInformation("Fetched agents from cache.");
                return Ok(agents);
            }

            var allAgents = await _unitOfWork.Agents.GetAllWithUsersAsync();
            var agentsList = _mapper.Map<List<AgentDto>>(allAgents);
            var serializedAgents = JsonConvert.SerializeObject(agentsList);

            await _cache.SetStringAsync(cacheKey, serializedAgents, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
            });

            _logger.LogInformation("Fetched agents from database.");
            return Ok(agentsList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAgentById(int id)
        {
            var agent = await _unitOfWork.Agents.GetByIdWithUsersAsync(id);

            if (agent == null)
            {
                _logger.LogWarning("Agent with ID {Id} not found.", id);
                return NotFound();
            }

            var agentDto = _mapper.Map<AgentDto>(agent);
            return Ok(agentDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAgent([FromBody] AgentCreateDto agentCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var agent = _mapper.Map<Agent>(agentCreateDto);
            await _unitOfWork.Agents.AddAsync(agent);
            await _unitOfWork.CompleteAsync();

            var createdAgentDto = _mapper.Map<AgentDto>(agent);
            return CreatedAtAction(nameof(GetAgentById), new { id = createdAgentDto.Id }, createdAgentDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAgent(int id, [FromBody] AgentCreateDto agentCreateDto)
        {
            if (id != agentCreateDto.Id || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var existingAgent = await _unitOfWork.Agents.GetByIdAsync(id);
            if (existingAgent == null)
            {
                return NotFound();
            }

            _mapper.Map(agentCreateDto, existingAgent);
            _unitOfWork.Agents.Update(existingAgent);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAgent(int id)
        {
            var agent = await _unitOfWork.Agents.GetByIdAsync(id);
            if (agent == null)
            {
                _logger.LogWarning("Agent with ID {Id} not found.", id);
                return NotFound();
            }

            _unitOfWork.Agents.Delete(agent);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}
