﻿namespace StudentInfoSystemApp.Application.Exceptions
{
    public class CustomException : ApplicationException
    {
        public int StatusCode { get; set; }
        public string? Message {  get; set; }
        public Dictionary<string, string>? Errors { get; set; } = new();
        public CustomException(int code, string message)
        {
            StatusCode = code;
            Message = message;
        }
        public CustomException(string errorKey,string errorMessage)
        {
            Errors.Add(errorKey, errorMessage);
        }
        public CustomException(int statusCode, string errorKey,string errorMessage,string message="")
        {
            StatusCode=statusCode;
            Message = message;
            Errors.Add(errorKey,errorMessage);
        }
    }
}