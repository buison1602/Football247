using Microsoft.AspNetCore.Mvc;

namespace Shared.Response
{
    public class MethodResult
    {
        public bool IsSuccess { get; set; } = true; 
        public int StatusCode { get; set; } = 200;  
        public string? ErrorMessage { get; set; }
        public string? ErrorField { get; set; }
        public object? ErrorValue { get; set; }

        public MethodResult()
        {
        }

        public MethodResult(bool isSuccess, string? errorMessage)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        public void AddError(string v, int statusCode, string errorMessage)
        {
            IsSuccess = false;
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }

        public void AddError(int statusCode, string errorCodeOrMessage, string fieldName)
        {
            IsSuccess = false;
            StatusCode = statusCode;
            ErrorMessage = errorCodeOrMessage; 
            ErrorField = fieldName;            
        }

        public void AddError(int statusCode, string errorCodeOrMessage, string fieldName, object? fieldValue = null)
        {
            IsSuccess = false;
            StatusCode = statusCode;
            ErrorMessage = errorCodeOrMessage;
            ErrorField = fieldName;
            if (fieldValue != null)
            {
                ErrorValue = fieldValue;
            }
        }

        public IActionResult GetActionResult()
        {
            if (IsSuccess)
            {
                StatusCode = 200;
            }
            else if (StatusCode == 200) 
            {
                StatusCode = 500;
            }

            ObjectResult objectResult = new ObjectResult(this);
            objectResult.StatusCode = StatusCode;

            return objectResult;
        }

    }

    public class MethodResult<T> : MethodResult
    {
        public T? Result { get; set; }

        public MethodResult() : base()
        {
        }

        public MethodResult(T? result) : base()
        {
            Result = result;
        }
    }
}
