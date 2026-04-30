
using IFix;
using UnityEngine;
using System.Collections.Generic;

public class TestInjectFixPlugins
{
    public int name;
    public bool bool1 = true;

    // ========== Patch 版本（解释执行）==========

    [Patch]
    public static int Calculate()
    {
        // 模拟游戏业务逻辑：计算一组敌人的伤害
        List<int> enemyDefense = new List<int> { 10, 25, 30, 15, 40, 22, 18, 35, 28, 12 };
        int totalDamage = 0;
        int attackPower = 100;
        float critRate = 0.3f;
        float critMultiplier = 1.5f;

        for (int i = 0; i < enemyDefense.Count; i++)
        {
            int baseDamage = attackPower - enemyDefense[i];
            if (baseDamage < 1) baseDamage = 1;
            // 模拟暴击判定
            if (UnityEngine.Random.Range(0f, 1f) < critRate)
            {
                baseDamage = (int)(baseDamage * critMultiplier);
            }
            
            // 模拟属性克制加成
            if (i % 3 == 0)
            {
                baseDamage = (int)(baseDamage * 1.2f);
            }
            // 模拟防御减伤
            if (enemyDefense[i] > 25)
            {
                baseDamage = (int)(baseDamage * 0.8f);
            }

            totalDamage += baseDamage;
        }

        // 字符串拼接生成战斗日志
        string log = "Total:" + totalDamage + ",Avg:" + (totalDamage / enemyDefense.Count);

        // 查找防御最高的敌人索引
        int maxDef = 0;
        int maxIndex = 0;
        for (int i = 0; i < enemyDefense.Count; i++)
        {
            if (enemyDefense[i] > maxDef)
            {
                maxDef = enemyDefense[i];
                maxIndex = i;
            }
        }

        // 过滤出高防御敌人
        List<int> highDefense = new List<int>();
        for (int i = 0; i < enemyDefense.Count; i++)
        {
            if (enemyDefense[i] > 20)
            {
                highDefense.Add(enemyDefense[i]);
            }
        }

        // 计算高防御敌人的总减伤比例
        float reductionRate = 0f;
        for (int i = 0; i < highDefense.Count; i++)
        {
            reductionRate += (highDefense[i] / 100f);
        }

        return totalDamage + maxIndex + highDefense.Count + (int)reductionRate;
    }

    // ========== 原生版本（AOT 编译执行）==========

    public static int CalculateNative()
    {
        List<int> enemyDefense = new List<int> { 10, 25, 30, 15, 40, 22, 18, 35, 28, 12 };
        int totalDamage = 0;
        int attackPower = 100;
        float critRate = 0.3f;
        float critMultiplier = 1.5f;

        for (int i = 0; i < enemyDefense.Count; i++)
        {
            int baseDamage = attackPower - enemyDefense[i];
            if (baseDamage < 1) baseDamage = 1;

            if (UnityEngine.Random.Range(0f, 1f) < critRate)
            {
                baseDamage = (int)(baseDamage * critMultiplier);
            }

            if (i % 3 == 0)
            {
                baseDamage = (int)(baseDamage * 1.2f);
            }

            if (enemyDefense[i] > 25)
            {
                baseDamage = (int)(baseDamage * 0.8f);
            }

            totalDamage += baseDamage;
        }

        string log = "Total:" + totalDamage + ",Avg:" + (totalDamage / enemyDefense.Count);

        int maxDef = 0;
        int maxIndex = 0;
        for (int i = 0; i < enemyDefense.Count; i++)
        {
            if (enemyDefense[i] > maxDef)
            {
                maxDef = enemyDefense[i];
                maxIndex = i;
            }
        }

        List<int> highDefense = new List<int>();
        for (int i = 0; i < enemyDefense.Count; i++)
        {
            if (enemyDefense[i] > 20)
            {
                highDefense.Add(enemyDefense[i]);
            }
        }

        float reductionRate = 0f;
        for (int i = 0; i < highDefense.Count; i++)
        {
            reductionRate += (highDefense[i] / 100f);
        }

        return totalDamage + maxIndex + highDefense.Count + (int)reductionRate;
    }
}
