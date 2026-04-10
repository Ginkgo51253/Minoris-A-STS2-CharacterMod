VFX 功能

- 资源缓存：按路径缓存 PackedScene，避免重复加载。
- 场景播放：在任意父节点下实例化 VFX 场景，支持设置位置、ZIndex、速度倍率、播放完自动释放。
- 动画驱动优先级：
  - 场景中若存在 MinorisAnimation2D（Animations 文件夹提供）优先调用它进行播放与回收。
  - 否则回退到 AnimationPlayer，设置 SpeedScale 并在动画结束时回收。
- 骨骼挂点：支持绑定到 Skeleton2D 指定 BoneName（BoneAttachment2D），动画结束后自动清理。
使用方法示例（只展示用法，不改其他文件）

- 在父节点下播放一次性 VFX
using Godot;
using Minoris.MinorisCode.VfxSe;

public void SpawnSlash(Node parent, Vector2 pos)
{
    VfxService.Play(
        scenePath: "res://Animations/SlashVfx.tscn",
        parent: parent,
        animationName: "attack",
        position: pos,
        zIndex: 100,
        speedMultiplier: 1.0f,
        autoFree: true
    );
}

- 将 VFX 绑定到角色骨骼
using Godot;
using Minoris.MinorisCode.VfxSe;

public void AttachGlow(Skeleton2D skel)
{
    VfxService.PlayAtBone(
        scenePath: "res://Animations/HandGlow.tscn",
        host: skel,
        boneName: "hand.R",
        animationName: "glow_loop",
        speedMultiplier: 1.0f,
        autoFree: false
    );
}

- 动画（MinorisAnimationFactory，驱动 MinorisAnimation2D 或场景内 AnimationPlayer）
using Godot;
using Minoris.MinorisCode.Animations;

public void PlayAnim(Node parent, Vector2 where)
{
    MinorisAnimationFactory.Play(
        scenePath: "SlashVfx",      // -> res://Minoris/animations/SlashVfx.tscn
        parent: parent,
        animationName: "attack",
        position: where,
        speedMultiplier: 1.0f,
        autoFree: true
    );
}

- VFX（VfxService，场景根为 Node2D）
using Godot;
using Minoris.MinorisCode.VfxSe;

public void SpawnVfx(Node parent, Vector2 where)
{
    VfxService.Play(
        scenePath: "BigSlash",      // -> res://Minoris/vfx/BigSlash.tscn
        parent: parent,
        animationName: "play",
        position: where,
        zIndex: 200,
        speedMultiplier: 1.0f,
        autoFree: true
    );
}

- 适配 Animations 体系
  - 若你的 VFX 场景根或子节点已有 MinorisAnimation2D（Animations 文件夹提供的脚本），VfxService 会优先使用它进行 Play、速度倍率与回收；否则自动回退到 AnimationPlayer。
  - 这使 VFX&SE 与 Animations 可无缝协作。
SE 功能

- 资源缓存：按路径缓存 AudioStream 资源。
- UI 音效播放（非位置）
using Godot;
using Minoris.MinorisCode.VfxSe;

public void PlayClick(Node parent)
{
    SeService.Play(
        streamPath: "click",        // -> res://Minoris/se/click.ogg/.wav/.mp3
        parent: parent,
        volumeDb: -4f,
        pitchScale: 1.0f,
        bus: "Master",
        autoFree: true
    );
}

- 2D 定位音效播放（位置型）
using Godot;
using Minoris.MinorisCode.VfxSe;

public void PlayImpact(Node parent, Vector2 pos)
{
    SeService.Play2D(
        streamPath: "res://audio/vfx/impact.ogg",
        parent: parent,
        position: pos,
        volumeDb: 0f,
        pitchScale: 1.0f,
        bus: "Master",
        autoFree: true
    );
}

- 随机音效组
using Godot;
using Minoris.MinorisCode.VfxSe;

public void PlayRandomSwing(Node parent)
{
    SeService.PlayRandom(
        new[] {
            "res://audio/swing/swing1.ogg",
            "res://audio/swing/swing2.ogg",
            "res://audio/swing/swing3.ogg"
        },
        parent: parent,
        volumeDb: -2f,
        pitchScale: 1.0f,
        bus: "Master",
        autoFree: true
    );
}

场景与资产组织建议

- VFX 场景：推荐保存在 res://Animations 或 res://Vfx 下，根节点为 Node2D，内部包含 Skeleton2D + AnimationPlayer，或使用 MinorisAnimation2D 以获得事件与时间缩放支持。
- 音效资源：保存在 res://audio/ui、res://audio/vfx 等子目录；可先用 SeService.Preload(paths...) 进行缓存预热。
与现有动画体系的衔接

- 若你的 VFX 场景内部使用 MinorisAnimation2D，则 VfxService 会自动调用其 Play、速度倍率与 AutoFreeOnFinish，无需额外代码。
- 若没有 MinorisAnimation2D，则确保场景中包含 AnimationPlayer，并提供匹配的动画名以便自动回收。
如需，我可以再提供一个最小的 VFX 场景模板（Node2D + Skeleton2D + AnimationPlayer）与一个 UI/位置音效的资源布局示例，方便你直接拷贝使用。


VfxService.Play("attacks/slash_heavy", parent, animationName: "play");
// -> res://Minoris/vfx/attacks/slash_heavy.tscn

SeService.Play("ui/confirm", parent);
// -> res://Minoris/se/ui/confirm.ogg/.wav/.mp3