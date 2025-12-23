namespace Common.Application.Common.Models;

public record CodeNameValueItem
{
    [JsonConverter(typeof(ZCodeJsonConverter))]
    public long Id { get; set; }

    public string Code { get; set; }
    public string Name { get; set; }

    public CodeNameValueItem()
    {
    }

    public CodeNameValueItem(long id) : this()
    {
        Id = id;
    }

    public CodeNameValueItem(long id, string code, string name) : this(id)
    {
        Code = code;
        Name = name;
    }
}