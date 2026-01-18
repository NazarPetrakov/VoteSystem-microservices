using Common.Contracts.User;
using Common.Errors;
using Common.Extensions;
using Common.Pagination;
using Common.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PollService.API.Extensions;
using PollService.API.Helpers;
using PollService.API.Interfaces;

namespace PollService.API.Controllers
{
    [Route("api/polls")]
    [ApiController]
    public class PollsController(IPollOrchestrator pollOrchestrator) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedList<PollResponse>>> GetAllPaged(
            [FromQuery] PollQueryParams pollParams)
        {
            var result = await pollOrchestrator.GetAllPagedAsync(pollParams);

            if (result.IsSuccess && result.Data is not null)
            {
                Response.AddPaginationHeader(result.Data);
            }

            return result.Match<PagedList<PollResponse>, ActionResult>(
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
        [Authorize(Roles = Roles.Member)]
        [HttpPost]
        public async Task<ActionResult<PollResponse>> Create(CreatePollRequest createPollRequest,
            CancellationToken cancellationToken)
        {
            var userIdResult = User.GetId();

            if (!userIdResult.IsSuccess)
                return Unauthorized();

            var createdPoll = await pollOrchestrator.CreateAsync(createPollRequest, userIdResult.Data, cancellationToken);

            return createdPoll.Match<PollResponse, ActionResult>(
                onSuccess: value => CreatedAtAction(nameof(Get), new { id = value.Id }, value),
                onFailure: BadRequest
            );
        }
        [Authorize(Roles = Roles.Member)]
        [HttpPut]
        public async Task<ActionResult> Update(UpdatePollRequest updatePollRequest)
        {
            var userIdResult = User.GetId();

            if (!userIdResult.IsSuccess)
                return Unauthorized();

            var result = await pollOrchestrator.UpdateAsync(updatePollRequest, userIdResult.Data);

            if (result.Errors.Contains(AuthErrors.Forbidden))
                return Forbid(AuthErrors.Forbidden.Description);

            return result.Match<ActionResult>(
                onSuccess: Ok,
                onFailure: BadRequest
            );
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpPatch("{id}/close")]
        public async Task<ActionResult> ClosePoll(Guid id)
        {
            var userIdResult = User.GetId();

            if (!userIdResult.IsSuccess)
                return Unauthorized();

            var result = await pollOrchestrator.ClosePollAsync(id, userIdResult.Data);

            return result.Match<ActionResult>(
                    onSuccess: Ok,
                    onFailure: BadRequest
                );
        }
        [Authorize(Roles = Roles.Member)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var userIdResult = User.GetId();

            if (!userIdResult.IsSuccess)
                return Unauthorized();

            var result = await pollOrchestrator.DeleteAsync(id, userIdResult.Data, cancellationToken);

            if (result.Errors.Contains(AuthErrors.Forbidden))
                return Forbid(AuthErrors.Forbidden.Description);

            return result.Match<ActionResult>(
                onSuccess: Ok,
                onFailure: BadRequest
            );
        }
    }
}
