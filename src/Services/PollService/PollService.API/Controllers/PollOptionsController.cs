using Common.Contracts.User;
using Common.Extensions;
using Common.Pagination;
using Common.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PollService.API.Helpers;
using PollService.API.Interfaces;

namespace PollService.API.Controllers
{
    [Route("api/pollOptions")]
    [ApiController]
    public class PollOptionsController(IPollOptionOrchestrator pollOptionOrchestrator) : ControllerBase
    {
        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        public async Task<ActionResult<PagedList<PollOptionResponse>>> GetAllPaged(
            [FromQuery] PaginationParams paginationParams)
        {
            var result = await pollOptionOrchestrator.GetAllPagedAsync(paginationParams);

            if (result.IsSuccess && result.Data is not null)
            {
                Response.AddPaginationHeader(result.Data);
            }

            return result.Match<PagedList<PollOptionResponse>, ActionResult>(
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
        [Authorize(Roles = Roles.Member)]
        [HttpPost]
        public async Task<ActionResult<PollOptionResponse>> Create(
            CreatePollOptionRequest createPollOptionRequest,
            CancellationToken cancellationToken)
        {
            var createdPollOption = await pollOptionOrchestrator.CreateAsync(createPollOptionRequest, cancellationToken);

            return createdPollOption.Match<PollOptionResponse, ActionResult>(
                onSuccess: value => CreatedAtAction(nameof(Get), new { id = value.Id }, value),
                onFailure: BadRequest
            );
        }
        [Authorize(Roles = Roles.Member)]
        [HttpPut]
        public async Task<ActionResult> Update(UpdatePollOptionRequest updatePollOptionRequest)
        {
            var result = await pollOptionOrchestrator.UpdateAsync(updatePollOptionRequest);

            return result.Match<ActionResult>(
                onSuccess: Ok,
                onFailure: BadRequest
            );
        }
        [Authorize(Roles = Roles.Member)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await pollOptionOrchestrator.DeleteAsync(id, cancellationToken);

            return result.Match<ActionResult>(
                onSuccess: Ok,
                onFailure: BadRequest
            );
        }
    }
}
