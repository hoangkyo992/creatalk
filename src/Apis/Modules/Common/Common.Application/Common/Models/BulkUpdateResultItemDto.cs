using HungHd.Shared.Utilities;

namespace Common.Application.Common.Models;

public class BulkUpdateResultItemDto : IdDto
{
    public BulkUpdateResultItemDto()
    {
        IsSuccess = false;
        StatusCode = (int)HttpStatusCode.OK;
        IsUpdatable = true;
    }

    public BulkUpdateResultItemDto(long id) : this() => Id = id;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string RefValue { get; set; }

    public bool IsUpdatable { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int StatusCode { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string FailureReason { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IDictionary Data { get; set; }

    public bool IsSuccess { get; set; }

    public void Fail(string reason, int statusCode, IDictionary? data = null)
    {
        IsUpdatable = false;
        FailureReason = reason;
        StatusCode = statusCode;
        if (data != null)
        {
            Data = data;
        }
    }

    public void Fail(string reason, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        Fail(reason, (int)statusCode, null);
    }

    public void Fail(string reason, string key, object value, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        Fail(reason, (int)statusCode, (IDictionary?)new Dictionary<string, object> { { key, value } });
    }

    public void Fail(string reason, HttpStatusCode statusCode, IDictionary errorData)
    {
        Fail(reason, (int)statusCode, errorData);
    }

    public void Fail(Exception exception)
    {
        Fail(exception.Message, exception.GetHttpStatusCode(), exception.Data);
    }
}