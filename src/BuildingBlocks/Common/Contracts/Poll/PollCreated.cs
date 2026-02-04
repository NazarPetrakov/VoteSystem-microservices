namespace Common.Contracts.Poll;

public record PollCreated(Guid PollId, int UserId, ICollection<Guid> OptionIds) { }