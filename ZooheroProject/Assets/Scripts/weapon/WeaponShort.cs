using NUnit.Framework;
using UnityEngine;

public class WeaponShort : WeaponBase
{
<<<<<<< Updated upstream
    // µ±ÎäÆ÷Åö×²ÌåÓëÆäËûÅö×²Ìå½Ó´¥Ê±×Ô¶¯µ÷ÓÃ
=======

    public new void Awake()
    {
        //çˆ¶å…ƒç´ Awake
        base.Awake();

        moveSpeed = 10;

    }


    //è¿‘æˆ˜å¼€ç«
    public override IEnumerator Fire()
    {
        // æ£€æŸ¥æ­¦å™¨æ˜¯å¦åœ¨å†·å´ä¸­ï¼Œå¦‚æœæ˜¯åˆ™ç›´æ¥é€€å‡ºï¼Œä¸æ‰§è¡Œå‘å°„
        if (isCooling)
        {
            yield break;
        }
        isCooling = true;
        for (int i = 0; i < data.attackcount; i++)
        {
            // å¯ç”¨æ­¦å™¨çš„ç¢°æ’ä½“ï¼Œä½¿å…¶èƒ½å¤Ÿä¸æ•Œäººå‘ç”Ÿç¢°æ’æ£€æµ‹
            gameObject.GetComponent<CapsuleCollider2D>().enabled = true;

            //å…³é—­ç„å‡†ç§»åŠ¨æ—¶å€™ä¸æ”¹å˜å‡ºå»æ–¹å‘
            isAiming = false;

            // å¯åŠ¨åç¨‹ï¼šè®©æ­¦å™¨å‘æ•Œäººä½ç½®ç§»åŠ¨
            StartCoroutine(Goposition());
            
            yield return new WaitForSeconds(0.3f);
        }

       

        // å°†æ­¦å™¨çŠ¶æ€è®¾ç½®ä¸ºå†·å´ä¸­ï¼Œé˜²æ­¢è¿ç»­å‘å°„
        isCooling = true;
    }

    // å½“æ­¦å™¨ç¢°æ’ä½“ä¸å…¶ä»–ç¢°æ’ä½“æ¥è§¦æ—¶è‡ªåŠ¨è°ƒç”¨
>>>>>>> Stashed changes
    private void OnTriggerEnter2D(Collider2D col)
    {
        // ¼ì²éÅö×²µ½µÄÎïÌåÊÇ·ñ±ê¼ÇÎª"Enemy"±êÇ©
        if (col.CompareTag("Enemy"))
        {
<<<<<<< Updated upstream
            // ¶ÔµĞÈËÔì³ÉÉËº¦£º»ñÈ¡µĞÈË×é¼ş²¢µ÷ÓÃÊÜÉË·½·¨£¬´«ÈëÎäÆ÷ÉËº¦Öµ
            col.GetComponent<EnemyBase>().Injured(data.damage);

            // Á¢¼´¹Ø±ÕÎäÆ÷µÄÅö×²Ìå£¬·ÀÖ¹Í¬Ò»Ö¡ÄÚ¶à´Î´¥·¢ÉËº¦
            gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
=======
            // å¯¹æ•Œäººé€ æˆä¼¤å®³ï¼šè·å–æ•Œäººç»„ä»¶å¹¶è°ƒç”¨å—ä¼¤æ–¹æ³•ï¼Œä¼ å…¥æ­¦å™¨ä¼¤å®³å€¼
           

            bool isCritcal = CriicalHits();
            if (isCritcal)  
            {
                //äº§ç”Ÿæš´å‡»
                col.GetComponent<EnemyBase>().Injured(data.damage*=data.critical_strikes_multiple);
            }
            else
            {
                //æ²¡æš´å‡»
                col.GetComponent<EnemyBase>().Injured(data.damage);
            }
            
            
            // ç«‹å³å…³é—­æ­¦å™¨çš„ç¢°æ’ä½“ï¼Œé˜²æ­¢åŒä¸€å¸§å†…å¤šæ¬¡è§¦å‘ä¼¤å®³
            //gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
>>>>>>> Stashed changes
        }
    }
}
