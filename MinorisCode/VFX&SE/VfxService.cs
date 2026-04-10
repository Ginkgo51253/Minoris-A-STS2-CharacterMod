namespace Minoris.MinorisCode.VfxSe;

public static class VfxService
{
    static readonly Dictionary<string, PackedScene> SceneCache = new(StringComparer.OrdinalIgnoreCase);
    public static string VfxRoot { get; set; } = $"res://{Minoris.MainFile.ModId}/vfx";

    static string ResolveScenePath(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Empty scene key");
        var k = key.Replace('\\', '/');
        if (k.StartsWith("res://", StringComparison.OrdinalIgnoreCase)) return k;
        if (k.EndsWith(".tscn", StringComparison.OrdinalIgnoreCase)) return $"{VfxRoot}/{k}";
        return $"{VfxRoot}/{k}.tscn";
    }

    static PackedScene LoadScene(string path)
    {
        var resolved = ResolveScenePath(path);
        if (SceneCache.TryGetValue(resolved, out var s)) return s;
        var res = ResourceLoader.Load<PackedScene>(resolved);
        if (res == null) throw new InvalidOperationException($"Scene not found: {resolved}");
        SceneCache[resolved] = res;
        return res;
    }

    static T? FindNodeOfType<T>(Node node) where T : class
    {
        if (node is T t) return t;
        foreach (var child in node.GetChildren())
        {
            if (child is Node n)
            {
                var found = FindNodeOfType<T>(n);
                if (found != null) return found;
            }
        }
        return null;
    }

    public static Node2D Play(string scenePath, Node parent, string? animationName = null, Vector2? position = null, int? zIndex = null, float speedMultiplier = 1f, bool autoFree = true)
    {
        var scene = LoadScene(scenePath);
        var inst = scene.Instantiate();
        if (inst is not Node2D n2d) throw new InvalidOperationException("VFX scene root must be Node2D");
        if (position.HasValue) n2d.Position = position.Value;
        if (zIndex.HasValue) n2d.ZIndex = zIndex.Value;
        parent.AddChild(n2d);

        var minorisAnim = FindNodeOfType<MinorisAnimation2D>(n2d);
        if (minorisAnim != null)
        {
            minorisAnim.SetSpeedMultiplier(speedMultiplier);
            minorisAnim.AutoFreeOnFinish = autoFree;
            if (!string.IsNullOrEmpty(animationName)) minorisAnim.Play(animationName);
            return n2d;
        }

        var anim = FindNodeOfType<AnimationPlayer>(n2d);
        if (anim != null)
        {
            anim.SpeedScale = speedMultiplier;
            if (autoFree)
            {
                anim.AnimationFinished += (StringName _) => n2d.QueueFree();
            }
            if (!string.IsNullOrEmpty(animationName)) anim.Play(animationName);
            return n2d;
        }

        if (autoFree)
        {
            n2d.QueueFree();
            throw new InvalidOperationException("VFX scene has no AnimationPlayer or MinorisAnimation2D to drive playback");
        }
        return n2d;
    }

    public static Node2D PlayAtBone(string scenePath, Skeleton2D host, string boneName, string? animationName = null, float speedMultiplier = 1f, bool autoFree = true)
    {
        var scene = LoadScene(scenePath);
        var inst = scene.Instantiate();
        if (inst is not Node2D n2d) throw new InvalidOperationException("VFX scene root must be Node2D");
        var bone = FindBoneByName(host, boneName) ?? throw new InvalidOperationException($"Bone not found: {boneName}");
        var attach = new Node2D();
        bone.AddChild(attach);
        attach.AddChild(n2d);

        var minorisAnim = FindNodeOfType<MinorisAnimation2D>(n2d);
        if (minorisAnim != null)
        {
            minorisAnim.SetSpeedMultiplier(speedMultiplier);
            minorisAnim.AutoFreeOnFinish = autoFree;
            if (autoFree)
            {
                minorisAnim.AnimationFinished += _ =>
                {
                    if (IsValidNode(n2d)) n2d.QueueFree();
                    if (IsValidNode(attach)) attach.QueueFree();
                };
            }
            if (!string.IsNullOrEmpty(animationName)) minorisAnim.Play(animationName);
            return n2d;
        }

        var anim = FindNodeOfType<AnimationPlayer>(n2d);
        if (anim != null)
        {
            anim.SpeedScale = speedMultiplier;
            if (autoFree)
            {
                anim.AnimationFinished += (StringName _) =>
                {
                    if (IsValidNode(n2d)) n2d.QueueFree();
                    if (IsValidNode(attach)) attach.QueueFree();
                };
            }
            if (!string.IsNullOrEmpty(animationName)) anim.Play(animationName);
            return n2d;
        }

        if (autoFree)
        {
            if (IsValidNode(n2d)) n2d.QueueFree();
            if (IsValidNode(attach)) attach.QueueFree();
            throw new InvalidOperationException("VFX scene has no AnimationPlayer or MinorisAnimation2D to drive playback");
        }
        return n2d;
    }

    static bool IsValidNode(Node? n)
    {
        return n != null && GodotObject.IsInstanceValid(n);
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
