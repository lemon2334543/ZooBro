using System;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    // 可在Inspector面板调整的位置范围（方便后续修改，比硬编码更灵活）
    [Header("云朵随机位置范围")]
    [Tooltip("X轴最小值（左边界）")]
    public float minX = -18.96f;
    [Tooltip("X轴最大值（右边界）")]
    public float maxX = 17.6f;
    [Tooltip("Y轴最小值（下边界）")]
    public float minY = -9.37f;
    [Tooltip("Y轴最大值（上边界）")]
    public float maxY = 9.87f;

    [Header("云朵移动配置")]
    [Tooltip("云朵移动速度（控制移动快慢，建议0.5~2之间）")]
    public float moveSpeed = 1f;
    [Tooltip("方向随机化的最小角度（度），越大方向变化越明显，建议30~90")]
    public float minRandomAngle = 45f;
    [Tooltip("方向随机化的最大角度（度）")]
    public float maxRandomAngle = 90f;

    // 存储当前的移动方向
    private Vector2 _moveDirection;
    private Transform _transform;

    private void Awake()
    {
        // 缓存Transform组件，减少性能消耗
        _transform = transform;
        setCloud();
        // 初始化移动方向（随机方向）
        InitRandomDirection();
    }

    // 初始化随机移动方向
    private void InitRandomDirection()
    {
        // 生成0~360度的随机角度
        float randomAngle = UnityEngine.Random.Range(0f, 360f);
        // 将角度转换为方向向量（单位向量，只保留方向，不保留长度）
        _moveDirection = new Vector2(
            Mathf.Cos(randomAngle * Mathf.Deg2Rad), // 角度转弧度计算余弦（X方向）
            Mathf.Sin(randomAngle * Mathf.Deg2Rad)  // 角度转弧度计算正弦（Y方向）
        );
    }

    // 随机改变移动方向（超出范围时调用）
    private void ChangeRandomDirection()
    {
        // 生成指定范围内的随机角度偏移
        float randomAngle = UnityEngine.Random.Range(minRandomAngle, maxRandomAngle);
        // 随机决定是顺时针还是逆时针旋转
        int rotateDirection = UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1;
        // 将角度偏移转换为弧度
        float angleRad = randomAngle * rotateDirection * Mathf.Deg2Rad;

        // 旋转方向向量（二维向量旋转公式）
        float newX = _moveDirection.x * Mathf.Cos(angleRad) - _moveDirection.y * Mathf.Sin(angleRad);
        float newY = _moveDirection.x * Mathf.Sin(angleRad) + _moveDirection.y * Mathf.Cos(angleRad);
        _moveDirection = new Vector2(newX, newY).normalized; // 归一化，保证方向向量长度为1
    }

    private void cloudMove()
    {
        // 计算每帧的移动偏移量（速度 * 时间，保证帧率无关）
        Vector3 moveOffset = new Vector3(_moveDirection.x, _moveDirection.y, 0) * moveSpeed * Time.deltaTime;
        // 应用移动
        _transform.position += moveOffset;

        // 检测是否超出范围，超出则改变方向
        CheckBoundary();
    }

    // 检测是否超出位置范围，并处理
    private void CheckBoundary()
    {
        Vector3 currentPos = _transform.position;
        bool isOutOfBound = false;

        // 检测X轴边界，超出则限制位置并标记需要改变方向
        if (currentPos.x < minX)
        {
            currentPos.x = minX;
            isOutOfBound = true;
        }
        else if (currentPos.x > maxX)
        {
            currentPos.x = maxX;
            isOutOfBound = true;
        }

        // 检测Y轴边界，超出则限制位置并标记需要改变方向
        if (currentPos.y < minY)
        {
            currentPos.y = minY;
            isOutOfBound = true;
        }
        else if (currentPos.y > maxY)
        {
            currentPos.y = maxY;
            isOutOfBound = true;
        }

        // 若超出范围，更新位置并随机改变方向
        if (isOutOfBound)
        {
            _transform.position = currentPos;
            ChangeRandomDirection();
        }
    }

    private void setCloud()
    {
        // 1. 生成X轴随机值（范围：-18.96 到 17.6）
        float randomX = UnityEngine.Random.Range(minX, maxX);
        // 2. 生成Y轴随机值（范围：-9.37 到 9.87）
        float randomY = UnityEngine.Random.Range(minY, maxY);
        // 3. 保持Z轴位置不变（2D场景中Z轴通常控制层级，无需修改）
        float currentZ = _transform.position.z;

        // 4. 给对象设置随机位置
        _transform.position = new Vector3(randomX, randomY, currentZ);
    }

    void Start()
    {
        
    }

    void Update()
    {
        // 每帧执行移动逻辑（必须在Update中调用，否则移动不会持续）
        cloudMove();
    }
}