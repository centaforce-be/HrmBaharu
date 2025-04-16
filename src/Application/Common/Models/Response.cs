namespace HrmBaharu.Application.Common.Models;
public class Response<T>
{
    public Response()
    {
        Errors = [];
    }
    public T? Data { get; set; }
    public bool Success { get; set; } = true;
    public List<KeyValuePair<string, string>> Errors { get; set; }
    public static Response<T> Fail(string error)
    {
        return new Response<T>
        {
            Success = false,
            Errors = [new KeyValuePair<string, string>("Error", error)]
        };
    }
    public static Response<T> Fail(List<string> errors)
    {
        return new Response<T>
        {
            Success = false,
            Errors = errors.Select(e => new KeyValuePair<string, string>("Error", e)).ToList()
        };
    }
}
