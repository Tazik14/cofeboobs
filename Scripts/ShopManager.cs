using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ShopConfiguration
{
    public int cupIndex;
    public int cupPrice;
    public bool isSold;
    public string cupName;
    public Sprite cupSprite;
    public GameObject cupObject;
}

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    public int coins;
    public TextMeshProUGUI coinText;

    public ShopConfiguration[] cups;
    public Transform cupsRoot;
    public GameObject cupButton;
    private GameObject cupEquip;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        CreateShop();
        ChangeCup(0);
    }

    public void CreateShop()
    {
        ClearRoot();
        for (int i = 0; i < cups.Length; i++)
        {
            GameObject obj = Instantiate(cupButton, cupsRoot);

            Image ing = obj.GetComponent<Image>();
            ing.sprite = cups[i].cupSprite;

            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = cups[i].cupName;

            if (cups[i].isSold)
            {
                Button di = obj.GetComponent<Button>();
                di.onClick.AddListener(() => ChangeCup(obj.transform.GetSiblingIndex()));
                obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Equip";
            }
            else
            {
                Button di = obj.GetComponent<Button>();
                di.onClick.AddListener(() => BuyCup(obj.transform.GetSiblingIndex()));
                obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = cups[i].cupPrice + " $";
            }
        }
    }

    public void ClearRoot()
    {
        foreach (Transform child in cupsRoot)
        {
            Destroy(child.gameObject);
        }
    }

    public void ChangeCup(int index)
    {
        if (cupEquip != null)
        {
            Destroy(cupEquip);
        }

        cupEquip = Instantiate(cups[index].cupObject, CoffeManager.instance.coffeCupSpawn);
    }

    public void BuyCup(int index)
    {
        if (coins >= cups[index].cupPrice)
        {
            coins -= cups[index].cupPrice;
            cups[index].isSold = true;
            ChangeCup(index);
            CreateShop();

            coinText.text = coins + " $";
        }
    }

    public void AddMoney(int value)
    {
        coins += value;
        coinText.text = coins + " $";
    }
}
