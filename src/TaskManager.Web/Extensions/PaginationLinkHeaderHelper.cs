namespace TaskManager.Web.Extensions;

public static class PaginationLinkHeaderHelper
{
  public static void AddPaginationLinkHeader(
    this HttpContext httpContext, 
    int currentPage, 
    int perPage, 
    int totalPages)
  {
    var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}";
    
    string Link(string rel, int page) => 
      $"<{baseUrl}?page={page}&per_page={perPage}>; rel=\"{rel}\"";

    var parts = new List<string>();
    
    if (currentPage > 1)
    {
      parts.Add(Link("first", 1));
      parts.Add(Link("prev", currentPage - 1));
    }
    
    if (currentPage < totalPages)
    {
      parts.Add(Link("next", currentPage + 1));
      parts.Add(Link("last", totalPages));
    }

    if (parts.Count > 0)
    {
      httpContext.Response.Headers["Link"] = string.Join(", ", parts);
    }
  }
}
