using System.Collections.Concurrent;

namespace QaSecondBrain.TestKit.Fakes;

/// <summary>
/// Returns canned JSON per agent from <c>tests/fixtures/llm/&lt;agent&gt;.json</c>. Never calls a real model.
/// Records every call so tests can assert on prompts.
/// </summary>
public sealed class FakeLlmClient(string? fixtureDirectory = null)
{
    private readonly string _dir = fixtureDirectory ?? FixturePaths.Llm;
    private readonly ConcurrentQueue<FakeLlmCall> _calls = new();
    private readonly ConcurrentDictionary<string, string> _overrides = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyCollection<FakeLlmCall> Calls => _calls.ToArray();

    /// <summary>Replace the fixture for one agent inside a single test.</summary>
    public FakeLlmClient With(string agent, string json)
    {
        _overrides[agent] = json;
        return this;
    }

    public async Task<string> CompleteAsync(string agent, string prompt, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        _calls.Enqueue(new FakeLlmCall(agent, prompt));

        if (_overrides.TryGetValue(agent, out var json))
        {
            return json;
        }

        var path = Path.Combine(_dir, agent + ".json");
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"No LLM fixture for agent '{agent}'", path);
        }

        return await File.ReadAllTextAsync(path, ct);
    }
}

public sealed record FakeLlmCall(string Agent, string Prompt);
