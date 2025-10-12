using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using ApartmentManagementSystem.Exceptions;

namespace ApartmentManagementSystem.Response
{
    public class ResponseData<TData> : ResponseDataBase
    {
        public ResponseData(HttpStatusCode status, TData data, ErrorResponse error = null, MetaData metadata = null) : base(status, error, metadata)
        {
            Data = data;
        }
        [JsonPropertyName("data")]
        public TData Data { get; set; }
    }
    public class ResponseDataBase
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }
        [JsonPropertyName("metadata")]
        public MetaData Metadata { get; set; }
        [JsonPropertyName("error")]
        public ErrorResponse Error { get; set; }
        public ResponseDataBase(HttpStatusCode status, ErrorResponse error = null, MetaData metadata = null)
        {
            Status = (int)status; Error = error; Metadata = metadata;
        }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
    public class MetaData
    {
        public int Page { get; set; }
        public int PerPage { get; set; }
        public int Total { get; set; }
        public string Sort { get; set; }
    }
}