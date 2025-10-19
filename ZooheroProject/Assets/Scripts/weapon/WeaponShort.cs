using NUnit.Framework;
using UnityEngine;

public class WeaponShort : WeaponBase
{
    // ��������ײ����������ײ��Ӵ�ʱ�Զ�����
    private void OnTriggerEnter2D(Collider2D col)
    {
        // �����ײ���������Ƿ���Ϊ"Enemy"��ǩ
        if (col.CompareTag("Enemy"))
        {
            // �Ե�������˺�����ȡ����������������˷��������������˺�ֵ
            col.GetComponent<EnemyBase>().Injured(data.damage);

            // �����ر���������ײ�壬��ֹͬһ֡�ڶ�δ����˺�
            gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        }
    }
}
