using System;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static ShockerKnight.Operations;

namespace ShockerKnight;

public abstract class PiShockAPI
{
    private bool _initialized = false;

    protected abstract Task<string> SendOperationAsync(BaseOperation operation, string nameExtra = "");

    public virtual async Task<string> Initialize()
    {
        _initialized = true;

        return string.Empty;
    }

    public virtual async Task<string> Dispose()
    {
        _initialized = false;

        return string.Empty;
    }

    public bool IsInitialized()
    {
        return _initialized;
    }

    private async Task<string> DoOperation(BaseOperation operation, string nameExtra = "")
    {
        if (!IsInitialized())
        {
            throw new InvalidOperationException("PiShockAPI was not initialized");
        }

        return await SendOperationAsync(operation, nameExtra);
    }

    public async Task<string> SendShockAsync(double duration, int intensity, string nameExtra = "")
    {
        return await DoOperation(new ShockOperation(duration, intensity), nameExtra);
    }

    public async Task<string> SendVibrationAsync(double duration, int intensity, string nameExtra = "")
    {
        return await DoOperation(new VibrationOperation(duration, intensity), nameExtra);
    }

    public async Task<string> SendBeepAsync(double duration, string nameExtra = "")
    {
        return await DoOperation(new BeepOperation(duration), nameExtra);
    }
}