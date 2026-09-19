using System;
using Unity.VisualScripting;
using UnityEngine;

public enum E_WeapomState
{
    /// <summary>
    /// 武器的准备阶段，上下浮动
    /// </summary>
    ready,
    /// <summary>
    /// 武器的发射阶段，飞出去
    /// </summary>
    shout
}



/// <summary>
///武器脚本，主要实现武器的点击发射 和 通过脚本实现其待机动画
/// </summary>
public class Weapon : MonoBehaviour
{
    #region 武器配置
    //浮动相关参数
    [SerializeField] private float floatingSpeed;   //武器浮动的速度
    [SerializeField] private float floatingRange;   //武器浮动的范围
    //发射相关参数
    [SerializeField] private float shoutSpeed;  //武器发射的速度
    [SerializeField] private Vector3 startPos;  //武器发射的初始位置
    //
    private E_WeapomState _weapomState = E_WeapomState.ready;
    public E_WeapomState WeapomState => _weapomState;   //对外的武器状态属性（只读）
    #endregion


    #region 生命周期函数
    private void Update()
    {
        if(_weapomState == E_WeapomState.ready&&Input.GetMouseButtonDown(0))
        {
            //启动发射
            Launch(shoutSpeed);
        }
        switch (_weapomState)
        {
            case E_WeapomState.ready:
                ReadyWeapon(); break;
            case E_WeapomState.shout:
                ShoutWeapon(); break;
        }
    }
    #endregion

    #region 武器的发射与待机方法
    public void Launch(float speed)
    {
        _weapomState = E_WeapomState.shout;
        shoutSpeed = speed;
    }

    public void ShoutWeapon()
    {
        this.transform.position += Vector3.up * shoutSpeed * Time.deltaTime;
    }
    public void ReadyWeapon()
    {
        float floatPos = Mathf.Sin(Time.time * floatingSpeed) * floatingRange;
        this.transform.position = startPos + new Vector3(0, floatPos,0);
    }

    #endregion

}
