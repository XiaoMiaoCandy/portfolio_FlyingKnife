using UnityEngine;

/// <summary>
/// 游戏管理器，实现武器的创建与发射
/// </summary>
public class GameManager : MonoBehaviour
{
    #region 参数配置
    [Header("武器发射相关参数")]
    [Tooltip("武器的发射速度")][SerializeField] private float weapomShoutSpeed = 20f;
    [Tooltip("当前可发射武器")][SerializeField] private Weapon _currentWeapon;
    [Tooltip("武器的初始位置")][SerializeField] private Transform _startPos;
    [Tooltip("目标靶子")][SerializeField] private TargetController _targetController;
    private bool _isHasLaunchedWeapon;    //当前武器处于发射状态
    #endregion

    #region 生命周期函数
    private void Start()
    {
        _currentWeapon.SetWeaponPosition(_startPos.position);
    }
    private void Update()
    {
        //进行发射飞刀尝试
        HandleFireInput();
        if(_currentWeapon == null) return;
        //检测当前武器的发射状态
        E_HitResult e_HitResult = _currentWeapon.CheckHitResult(_targetController);
        if(e_HitResult == E_HitResult.stuck)
        {
            //将当前可发射飞刀置空
            _currentWeapon = null;
        }
    }

    #endregion

    #region 发射相关方法
    private void HandleFireInput()
    {
        //检测是否有武器 且 武器可以发射
        if(_isHasLaunchedWeapon || _currentWeapon == null) return;
        //当鼠标按下进行发射(这里在函数中，如果直接检测很可能进不去)
        if(!Input.GetMouseButtonDown(0)) return;
       _currentWeapon.Launch(weapomShoutSpeed);
        _isHasLaunchedWeapon = true;
    }
    #endregion
}
