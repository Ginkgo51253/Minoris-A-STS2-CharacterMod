namespace Minoris.MinorisCode.VfxSe;

public static class SeService
{
    static readonly Dictionary<string, AudioStream> StreamCache = new(StringComparer.OrdinalIgnoreCase);
    public static string SeRoot { get; set; } = $"res://{Minoris.MainFile.ModId}/se";

    static bool HasKnownAudioExtension(string key)
    {
        return key.EndsWith(".ogg", StringComparison.OrdinalIgnoreCase)
               || key.EndsWith(".wav", StringComparison.OrdinalIgnoreCase)
               || key.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase);
    }

    static string ResolveStreamPath(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Empty audio key");
        var k = key.Replace('\\', '/');
        if (k.StartsWith("res://", StringComparison.OrdinalIgnoreCase)) return k;
        if (HasKnownAudioExtension(k))
        {
            var p = $"{SeRoot}/{k}";
            return p;
        }
        var basePath = $"{SeRoot}/{k}";
        var ogg = basePath + ".ogg";
        if (ResourceLoader.Exists(ogg)) return ogg;
        var wav = basePath + ".wav";
        if (ResourceLoader.Exists(wav)) return wav;
        var mp3 = basePath + ".mp3";
        if (ResourceLoader.Exists(mp3)) return mp3;
        return ogg;
    }

    static AudioStream LoadStream(string path)
    {
        var resolved = ResolveStreamPath(path);
        if (StreamCache.TryGetValue(resolved, out var s)) return s;
        var res = ResourceLoader.Load<AudioStream>(resolved);
        if (res == null) throw new InvalidOperationException($"Audio stream not found: {resolved}");
        StreamCache[resolved] = res;
        return res;
    }

    public static AudioStreamPlayer Play(string streamPath, Node parent, float volumeDb = 0f, float pitchScale = 1f, string? bus = null, bool autoFree = true)
    {
        var stream = LoadStream(streamPath);
        var player = new AudioStreamPlayer
        {
            Stream = stream,
            VolumeDb = volumeDb,
            PitchScale = pitchScale,
            Bus = bus ?? "Master",
            Autoplay = false
        };
        parent.AddChild(player);
        if (autoFree)
        {
            player.Finished += () =>
            {
                if (GodotObject.IsInstanceValid(player)) player.QueueFree();
            };
        }
        player.Play();
        return player;
    }

    public static AudioStreamPlayer2D Play2D(string streamPath, Node parent, Vector2? position = null, float volumeDb = 0f, float pitchScale = 1f, string? bus = null, bool autoFree = true)
    {
        var stream = LoadStream(streamPath);
        var player = new AudioStreamPlayer2D
        {
            Stream = stream,
            VolumeDb = volumeDb,
            PitchScale = pitchScale,
            Bus = bus ?? "Master",
            Autoplay = false
        };
        if (position.HasValue) player.Position = position.Value;
        parent.AddChild(player);
        if (autoFree)
        {
            player.Finished += () =>
            {
                if (GodotObject.IsInstanceValid(player)) player.QueueFree();
            };
        }
        player.Play();
        return player;
    }

    public static AudioStreamPlayer PlayRandom(IReadOnlyList<string> streamPaths, Node parent, float volumeDb = 0f, float pitchScale = 1f, string? bus = null, bool autoFree = true, int? seed = null)
    {
        if (streamPaths == null || streamPaths.Count == 0) throw new ArgumentException("streamPaths empty");
        var rng = seed.HasValue ? new Random(seed.Value) : new Random();
        var idx = rng.Next(0, streamPaths.Count);
        return Play(streamPaths[idx], parent, volumeDb, pitchScale, bus, autoFree);
    }

    public static void Preload(params string[] streamPaths)
    {
        if (streamPaths == null) return;
        foreach (var p in streamPaths)
        {
            var resolved = ResolveStreamPath(p);
            if (!StreamCache.ContainsKey(resolved))
            {
                var s = ResourceLoader.Load<AudioStream>(resolved);
                if (s != null) StreamCache[resolved] = s;
            }
        }
    }
}
