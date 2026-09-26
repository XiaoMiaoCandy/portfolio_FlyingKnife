using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 该脚本通过调整摄像机视口 和 背景图片大小来实现 背景图的自动适配
/// </summary>
public class CameraAdapter : MonoBehaviour
{
    #region 
    [SerializeField] private float baseAspect = 9f / 16f;
    [SerializeField] private float baseOrthoSize = 5.4f;
    [SerializeField] private SpriteRenderer bkRenderer;

    //脚本中的临时私有变量
    private Camera _camera;
    private float _lastAspect = -1;
    #endregion

    private void Awake()
    {
        //进入游戏后自适应游戏屏幕
        _camera = GetComponent<Camera>();
        Refresh(_camera.aspect);
    }

    private void Update()
    {
        float currentAspect = _camera.aspect;
        if(!Mathf.Approximately(currentAspect,_lastAspect))
        {
            Refresh(_camera.aspect);
        }
    }

    #region 实现背景自适应的辅助方法
    public void Refresh(float currentAspect)
    {
        //将当前设备的比例记录
        _lastAspect = currentAspect;
        //设置摄像机的半高大小
        AdjustCameraSize(currentAspect);
        //设置图片的大小
        SetSpriteSize(currentAspect);
    }


    /// <summary>
    /// 设置正交相机的半高，进入游戏刷新，当大小变化时刷新
    /// 计算公式为：
    /// </summary>
    /// <param name="currentAspect">当前设备的宽高比/param>
    public void AdjustCameraSize(float currentAspect)
    {
        if (currentAspect < baseAspect)
            _camera.orthographicSize = baseOrthoSize * (baseAspect / currentAspect );
        else 
            _camera.orthographicSize = baseOrthoSize;
    }


    /// <summary>
    /// 设置背景图片的小
    /// </summary>
    /// <param name="currentAspect"></param>
    public void SetSpriteSize(float currentAspect)
    {
        //当前屏幕的高,宽  ===》 宽 = 高 * 宽高比
        float cameraH = _camera.orthographicSize * 2f;
        float cameraW = cameraH * currentAspect;
        //背景图片的大小
        Vector2 spriteSize = bkRenderer.sprite.bounds.size; 
        //计算缩放比例 图片要完整的显示在屏幕
        float scale = Mathf.Max(cameraW/spriteSize.x, cameraH/spriteSize.y);
        bkRenderer.transform.localScale = new Vector3(scale,scale,0);
    }
    
    #endregion
}
