namespace Minoris.MinorisCode.Animations;

public static class MinorisAnimationFactory
{
    static readonly Dictionary<string, PackedScene> Cache = new(StringComparer.OrdinalIgnoreCase);

    public static string AnimationsRoot { get; set; } = $"res://{Minoris.MainFile.ModId}/animations";

    static string ResolveScenePath(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Empty scene key");
        var k = key.Replace('\\', '/');
        if (k.StartsWith("res://", StringComparison.OrdinalIgnoreCase)) return k;
        if (k.EndsWith(".tscn", StringComparison.OrdinalIgnoreCase)) return $"{AnimationsRoot}/{k}";
        return $"{AnimationsRoot}/{k}.tscn";
    }

    static PackedScene LoadScene(string path)
    {
        var resolved = ResolveScenePath(path);
        if (Cache.TryGetValue(resolved, out var s)) return s;
        var res = ResourceLoader.Load<PackedScene>(resolved);
        if (res == null) throw new InvalidOperationException($"Scene not found: {resolved}");
        Cache[resolved] = res;
        return res;
    }

    static MinorisAnimation2D? FindVfx(Node node)
    {
        if (node is MinorisAnimation2D m) return m;
        foreach (var child in node.GetChildren())
        {
            if (child is Node n)
            {
                var found = FindVfx(n);
                if (found != null) return found;
            }
        }
        return null;
    }

    public static MinorisAnimation2D Instance(string scenePath)
    {
        var scene = LoadScene(scenePath);
        var node = scene.Instantiate();
        if (node is MinorisAnimation2D m1) return m1;
        if (node is Node n)
        {
            var m2 = FindVfx(n);
            if (m2 != null) return m2;
        }
        throw new InvalidOperationException($"Scene does not contain MinorisAnimation2D: {scenePath}");
    }

    public static MinorisAnimation2D Play(string scenePath, Node parent, string animationName, Vector2? position = null, float speedMultiplier = 1f, bool autoFree = false, int? zIndex = null)
    {
        var vfx = Instance(scenePath);
        if (vfx is Node2D n2d)
        {
            if (position.HasValue) n2d.Position = position.Value;
            if (zIndex.HasValue) n2d.ZIndex = zIndex.Value;
        }
        parent.AddChild(vfx);
        vfx.SetSpeedMultiplier(speedMultiplier);
        vfx.AutoFreeOnFinish = autoFree;
        vfx.Play(animationName);
        return vfx;
    }

    public static MinorisAnimation2D PlayAtBone(string scenePath, Skeleton2D host, string boneName, string animationName, float speedMultiplier = 1f, bool autoFree = true)
    {
        var vfx = Instance(scenePath);
        var bone = FindBoneByName(host, boneName) ?? throw new InvalidOperationException($"Bone not found: {boneName}");
        var attach = new Node2D();
        bone.AddChild(attach);
        attach.AddChild(vfx);
        vfx.SetSpeedMultiplier(speedMultiplier);
        vfx.AutoFreeOnFinish = autoFree;
        vfx.Play(animationName);
        return vfx;
    }

    static Bone2D? FindBoneByName(Skeleton2D skeleton, string name)
    {
        for (int i = 0; i < skeleton.GetBoneCount(); i++)
        {
            var b = skeleton.GetBone(i);
            if (string.Equals(b.Name, name, StringComparison.OrdinalIgnoreCase)) return b;
        }
        return null;
    }
}
