using Common.Result;
using Microsoft.AspNetCore.Mvc;
using PollService.API.Helpers;
using PollService.API.Interfaces;

namespace PollService.API.Controllers
{
    [Route("api/polls")]
    [ApiController]
    public class PollsController(IPollOrchestrator pollOrchestrator) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<PollResponse>>> GetAll()
        {
            var result = await pollOrchestrator.GetAllAsync();

            return result.Match<List<PollResponse>, ActionResult>(
                onSuccess: Ok,
                onFailure: NotFound
            );
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<PollResponse>> Get(Guid id)
        {
            var result = await pollOrchestrator.GetAsync(id);

            return result.Match<PollResponse, ActionResult>(
                onSuccess: Ok,
                onFailure: NotFound
            );
        }
        [HttpPost]
        public async Task<ActionResult<PollResponse>> Create(CreatePollRequest createPollRequest,
            CancellationToken cancellationToken)
        {
            var createdPoll = await pollOrchestrator.CreateAsync(createPollRequest, cancellationToken);

            return createdPoll.Match<PollResponse, ActionResult>(
                onSuccess: value => CreatedAtAction(nameof(Get), new { id = value.Id }, value),
                onFailure: BadRequest
            );
        }
        [HttpPut]
        public async Task<ActionResult> Update(UpdatePollRequest updatePollRequest)
        {
            var result = await pollOrchestrator.UpdateAsync(updatePollRequest);

            return result.Match<ActionResult>(
                onSuccess: Ok,
                onFailure: BadRequest
            );
        }
        [HttpPatch("{id}/close")]
        public async Task<ActionResult> ClosePoll(Guid id)
        {
            var result = await pollOrchestrator.ClosePollAsync(id);

            return result.Match<ActionResult>(
                    onSuccess: Ok,
                    onFailure: BadRequest
                );
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await pollOrchestrator.DeleteAsync(id, cancellationToken);

            return result.Match<ActionResult>(
                onSuccess: Ok,
                onFailure: BadRequest
            );
        }
    }
}
