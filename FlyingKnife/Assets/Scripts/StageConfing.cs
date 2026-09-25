using UnityEngine;

/// <summary>
/// 游戏配置文件
/// </summary>
[System.Serializable]
public class StageConfing
{
    [Header("视觉表现")]
    public Sprite backgroundSprite; //背景图片 =》 由Camera获取BackGround上的SpriteRenderer，并修改图片
    public Sprite targerSprite;     //靶子图片 =》 由TargetController获取并修改图片
    public float targtScale = 1f;   //靶子的缩放比列，在关卡进入时用于初始化靶子

    [Header("基础参数")]
    public float rotationSpeed = 80f;   //靶子的转速

    [Header("通关目标")]
    public int weaponCount;     //通关需要的武器命中数量
}
