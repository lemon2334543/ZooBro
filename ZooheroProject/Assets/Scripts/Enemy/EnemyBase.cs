<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
=======
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
using Enemy;
>>>>>>> Stashed changes
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    public float hp; //Ñªï¿½ï¿½
    public float damage; //ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½
    public float speed; //ï¿½Æ¶ï¿½ï¿½Ù¶ï¿½
    public float attackTime; //ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Ê±
    public float attackTimer = 0; //ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Ê±ï¿½ï¿½
    public bool isContact = false; //ï¿½Ç·ï¿½Ó´ï¿½ï¿½ï¿½ï¿½
    public bool isCooling = false; //ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½È´
    public int provideExp = 1; //ï¿½ï¿½ï¿½ï¿½Öµ

    public GameObject money_prefab;//ï¿½ï¿½ï¿½Ô¤ï¿½ï¿½ï¿½ï¿½
 

    private void Awake()
    {
        money_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Money");//ä¿®æ”¹å
        // money_prefab = Resources.Load<GameObject>("Prefabs/Money");  //ä¿®æ”¹å‰
=======
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes

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

    
    private void Awake()
    {
        money_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Money");//ĞŞ¸Äºó
        // money_prefab = Resources.Load<GameObject>("Prefabs/Money");  //ĞŞ¸ÄÇ°
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        //ï¿½Ãµï¿½ï¿½ï¿½Ò»ï¿½ï¿½ï¿½ï¿½Ö±ï¿½ß¾ï¿½ï¿½ë£¬È»ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ * ï¿½Ù¶ï¿½ * ï¿½Ì¶ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Ù¶ï¿½
=======
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        //µÃµ½¹éÒ»»¯µÄÖ±Ïß¾àÀë£¬È»ºóµ÷ÓÃ ¾àÀë * ËÙ¶È * ¹Ì¶¨ÔËĞĞËÙ¶È
        //ï¿½Ãµï¿½ï¿½ï¿½Ò»ï¿½ï¿½ï¿½ï¿½Ö±ï¿½ß¾ï¿½ï¿½ë£¬È»ï¿½ï¿½ï¿½ï¿½ï¿? ï¿½ï¿½ï¿½ï¿½ * ï¿½Ù¶ï¿½ * ï¿½Ì¶ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Ù¶ï¿½
        if (skilling)
        {
            return;
        }
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream

        //ï¿½Ğ¶Ï±ï¿½ï¿½Î¹ï¿½ï¿½ï¿½ï¿½Ç·ï¿½ï¿½ï¿½ï¿½ï¿½
=======
        
        //ÅĞ¶Ï±¾´Î¹¥»÷ÊÇ·ñËÀÍö
>>>>>>> Stashed changes
=======
        
        //ÅĞ¶Ï±¾´Î¹¥»÷ÊÇ·ñËÀÍö
>>>>>>> Stashed changes
=======
        
        //ÅĞ¶Ï±¾´Î¹¥»÷ÊÇ·ñËÀÍö
>>>>>>> Stashed changes
=======
        
        //ÅĞ¶Ï±¾´Î¹¥»÷ÊÇ·ñËÀÍö
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        //ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Ò¾ï¿½ï¿½ï¿½Öµ
        Player.Instance.exp += provideExp;
=======
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        //Ôö¼ÓÍæ¼Ò¾­ÑéÖµ
        GameManager.Instance.exp += EnemyDate.provideExp;
        //ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Ò¾ï¿½ï¿½ï¿½Ö?
        Player.Instance.exp += EnemyDate.provideExp;
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        GamePanel.Instance.RenewExp();

        //µôÂä½ğ±Ò
        Instantiate(money_prefab, transform.position, Quaternion.identity);

        //Ïú»Ù×Ô¼º
        Destroy(gameObject);
    }

}