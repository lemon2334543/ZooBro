using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public float hp; //ÑªÁ¿
    public float damage; //¹¥»÷Á¦
    public float speed; //ÒÆ¶¯ËÙ¶È
    public float attackTime; //¹¥»÷¶¨Ê±
    public float attackTimer = 0; //¹¥»÷¶¨Ê±Æ÷
    public bool isContact = false; //ÊÇ·ñ½Ó´¥Íæ¼Ò
    public bool isCooling = false; //¹¥»÷ÀäÈ´
    public int provideExp = 1; //¾­ÑéÖµ

    public GameObject money_prefab;//½ð±ÒÔ¤ÖÆÌå
 

    private void Awake()
    {
        money_prefab = Resources.Load<GameObject>("Prefabs/Money");
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


        Move();//ÒÆ¶¯

        //¹¥»÷ÅÐ¶Ï
        if (isContact && !isCooling)
        {
            Attack();
        }

        //¸üÐÂ¼ÆÊ±Æ÷
        if (isCooling)
        {
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0)
            {
                attackTimer = 0;
                isCooling = false;
            }
        }

    }

    public void OnTriggerEnter2D(Collider2D other)
    { 
        if (other.CompareTag("Player"))
        {
            isContact = true;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        isContact = false;
    }

    //×Ô¶¯ÒÆ¶¯
    public void Move() 
    {
        //µÃµ½¹éÒ»»¯µÄÖ±Ïß¾àÀë£¬È»ºóµ÷ÓÃ ¾àÀë * ËÙ¶È * ¹Ì¶¨ÔËÐÐËÙ¶È
        Vector2 direction = (Player.Instance.transform.position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime);

        TurnAround();
    }


    //×Ô¶¯×ªÏò
    public void TurnAround() 
    {
        //¼ì²â¾àÀëÏà¼õÖªµÀ·½Ïò
        if (Player.Instance.transform.position.x - transform.position.x >= 0.1)
        {
            //È¡localScale.x¾ø¶ÔÖµÕâÑù×Ó²»»áµ¼ÖÂºóËõ·ÅÎÊÌâ
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (Player.Instance.transform.position.x - transform.position.x < 0.1)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }


    //¹¥»÷
    public void Attack() 
    {
        //Èç¹û¹¥»÷ÀäÈ´£¬Ôò·µ»Ø
        if (isCooling)
        {
            return;
        }

        Player.Instance.Injured(damage);

        //¹¥»÷½øÈëÀäÈ´
        isCooling = true;
        attackTimer = attackTime;
    }

    //ÊÜÉË
    public void Injured(float attack)
    {
        //if (isDead)
        //{
        //    return;
        //}

        //ÅÐ¶Ï±¾´Î¹¥»÷ÊÇ·ñËÀÍö
        if (hp - attack <= 0)
        {
            hp = 0;
            Dead();
        }
        else
        {
            hp -= attack;
        }



    }



    //ËÀÍö
    public void Dead()
    {
<<<<<<< Updated upstream
        //Ôö¼ÓÍæ¼Ò¾­ÑéÖµ
        Player.Instance.exp += provideExp;
=======
        //ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Ò¾ï¿½ï¿½ï¿½Öµ
        GameManager.Instance.exp += provideExp;
>>>>>>> Stashed changes
        GamePanel.Instance.RenewExp();

        //µôÂä½ð±Ò
        Instantiate(money_prefab, transform.position, Quaternion.identity);

        //Ïú»Ù×Ô¼º
        Destroy(gameObject);
    }

}
