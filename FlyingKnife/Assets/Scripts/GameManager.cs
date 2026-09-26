using UnityEngine;

/// <summary>
/// 游戏管理器，实现武器的创建与发射
/// </summary>
public class GameManager : MonoBehaviour
{
    #region 参数配置
    [Header("武器发射相关参数")]
    [Tooltip("最大武器数量")][SerializeField] private int maxKnifeCount;
    [Tooltip("生成武器的间隔")][SerializeField] private float createGap = 0.15f;   
    [Tooltip("武器的发射速度")][SerializeField] private float weapomShoutSpeed = 20f;
    [Tooltip("武器预设体")][SerializeField] private Weapon weaponPrefab;
    [Tooltip("武器的初始位置")][SerializeField] private Transform _createWeaponPos;
    [Tooltip("目标靶子")][SerializeField] private TargetController _targetController;
    //private bool _isHasLaunchedWeapon;  //当前武器处于发射状态
    private Weapon _currentWeapon;      //当前的发射武器
    private int _weaponRemaining;       //当前武器的库存
    #endregion

    #region 生命周期函数
    private void Start()
    {
        //游戏一开始时 获取武器数量 并创建第一把武器
        _weaponRemaining = maxKnifeCount;
        CreateWeapon();
    }
    private void Update()
    {
        //进行发射飞刀尝试
        HandleFireInput();
        if(_currentWeapon == null) return;
        //检测当前武器的发射状态
        //E_HitResult e_HitResult = _currentWeapon.CheckHitResult(_targetController);
        //if(e_HitResult != E_HitResult.stuck) return;
        //根据武器的状态来进行掉落或附着的逻辑
        CheckCurrentWeaponHit();
    }

    #endregion

    #region 发射相关方法
    /// <summary>
    /// 进行点击发射
    /// </summary>
    private void HandleFireInput()
    {
        //检测是否有武器 且 只有准备状态的武器可发射
        if(_currentWeapon == null||_currentWeapon.WeapomState != E_WeapomState.ready ) return;
        //当鼠标按下进行发射(这里在函数中，如果直接检测很可能进不去)
        if(!Input.GetMouseButtonDown(0)) return;
        _currentWeapon.LaunchTrigger(weapomShoutSpeed);
        //武器发射成功
        _weaponRemaining--;
    }
    
    private void CreateWeapon()
    {
        if(_weaponRemaining<=0) return; 
        //创建武器
        _currentWeapon = Instantiate(weaponPrefab,_createWeaponPos);
        //设置武器位置，这里武器预设体为000，会直接到创建点,这里我不了解为什么会直接去世界坐标的000
        //手动设置位置
        _currentWeapon.SetWeaponPosition(_createWeaponPos.position);

    }

    private void HandleWeaponStuck()
    {
        _currentWeapon = null;
        if (_weaponRemaining <= 0) return;
        //如果现在还存在武器，那么就创建并重置武器
        Invoke(nameof(CreateWeapon), createGap);
    }
    private void HandleWeaponMissed()
    {
        _currentWeapon = null;
        if (_weaponRemaining <= 0) return;
        //如果现在还存在武器，那么就创建并重置武器
        Invoke(nameof(CreateWeapon), createGap*2);
    }
    private void CheckCurrentWeaponHit()
    {
        if(_currentWeapon == null||_currentWeapon.WeapomState!=E_WeapomState.shout) return;
        E_HitResult result = _currentWeapon.CheckHitResult(_targetController);
        if(result == E_HitResult.stuck)
            HandleWeaponStuck();
        else if(result == E_HitResult.missed)
            HandleWeaponMissed();
    }
    #endregion
}
