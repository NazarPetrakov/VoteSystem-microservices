using Common.Result;
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
        public async Task<ActionResult<List<VoteResponse>>> GetVotes()
        {
            var votesResult = await voteOrchestrator.GetAllAsync();

            return votesResult.Match<List<VoteResponse>, ActionResult>
            (
                onSuccess: Ok,
                onFailure: NotFound
            );
        }
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
        [HttpDelete]
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
