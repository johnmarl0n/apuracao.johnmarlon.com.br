using System;
using System.Collections.Concurrent;

namespace apuracao.johnmarlon.com.br.Services
{
    public class OnlinePresenceService
    {
        private static readonly ConcurrentDictionary<string, DateTime> _activeSessions = new();
        private const int MaxSessoesEmMemoria = 15000;
        private static DateTime _ultimaLimpeza = DateTime.UtcNow;
        private static readonly object _cleanupLock = new();

        public void Ping(string connectionId)
        {
            if (string.IsNullOrWhiteSpace(connectionId) || connectionId.Length > 64) return;

            // Previne estouro de memória sob ataque volumétrico
            if (_activeSessions.Count >= MaxSessoesEmMemoria && !_activeSessions.ContainsKey(connectionId))
            {
                return;
            }

            _activeSessions[connectionId] = DateTime.UtcNow;
            VerificarLimpezaPeriodica();
        }

        public void Disconnect(string connectionId)
        {
            if (string.IsNullOrWhiteSpace(connectionId) || connectionId.Length > 64) return;
            _activeSessions.TryRemove(connectionId, out _);
        }

        public int GetOnlineCount()
        {
            VerificarLimpezaPeriodica();
            return Math.Max(1, _activeSessions.Count);
        }

        private void VerificarLimpezaPeriodica()
        {
            // Executa limpeza em lote a cada 15 segundos no máximo, evitando overhead O(N) por ping
            if (DateTime.UtcNow - _ultimaLimpeza < TimeSpan.FromSeconds(15)) return;

            if (System.Threading.Monitor.TryEnter(_cleanupLock))
            {
                try
                {
                    if (DateTime.UtcNow - _ultimaLimpeza < TimeSpan.FromSeconds(15)) return;
                    _ultimaLimpeza = DateTime.UtcNow;

                    var cutoff = DateTime.UtcNow.AddSeconds(-35);
                    foreach (var kvp in _activeSessions)
                    {
                        if (kvp.Value < cutoff)
                        {
                            _activeSessions.TryRemove(kvp.Key, out _);
                        }
                    }
                }
                finally
                {
                    System.Threading.Monitor.Exit(_cleanupLock);
                }
            }
        }
    }
}
