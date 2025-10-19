using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    public WeaponData data;//������������

    public bool isAttack = false;//�Ƿ���Թ����������ڹ�����Χ��
    public bool isCooling = false;//������ȴ
    public bool isAiming = true; //�Ƿ��Զ���׼
    public float AttackTimer = 0;//������ʱ��
    public float moveSpeed;//�ƶ��ٶ�
    public Transform enemy;//��⹥������
    public float originZ;

    private void Awake()
    {
        originZ = transform.eulerAngles.z;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Player.Instance.isDead)
        {
            return;
        }

        //�Զ���׼
        if (isAiming)
        {
            Aiming();
        }


        //�жϹ���
        if (isAttack && !isCooling)
        {
            Fire();
        }


        // ������ȴ����
        if (isCooling)
        {
            // �ۼ���ȴ��ʱ����ÿ֡���Ӿ�����ʱ��
            AttackTimer += Time.deltaTime;

            // ����Ƿ��������ȴʱ��
            if (AttackTimer >= data.cooling)
            {
                // ������ȴ��ʱ��
                AttackTimer = 0;
                // ����ȴ״̬����Ϊfalse����ʾ�����ٴι���
                isCooling = false;
            }
        }



    }

    private void Aiming()
    {
        // 1. ��⹥����Χ�ڵ����е���
        // ʹ��Բ�μ�������ҳ������ڷ�Χ�ڵĵ�����ײ��
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(
            transform.position,       // ������ĵ㣺��ǰ����λ��
            data.range,               // ���뾶����WeaponData�л�ȡ������Χ
            LayerMask.GetMask("Enemy")// ���㼶��ֻ�����Ϊ"Enemy"�������
        );

        // 2. �ж��Ƿ��⵽����
        if (enemiesInRange.Length > 0) // �����Χ��������һ������
        {
            isAttack = true; // ����Ϊ����״̬����ʾ��Ŀ��ɹ���

            // 3. �Ӽ�⵽�ĵ������ҳ����������һ��
            Collider2D nearestEnemy = enemiesInRange
                // ���������򣺼���ÿ�������������ľ��룬��С��������
                .OrderBy(enemy => Vector2.Distance(
                    transform.position,              // ������ǰλ��
                    enemy.transform.position         // ����λ��
                ))
                .First(); // ȡ��һ��������������ĵ��ˣ�

            // 4. ����������˵�Transform���ã����ں�������
            enemy = nearestEnemy.transform;

            // 5. ��������Ӧ����ת�ĽǶȣ�ʹ��ָ�����
            Vector2 enemyPos = enemy.position;                    // ����λ��
            Vector2 direction = enemyPos - (Vector2)transform.position; // ����������������ָ�����
            float angleDegrees = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // ������ת��Ϊ�Ƕ�

            // 6. Ӧ����ת�Ƕȣ�ʹ����ָ����ˣ�����ԭʼZ��ƫ�ƣ�
            transform.eulerAngles =
                new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, angleDegrees + originZ);
        }
        else
        {
            // 7. ���û�м�⵽���ˣ�����״̬
            isAttack = false;    // ����Ϊ�ǹ���״̬
            enemy = null;        // �������Ŀ������
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, originZ); // ���������Ƕȵ�ԭʼ����
        }
    }

    public void Fire()
    {
        // ��������Ƿ�����ȴ�У��������ֱ���˳�����ִ�з���
        if (isCooling)
        {
            return;
        }

        // ������������ײ�壬ʹ���ܹ�����˷�����ײ���
        gameObject.GetComponent<CapsuleCollider2D>().enabled = true;

        //�ر���׼�ƶ�ʱ�򲻸ı��ȥ����
        isAiming = false;

        // ����Э�̣������������λ���ƶ�
        StartCoroutine(Goposition());

        // ������״̬����Ϊ��ȴ�У���ֹ��������
        isCooling = true;
    }


    IEnumerator Goposition()
    {
        // ����Ҫ�ƶ�����Ŀ��λ�ã�����ײ����� + ����߶ȵ�һ�� = �����������ĵ�
        var enemyPos = enemy.position + new Vector3(0, enemy.GetComponent<SpriteRenderer>().size.y / 2, 0);

        // ֻҪ��ǰ�������Ŀ��㻹����0.1�ף��ͼ����ƶ�
        while (Vector2.Distance(transform.position, enemyPos) > 0.1f)
        {
            // �����ƶ����򣺴ӵ�ǰλ��ָ��Ŀ��λ�ã�����׼���ɳ���Ϊ1������
            Vector3 direction = (enemyPos - transform.position).normalized;

            // ������һ֡Ҫ�ƶ��ľ��룺���� �� �ٶ� �� ʱ��
            Vector3 moveAmount = direction * moveSpeed * Time.deltaTime;

            // ʵ���ƶ��������嵱ǰλ�ü�����һ֡Ҫ�ƶ��ľ���
            transform.position += moveAmount;

            // ��ͣһ֡���ȴ���һ֡�ټ���ִ�����ѭ��
            yield return null;
        }

        
        // �ر���������ײ�壬ʹ���ܹ�����˷�����ײ���
        gameObject.GetComponent<CapsuleCollider2D>().enabled = false;

        // ����Ŀ��λ�ú󣬿�ʼִ�з���ԭλ�õ�Э��
        StartCoroutine(ReturnPosition());

        
    }

    IEnumerator ReturnPosition()
    {
        // ѭ����������������뱾������ϵԭ�����0.1����λʱ�����ƶ�
        // Vector3.zero �� (0,0,0)��transform.localPosition ������ڸ������λ��
        // ���ѭ����������ص����ĳ�ʼλ�ã�����ڸ����壩
        while ((Vector3.zero - transform.localPosition).magnitude > 0.1f)
        {
            // �����ƶ����򣺴ӵ�ǰλ��ָ��ԭ�㣬����׼��Ϊ����Ϊ1������
            Vector3 direction = (Vector3.zero - transform.localPosition).normalized;

            // �ƶ����壺��ǰλ�� + ���� �� �ٶ� �� ʱ��
            // ������ÿ֡��ԭ���ƶ�һС�ξ���
            transform.localPosition += direction * moveSpeed * Time.deltaTime;

            // ��ͣһ֡���ȴ���һ֡����ִ���ƶ�
            // �����������ƶ�����ƽ���ֲ��ڲ�ͬ֡��
            yield return null;
        }

        //�ع�ԭ�������׼����ʽ�������̸ı�ת��
        isAiming = true;

    }
}
