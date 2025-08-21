using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Shared.ModelViews
{
    public abstract class ErrorDetails : ProblemDetails
    {

        [Description("Response status in accord to RFC 9110 and RFC 6585.")]
        public new abstract int Status { get; init; }

        [Description("Title error.")]
        public new abstract string Title { get; init; }

        [Description("Detailed error description.")]
        public new abstract string Detail { get; init; }

        [Description("URI with an error reference.")]
        public new abstract string Type { get; init; }

        [Description("Error instance ID.")]
        public new abstract string Instance { get; init; }
    }

    public class ProblemDetails
    {
        public string Type { get; set; } = "about:blank";
        public int Status { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
        public string Instance { get; set; } = string.Empty;
    }

    public class InternalError : ErrorDetails {

        private readonly IHttpContextAccessor _httpContextAccessor = new HttpContextAccessor();

        [DefaultValue("https://datatracker.ietf.org/doc/html/rfc9110#status.500")]
        public override string Type { get; init; }

        [DefaultValue(500)]
        public override int Status { get; init; }

        [DefaultValue("Internal Error")]
        public override string Title { get; init; }

        [DefaultValue("Something went wrong at our side")]
        public override string Detail { get; init; }

        [DefaultValue("/api/endpointPath/")]
        public override string Instance { get; init; }
        public InternalError()
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc9110#status.500";
            Status = StatusCodes.Status500InternalServerError;
            Title = "Internal error";
            Detail = "Something went wrong at our side";
            Instance = _httpContextAccessor.HttpContext?.Request.Path ?? "/unknown";
        }
    }
    public class BadRequest : ErrorDetails {

        private readonly IHttpContextAccessor _httpContextAccessor = new HttpContextAccessor();
        
        [DefaultValue("https://datatracker.ietf.org/doc/html/rfc9110#status.400")]
        public override string Type { get; init; }

        [DefaultValue(400)]
        public override int Status {get; init; }

        [DefaultValue("Bad Request")]
        public override string Title { get; init; }

        [DefaultValue("Request Malformed")]
        public override string Detail { get; init; }

        [DefaultValue("/api/endpointPath/")]
        public override string Instance { get; init; }

        public BadRequest(string detail)
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc9110#status.400";
            Status = StatusCodes.Status400BadRequest;
            Title = "Bad Request";
            Detail = detail ?? "Request Malformed";
            Instance = _httpContextAccessor.HttpContext?.Request.Path ?? "/unknown";
        }

    }

    public class NotFound : ErrorDetails {

        private readonly IHttpContextAccessor _httpContextAccessor = new HttpContextAccessor();

        [DefaultValue("https://datatracker.ietf.org/doc/html/rfc9110#status.404")]
        public override string Type { get; init; }

        [DefaultValue(404)]
        public override int Status {get; init; }

        [DefaultValue("Not Found")]
        public override string Title { get; init; }

        [DefaultValue("Couldn't find your requisition.")]
        public override string Detail { get; init; }

        [DefaultValue("/api/endpointPath/")]
        public override string Instance { get; init; }

        public NotFound( string detail)
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc9110#status.404";
            Status = StatusCodes.Status404NotFound;
            Title = "Not Found";
            Detail = detail ?? "We Couldn't Find Your Request.";
            Instance = _httpContextAccessor.HttpContext?.Request.Path ?? "/unknown";
        }

    }

    public class TooManyRequests : ErrorDetails {

        private readonly IHttpContextAccessor _httpContextAccessor = new HttpContextAccessor();

        [DefaultValue("https://datatracker.ietf.org/doc/html/rfc6585#section-4")]
        public override string Type { get; init; }

        [DefaultValue(429)]
        public override int Status {get; init; }

        [DefaultValue("Too Many Requests")]
        public override string Title { get; init; }

        [DefaultValue("Too many attemps, try again later.")]
        public override string Detail { get; init; }

        [DefaultValue("Unknown")]
        public override string Instance { get; init; }

        public TooManyRequests(string title, string detail)
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc6585#section-4";
            Status = StatusCodes.Status429TooManyRequests;
            Title = title ?? "Too Many Requests";
            Detail = detail ?? "You are under cooldown or rate limit, try again later.";
            Instance = _httpContextAccessor.HttpContext?.Request.Path ?? "/unknown";
        }
    }
}