﻿namespace Linkdev.TeamTrack.Core.Responses
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = default!;
        public List<string>? Errors { get; set; }
    }
}
