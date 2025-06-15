using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodcraft_Re.LevelSystem;
/// <summary>
/// 经验计算器
/// 负责所有与经验计算相关的逻辑
/// </summary>
public static class ExperienceCalculator
{
    /// <summary>
    /// 每级所需经验的基础值
    /// </summary>
    private const float BASE_EXPERIENCE_PER_LEVEL = 100f;

    /// <summary>
    /// 经验增长指数
    /// </summary>
    private const float EXPERIENCE_GROWTH_FACTOR = 1.1f;

    /// <summary>
    /// 计算指定等级所需的总经验值
    /// </summary>
    /// <param name="level">目标等级</param>
    /// <returns>达到该等级所需的总经验值</returns>
    public static float CalculateExperienceForLevel(int level)
    {
        if (level <= 0) return 0f;

        float totalExperience = 0f;

        // 累计计算每级所需经验
        for (int i = 1; i <= level; i++)
        {
            totalExperience += BASE_EXPERIENCE_PER_LEVEL * MathF.Pow(EXPERIENCE_GROWTH_FACTOR, i - 1);
        }

        return totalExperience;
    }

    /// <summary>
    /// 根据总经验值计算对应的等级
    /// </summary>
    /// <param name="totalExperience">总经验值</param>
    /// <returns>对应的等级</returns>
    public static int CalculateLevelFromExperience(float totalExperience)
    {
        if (totalExperience <= 0f) return 0;

        int level = 0;
        float experienceForCurrentLevel = 0f;

        // 逐级计算直到找到对应等级
        while (experienceForCurrentLevel <= totalExperience && level < LevelingConfiguration.MaxPlayerLevel)
        {
            level++;
            experienceForCurrentLevel = CalculateExperienceForLevel(level);
        }

        return Math.Max(0, level - 1);
    }

    /// <summary>
    /// 计算当前等级的进度百分比
    /// </summary>
    /// <param name="currentExperience">当前总经验值</param>
    /// <returns>当前等级进度百分比 (0-100)</returns>
    public static int CalculateLevelProgress(float currentExperience)
    {
        int currentLevel = CalculateLevelFromExperience(currentExperience);

        if (currentLevel >= LevelingConfiguration.MaxPlayerLevel)
            return 100;

        float currentLevelExperience = CalculateExperienceForLevel(currentLevel);
        float nextLevelExperience = CalculateExperienceForLevel(currentLevel + 1);

        float experienceInCurrentLevel = currentExperience - currentLevelExperience;
        float experienceNeededForNextLevel = nextLevelExperience - currentLevelExperience;

        if (experienceNeededForNextLevel <= 0) return 100;

        return (int)Math.Floor(experienceInCurrentLevel / experienceNeededForNextLevel * 100);
    }

    /// <summary>
    /// 计算基础经验获取量
    /// </summary>
    /// <param name="victimLevel">受害者等级</param>
    /// <param name="victimHealth">受害者血量</param>
    /// <param name="isVBlood">是否为VBlood</param>
    /// <returns>基础经验值</returns>
    public static float CalculateBaseExperience(int victimLevel, float victimHealth, bool isVBlood)
    {
        // 基础经验 = 受害者等级 + 血量加成
        float baseExperience = victimLevel * LevelingConfiguration.BaseExperienceMultiplier;
        float healthBonus = victimHealth / 10f; // 血量加成

        baseExperience += healthBonus;

        // VBlood额外倍率
        if (isVBlood)
        {
            baseExperience *= LevelingConfiguration.VBloodExperienceMultiplier;
        }
        else
        {
            baseExperience *= LevelingConfiguration.UnitExperienceMultiplier;
        }

        return Math.Max(1f, baseExperience);
    }

    /// <summary>
    /// 应用等级差异缩放
    /// 击杀低等级敌人时减少经验获取
    /// </summary>
    /// <param name="baseExperience">基础经验值</param>
    /// <param name="playerLevel">玩家等级</param>
    /// <param name="victimLevel">受害者等级</param>
    /// <returns>缩放后的经验值</returns>
    public static float ApplyLevelScaling(float baseExperience, int playerLevel, int victimLevel)
    {
        int levelDifference = playerLevel - victimLevel;

        if (levelDifference <= 0)
            return baseExperience; // 击杀同级或高级敌人，无惩罚

        // 等级差异惩罚计算
        float scalingFactor = MathF.Exp(-LevelingConfiguration.LevelScalingFactor * levelDifference);

        return baseExperience * scalingFactor;
    }

    /// <summary>
    /// 应用组队经验倍率
    /// </summary>
    /// <param name="baseExperience">基础经验值</param>
    /// <param name="isInGroup">是否在组队中</param>
    /// <returns>应用倍率后的经验值</returns>
    public static float ApplyGroupMultiplier(float baseExperience, bool isInGroup)
    {
        if (isInGroup)
            return baseExperience * LevelingConfiguration.GroupExperienceMultiplier;

        return baseExperience;
    }
}
