using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodcraft_Re.LevelSystem;

/// <summary>
/// 等级系统配置类
/// 包含所有等级相关的配置参数
/// </summary>
public static class LevelingConfiguration
{
    /// <summary>
    /// 最大玩家等级
    /// </summary>
    public static int MaxPlayerLevel { get; set; } = 100;

    /// <summary>
    /// 基础经验倍率
    /// </summary>
    public static float BaseExperienceMultiplier { get; set; } = 1.0f;

    /// <summary>
    /// 组队经验倍率
    /// </summary>
    public static float GroupExperienceMultiplier { get; set; } = 1.2f;

    /// <summary>
    /// VBlood击杀经验倍率
    /// </summary>
    public static float VBloodExperienceMultiplier { get; set; } = 5.0f;

    /// <summary>
    /// 普通单位击杀经验倍率
    /// </summary>
    public static float UnitExperienceMultiplier { get; set; } = 1.0f;

    /// <summary>
    /// 等级差异缩放因子
    /// 用于调整击杀低等级敌人时的经验获取
    /// </summary>
    public static float LevelScalingFactor { get; set; } = 0.1f;

    /// <summary>
    /// 等级升级时是否显示特效
    /// </summary>
    public static bool ShowLevelUpEffects { get; set; } = true;

    /// <summary>
    /// 是否启用经验日志显示
    /// </summary>
    public static bool ShowExperienceLog { get; set; } = true;

    /// <summary>
    /// 是否启用滚动战斗文本
    /// </summary>
    public static bool ShowScrollingCombatText { get; set; } = true;
}