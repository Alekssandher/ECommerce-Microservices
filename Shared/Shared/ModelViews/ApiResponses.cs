using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Shared.ModelViews
{
    public abstract class ApiResponse<T>
    {
        [Description("URI with an instance reference.")]
        [JsonPropertyName("type")]
        public string Type { get; init; }

        [Description("Response status in accord to RFC 7807.")]
        [JsonPropertyName("status")]
        public int Status { get; init; }

        [Description("Title response.")]
        [JsonPropertyName("title")]
        public string Title { get; init; }

        [Description("Detailed response description.")]
        [JsonPropertyName("detail")]
        public string Detail { get; init; }

        [Description("Response instance ID.")]
        [JsonPropertyName("instance")]
        public string Instance { get; init; }

        [Description("Response Data")]
        [JsonPropertyName("data")]
        public T? Data { get; init; }

        protected ApiResponse(int status, string type, string title, string detail, string instance, T? data)
        {
            Status = status;
            Type = type;
            Title = title;
            Detail = detail;
            Instance = instance;
            Data = data;
        }

        protected ApiResponse(int status, string type, string title, string detail, PathString instance)
        {
            Status = status;
            Type = type;
            Title = title;
            Detail = detail;
            Instance = instance;
        }
    }

    public class OkResponse<T> : ApiResponse<T>
    {
        private static readonly IHttpContextAccessor _httpContextAccessor = new HttpContextAccessor();
        

        public OkResponse(string title, string detail, T? data)
            : base(
                  status: StatusCodes.Status200OK,
                  type: "https://datatracker.ietf.org/doc/html/rfc9110#name-200-ok",
                  title: title ?? "Request Successful",
                  detail: detail ?? "Request fetched successfully.",
                  instance: _httpContextAccessor.HttpContext?.Request.Path ?? "/unknown",
                  data: data)
        {
          
        }
    }
}