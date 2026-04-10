namespace Minoris.MinorisCode.Animations;

public partial class MinorisAnimation2D : Node2D
{
    [Export] public AnimationPlayer? Anim { get; set; }
    [Export] public Skeleton2D? Skeleton { get; set; }
    [Export] public bool AutoFreeOnFinish { get; set; }
    [Export] public bool UseEngineTimeScale { get; set; } = true;
    [Export] public float BaseSpeed { get; set; } = 1f;

    public float SpeedMultiplier { get; set; } = 1f;

    public event Action<string>? EventFired;
    public event Action<string>? AnimationFinished;

    public override void _Ready()
    {
        if (Anim != null)
        {
            Anim.AnimationFinished += OnAnimFinished;
        }
    }

    public override void _ExitTree()
    {
        if (Anim != null)
        {
            Anim.AnimationFinished -= OnAnimFinished;
        }
    }

    public override void _Process(double delta)
    {
        if (Anim != null)
        {
            var t = UseEngineTimeScale ? Engine.TimeScale : 1f;
            Anim.SpeedScale = BaseSpeed * SpeedMultiplier * (float)t;
        }
    }

    public void Play(string animationName)
    {
        if (Anim != null)
        {
            Anim.Play(animationName);
        }
    }

    public void PlayOnceThenFree(string animationName)
    {
        AutoFreeOnFinish = true;
        Play(animationName);
    }

    public void Stop()
    {
        if (Anim != null) Anim.Stop();
    }

    void OnAnimFinished(StringName name)
    {
        AnimationFinished?.Invoke(name.ToString());
        if (AutoFreeOnFinish) QueueFree();
    }

    public void OnEvent(string evt)
    {
        EventFired?.Invoke(evt);
    }

    public void SetSpeedMultiplier(float value)
    {
        SpeedMultiplier = value;
    }
}

