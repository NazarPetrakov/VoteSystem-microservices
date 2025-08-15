using Microsoft.AspNetCore.Mvc;
using PollService.API.Extensions;
using PollService.API.Helpers;
using PollService.API.Interfaces;

namespace PollService.API.Controllers
{
    [Route("api/pollOptions")]
    [ApiController]
    public class PollOptionsController(IPollOptionOrchestrator pollOptionOrchestrator) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<PollOptionResponse>>> GetAll()
        {
            var result = await pollOptionOrchestrator.GetAllAsync();

            return result.Match<List<PollOptionResponse>, ActionResult>(
                onSuccess: Ok,
                onFailure: NotFound
            );
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<PollOptionResponse>> Get(Guid id)
        {
            var result = await pollOptionOrchestrator.GetAsync(id);

            return result.Match<PollOptionResponse, ActionResult>(
                onSuccess: Ok,
                onFailure: NotFound
            );
        }
        [HttpPost]
        public async Task<ActionResult<PollOptionResponse>> Create(CreatePollOptionRequest createPollOptionRequest)
        {
            var createdPollOption = await pollOptionOrchestrator.CreateAsync(createPollOptionRequest);

            return createdPollOption.Match<PollOptionResponse, ActionResult>(
                onSuccess: value => CreatedAtAction(nameof(Get), new { id = value.Id }, value),
                onFailure: BadRequest
            );
        }
        [HttpPut]
        public async Task<ActionResult> Update(UpdatePollOptionRequest updatePollOptionRequest)
        {
            var result = await pollOptionOrchestrator.UpdateAsync(updatePollOptionRequest);

            return result.Match<ActionResult>(
                onSuccess: Ok,
                onFailure: BadRequest
            );
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await pollOptionOrchestrator.DeleteAsync(id);

            return result.Match<ActionResult>(
                onSuccess: Ok,
                onFailure: BadRequest
            );
        }
    }
}
