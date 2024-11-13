using System;

namespace BuildingBlocks.Contracts.Exceptions;

public static class ExceptionFactory
{
    private static readonly int _probability;
    static ExceptionFactory()
        => _probability = Environment.GetEnvironmentVariable("EXCEPTION_PROBABILITY") is { } probability
            ? int.Parse(probability)
            : 0;


    public static void ProbablyThrow<TContext>()
        where TContext : class
    {
        if(_probability == 0)
        {
            return;
        }

        var value = Random.Shared.Next(0, 101);
        if(value < _probability)
        {
            throw new ApplicationException($"[RANDOM][EXCEPTION][{typeof(TContext).FullName}] A random exception was thrown");
        }
    }
}
