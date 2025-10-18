using Common.Contracts.User;
using Common.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoteService.API.Contracts.Dtos;
using VoteService.API.Interfaces;

namespace VoteService.API.Controllers
{
    [Route("api/votes")]
    [ApiController]
    public class VotesController(IVoteOrchestrator voteOrchestrator) : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<List<VoteResponse>>> GetVotes()
        {
            var votesResult = await voteOrchestrator.GetAllAsync();

            return votesResult.Match<List<VoteResponse>, ActionResult>
            (
                onSuccess: Ok,
                onFailure: NotFound
            );
        }
        [HttpGet("by-user/{userId}")]
        [Authorize(Roles = Roles.Member)]
        public async Task<ActionResult<List<VoteResponse>>> GetUserVotes(int userId)
        {
            var votesResult = await voteOrchestrator.GetAllAsync(v => v.UserId == userId);

            return votesResult.Match<List<VoteResponse>, ActionResult>
            (
                onSuccess: Ok,
                onFailure: NotFound
            );
        }
        [Authorize(Roles = Roles.Member)]
        [HttpGet("{id}")]
        public async Task<ActionResult<VoteResponse>> GetVote(Guid id)
        {
            var voteResult = await voteOrchestrator.GetAsync(id);

            return voteResult.Match<VoteResponse, ActionResult>
            (
                onSuccess: Ok,
                onFailure: NotFound
            );
        }
        [Authorize(Roles = Roles.Member)]
        [HttpPost]
        public async Task<ActionResult<VoteResponse>> CreateVote(CreateVoteRequest createVoteRequest)
        {
            var createResult = await voteOrchestrator.CreateAsync(createVoteRequest);

            return createResult.Match<VoteResponse, ActionResult>
            (
                onSuccess: value => CreatedAtAction(nameof(GetVote), new { Id = value.VoteId }, value),
                onFailure: BadRequest
            );
        }
        [Authorize(Roles = Roles.Member)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteVote(Guid id)
        {
            var result = await voteOrchestrator.DeleteAsync(id);

            return result.Match<ActionResult>
            (
                onSuccess: Ok,
                onFailure: BadRequest
            );
        }
    }
}
