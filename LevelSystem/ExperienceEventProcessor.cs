using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodcraft_Re.LevelSystem;

/// <summary>
/// 经验事件处理器
/// 处理击杀敌人时的经验获取逻辑
/// </summary>
public static class ExperienceEventProcessor
{
    /// <summary>
    /// 处理敌人死亡事件，为参与击杀的玩家分配经验
    /// </summary>
    /// <param name="victimLevel">受害者等级</param>
    /// <param name="victimHealth">受害者最大血量</param>
    /// <param name="isVBlood">是否为VBlood</param>
    /// <param name="participants">参与击杀的玩家SteamID列表</param>
    public static void ProcessKillExperience(int victimLevel, float victimHealth, bool isVBlood, params ulong[] participants)
    {
        if (participants == null || participants.Length == 0)
            return;

        bool isGroupKill = participants.Length > 1;

        // 计算基础经验
        float baseExperience = ExperienceCalculator.CalculateBaseExperience(victimLevel, victimHealth, isVBlood);

        // 应用组队倍率
        float finalExperience = ExperienceCalculator.ApplyGroupMultiplier(baseExperience, isGroupKill);

        // 为每个参与者分配经验
        foreach (ulong steamId in participants)
        {
            ProcessIndividualExperience(steamId, finalExperience, victimLevel);
        }
    }

    /// <summary>
    /// 处理单个玩家的经验获取
    /// </summary>
    /// <param name="steamId">玩家SteamID</param>
    /// <param name="baseExperience">基础经验值</param>
    /// <param name="victimLevel">受害者等级</param>
    private static void ProcessIndividualExperience(ulong steamId, float baseExperience, int victimLevel)
    {
        // 检查是否已达最大等级
        if (LevelingSystem.IsPlayerMaxLevel(steamId))
            return;

        int playerLevel = LevelingSystem.GetPlayerLevel(steamId);

        // 应用等级差异缩放
        float scaledExperience = ExperienceCalculator.ApplyLevelScaling(baseExperience, playerLevel, victimLevel);

        // 确保最小经验获取
        scaledExperience = Math.Max(1f, scaledExperience);

        // 增加经验
        bool leveledUp = LevelingSystem.AddExperience(steamId, scaledExperience);

        // 这里可以添加通知玩家的逻辑
        // NotifyPlayer(steamId, scaledExperience, leveledUp);
    }

    /// <summary>
    /// 直接给予玩家经验（管理员命令等用途）
    /// </summary>
    /// <param name="steamId">玩家SteamID</param>
    /// <param name="experience">经验值</param>
    public static void GiveExperience(ulong steamId, float experience)
    {
        if (experience <= 0) return;

        bool leveledUp = LevelingSystem.AddExperience(steamId, experience);

        // 这里可以添加通知玩家的逻辑
        // NotifyPlayer(steamId, experience, leveledUp);
    }

    /// <summary>
    /// 批量处理经验事件（用于特殊情况，如任务完成等）
    /// </summary>
    /// <param name="experienceAmount">经验数量</param>
    /// <param name="participants">参与者SteamID列表</param>
    public static void ProcessBatchExperience(float experienceAmount, params ulong[] participants)
    {
        if (participants == null || participants.Length == 0 || experienceAmount <= 0)
            return;

        foreach (ulong steamId in participants)
        {
            if (!LevelingSystem.IsPlayerMaxLevel(steamId))
            {
                LevelingSystem.AddExperience(steamId, experienceAmount);
            }
        }
    }
}