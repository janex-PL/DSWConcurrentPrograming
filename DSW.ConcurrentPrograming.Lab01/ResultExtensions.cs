using FluentResults;

namespace DSW.ConcurrentPrograming.Lab01;

public static class ResultExtensions
{
    public static string GetLogMessage(this Result result) => string.Join(';', result.Reasons.Select(x => x.Message));
}