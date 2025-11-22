using System;
using UnityEngine;
using UnityEngine.UI;

public class Weaponset : MonoBehaviour
{
    public Weaponset Instance;
    public Image _avater;
    public Image _backImage;
    public WeaponData WeaponData;

    private void Awake()
    {
        Instance = this;
        _avater = transform.Find("propIcom").GetComponent<Image>();
        _backImage = transform.GetComponent<Image>();

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setDate(WeaponData weaponData)
    {
        transform.GetComponent<CheckThereOne>().WeaponData = weaponData;
        this.WeaponData = weaponData;
        _avater.sprite = UnityEngine.Resources.Load<Sprite>(weaponData.avatar);

        if (weaponData.rank==1)
        {
            _backImage.color = GameManager.Instance.color0;
        }
        else if (weaponData.rank == 2)
        {
            _backImage.color = GameManager.Instance.color1;
        }
        else if (weaponData.rank == 3)
        {
            _backImage.color = GameManager.Instance.color2;
        }
        else if (weaponData.rank == 4)
        {
            _backImage.color = GameManager.Instance.color3;
        }
        else if (weaponData.rank == 5)
        {
            _backImage.color = GameManager.Instance.color4;
        }
        else if (weaponData.rank == 6)
        {
            _backImage.color = GameManager.Instance.color5;
        }
        
    }
}
