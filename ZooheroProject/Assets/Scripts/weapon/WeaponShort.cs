using NUnit.Framework;
using UnityEngine;

public class WeaponShort : WeaponBase
{
<<<<<<< Updated upstream
    // ��������ײ����������ײ��Ӵ�ʱ�Զ�����
=======

    public new void Awake()
    {
        //父元素Awake
        base.Awake();

        moveSpeed = 10;

    }


    //近战开火
    public override IEnumerator Fire()
    {
        // 检查武器是否在冷却中，如果是则直接退出，不执行发射
        if (isCooling)
        {
            yield break;
        }
        isCooling = true;
        for (int i = 0; i < data.attackcount; i++)
        {
            // 启用武器的碰撞体，使其能够与敌人发生碰撞检测
            gameObject.GetComponent<CapsuleCollider2D>().enabled = true;

            //关闭瞄准移动时候不改变出去方向
            isAiming = false;

            // 启动协程：让武器向敌人位置移动
            StartCoroutine(Goposition());
            
            yield return new WaitForSeconds(0.3f);
        }

       

        // 将武器状态设置为冷却中，防止连续发射
        isCooling = true;
    }

    // 当武器碰撞体与其他碰撞体接触时自动调用
>>>>>>> Stashed changes
    private void OnTriggerEnter2D(Collider2D col)
    {
        // �����ײ���������Ƿ���Ϊ"Enemy"��ǩ
        if (col.CompareTag("Enemy"))
        {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
            // �Ե�������˺�����ȡ����������������˷��������������˺�ֵ
            col.GetComponent<EnemyBase>().Injured(data.damage);

            // �����ر���������ײ�壬��ֹͬһ֡�ڶ�δ����˺�
            gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
=======
            // 对敌人造成伤害：获取敌人组件并调用受伤方法，传入武器伤害值
           

=======
            // 对敌人造成伤害：获取敌人组件并调用受伤方法，传入武器伤害值
           

>>>>>>> Stashed changes
=======
            // 对敌人造成伤害：获取敌人组件并调用受伤方法，传入武器伤害值
           

>>>>>>> Stashed changes
            bool isCritcal = CriicalHits();
            if (isCritcal)  
            {
                //产生暴击
                col.GetComponent<EnemyBase>().Injured(data.damage*=data.critical_strikes_multiple);
            }
            else
            {
                //没暴击
                col.GetComponent<EnemyBase>().Injured(data.damage);
            }
            
            
            // 立即关闭武器的碰撞体，防止同一帧内多次触发伤害
            //gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
>>>>>>> Stashed changes
        }
    }
}
