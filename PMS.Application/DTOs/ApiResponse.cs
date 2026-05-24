using System.Text.Json.Serialization;

namespace PMS.Application.DTOs
{
    public class ApiResponse<T>
    {
        [JsonPropertyName("success")]
        public bool IsSuccess { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public T? Data { get; set; }

        [JsonPropertyName("errors")]
        public List<string>? Errors { get; set; }

        public static ApiResponse<T> Success(T? data, string message) => new()
        {
            IsSuccess = true,
            Message = message,
            Data = data,
            Errors = null
        };

        public static ApiResponse<T> Fail(string message, List<string>? errors = null) => new()
        {
            IsSuccess = false,
            Message = message,
            Data = default,
            Errors = errors
        };
    }
}