using System;
using System.Net;
using StackExchange.Redis;

namespace Core.Önbellek
{
    public interface IRedisConnectionWrapper : IDisposable
    {
        IDatabase GetDatabase(int? db = null);
        IServer GetServer(EndPoint endPoint);
        EndPoint[] GetEndPoints();
        void FlushDatabase(int? db = null);
        bool PerformActionWithLock(string resource, TimeSpan expirationTime, Action action);
    }
}
