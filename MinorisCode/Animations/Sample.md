示例一：在某个父节点下播放一次性 VFX（播放完自动销毁）
using Minoris.MinorisCode.Animations;
using Godot;

public async void SpawnSlashVfx(Node parent, Vector2 at)
{
    var vfx = MinorisAnimationFactory.Play(
        scenePath: "res://Animations/SlashVfx.tscn",
        parent: parent,
        animationName: "attack",
        position: at,
        speedMultiplier: 1.0f,
        autoFree: true
    );

    vfx.EventFired += evt =>
    {
        if (evt == "hit")
        {
            // 命中帧触发的逻辑
        }
    };
}

示例二：绑定到角色 Skeleton2D 的骨骼（跟随骨骼移动）
using Minoris.MinorisCode.Animations;
using Godot;

public void AttachGlowToHand(Skeleton2D characterSkeleton)
{
    var vfx = MinorisAnimationFactory.PlayAtBone(
        scenePath: "res://Animations/HandGlow.tscn",
        host: characterSkeleton,
        boneName: "hand.R",
        animationName: "glow_loop",
        speedMultiplier: 1.0f,
        autoFree: false
    );

    vfx.EventFired += evt =>
    {
        if (evt == "pulse")
        {
            // 发光脉冲事件
        }
    };
}

第一点：核心能力

- 基于 Node2D 的动画节点 MinorisAnimation2D：
  - 导出属性绑定 AnimationPlayer 和 Skeleton2D。
  - Play、PlayOnceThenFree、Stop 控制动画。
  - 支持 Engine.TimeScale 同步（UseEngineTimeScale 可切换）。
  - 支持速度倍率（BaseSpeed 与 SpeedMultiplier）。
  - 动画结束触发 AnimationFinished；事件轨道调用 OnEvent 触发 EventFired。
- 工厂与加载 MinorisAnimationFactory：
  - 从场景路径加载 PackedScene 并缓存。
  - Play 在任意父节点下实例化并播放，支持设置位置、ZIndex、速度倍率、播放完自动释放。
  - PlayAtBone 将动画实例挂到指定 Skeleton2D 的某个骨骼（BoneAttachment2D），适合跟随角色骨骼的光效/特效。
如何在其他代码中使用

- 直接在你的游戏逻辑中按需播放动画（稍后再接入；此处只给出用法示例，不修改其他文件）：

第二点：如何在 Godot 中创建这种动画

- 场景结构建议（保存到 res://Animations/...）：
  - 根节点：Node2D（命名随意，例如 SlashVfx）
  - 添加子节点：Skeleton2D（建立骨骼、层级）
  - 在骨骼下添加 Sprite2D/Polygon2D/Particles2D 等作为显示部件
  - 添加子节点：AnimationPlayer（用于关键帧与事件）
  - 将根节点脚本设置为 MinorisAnimation2D.cs
  - 在 Inspector 中把根脚本的 Anim 指向 AnimationPlayer，Skeleton 指向 Skeleton2D
- 动画与事件：
  - 创建动画条目（例如 attack、glow_loop）
  - 在 AnimationPlayer 里给根节点添加“方法调用轨道”，在命中帧调用 OnEvent，并传入字符串参数（例如 "hit"）
  - 若是一次性动画，调用端用 PlayOnceThenFree 或者在实例化时把 autoFree 设为 true
- 时序与节奏：
  - 若要跟随游戏全局时间缩放，保持 UseEngineTimeScale 为 true（默认开启）
  - 需要调整快慢时，设置 BaseSpeed 或在代码里设置 SpeedMultiplier
推荐的资源组织

- 代码：已在 Minoris/MinorisCode/Animations 下就绪
- 动画场景与贴图资源：建议放在 Minoris/Animations 下（res://Animations/...）
  - 例如 res://Animations/SlashVfx.tscn、res://Animations/HandGlow.tscn
  - 场景根节点脚本设为 MinorisAnimation2D，导出字段指好 AnimationPlayer 和 Skeleton2D
事件桥接规范

- 在动画需要对齐逻辑触发点时，在 AnimationPlayer 的方法调用轨道里调用根节点方法 OnEvent，并传入具体事件名：
  - 命中帧："hit"
  - 生成/消失："spawn"/"despawn"
  - 自定义事件："pulse"、"section_end" 等
- 运行时在 vfx.EventFired 订阅这些事件即可完成逻辑对齐
后续可选动作

- 如果你希望，我可以再提供一个最小示例场景的骨架说明（骨骼层级、关键帧设置、方法轨道截图说明）或添加一个模板 .tscn 文件，便于直接拷贝复用。
- 当前没有在其他代码中接线；等你确认路径与场景命名后，我可以帮你把某个 Power/Card 的演出接上这套系统。
说明

- 本次只在 Animations 文件夹中新增了两个 C# 文件，未在其他模块做任何引用或改动。
- 目前工程未配置统一的 lint/typecheck 命令；如需我在本地尝试编译或跑特定脚本，请告知你的标准构建命令。