using System;
using System.Collections.Concurrent;

namespace apuracao.johnmarlon.com.br.Services
{
    public class OnlinePresenceService
    {
        private static readonly ConcurrentDictionary<string, DateTime> _activeSessions = new();

        public void Ping(string connectionId)
        {
            if (string.IsNullOrWhiteSpace(connectionId)) return;
            _activeSessions[connectionId] = DateTime.UtcNow;
            Cleanup();
        }

        public void Disconnect(string connectionId)
        {
            if (string.IsNullOrWhiteSpace(connectionId)) return;
            _activeSessions.TryRemove(connectionId, out _);
        }

        public int GetOnlineCount()
        {
            Cleanup();
            return Math.Max(1, _activeSessions.Count);
        }

        private void Cleanup()
        {
            var cutoff = DateTime.UtcNow.AddSeconds(-35);
            foreach (var kvp in _activeSessions)
            {
                if (kvp.Value < cutoff)
                {
                    _activeSessions.TryRemove(kvp.Key, out _);
                }
            }
        }
    }
}
