using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientQuestData
{
    public string questText;
    public List<int> questIngredients;

    public IngredientQuestData(string ingText, List<int> ing)
    {
        questText = ingText;
        questIngredients = ing;
    }
}

[System.Serializable]
public class IngredientConfiguration
{
    public int ingredientIndex;
    public string ingredientName;
    public Sprite ingredientSprite;
    public GameObject ingredientObject;
}

public class CoffeManager : MonoBehaviour
{
    public static CoffeManager instance;

    [Header("Configuration")]
    public IngredientConfiguration[] ingredients;
    public Transform ingredientRoot;
    public GameObject ingredientButton;
    public Transform winRoot;
    public Transform loseRoot;

    private List<int> ingredientAdded = new List<int>();
    private List<int> ingredientQuest;
    private IngredientQuestData questData;

    [Space(10)]
    public TextMeshProUGUI textIngredients;
    public TextMeshPro progressText;
    public int coffeProgress;

    [Header("Objects")]
    public Transform coffeCupSpawn;

    public ParticleSystem stinkyParticle;
    public ParticleSystem cloudParticle;
    public Material coffeStreamMat;

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
        GenerateScene();
    }

    //Забей, говнокод
    void Update()
    {
        if (ingredientAdded.Count == ingredientQuest.Count)
        {
            ingredientRoot.gameObject.SetActive(false);
        }
        else
        {
            ingredientRoot.gameObject.SetActive(true);
        }
    }

    public void GenerateScene()
    {
        //Заполнение кнопок с ингредиентами
        ClearRoot();
        for (int i = 0; i < ingredients.Length; i++)
        {
            GameObject obj = Instantiate(ingredientButton, ingredientRoot);

            Image ing = obj.GetComponent<Image>();
            ing.sprite = ingredients[i].ingredientSprite;

            DragableItem di = obj.GetComponent<DragableItem>();
            di.objectIndex = ingredients[i].ingredientIndex;
            di.Object = ingredients[i].ingredientObject;

            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ingredients[i].ingredientName;
        }

        winRoot.gameObject.SetActive(false);
        loseRoot.gameObject.SetActive(false);

        //Генерация задания
        questData = IngQuest();
        textIngredients.text = questData.questText;
        ingredientQuest = questData.questIngredients;
        ingredientAdded.Clear();
        progressText.text = "0%";
        coffeProgress = 0;

        //радном цвет эффектов
        Color cloudColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        ParticleSystem.MainModule psC = cloudParticle.main;
        psC.startColor = 
            new ParticleSystem.MinMaxGradient(cloudColor, Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
        ParticleSystem.MainModule psS = stinkyParticle.main;
        psS.startColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        coffeStreamMat.color = cloudColor;
    }

    public void ReloadScene()
    {
        //Генерация задания
        textIngredients.text = questData.questText;
        ingredientQuest = questData.questIngredients;
        progressText.text = "0%";
        coffeProgress = 0;

        winRoot.gameObject.SetActive(false);
        loseRoot.gameObject.SetActive(false);

        ingredientAdded.Clear();

        cloudParticle.Stop();
        stinkyParticle.Stop();
    }

    public IngredientQuestData IngQuest()
    {
        string ingText = "";
        List<int> ing = new List<int>();
        int repeatCount = Random.Range(2, 5);

        for (int i = 0; i < repeatCount; i++)
        {
            int indx = Random.Range(0, ingredients.Length);
            ing.Add(ingredients[indx].ingredientIndex);
            ingText += ingredients[indx].ingredientName + ", ";
        }
        ingText = ingText.Substring(0, ingText.Length - 2);

        return new IngredientQuestData(ingText, ing);
    }

    public void ClearRoot()
    {
        foreach (Transform child in ingredientRoot)
        {
            Destroy(child.gameObject);
        }
    }

    public void AddIngredient(int index)
    {
        ingredientAdded.Add(index);
    }

    public bool GetIngredient()
    {
        if (ingredientAdded.Count == ingredientQuest.Count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CompleteCoffe()
    {
        if (ingredientQuest.SequenceEqual(ingredientAdded))
        {
            winRoot.gameObject.SetActive(true);
            ShopManager.instance.AddMoney(20);
        }
        else
        {
            loseRoot.gameObject.SetActive(true);
        }
    }
}
