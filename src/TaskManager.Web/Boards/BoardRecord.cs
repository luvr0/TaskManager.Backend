namespace TaskManager.Web.Boards;

public record BoardRecord(int Id, string Name, int OwnerId, int MemberCount, List<MemberSummaryResponse> Members);

public record MemberSummaryResponse(int UserId, string Role, string Email, string Alias);
