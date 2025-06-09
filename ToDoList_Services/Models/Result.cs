﻿namespace ToDoList_Services.Models;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public bool IsError { get { return !IsSuccess; } }
    public bool IsValidationError { get { return ResultType == EnumResultType.ValidationError; } }
    public bool IsNotFoundError { get { return ResultType == EnumResultType.NotFoundError; } }
    private EnumResultType ResultType { get; set; }
    public T? Data { get; private set; }
    public string? Message { get; private set; }

    private Result(bool isSuccess, T? data, string? message)
    {
        IsSuccess = isSuccess;
        Data = data;
        Message = message;
    }

    public static Result<T> Success(T data, string? message = "Operation Successful.")
    {
        var item = new Result<T>(true, data, message);
        item.ResultType = EnumResultType.Success;
        return item;
    }

    public static Result<T> Failure(string errorMessage)
    {
        var item = new Result<T>(false, default, errorMessage);
        item.ResultType = EnumResultType.Failure;
        return item;
    }

    public static Result<T> ValidationError(string errorMessage)
    {
        var item = new Result<T>(false, default, errorMessage);
        item.ResultType = EnumResultType.ValidationError;
        return item;
    }

    public static Result<T> NotFoundError(string errorMessage = "Data not found")
    {
        var item = new Result<T>(false, default, errorMessage);
        item.ResultType = EnumResultType.NotFoundError;
        return item;
    }

}