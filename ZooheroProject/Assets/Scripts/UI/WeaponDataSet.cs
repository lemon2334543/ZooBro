using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class WeaponDataSet : MonoBehaviour
{
    public WeaponDataSet Instance;
    public WeaponData WeaponData;
    public TextMeshProUGUI _WeaponName;
    public Image _WeaponImage;
    public TextMeshProUGUI _WeaponInfo;
    public TextMeshProUGUI _Pricetext;
    public GameObject _AttckspeedCirtext;
    public GameObject _attckText;
    public GameObject _attckText1;
    public Image _BackColor;
    public bool isBuy = false;
    public bool isSell = false;
    public bool isEquipbord = false;
    
    
    public int num;
    private void Awake()
    {

        Instance = this;
        _WeaponName = transform.Find("WeaponNameBack").Find("WeaponName").GetComponent<TextMeshProUGUI>();
        _WeaponInfo = transform.Find("WeaponInfoBack").Find("WeaponInfo").GetComponent<TextMeshProUGUI>();
        _Pricetext = transform.Find("Price").Find("Pricetext").GetComponent<TextMeshProUGUI>();

        _AttckspeedCirtext = transform.Find("AttacInfo").Find("AttckspeedCir").Find("attckspeedText").gameObject;
        _attckText = transform.Find("AttacInfo").Find("AttckCir").Find("attckText").gameObject;
        _attckText1 = transform.Find("AttacInfo").Find("AttckCir").Find("attckText (1)").gameObject;
        
        _WeaponImage = transform.Find("WeaponImageBack").Find("WeaponImage").GetComponent<Image>();

        _BackColor = transform.Find("BackColor").GetComponent<Image>();

    }

    void Start()
    {
        transform.rotation =  Quaternion.Euler(0, 0, GetRandomFloat());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setDate(WeaponData weaponData, int num)
    {
        this.num = num;
        int x = -510 + 344 * this.num;
        Instance.transform.localPosition = new Vector3(x, 0, 0);
        
        this.WeaponData = weaponData;
        _WeaponName.text = weaponData.name;
        _WeaponInfo.text = weaponData.describe;
        _Pricetext.text = weaponData.price.ToString();
        _WeaponImage.sprite = UnityEngine.Resources.Load<Sprite>(weaponData.avatar);

        _AttckspeedCirtext.GetComponent<TextMeshProUGUI>().text = weaponData.cooling.ToString();
        _attckText.GetComponent<TextMeshProUGUI>().text = weaponData.damage.ToString();

        if (weaponData.attackcount==1)
        {
            GameManager.Instance.GameObjectHide(_attckText1.GetComponent<CanvasGroup>());
        }
        else
        {
            GameManager.Instance.GameObjectShow(_attckText1.GetComponent<CanvasGroup>());
            _attckText1.GetComponent<TextMeshProUGUI>().text = "X" + weaponData.attackcount;
        }

        if (weaponData.rank==1)
        {
            _BackColor.color = GameManager.Instance.color0;
        }
        else if (weaponData.rank==2)
        {
            _BackColor.color = GameManager.Instance.color1;
        }
        else if (weaponData.rank==3)
        {
            _BackColor.color = GameManager.Instance.color2;
        }
        else if (weaponData.rank==4)
        {
            _BackColor.color = GameManager.Instance.color3;
        }
        else if (weaponData.rank==5)
        {
            _BackColor.color = GameManager.Instance.color4;
        }
        else if (weaponData.rank==6)
        {
            _BackColor.color = GameManager.Instance.color5;
        }

        

    }

    public void setDateForProp(WeaponData weaponData,int num)
    {
        this.num = num;
        
// 设置最终位置
        Instance.transform.localPosition = new Vector3(CalculateX(num), -30, 0);
        
        
        transform.rotation =  Quaternion.Euler(0.8f, 0.8f, GetRandomFloat());
        this.WeaponData = weaponData;
        _WeaponName.text = weaponData.name;
        _WeaponInfo.text = weaponData.describe;
        _Pricetext.text = weaponData.price.ToString();
        _WeaponImage.sprite = UnityEngine.Resources.Load<Sprite>(weaponData.avatar);

        _AttckspeedCirtext.GetComponent<TextMeshProUGUI>().text = weaponData.cooling.ToString();
        _attckText.GetComponent<TextMeshProUGUI>().text = weaponData.damage.ToString();

        if (weaponData.attackcount==1)
        {
            GameManager.Instance.GameObjectHide(_attckText1.GetComponent<CanvasGroup>());
        }
        else
        {
            GameManager.Instance.GameObjectShow(_attckText1.GetComponent<CanvasGroup>());
            _attckText1.GetComponent<TextMeshProUGUI>().text = "X" + weaponData.attackcount;
        }

        if (weaponData.rank==1)
        {
            _BackColor.color = GameManager.Instance.color0;
        }
        else if (weaponData.rank==2)
        {
            _BackColor.color = GameManager.Instance.color1;
        }
        else if (weaponData.rank==3)
        {
            _BackColor.color = GameManager.Instance.color2;
        }
        else if (weaponData.rank==4)
        {
            _BackColor.color = GameManager.Instance.color3;
        }
        else if (weaponData.rank==5)
        {
            _BackColor.color = GameManager.Instance.color4;
        }
        else if (weaponData.rank==6)
        {
            _BackColor.color = GameManager.Instance.color5;
        }


    }
    
    public float GetRandomFloat()
    {
        // Random.Range(float min, float max) 的 max 是 inclusive（包含）
        return UnityEngine.Random.Range(-3.0f, 3.0f);
    }

    public int CalculateX(int num)
    {
        
// 核心参数（已明确：卡片宽度344，容器宽度1339.428）
        float cardWidth = 344f;
        float containerWidth = 900.428f;

// 获取武器列表总数（避免空列表报错，最少按1个计算）
        int totalCount = Mathf.Max(GameManager.Instance.NotEquippedcurrentWeapons.Count, 1);

// 计算总可分配的“间距空间”（容器宽度 - 所有卡片宽度总和）
        float totalSpacingSpace = containerWidth - (cardWidth * totalCount);

// 计算单个间距（总间距空间 ÷ 间距数量，n个卡片有n+1个间距，左右留边更合理）
        float singleSpacing = totalCount > 0 ? totalSpacingSpace / (totalCount + 1) : 0;

// 计算x坐标：num=0时固定-850，其余按“卡片宽度+单个间距”累加偏移
// 偏移基准 = 卡片宽度 + 单个间距（确保相邻卡片间距一致）
        float offsetStep = cardWidth + singleSpacing;
        int x = (int)(-630 + offsetStep * num);
        return x;
    }
}
