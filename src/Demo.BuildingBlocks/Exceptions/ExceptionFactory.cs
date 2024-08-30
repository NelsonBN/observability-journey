namespace BuildingBlocks.Exceptions;

public static class ExceptionFactory
{
    public static void ProbablyThrow<TContext>(int probability)
        where TContext : class
    {
        var value = Random.Shared.Next(0, 101);
        if(value < probability)
        {
            throw new ApplicationException($"[RANDOM][EXCEPTION][{typeof(TContext).FullName}] A random exception was thrown");
        }
    }
}
