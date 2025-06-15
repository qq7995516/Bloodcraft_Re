using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodcraft_Re.LevelSystem;

/// <summary>
/// 等级系统工具类
/// 提供各种便捷的等级和经验相关功能
/// </summary>
public static class LevelingUtilities
{
    /// <summary>
    /// 格式化经验值显示
    /// </summary>
    /// <param name="experience">经验值</param>
    /// <returns>格式化后的字符串</returns>
    public static string FormatExperience(float experience)
    {
        if (experience >= 1000000)
            return $"{experience / 1000000:F1}M";
        if (experience >= 1000)
            return $"{experience / 1000:F1}K";
        return $"{experience:F0}";
    }

    /// <summary>
    /// 获取等级显示颜色（基于等级高低）
    /// </summary>
    /// <param name="level">等级</param>
    /// <returns>颜色代码字符串</returns>
    public static string GetLevelColor(int level)
    {
        return level switch
        {
            < 10 => "#CCCCCC",      // 灰色 - 新手
            < 25 => "#00FF00",      // 绿色 - 初级
            < 50 => "#0080FF",      // 蓝色 - 中级
            < 75 => "#8000FF",      // 紫色 - 高级
            < 90 => "#FF8000",      // 橙色 - 专家
            _ => "#FF0000"          // 红色 - 大师
        };
    }

    /// <summary>
    /// 获取等级称号
    /// </summary>
    /// <param name="level">等级</param>
    /// <returns>称号字符串</returns>
    public static string GetLevelTitle(int level)
    {
        return level switch
        {
            < 10 => "新手",
            < 25 => "学徒",
            < 50 => "冒险者",
            < 75 => "专家",
            < 90 => "大师",
            >= 90 when level < LevelingConfiguration.MaxPlayerLevel => "传奇",
            _ => "至尊"
        };
    }

    /// <summary>
    /// 创建等级进度条
    /// </summary>
    /// <param name="steamId">玩家SteamID</param>
    /// <param name="barLength">进度条长度（字符数）</param>
    /// <returns>进度条字符串</returns>
    public static string CreateProgressBar(ulong steamId, int barLength = 20)
    {
        int progress = LevelingSystem.GetPlayerLevelProgress(steamId);
        int filledLength = (int)Math.Round(barLength * (progress / 100.0));

        string filled = new('█', filledLength);
        string empty = new('░', barLength - filledLength);

        return $"[{filled}{empty}] {progress}%";
    }

    /// <summary>
    /// 获取完整的玩家等级信息
    /// </summary>
    /// <param name="steamId">玩家SteamID</param>
    /// <returns>格式化的等级信息字符串</returns>
    public static string GetPlayerLevelInfo(ulong steamId)
    {
        int level = LevelingSystem.GetPlayerLevel(steamId);
        float experience = LevelingSystem.GetPlayerExperience(steamId);
        int progress = LevelingSystem.GetPlayerLevelProgress(steamId);
        float toNextLevel = LevelingSystem.GetExperienceToNextLevel(steamId);

        string color = GetLevelColor(level);
        string title = GetLevelTitle(level);
        string progressBar = CreateProgressBar(steamId);

        if (LevelingSystem.IsPlayerMaxLevel(steamId))
        {
            return $"<color={color}>等级 {level}</color> ({title}) - 已达最大等级！\n" +
                   $"总经验: {FormatExperience(experience)}";
        }

        return $"<color={color}>等级 {level}</color> ({title})\n" +
               $"经验: {FormatExperience(experience)} | 下一级还需: {FormatExperience(toNextLevel)}\n" +
               $"进度: {progressBar}";
    }

    /// <summary>
    /// 验证等级范围
    /// </summary>
    /// <param name="level">等级</param>
    /// <returns>是否在有效范围内</returns>
    public static bool IsValidLevel(int level)
    {
        return level >= 0 && level <= LevelingConfiguration.MaxPlayerLevel;
    }

    /// <summary>
    /// 验证经验值
    /// </summary>
    /// <param name="experience">经验值</param>
    /// <returns>是否为有效经验值</returns>
    public static bool IsValidExperience(float experience)
    {
        return experience >= 0 && !float.IsNaN(experience) && !float.IsInfinity(experience);
    }

    /// <summary>
    /// 计算两个等级之间的总经验差
    /// </summary>
    /// <param name="fromLevel">起始等级</param>
    /// <param name="toLevel">目标等级</param>
    /// <returns>经验差值</returns>
    public static float CalculateExperienceDifference(int fromLevel, int toLevel)
    {
        if (!IsValidLevel(fromLevel) || !IsValidLevel(toLevel))
            return 0f;

        float fromExp = ExperienceCalculator.CalculateExperienceForLevel(fromLevel);
        float toExp = ExperienceCalculator.CalculateExperienceForLevel(toLevel);

        return Math.Abs(toExp - fromExp);
    }

    /// <summary>
    /// 估算升级所需时间（基于平均经验获取速度）
    /// </summary>
    /// <param name="steamId">玩家SteamID</param>
    /// <param name="averageExpPerHour">每小时平均经验获取</param>
    /// <returns>预估升级时间（小时）</returns>
    public static double EstimateTimeToNextLevel(ulong steamId, float averageExpPerHour)
    {
        if (averageExpPerHour <= 0 || LevelingSystem.IsPlayerMaxLevel(steamId))
            return 0;

        float expToNext = LevelingSystem.GetExperienceToNextLevel(steamId);
        return expToNext / averageExpPerHour;
    }
}