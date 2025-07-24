namespace Football247.SignalR
{
    public class ConnectionMapping<T>
    {
        private readonly Dictionary<T, HashSet<string>> _connections = new();
        private readonly object _lock = new();

        public void Add(T key, string connectionId)
        {
            lock (_lock)
            {
                if (!_connections.TryGetValue(key, out var connections))
                {
                    connections = new HashSet<string>();
                    _connections[key] = connections;
                }

                connections.Add(connectionId);
            }
        }

        public IEnumerable<string> GetConnections(T key)
        {
            lock (_lock)
            {
                return _connections.TryGetValue(key, out var connections) ? connections.ToList() : Enumerable.Empty<string>();
            }
        }

        public void Remove(T key, string connectionId)
        {
            lock (_lock)
            {
                if (!_connections.TryGetValue(key, out var connections)) return;

                connections.Remove(connectionId);
                if (connections.Count == 0)
                {
                    _connections.Remove(key);
                }
            }
        }
    }

}
