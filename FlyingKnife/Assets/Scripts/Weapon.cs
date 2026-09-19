using System;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// 武器的状态
/// </summary>
public enum E_WeapomState
{
    /// <summary>
    /// 武器的准备阶段，上下浮动
    /// </summary>
    ready,
    /// <summary>
    /// 武器的发射阶段，飞出去
    /// </summary>
    shout,
    /// <summary>
    /// 武器的命中状态，武器依附目标进行旋转
    /// </summary>
    stuck
}


/// <summary>
/// 武器的命中结果类型
/// </summary>
public enum E_HitResult
{
    /// <summary>
    /// 无命中目标
    /// </summary>
    none,
    /// <summary>
    /// 附着在目标上
    /// </summary>
    stuck
}


/// <summary>
///武器脚本，主要实现武器的点击发射 和 通过脚本实现其待机动画
/// </summary>
public class Weapon : MonoBehaviour
{
    #region 武器配置
    [Header("浮动相关参数")]
    [SerializeField] private float floatingSpeed;   //武器浮动的速度
    [SerializeField] private float floatingRange;   //武器浮动的范围
    [Header("发射相关参数")]
    [SerializeField] private float _shoutSpeed;  //武器发射的速度
    [SerializeField] private Vector3 _startPos; //武器发射的初始位置
    //武器的当前状态
    private E_WeapomState _weapomState = E_WeapomState.ready;
    public E_WeapomState WeapomState => _weapomState;   //对外的武器状态属性（只读）
    public float StuckAngle { get;private set; }    //武器嵌入角度
    #endregion

    #region 生命周期函数
    private void Update()
    {
        //if(_weapomState == E_WeapomState.ready&&Input.GetMouseButtonDown(0))
        //{
        //    //启动发射
        //    Launch(shoutSpeed);
        //}
        switch (_weapomState)
        {
            case E_WeapomState.ready:
                ReadyWeapon(); break;
            case E_WeapomState.shout:
                ShoutWeapon(); break;
        }
    }
    #endregion

    #region 武器的发射与待机方法 设置武器的初始位置
   /// <summary>
   /// 武器的发射准备，在武器发射时调用
   /// 设置武器的当前状态，和发射速度
   /// </summary>
   /// <param name="speed"></param>
    public void Launch(float speed)
    {
        _weapomState = E_WeapomState.shout;
        _shoutSpeed = speed;
    }

    /// <summary>
    /// 设置武器的初始位置，在准备武器时使用
    /// </summary>
    /// <param name="pos">武器的位置</param>
    public void SetWeaponPosition(Vector3 pos)
    {
        _startPos = pos;
        transform.position = _startPos;
    }

    /// <summary>
    /// 武器发射状态的行为
    /// </summary>
    private void ShoutWeapon()
    {
        this.transform.position += Vector3.up * _shoutSpeed * Time.deltaTime;
    }
   
    /// <summary>
    /// 武器准备状态的行为
    /// </summary>
    private void ReadyWeapon()
    {
        float floatPos = Mathf.Sin(Time.time * floatingSpeed) * floatingRange;
        this.transform.position = _startPos + new Vector3(0, floatPos,0);
    }
    #endregion


    #region 武器命中目标的相关
    /// <summary>
    /// 检测飞到射击的结果
    /// </summary>
    /// <param name="tsrget">射击的对象</param>
    /// <returns></returns>
    public E_HitResult CheckHitResult(TargetController target)
    {
        //当飞刀还是准备状态时，射击状态仍保持none
        if(_weapomState == E_WeapomState.ready)return E_HitResult.none;

        #region 计算命中位置，通过飞刀 距 靶子的位置计算(有问题，为什么会飞过去)
        ////计算武器距靶子的距离
        //float length = Vector3.Distance(this.transform.position, target.transform.position);
        ////当距离大于半径时，不会命中
        //if (length > target.TargetRadius) return E_HitResult.none;
        ////飞刀进入，计算进入的方向
        //Vector3 dir = (transform.position - target.transform.position).normalized;
        ////将飞到固定刀指定位置
        //transform.position = new Vector3(transform.position.x * dir.x, target.TargetRadius * dir.y, transform.position.z * dir.z) ;
        //_weapomState = E_WeapomState.stuck;
        //return E_HitResult.stuck;
        #endregion

        //计算命中点
        float hitPosY = target.transform.position.y - target.TargetRadius;
        if (transform.position.y < target.TargetRadius) return E_HitResult.none;
        //将飞刀的位置固定
        //this.transform.position = new Vector3(transform.position.x, hitPosY, transform.position.z);
        //_weapomState = E_WeapomState.stuck;
        float inpactAngle = target.IncomingAngle;
        StickToTarget(target, inpactAngle);
        return E_HitResult.stuck;
    }

    private void StickToTarget(TargetController target,float impactAngle)
    {
        //武器嵌入目标,记录嵌入角度
        _weapomState = E_WeapomState.stuck;
       StuckAngle = impactAngle;
        //设置嵌入的位置
        Vector3 localPos = Quaternion.Euler(0, 0, impactAngle) * Vector3.down * target.WeaponOffset;
        transform.SetParent(target.transform);
        transform.localPosition = localPos;
        transform.localRotation = Quaternion.Euler(0, 0, impactAngle);
    }
    #endregion
}
