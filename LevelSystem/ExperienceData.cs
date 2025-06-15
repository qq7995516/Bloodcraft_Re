using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Entities;
using UnityEngine;

namespace Bloodcraft_Re.LevelSystem;

/// <summary>
/// 玩家经验数据模型
/// 用于存储玩家的等级和经验值信息
/// </summary>
public class ExperienceData
{
    /// <summary>
    /// 玩家当前等级
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 玩家当前经验值
    /// </summary>
    public float Experience { get; set; }

    /// <summary>
    /// 上次更新时间
    /// </summary>
    public DateTime LastUpdated { get; set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="level">等级</param>
    /// <param name="experience">经验值</param>
    public ExperienceData(int level = 0, float experience = 0f)
    {
        Level = level;
        Experience = experience;
        LastUpdated = DateTime.UtcNow;
    }

    /// <summary>
    /// 更新经验数据
    /// </summary>
    /// <param name="newLevel">新等级</param>
    /// <param name="newExperience">新经验值</param>
    public void Update(int newLevel, float newExperience)
    {
        Level = newLevel;
        Experience = newExperience;
        LastUpdated = DateTime.UtcNow;
    }
}