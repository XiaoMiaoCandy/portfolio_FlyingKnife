using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    #region 目标靶子配置
    [Tooltip("武器偏移比列")][SerializeField] private float weaponOffestRatio = 0.85f;
    [Header("靶子摇晃相关")]
    [Tooltip("摇晃持续的时间")][SerializeField] private float shakeDuration = 0.12f;
    [Tooltip("摇晃的振幅")][SerializeField] private float shakeIntensity = 0.08f;
    private Vector3 _basePosition;          //靶子的初始位置，用来震动结束后位置复原
    private float _shakeTimer;              //震动的计时器
    private float _currentShakeIntensity;   //当前的振幅

    //私有变量
    //因为飞刀从固定角度射向靶子，所以可以找到一个切线，因此可以得到300，然后根据图像的真实大小，可以计算出比列
    private const float TargetRadiusRation = 300f / 350;
    private SpriteRenderer _targetSprite;   //目标靶子的图片
    private float _targetRadius = 1.5f;     //目标靶子的半径
    private bool _isRotating = true;        //靶子是否在旋转
    [SerializeField] private float _currentRotationSpeed = 80f; //当前的旋转速度
    public float TargetRadius => _targetRadius; //目标半径的属性，外部只读
    #endregion

    #region 武器命中靶子相关配置
    private float _weaponOffset;    
    //武器嵌入的深度
    public float WeaponOffset => _weaponOffset;
    //武器嵌入的角度
    public float IncomingAngle => Mathf.Repeat(-transform.eulerAngles.z, 360);  
    //命中靶子的列表
    private readonly List<Weapon> _stuckWeapons = new List<Weapon>();
    [Header("宽容度 容错")]
    [Range(0.1f, 1f)]
    [SerializeField] private float weaponOverlapTolerance = 0.7f;
    #endregion

    #region 生命周期相关
    private void Awake()
    {
        //获取靶子的初始位置
        _basePosition = transform.position;
        //获取目标图片
        _targetSprite = GetComponentInChildren<SpriteRenderer>();
        //计算目标的真实半径
        UpdateTargetRadius();
    }
    private void Update()
    {
        //进行目标的旋转
        if(_isRotating)
        {
            this.transform.Rotate(0,0,_currentRotationSpeed * Time.deltaTime);
        }
        UpdateShake();
    }
    #endregion

    #region 辅助方法

    /// <summary>
    /// 计算目标的真实半径，当进入游戏，或者目标的形态改变时使用
    /// 计算公式：目标半径 = 目标图片的半高 * 半径比列 * y轴缩放
    /// </summary>
    private void UpdateTargetRadius()
    {
        _targetRadius = _targetSprite.sprite.bounds.extents.y * TargetRadiusRation * Mathf.Abs(_targetSprite.transform.localScale.y);
        _weaponOffset = _targetRadius * weaponOffestRatio;
    }

    /// <summary>
    /// 重置目标状态，当进入新关卡时重置
    /// </summary>
    public void ResetTarget()
    {
        _isRotating = true;
        this.transform.rotation = Quaternion.identity;
    }
   
    /// <summary>
    /// 进行震动
    /// </summary>
    private void UpdateShake()
    {
        //检测震动是否结束
        if(_shakeTimer > 0)
        {
            //进行扣除时间
            _shakeTimer -= Time.deltaTime;
            //进行靶子的震动
            transform.position = _basePosition + (Vector3)(Random.insideUnitCircle*_currentShakeIntensity);
            return;
        }
        //震动结束复原位置
        transform.position = _basePosition;
    }
    /// <summary>
    /// 触发震动
    /// </summary>
    public void TriggerShake()
    {
        _shakeTimer = shakeDuration;
        _currentShakeIntensity = shakeIntensity;
    }
    
    /// <summary>
    /// 将命中靶子的武器 注册到命中_stuckWeapons列表中
    /// 在武器命中时调用
    /// </summary>
    /// <param name="weapon"></param>
    public void RegisterStuckWeapon(Weapon weapon)
    {
        _stuckWeapons.Add(weapon);
    }
    #endregion

    #region 武器撞刀相关
    /// <summary>
    /// 得到武器占的半宽角度
    /// 用来计算与另一个武器的半宽角度和，通过该和可以计算两角度的最小切入距离
    /// </summary>
    /// <param name="weapon"></param>
    /// <returns></returns>
    private float GetWeaponHalfAngle(Weapon weapon)
    {
        float halfWidth = weapon.Width * weaponOverlapTolerance * 0.5f;
        return Mathf.Asin(Mathf.Clamp01(halfWidth /_targetRadius)) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// 检测是否会发生撞刀
    /// </summary>
    /// <param name="candidate">当前发生武器</param>
    /// <param name="angle">当前武器的切入角度</param>
    /// <returns></returns>
    public bool CheckWeaponOverlap(Weapon candidate, float angle)
    {
        //计算当前发射武器的半高
        float candidateHalfAngle = GetWeaponHalfAngle(candidate);
        //遍历所有武器
        foreach (Weapon weapon in _stuckWeapons)
        {
            //发射武器 与 已存在武器的最小安全角度距离
            float minimumAngle = candidateHalfAngle + GetWeaponHalfAngle(weapon);
            //发射武器的切入角度 与 已存在武器命中角度的距离
            float angleDistance = Mathf.Abs(Mathf.DeltaAngle(angle, weapon.StuckAngle));
            //如果 小于最小安全距离则发生撞刀
            if (angleDistance < minimumAngle)
                return true;
        }
        return false;
    }
    #endregion
}
