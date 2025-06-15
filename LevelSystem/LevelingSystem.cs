using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodcraft_Re.LevelSystem;

/// <summary>
/// 等级系统主类
/// 负责处理玩家等级提升和经验管理
/// </summary>
public static class LevelingSystem
{
    /// <summary>
    /// 存储所有玩家的经验数据
    /// Key: SteamID, Value: 经验数据
    /// </summary>
    private static readonly Dictionary<ulong, ExperienceData> _playerExperienceData = new();

    /// <summary>
    /// 等级提升事件
    /// </summary>
    public static event Action<ulong, int, int> OnPlayerLevelUp; // steamId, oldLevel, newLevel

    /// <summary>
    /// 经验获取事件
    /// </summary>
    public static event Action<ulong, float> OnExperienceGained; // steamId, experienceGained

    /// <summary>
    /// 获取玩家经验数据
    /// </summary>
    /// <param name="steamId">玩家SteamID</param>
    /// <returns>玩家经验数据，如果不存在则创建新的</returns>
    public static ExperienceData GetPlayerExperienceData(ulong steamId)
    {
        if (!_playerExperienceData.TryGetValue(steamId, out var data))
        {
            data = new ExperienceData();
            _playerExperienceData[steamId] = data;
        }
        return data;
    }

    /// <summary>
    /// 设置玩家经验数据
    /// </summary>
    /// <param name="steamId">玩家SteamID</param>
    /// <param name="experienceData">经验数据</param>
    public static void SetPlayerExperienceData(ulong steamId, ExperienceData experienceData)
    {
        _playerExperienceData[steamId] = experienceData;
    }

    /// <summary>
    /// 获取玩家当前等级
    /// </summary>
    /// <param name="steamId">玩家SteamID</param>
    /// <returns>玩家等级</returns>
    public static int GetPlayerLevel(ulong steamId)
    {
        var data = GetPlayerExperienceData(steamId);
        return data.Level;
    }

    /// <summary>
    /// 获取玩家当前经验值
    /// </summary>
    /// <param name="steamId">玩家SteamID</param>
    /// <returns>玩家经验值</returns>
    public static float GetPlayerExperience(ulong steamId)
    {
        var data = GetPlayerExperienceData(steamId);
        return data.Experience;
    }

    /// <summary>
    /// 获取玩家当前等级进度
    /// </summary>
    /// <param name="steamId">玩家SteamID</param>
    /// <returns>进度百分比 (0-100)</returns>
    public static int GetPlayerLevelProgress(ulong steamId)
    {
        var data = GetPlayerExperienceData(steamId);
        return ExperienceCalculator.CalculateLevelProgress(data.Experience);
    }

    /// <summary>
    /// 为玩家增加经验值
    /// </summary>
    /// <param name="steamId">玩家SteamID</param>
    /// <param name="experienceToAdd">要增加的经验值</param>
    /// <returns>是否升级</returns>
    public static bool AddExperience(ulong steamId, float experienceToAdd)
    {
        if (experienceToAdd <= 0) return false;

        var data = GetPlayerExperienceData(steamId);
        int oldLevel = data.Level;

        // 增加经验
        data.Experience += experienceToAdd;

        // 计算新等级
        int newLevel = ExperienceCalculator.CalculateLevelFromExperience(data.Experience);

        // 检查是否超过最大等级
        if (newLevel > LevelingConfiguration.MaxPlayerLevel)
        {
            newLevel = LevelingConfiguration.MaxPlayerLevel;
            data.Experience = ExperienceCalculator.CalculateExperienceForLevel(LevelingConfiguration.MaxPlayerLevel);
        }

        // 更新等级
        bool leveledUp = newLevel > oldLevel;
        data.Update(newLevel, data.Experience);

        // 触发事件
        OnExperienceGained?.Invoke(steamId, experienceToAdd);

        if (leveledUp)
        {
            OnPlayerLevelUp?.Invoke(steamId, oldLevel, newLevel);
        }

        return leveledUp;
    }

    /// <summary>
    /// 设置玩家等级（管理员功能）
    /// </summary>
    /// <param name="steamId">玩家SteamID</param>
    /// <param name="level">目标等级</param>
    public static void SetPlayerLevel(ulong steamId, int level)
    {
        level = Math.Clamp(level, 0, LevelingConfiguration.MaxPlayerLevel);

        var data = GetPlayerExperienceData(steamId);
        int oldLevel = data.Level;

        float requiredExperience = ExperienceCalculator.CalculateExperienceForLevel(level);
        data.Update(level, requiredExperience);

        if (level != oldLevel)
        {
            OnPlayerLevelUp?.Invoke(steamId, oldLevel, level);
        }
    }

    /// <summary>
    /// 重置玩家经验和等级
    /// </summary>
    /// <param name="steamId">玩家SteamID</param>
    public static void ResetPlayerProgress(ulong steamId)
    {
        var data = GetPlayerExperienceData(steamId);
        int oldLevel = data.Level;

        data.Update(0, 0f);

        if (oldLevel > 0)
        {
            OnPlayerLevelUp?.Invoke(steamId, oldLevel, 0);
        }
    }

    /// <summary>
    /// 检查玩家是否达到最大等级
    /// </summary>
    /// <param name="steamId">玩家SteamID</param>
    /// <returns>是否达到最大等级</returns>
    public static bool IsPlayerMaxLevel(ulong steamId)
    {
        return GetPlayerLevel(steamId) >= LevelingConfiguration.MaxPlayerLevel;
    }

    /// <summary>
    /// 获取玩家升级到下一级所需的经验值
    /// </summary>
    /// <param name="steamId">玩家SteamID</param>
    /// <returns>所需经验值，如果已是最大等级则返回0</returns>
    public static float GetExperienceToNextLevel(ulong steamId)
    {
        var data = GetPlayerExperienceData(steamId);

        if (data.Level >= LevelingConfiguration.MaxPlayerLevel)
            return 0f;

        float nextLevelExperience = ExperienceCalculator.CalculateExperienceForLevel(data.Level + 1);
        return Math.Max(0f, nextLevelExperience - data.Experience);
    }

    /// <summary>
    /// 清理所有玩家数据（用于服务器重启等情况）
    /// </summary>
    public static void ClearAllData()
    {
        _playerExperienceData.Clear();
    }
}