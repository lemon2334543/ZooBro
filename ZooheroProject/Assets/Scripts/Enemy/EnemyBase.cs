using Enemy;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
<<<<<<< Updated upstream
    public float hp; //ÑªÁ¿
    public float damage; //¹¥»÷Á¦
    public float speed; //ÒÆ¶¯ËÙ¶È
    public float attackTime; //¹¥»÷¶¨Ê±
    public float attackTimer = 0; //¹¥»÷¶¨Ê±Æ÷
    public bool isContact = false; //ÊÇ·ñ½Ó´¥Íæ¼Ò
    public bool isCooling = false; //¹¥»÷ÀäÈ´
    public int provideExp = 1; //¾­ÑéÖµ

    public GameObject money_prefab;//½ğ±ÒÔ¤ÖÆÌå
 
=======
    // public float hp; //Ñªï¿½ï¿½
    // public float damage; //ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½
    // public float speed; //ï¿½Æ¶ï¿½ï¿½Ù¶ï¿½
    // public float attackTime; //ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½
    // public int provideExp = 1; //ï¿½ï¿½ï¿½ï¿½
    //
    
    public float attackTimer = 0; //ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Ê±ï¿½ï¿½
    public bool isContact = false; //ï¿½Ç·ï¿½Ó´ï¿½ï¿½ï¿½ï¿½
    public bool isCooling = false; //ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½È´
    

    public GameObject money_prefab;//ï¿½ï¿½ï¿½Ô¤ï¿½ï¿½ï¿½ï¿½
    
    [SerializeField]//ç¬¬å…«é›†
    public   EnemyDate EnemyDate;
>>>>>>> Stashed changes

    
    //æŠ€èƒ½è®¡ç®—å™¨
    public float skillTimer = 0;
    public bool skilling = false;//æŠ€èƒ½æŒç»­

    
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

        //¹¥»÷ÅĞ¶Ï
        if (isContact && !isCooling)
        {
            Attack();
        }

        //¸üĞÂ¼ÆÊ±Æ÷
        if (isCooling)
        {
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0)
            {
                attackTimer = 0;
                isCooling = false;
            }
        }

        UpdateSkill();

    }

    public void SetElite()
    {
        EnemyDate.hp *= 2;
        EnemyDate.damage *= 2;
        GetComponent<SpriteRenderer>().color = new Color(255 / 255f, 113 / 255f, 113 / 255f);
    }
    
    private void UpdateSkill()
    {
        if (EnemyDate.SkillTime<0)
        {
            return;
        }

        if (skillTimer<=0)
        {
            float dis = Vector2.Distance(transform.position, Player.Instance.transform.position);
            if (dis <= EnemyDate.range)
            {
                //è·ç¦»åˆ¤å®šï¼Œå‘åŠ¨æŠ€èƒ½ï¼›
                Vector2 dir = (Player.Instance.transform.position - transform.position).normalized;
                LaunchSkill(dir);
                skillTimer = EnemyDate.SkillTime;
            }

        }
        else
        {
            skillTimer -= Time.deltaTime;
            if (skillTimer<0)
            {
                skillTimer = 0;
            }
        }
    }
    //å­ç±»å®ç°
    public virtual void LaunchSkill(Vector2 dir)
    {
        
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
<<<<<<< Updated upstream
        //µÃµ½¹éÒ»»¯µÄÖ±Ïß¾àÀë£¬È»ºóµ÷ÓÃ ¾àÀë * ËÙ¶È * ¹Ì¶¨ÔËĞĞËÙ¶È
=======
        //ï¿½Ãµï¿½ï¿½ï¿½Ò»ï¿½ï¿½ï¿½ï¿½Ö±ï¿½ß¾ï¿½ï¿½ë£¬È»ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ * ï¿½Ù¶ï¿½ * ï¿½Ì¶ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Ù¶ï¿½
        if (skilling)
        {
            return;
        }
>>>>>>> Stashed changes
        Vector2 direction = (Player.Instance.transform.position - transform.position).normalized;
        transform.Translate(direction * EnemyDate.speed * Time.deltaTime);

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

        Player.Instance.Injured(EnemyDate.damage);

        //¹¥»÷½øÈëÀäÈ´
        isCooling = true;
        attackTimer = EnemyDate.attackTime;
    }

    //ÊÜÉË
    public void Injured(float attack)
    {
        //if (isDead)
        //{
        //    return;
        //}

<<<<<<< Updated upstream
        //ÅĞ¶Ï±¾´Î¹¥»÷ÊÇ·ñËÀÍö
        if (hp - attack <= 0)
=======
        //ï¿½Ğ¶Ï±ï¿½ï¿½Î¹ï¿½ï¿½ï¿½ï¿½Ç·ï¿½ï¿½ï¿½ï¿½ï¿½
        if (EnemyDate.hp - attack <= 0)
>>>>>>> Stashed changes
        {
            EnemyDate.hp = 0;
            Dead();
        }
        else
        {
            EnemyDate.hp -= attack;
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
        Player.Instance.exp += EnemyDate.provideExp;
>>>>>>> Stashed changes
        GamePanel.Instance.RenewExp();

        //µôÂä½ğ±Ò
        Instantiate(money_prefab, transform.position, Quaternion.identity);

        //Ïú»Ù×Ô¼º
        Destroy(gameObject);
    }

}
