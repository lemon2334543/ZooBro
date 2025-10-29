<<<<<<< HEAD
ï»¿using UnityEngine;
=======
using Enemy;
using UnityEngine;
>>>>>>> Bidoofa2

public class EnemyBase : MonoBehaviour
{
<<<<<<< HEAD
    public float hp; //Ñªï¿½ï¿½
    public float damage; //ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½
    public float speed; //ï¿½Æ¶ï¿½ï¿½Ù¶ï¿½
    public float attackTime; //ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Ê±
    public float attackTimer = 0; //ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Ê±ï¿½ï¿½
    public bool isContact = false; //ï¿½Ç·ï¿½Ó´ï¿½ï¿½ï¿½ï¿½
    public bool isCooling = false; //ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½È´
    public int provideExp = 1; //ï¿½ï¿½ï¿½ï¿½Öµ

    public GameObject money_prefab;//ï¿½ï¿½ï¿½Ô¤ï¿½ï¿½ï¿½ï¿½
 
=======

    public float hp; //ÑªÁ¿
    public float damage; //¹¥»÷Á¦
    public float speed; //ÒÆ¶¯ËÙ¶È
    public float attackTime; //¹¥»÷¶¨Ê±
    public float attackTimer = 0; //¹¥»÷¶¨Ê±Æ÷
    public bool isContact = false; //ÊÇ·ñ½Ó´¥Íæ¼Ò
    public bool isCooling = false; //¹¥»÷ÀäÈ´
    public int provideExp = 1; //¾­ÑéÖµ

    public GameObject money_prefab;//????????
    
    [SerializeField]//µÚ°Ë¼¯
    public   EnemyDate EnemyDate;
    
    //¼¼ÄÜ¼ÆËãÆ÷
    public float skillTimer = 0;
    public bool skilling = false;//¼¼ÄÜ³ÖĞø
>>>>>>> ç¬¬äºŒéƒ¨åˆ†Test

    
    private void Awake()
    {
<<<<<<< HEAD
        money_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Money");//ä¿®æ”¹å
        // money_prefab = Resources.Load<GameObject>("Prefabs/Money");  //ä¿®æ”¹å‰
=======
        money_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Money");//ĞŞ¸Äºó
        // money_prefab = Resources.Load<GameObject>("Prefabs/Money");  //ĞŞ¸ÄÇ°
>>>>>>> ç¬¬äºŒéƒ¨åˆ†Test
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


        Move();//ï¿½Æ¶ï¿½

        //ï¿½ï¿½ï¿½ï¿½ï¿½Ğ¶ï¿½
        if (isContact && !isCooling)
        {
            Attack();
        }

        //ï¿½ï¿½ï¿½Â¼ï¿½Ê±ï¿½ï¿½
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
                //¾àÀëÅĞ¶¨£¬·¢¶¯¼¼ÄÜ£»
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
    //×ÓÀàÊµÏÖ
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

    //ï¿½Ô¶ï¿½ï¿½Æ¶ï¿½
    public void Move() 
    {
<<<<<<< HEAD
        //ï¿½Ãµï¿½ï¿½ï¿½Ò»ï¿½ï¿½ï¿½ï¿½Ö±ï¿½ß¾ï¿½ï¿½ë£¬È»ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ * ï¿½Ù¶ï¿½ * ï¿½Ì¶ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Ù¶ï¿½
=======
        //µÃµ½¹éÒ»»¯µÄÖ±Ïß¾àÀë£¬È»ºóµ÷ÓÃ ¾àÀë * ËÙ¶È * ¹Ì¶¨ÔËĞĞËÙ¶È
        //ï¿½Ãµï¿½ï¿½ï¿½Ò»ï¿½ï¿½ï¿½ï¿½Ö±ï¿½ß¾ï¿½ï¿½ë£¬È»ï¿½ï¿½ï¿½ï¿½ï¿? ï¿½ï¿½ï¿½ï¿½ * ï¿½Ù¶ï¿½ * ï¿½Ì¶ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Ù¶ï¿½
        if (skilling)
        {
            return;
        }
>>>>>>> ç¬¬äºŒéƒ¨åˆ†Test
        Vector2 direction = (Player.Instance.transform.position - transform.position).normalized;
        transform.Translate(direction * EnemyDate.speed * Time.deltaTime);

        TurnAround();
    }


    //ï¿½Ô¶ï¿½×ªï¿½ï¿½
    public void TurnAround() 
    {
        //ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Öªï¿½ï¿½ï¿½ï¿½ï¿½ï¿½
        if (Player.Instance.transform.position.x - transform.position.x >= 0.1)
        {
            //È¡localScale.xï¿½ï¿½ï¿½ï¿½Öµï¿½ï¿½ï¿½ï¿½ï¿½Ó²ï¿½ï¿½áµ¼ï¿½Âºï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (Player.Instance.transform.position.x - transform.position.x < 0.1)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }


    //ï¿½ï¿½ï¿½ï¿½
    public void Attack() 
    {
        //ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½È´ï¿½ï¿½ï¿½ò·µ»ï¿½
        if (isCooling)
        {
            return;
        }

        Player.Instance.Injured(EnemyDate.damage);

        //ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½È´
        isCooling = true;
        attackTimer = EnemyDate.attackTime;
    }

    //ï¿½ï¿½ï¿½ï¿½
    public void Injured(float attack)
    {
        //if (isDead)
        //{
        //    return;
        //}
<<<<<<< HEAD

        //ï¿½Ğ¶Ï±ï¿½ï¿½Î¹ï¿½ï¿½ï¿½ï¿½Ç·ï¿½ï¿½ï¿½ï¿½ï¿½
=======
        
        //ÅĞ¶Ï±¾´Î¹¥»÷ÊÇ·ñËÀÍö
>>>>>>> ç¬¬äºŒéƒ¨åˆ†Test
        if (hp - attack <= 0)
        //ï¿½Ğ¶Ï±ï¿½ï¿½Î¹ï¿½ï¿½ï¿½ï¿½Ç·ï¿½ï¿½ï¿½ï¿½ï¿½
        if (EnemyDate.hp - attack <= 0)
        {
            EnemyDate.hp = 0;
            Dead();
        }
        else
        {
            EnemyDate.hp -= attack;
        }



    }



    //ï¿½ï¿½ï¿½ï¿½
    public void Dead()
    {
<<<<<<< HEAD
        //ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Ò¾ï¿½ï¿½ï¿½Öµ
=======
<<<<<<< Updated upstream
        //Ôö¼ÓÍæ¼Ò¾­ÑéÖµ
>>>>>>> ç¬¬äºŒéƒ¨åˆ†Test
        Player.Instance.exp += provideExp;
<<<<<<< HEAD
=======
        //ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Ò¾ï¿½ï¿½ï¿½Öµ
        GameManager.Instance.exp += provideExp;
>>>>>>> Stashed changes
=======
        //ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Ò¾ï¿½ï¿½ï¿½Ö?
        Player.Instance.exp += EnemyDate.provideExp;
>>>>>>> Bidoofa2
        GamePanel.Instance.RenewExp();

        //ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½
        Instantiate(money_prefab, transform.position, Quaternion.identity);

        //ï¿½ï¿½ï¿½ï¿½ï¿½Ô¼ï¿½
        Destroy(gameObject);
    }

}
