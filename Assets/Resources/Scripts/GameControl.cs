using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    private int index, areaprice, areaunlocked, rtval, maxSlotA, maxSlotB, cancelSlotA, cancelSlotB, QtSlotA, QtSlotB, currentSlotItems, areaNumbersStart, areaNumbersEnd, secretval, exp, targetExp ,StarPrice, ShopPrice, thingsAmount, reGameAmount = 1;
    private float secretTime, STime, NTime, SCTime, NCTime, StarAmount;
    private long MoneyAmount;
    private string methodstats, itemsname, shopstatus, bagval;
    private const string hexColorCode = "#292929";
    private bool shop = false, shoplist = false, area = true, textstats = false, exchange = false, bagstat = false, fence = false, pond = false, sold = false, plantFarmActive = false, animalFarmActive = false, secretfunc = false, levelmax = false, reGame = false, scOpt = false, plant = false , animals = false;
    
    private readonly List<int> lockarea = new List<int> { 1, 2, 4, 6, 7, 9, 10, 15, 18, 20 };
    private readonly List<int> lockanimal = new List<int> { 7, 10, 11, 15, 16, 17 };
    private readonly List<int> lockseeds = new List<int> { 0, 5, 8, 12 };
    private readonly List<int> animalsprice = new List<int> { 100, 200, 500, 1000, 2000, 3000 };
    private readonly List<int> seedsprice = new List<int> { 50, 120, 150, 250 };

    private Dictionary<int, int> plantQt = new Dictionary<int, int>();
    private Dictionary<int, int> animalQt = new Dictionary<int, int>();
    private Color Lock, Unlock = Color.white;
    private Transform AnimalName, SeedsName;
    private AudioManager audioManager;

    [Header("----------------------------Player Value---------------------------------")]
    [SerializeField] private int playerlevel;
    [SerializeField] private long Star;
    [SerializeField] private ulong Money;

    [Header("-------------------GameObject & Game System Value-------------------")]
    public int arealevels;
    [SerializeField] private GameObject pause, bagBtnSet, things, slot;
    [SerializeField] private List<TextMeshProUGUI> MoneyUpText, StarUpText ,MoneyText, StarText, LevelText, AlertMessageText, AnimalNameText, AnimalPriceText, SeedsNameText, SeedsPriceText, QuantityText, SoldText, SlotText;
    [SerializeField] private List<GameObject> AreaLevelLock, alert, AreaLock, bag, AnimalLock, SeedsLock, Quantity, InventoryBag, AnimalsDisable, BtnDisable, Block, animalObj, ArrowPoint, Cloud, MoneyUp, StarUp, EndGame;
    [SerializeField] private List<Image> Area, Seeds, Animal, Slot, plants;
    [SerializeField] private List<Animator> anim;
    [SerializeField] private List<int> inventory, plantsGrowLevel, itemPrice;
    [SerializeField] private List<ulong> animalFarmMoney, plantFarmMoney, animalFarmStar, plantFarmStar;
    [SerializeField] private List<float> time;
    [SerializeField] private List<bool> plantarea, animalarea;
    [SerializeField] private List<string> PlantA, PlantB, AnimalA, AnimalB;
    [SerializeField] private List<Sprite> newSprite, CarrotGrow, CornGrow, RadishGrow, TurnipGrow;
    [SerializeField] private List<RuntimeAnimatorController> animalAnimControl;

    private void Start()
    {
        NCTime = NTime = 40f;
        TargetExperience = 100;
        ValueOfPlayerExperience = 0;
        AreaPriceUpdate = 200;
        StarPriceUpdate = 1000;
        QtSlotA = QtSlotB = 0;
        if (ColorUtility.TryParseHtmlString(hexColorCode, out Lock))
        {
            foreach (Image image in Area.Concat(Seeds).Concat(Animal))
            {
                image.color = Lock;
            }
        }
    }

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Update()
    {
        ValueSetting();
        AreaLevelUnlock();
        PauseGame();
        PrepareTextUpdate();
        PriceOfItems();
    }

    private ReadOnlyCollection<int> LockArea => lockarea.AsReadOnly();

    private ReadOnlyCollection<int> LockAnimal => lockanimal.AsReadOnly();

    private ReadOnlyCollection<int> LockSeeds => lockseeds.AsReadOnly();

    private ReadOnlyCollection<int> AnimalsPrice => animalsprice.AsReadOnly();

    private ReadOnlyCollection<int> SeedsPrice => seedsprice.AsReadOnly();

    private List<int> Inventorys
    {
        get => inventory;
        set => inventory = value;
    }

    private List<int> ItemPriceUpdate
    {
        get => itemPrice;
        set => itemPrice = value;
    }

    private int ThingsAmount
    {
        get => thingsAmount;
        set => thingsAmount = value;
    }

    private string BagValue
    {
        get => bagval;
        set => bagval = value;
    }

    private int TargetExperience
    {
        get => targetExp;
        set => targetExp = value;
    }

    private int ValueIndexOfObject
    {
        get => index;
        set => index = value;
    }

    private string ShopStatus
    {
        get => shopstatus;
        set => shopstatus = value;
    }

    private int ValueOfPlayerLevel
    {
        get => playerlevel;
        set => playerlevel = value;
    }

    private int ValueOfPlayerExperience
    {
        get => exp;
        set => exp = value;
    }

    private ulong ValueOfPlayerMoney
    {
        get => Money;
        set => Money = value;
    }

    private long MoneyAmountExchanged
    {
        get => MoneyAmount;
        set => MoneyAmount = value;
    }

    private long ValueOfPlayerStar
    {
        get => Star;
        set => Star = value;
    }

    private float AmountOfExchangeStar
    {
        get => StarAmount;
        set => StarAmount = value;
    }

    private int StarPriceUpdate
    {
        get => StarPrice;
        set => StarPrice = value;
    }

    private int AreaPriceUpdate
    {
        get => areaprice;
        set => areaprice = value;
    }

    private int AreaUnlocked
    {
        get => areaunlocked;
        set => areaunlocked = value;
    }

    private string MethodStats
    {
        get => methodstats;
        set => methodstats = value;
    }

    private int ShopPriceUpdate
    {
        get => ShopPrice;
        set => ShopPrice = value;
    }

    private string ItemsName
    {
        get => itemsname;
        set => itemsname = value;
    }

    public void Click(int val)
    {
        if (val == 0)
        {
            audioManager.PlaySFX(audioManager.Click);
        }
        else if (val == 2)
        {
            audioManager.PlaySFX(audioManager.Door);
        }
        else if (val == 3)
        {
            audioManager.PlaySFX(audioManager.Pop2);
        }
        else if (val == 4)
        {
            audioManager.PlaySFX(audioManager.Bell);
        }
        else
        {
            audioManager.PlaySFX(audioManager.BackClick);
        }
    }

    private void PlayerExperience()
    {
        if (ValueOfPlayerExperience >= TargetExperience) 
        {
            audioManager.PlaySFX(audioManager.LevelUp);
            ValueOfPlayerLevel++;
            ValueOfPlayerExperience = 0;
            TargetExperience *= 2;
        } 
    }

    public void SecretFunc()
    {
        secretfunc = true;
        if (secretval == 10)
        {
            audioManager.PlaySFX(audioManager.LevelUp);
            ValueOfPlayerLevel+= 20;
            ValueOfPlayerMoney += 5000000;
            ValueOfPlayerStar += 1000;
            scOpt = true;
            secretval = 0;
        }
        else
        {
            secretval++;
        }
    }

    private IEnumerator ResetGame()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadSceneAsync("InGame");
    }

    public void RestartGame()
    {
        SceneManager.LoadSceneAsync("InGame");
        Time.timeScale = 1f;
    }

    public void SetItemsName(string name)
    {
        ItemsName = name;
    }

    //Setting Defualt Value
    private void ValueSetting()
    {
        SCTime += 0.020f;

        if (levelmax != true)
        {
            PlayerExperience();
        }
        else
        {
            if (ValueOfPlayerLevel == 20)
            {
                levelmax = true;
            }
        }

        if (ValueOfPlayerLevel == 0)
        {
            maxSlotA = maxSlotB = 1;
        }
        else
        {
            maxSlotA = maxSlotB = ValueOfPlayerLevel;
        }

        if (shop != false)
        {
            LevelUnlock(ShopStatus);
        }

        if (bagstat != false)
        {
            InventoryUpdate(BagValue);
            SetBagQuantityTextStat("Bag", BagValue);
        }

        if (Slot[1].gameObject.activeSelf && !Slot[0].gameObject.activeSelf && bagstat != false)
        {
            QtSlotA = QtSlotB;
            QtSlotB = 0;
            Slot[0].gameObject.SetActive(true);
            SlotText[0].gameObject.SetActive(true);
            Slot[0].GetComponent<Image>().sprite = Slot[1].GetComponent<Image>().sprite;
            Slot[1].GetComponent<Image>().sprite = null;
            Slot[0].gameObject.name = Slot[1].gameObject.name;
            Slot[1].gameObject.name = "Image";
            cancelSlotA = cancelSlotB;
            Slot[1].gameObject.SetActive(false);
            SlotText[1].gameObject.SetActive(false);
        }

        if (plantFarmActive != false)
        {
            //funcTime += 0.020f;
            PlantTiming();
        }

        if (animalFarmActive != false)
        {
            //funcTime1 += 0.020f;
            AnimalsTiming();
        }

        if (secretfunc != false)
        {

            if (secretTime >= 10f)
            {
                secretTime = 0f;
                secretval = 0;
                secretfunc = false;
            }
            else
            {
                secretTime += 0.020f;
            }
        }

        if (ValueOfPlayerMoney < 0)
        {
            ValueOfPlayerMoney = 0;
        }

        if (ValueOfPlayerLevel >= 20)
        {
            EndGame[0].SetActive(true);
            ValueOfPlayerLevel = 20;
        }

        if (ValueOfPlayerLevel < 0)
        {
            ValueOfPlayerLevel = 0;
        }

        if (ValueOfPlayerMoney < 0)
        {
            ValueOfPlayerMoney = 0;
        }

        if (ValueOfPlayerStar < 0)
        {
            ValueOfPlayerStar = 0;
        }

        if (StarPriceUpdate > 1000 || StarPriceUpdate < 1000)
        {
            StarPriceUpdate = 1000;
        }

        if (AmountOfExchangeStar < 0) 
        {
            AmountOfExchangeStar = 0;
            MoneyAmountExchanged = 0;
        }

        if (ThingsAmount < 0)
        {
            ThingsAmount = 0;
        }

        if (MoneyAmount < 0)
        {
            MoneyAmount = 0;
        }

        if (SCTime < NCTime && ValueOfPlayerLevel >= 20 && scOpt != true || SCTime < NCTime && ValueOfPlayerMoney > 1000 && scOpt != true || SCTime < NCTime && ValueOfPlayerStar > 50 && scOpt != true)
        {
            reGame = true;
            if (reGame != false && reGameAmount == 1)
            {
                audioManager.PlaySFX(audioManager.Pop2);
                alert[1].SetActive(true);
                alert[10].SetActive(true);
                StartCoroutine(ResetGame());
                reGameAmount++;
                reGame = false;
            }
        }

    }

    public void BackToGame()
    {
        Time.timeScale = 1f;
    }

    private void PauseGame()
    {

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKey("escape"))
        {
            pause.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void Setting()
    {
        pause.SetActive(true);
        Time.timeScale = 0f;
    }


    public void BuyArea()
    {
        if (ValueOfPlayerMoney >= (ulong)AreaPriceUpdate)
        {
            alert[2].SetActive(false);
            alert[1].SetActive(false);
            audioManager.PlaySFX(audioManager.Buy);
            ValueOfPlayerMoney -= (ulong)AreaPriceUpdate;
            AreaPriceUpdate *= 3;
            AreaUnlocked++;
            AreaUnlock();
        }
        else
        {
            audioManager.PlaySFX(audioManager.Pop2);
            alert[2].SetActive(false);
            alert[3].SetActive(true);
        }
    }

    private void ExchangeCalculator(int price)
    {
        AmountOfExchangeStar = Mathf.Ceil(((ulong)price - ValueOfPlayerMoney) / (float)StarPriceUpdate);
        MoneyAmountExchanged = (int)AmountOfExchangeStar * StarPriceUpdate;
        if (ValueOfPlayerMoney >= (ulong)AreaPriceUpdate && area == true)
        {
            AmountOfExchangeStar = 0;
            MoneyAmountExchanged = (int)AmountOfExchangeStar;

        }
        else if (ValueOfPlayerMoney >= (ulong)ShopPriceUpdate && shop == true)
        {
            AmountOfExchangeStar = 0;
            MoneyAmountExchanged = (int)AmountOfExchangeStar;
        }
    }

    private void AreaUnlock()
    {
        audioManager.PlaySFX(audioManager.Unlock);
        anim[1] = AreaLock[ValueIndexOfObject].GetComponent<Animator>();
        anim[1].SetTrigger("isFadeout");
        Area[ValueIndexOfObject].color = Unlock;
    }

    private void AreaLevelUnlock()
    {
        if (AreaLevelLock.Count >= LockArea.Count)
        {
            for (int i = 0; i < LockArea.Count; i++)
            {
                anim[0] = AreaLevelLock[i].GetComponent<Animator>();
                if (playerlevel >= LockArea[i])
                {
                    anim[0].SetTrigger("isFadeout");
                }
                else
                {
                    AreaLevelLock[i].SetActive(true);
                }
            }
        }
        else
        {
            audioManager.PlaySFX(audioManager.Pop2);
            alert[1].SetActive(true);
            alert[10].SetActive(true);
            StartCoroutine(ResetGame());
        }
    }

    public void Validate(int level)
    {
        int arealevel = 0;
        int animallevel = 0;
        int seedslevel = 0;
        string levels = null;

        if (level >= 0 && level <= 9)
        {
            levels = "Area";
        }
        else if (level >= 10 && level <= 15)
        {
            levels = "Animal";
        }
        else
        {
            levels = "Seeds";
        }

        switch (level)
        {

            case 0:
                arealevel = LockArea[0];
                break;
            case 1:
                arealevel = LockArea[1];
                break;
            case 2:
                arealevel = LockArea[2];
                break;
            case 3:
                arealevel = LockArea[3];
                break;
            case 4:
                arealevel = LockArea[4];
                break;
            case 5:
                arealevel = LockArea[5];
                break;
            case 6:
                arealevel = LockArea[6];
                break;
            case 7:
                arealevel = LockArea[7];
                break;
            case 8:
                arealevel = LockArea[8];
                break;
            case 9:
                arealevel = LockArea[9];
                break;
            case 10:
                animallevel = LockAnimal[0];
                break;
            case 11:
                animallevel = LockAnimal[1];
                break;
            case 12:
                animallevel = LockAnimal[2];
                break;
            case 13:
                animallevel = LockAnimal[3];
                break;
            case 14:
                animallevel = LockAnimal[4];
                break;
            case 15:
                animallevel = LockAnimal[5];
                break;
            case 16:
                seedslevel = LockSeeds[0];
                break;
            case 17:
                seedslevel = LockSeeds[1];
                break;
            case 18:
                seedslevel = LockSeeds[2];
                break;
            case 19:
                seedslevel = LockSeeds[3];
                break;
            default:
                arealevel = LockArea[9];
                break;
        }

        if (levels == "Area")
        {
            if (playerlevel < arealevel)
            {
                audioManager.PlaySFX(audioManager.Denied);
                alert[0].SetActive(true);
            }
            else
            {
                ValueIndexOfObject = level;
                if (AreaUnlocked < level)
                {
                    audioManager.PlaySFX(audioManager.Denied);
                    alert[5].SetActive(true);
                    ArrowPoint[AreaUnlocked].SetActive(true) ;
                }
                else
                {
                    audioManager.PlaySFX(audioManager.Pop2);
                    alert[1].SetActive(true);
                    alert[2].SetActive(true);

                    if (arealevels < 0)
                    {
                        arealevels = LockArea[0];
                    }
                    else if (arealevels > 20)
                    {
                        arealevels = LockArea[9];
                    }
                    else
                    {
                        arealevels = arealevel;
                    }
                }
            }
        }
        else if (levels == "Animal")
        {
            if (playerlevel < animallevel)
            {
                audioManager.PlaySFX(audioManager.Denied);
                alert[0].SetActive(true);
            }
            else
            {
                audioManager.PlaySFX(audioManager.Pop2);
                alert[1].SetActive(true);
                alert[2].SetActive(true);
            }
        }
        else if (levels == "Seeds")
        {
            if (playerlevel < seedslevel)
            {
                audioManager.PlaySFX(audioManager.Denied);
                alert[0].SetActive(true);
            }
            else
            {
                audioManager.PlaySFX(audioManager.Pop2);
                alert[1].SetActive(true);
                alert[2].SetActive(true);
            }
        }
        else
        {
            audioManager.PlaySFX(audioManager.Pop2);
            alert[1].SetActive(true);
            alert[10].SetActive(true);
            StartCoroutine(ResetGame());
        }
    }

    private void InventoryUpdate(string val)
    {
        var cond = default(ReadOnlyCollection<int>);
        int start = 0, end = 0;
        if (val == "Animals")
        {
            start = 0;
            end = 5;
            cond = LockAnimal;
        }
        else if (val == "Seeds")
        {
            start = 6;
            end = 9;
            cond = LockSeeds;
        }
        else
        {
            audioManager.PlaySFX(audioManager.Pop2);
            alert[1].SetActive(true);
            alert[10].SetActive(true);
            StartCoroutine(ResetGame());
        }

        if (ValueOfPlayerLevel >= LockAnimal[0] && shoplist != false)
        {
            bagBtnSet.SetActive(true);
        }
        else
        {
            bagBtnSet.SetActive(false);
        }

        int y = 0;
        for (int x = start; x <= end; x++)
        {
            if (val == "Seeds" && y <= 4 && ValueOfPlayerLevel >= cond[y] || val == "Animals" && y <= 6 && ValueOfPlayerLevel >= cond[y])
            {
                InventoryBag[x].SetActive(true);
                y++;
                if (shoplist == false && fence == true && val == "Animals")
                {
                    AnimalsDisable[0].SetActive(true);
                    for (int q = 1; q < AnimalsDisable.Count; q++)
                    {
                        AnimalsDisable[q].SetActive(false);
                    }

                    fence = false;
                }
                else if (shoplist == false && pond == true && val == "Animals")
                {
                    if (ValueOfPlayerLevel >= 17)
                    {
                        for (int z = 1; z < AnimalsDisable.Count; z++)
                        {
                            AnimalsDisable[z].SetActive(true);
                        }

                    }
                    else
                    {
                        if (ValueOfPlayerLevel >= 10)
                        {
                            AnimalsDisable[1].SetActive(true);
                            if (ValueOfPlayerLevel >= 11)
                            {
                                AnimalsDisable[2].SetActive(true);
                                if (ValueOfPlayerLevel >= 15)
                                {
                                    AnimalsDisable[3].SetActive(true);
                                    if (ValueOfPlayerLevel >= 16)
                                    {
                                        AnimalsDisable[4].SetActive(true);
                                    }
                                }
                            }
                        }
                    }

                    if (GameObject.Find("UI 2/Bag/Animals/Disable/1").activeSelf)
                    {
                        AnimalsDisable[0].SetActive(false);
                    }
                    pond = false;
                }
                else
                {
                    if (shoplist == false && fence == true || shoplist == false && pond == true)
                    {
                        audioManager.PlaySFX(audioManager.Pop2);
                        alert[1].SetActive(true);
                        alert[10].SetActive(true);
                        StartCoroutine(ResetGame());
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            else
            {
                InventoryBag[x].SetActive(false);
            }
        }
    }

    public void Bag(int numbers)
    {
        bagstat = true;

        if (numbers == 0)
        {
            bag[0].SetActive(true);
            MethodStats = bag[0].name;
            BagValue = "Seeds";
            slot.SetActive(true);
            GameObject.Find("UI 2/Bag/Seeds").SetActive(true);
            bagBtnSet.SetActive(false);
        }
        else if (numbers == 1)
        {
            bag[1].SetActive(true);
            MethodStats = bag[1].name;
            BagValue = "Animals";
            slot.SetActive(true);
            GameObject.Find("UI 2/Bag/Animals").SetActive(true);
            bagBtnSet.SetActive(false);
        }
        else
        {
            bag[0].SetActive(true);
            bag[2].SetActive(true);
            MethodStats = bag[0].name;
            shoplist = true;
            BagValue = "Seeds";
        }
    }

    public void BagClose()
    {
        bagstat = false;
        GameObject uiObj = GameObject.Find("UI 2");
        GameObject shop = uiObj.transform.Find("Shop").gameObject;

        if (MethodStats == "Seeds" && shoplist == true || MethodStats == "Animals" && shoplist == true)
        {
            shop.SetActive(true);
            shoplist = false;
        }
        else
        {
            uiObj.SetActive(false);
            slot.SetActive(false);
        }
    }

    public void ShopPrices(int price)
    {
        if (ValueOfPlayerMoney >= (ulong)price)
        {
            audioManager.PlaySFX(audioManager.Buy);
            ValueOfPlayerMoney -= (ulong)price;
            QuantityOfPlayerItems(ItemsName);
        }
        else
        {
            ShopPriceUpdate = price;
            //SetTextStatus();
            audioManager.PlaySFX(audioManager.Pop2);
            alert[1].SetActive(true);
            alert[3].SetActive(true);
        }
    }

    public void AnimalShop()
    {
        gameObject.SetActive(true);

    }

    private void LevelUnlock(string shop)
    {

        if (shop == "Animal")
        {
            for (int i = 0; i < LockAnimal.Count; i++)
            {
                if (playerlevel >= LockAnimal[i])
                {
                    AnimalName = Animal[i].transform.parent;
                    Animal[i].color = Unlock;
                    AnimalNameText[i].text = AnimalName.name;
                    if (ValueOfPlayerMoney < (ulong)AnimalsPrice[i])
                    {
                        AnimalPriceText[i].text = $"<color=red>{CurrencyTextFormat((ulong)AnimalsPrice[i])}</color>" + " BTH";
                    }
                    else
                    {
                        AnimalPriceText[i].text = $"<color=green>{CurrencyTextFormat((ulong)AnimalsPrice[i])}</color>" + " BTH";
                    }
                    SetShopQuantityTextStat(shop);
                    Destroy(AnimalLock[i]);

                }
                else
                {
                    AnimalNameText[i].text = "Level " + LockAnimal[i];
                    AnimalPriceText[i].text = "??? BTH";

                }
            }
        }
        else
        {
            if (shop == "Seeds")
            {
                for (int i = 0; i < LockSeeds.Count; i++)
                {
                    if (ValueOfPlayerLevel >= LockSeeds[i])
                    {
                        SeedsName = Seeds[i].transform.parent;
                        Seeds[i].color = Unlock;
                        SeedsNameText[i].text = SeedsName.name;
                        if (ValueOfPlayerMoney < (ulong)SeedsPrice[i])
                        {
                            SeedsPriceText[i].text = $"<color=red>{CurrencyTextFormat((ulong)SeedsPrice[i])}</color>" + " BTH";
                        }
                        else
                        {
                            SeedsPriceText[i].text = $"<color=green>{CurrencyTextFormat((ulong)SeedsPrice[i])}</color>" + " BTH";
                        }
                        SetShopQuantityTextStat(shop);
                        Destroy(SeedsLock[i]);

                    }
                    else
                    {
                        SeedsNameText[i].text = "Level " + LockSeeds[i];
                        SeedsPriceText[i].text = "??? BTH";

                    }
                }
            }
        }
    }

    private void SetBagQuantityTextStat(string stat, string qtt_name)
    {

        int start1 = 0, minus = 0, end = 0;
        if (stat == "Bag" && qtt_name == "Seeds")
        {
            start1 = 10;
            end = LockSeeds.Count * 3;
            minus = LockSeeds.Count;
        }
        else if (stat == "Bag" && qtt_name == "Animals")
        {
            start1 = 14;
            end = AnimalLock.Count * 3;
            minus = (AnimalLock.Count * 2) + 2;
        }
        else
        {
            audioManager.PlaySFX(audioManager.Pop2);
            alert[1].SetActive(true);
            alert[10].SetActive(true);
            StartCoroutine(ResetGame());
        }

        for (int j = start1; j <= end + 1; j++)
        {
            if (qtt_name == "Seeds")
            {
                //GameObject.Find("UI 2/Bag/Quantity/Animals").SetActive(false);
                if (InventoryBag[j - minus].activeSelf && Inventorys[j - minus] > 0)
                {
                    Quantity[j].SetActive(true);
                    QuantityText[j].text = $"x {CurrencyTextFormat((ulong)Inventorys[j - minus])}";
                }
                else
                {
                    Inventorys[j - minus] = 0;
                    Quantity[j].SetActive(false);
                }
            }
            else if (qtt_name == "Animals")
            {
                //GameObject.Find("UI 2/Bag/Quantity/Seeds").SetActive(false);
                if (InventoryBag[j - minus].activeSelf && Inventorys[j - minus] > 0)
                {
                    Quantity[j].SetActive(true);
                    QuantityText[j].text = $"x {CurrencyTextFormat((ulong)Inventorys[j - minus])}";
                }
                else
                {
                    Inventorys[j - minus] = 0;
                    Quantity[j].SetActive(false);
                }
            }
            else
            {
                audioManager.PlaySFX(audioManager.Pop2);
                alert[1].SetActive(true);
                alert[10].SetActive(true);
                StartCoroutine(ResetGame());
            }
        }
    }

    private void SetShopQuantityTextStat(string shop)
    {
        if (shop == "Animal")
        {
            for (int j = 0; j < LockAnimal.Count; j++)
            {
                if (j != 0 && j <= 4)
                {
                    if (GameObject.Find("UI 2") != null && GameObject.Find("Canvas/UI 2/Animal Shop/Bar 1").activeSelf)
                    {
                        if (Inventorys[j] > 0)
                        {
                            Quantity[j].SetActive(true);
                            QuantityText[j].text = $"x {CurrencyTextFormat((ulong)Inventorys[j])}";
                        }
                        else
                        {
                            Quantity[j].SetActive(false);
                        }
                    }
                    else
                    {
                        Quantity[j].SetActive(false);
                    }
                }
                else
                {
                    if (GameObject.Find("UI 2") != null && GameObject.Find("Canvas/UI 2/Animal Shop/Bar 2").activeSelf)
                    {
                        if (Inventorys[j] > 0)
                        {
                            Quantity[j].SetActive(true);
                            QuantityText[j].text = $"x {CurrencyTextFormat((ulong)Inventorys[j])}";
                        }
                        else
                        {
                            Quantity[j].SetActive(false);
                        }
                    }
                    else
                    {
                        Quantity[j].SetActive(false);

                    }
                }

            }
        }
        else
        {
            for (int j = 6; j <= (SeedsLock.Count * 2) + 1; j++)
            {
                if (Inventorys[j] > 0)
                {
                    Quantity[j].SetActive(true);
                    QuantityText[j].text = $"x {CurrencyTextFormat((ulong)Inventorys[j])}";
                }
                else
                {
                    Quantity[j].SetActive(false);
                }
            }
        }
    }

    private void QuantityOfPlayerItems(string name)
    {
        const int qtt = 1;

        switch (name)
        {
            case "Fish":
                Inventorys[0] += qtt;
                break;
            case "Chicken":
                Inventorys[1] += qtt;
                break;
            case "Pig":
                Inventorys[2] += qtt;
                break;
            case "Buffalo":
                Inventorys[3] += qtt;
                break;
            case "Cow":
                Inventorys[4] += qtt;
                break;
            case "Horse":
                Inventorys[5] += qtt;
                break;
            case "Carrot":
                Inventorys[6] += qtt;
                break;
            case "Corn":
                Inventorys[7] += qtt;
                break;
            case "Radish":
                Inventorys[8] += qtt;
                break;
            case "Turnip":
                Inventorys[9] += qtt;
                break;
            default:
                audioManager.PlaySFX(audioManager.Pop2);
                alert[1].SetActive(true);
                alert[10].SetActive(true);
                StartCoroutine(ResetGame());
                break;
        }
    }

    public void SeedShop()
    {
        gameObject.SetActive(true);
    }

    public void Exchage()
    {
        if (textstats == true && exchange != true)
        {
            if (ValueOfPlayerStar >= AmountOfExchangeStar)
            {
                ValueOfPlayerStar -= (int)AmountOfExchangeStar;
                ValueOfPlayerMoney += (ulong)MoneyAmountExchanged;
                if (area != false)
                {
                    BuyArea();
                }
                else if (shop != false)
                {
                    ShopPrices(ShopPriceUpdate);
                }
                AmountOfExchangeStar = 0;
                alert[4].SetActive(false);
                alert[1].SetActive(false);
            }
            else
            {
                audioManager.PlaySFX(audioManager.Pop2);
                alert[4].SetActive(false);
                alert[6].SetActive(true);
            }

        }
        else
        {
            //Star shop
            if (ValueOfPlayerStar >= AmountOfExchangeStar && AmountOfExchangeStar != 0)
            {
                audioManager.PlaySFX(audioManager.Buy);
                ValueOfPlayerStar -= (int)AmountOfExchangeStar;
                ValueOfPlayerMoney += (ulong)MoneyAmountExchanged;
                AmountOfExchangeStar = 0;
                MoneyAmountExchanged = 0;
            }
            else
            {
                alert[1].SetActive(true);

                if (AmountOfExchangeStar <= 0)
                {
                    audioManager.PlaySFX(audioManager.Pop2);
                    alert[7].SetActive(true);
                }
                else
                {
                    audioManager.PlaySFX(audioManager.Pop2);
                    //alert[4].SetActive(false);
                    alert[6].SetActive(true);
                    AmountOfExchangeStar = 0;
                    MoneyAmountExchanged = 0;
                }
            }
        }
        if (exchange != true)
        {
            SetTextStatus();
        }
    }

    private void PriceOfItems()
    {
        if (STime >= NTime && STime <= NTime + 0.1f)
        {
            STime = 0f;

            for (int p = 0; p < Inventorys.Count; p++)
            {
                switch (p)
                {
                    case 0:
                        ItemPriceUpdate[p] = UnityEngine.Random.Range(animalsprice[0] - 50, animalsprice[0] + 50 + 1);
                        break;
                    case 1:
                        ItemPriceUpdate[p] = UnityEngine.Random.Range(animalsprice[1] - 100, animalsprice[1] + 100 + 1);
                        break;
                    case 2:
                        ItemPriceUpdate[p] = UnityEngine.Random.Range(animalsprice[2] - 200, animalsprice[2] + 2000 + 1);
                        break;
                    case 3:
                        ItemPriceUpdate[p] = UnityEngine.Random.Range(animalsprice[3] - 300, animalsprice[3] + 300 + 1);
                        break;
                    case 4:
                        ItemPriceUpdate[p] = UnityEngine.Random.Range(animalsprice[4] - 400, animalsprice[4] + 400 + 1);
                        break;
                    case 5:
                        ItemPriceUpdate[p] = UnityEngine.Random.Range(animalsprice[5] - 500, animalsprice[5] + 500 + 1);
                        break;
                    case 6:
                        ItemPriceUpdate[p] = UnityEngine.Random.Range(seedsprice[0] - 10, seedsprice[0] + 10 + 1);
                        break;
                    case 7:
                        ItemPriceUpdate[p] = UnityEngine.Random.Range(seedsprice[1] - 20, seedsprice[1] + 20 + 1);
                        break;
                    case 8:
                        ItemPriceUpdate[p] = UnityEngine.Random.Range(seedsprice[2] - 30, seedsprice[2] + 30 + 1);
                        break;
                    case 9:
                        ItemPriceUpdate[p] = UnityEngine.Random.Range(seedsprice[3] - 40, seedsprice[3] + 40 + 1);
                        break;
                    default:
                        audioManager.PlaySFX(audioManager.Pop2);
                        alert[1].SetActive(true);
                        alert[10].SetActive(true);
                        StartCoroutine(ResetGame());
                        break;
                }
            }
        }
        else
        {
            STime += 0.020f;
        }
    }

    public void Selling()
    {
        if (sold != false)
        {
            if (Inventorys[rtval] >= ThingsAmount && ThingsAmount != 0)
            {
                Inventorys[rtval] -= ThingsAmount;
                ValueOfPlayerMoney += (ulong)MoneyAmountExchanged;
                ThingsAmount = 0;
            }
            else if (ThingsAmount <= 0)
            {
                audioManager.PlaySFX(audioManager.Pop2);
                alert[1].SetActive(true);
                alert[8].SetActive(false);
                alert[11].SetActive(true);
            }
            else
            {
                audioManager.PlaySFX(audioManager.Pop2);
                alert[1].SetActive(true);
                alert[9].SetActive(true);
            }
        }
        else
        {
            audioManager.PlaySFX(audioManager.Pop2);
            alert[1].SetActive(true);
            alert[10].SetActive(true);
            StartCoroutine(ResetGame());
        }
    }

    public void StarShopSetBool(bool val)
    {
        exchange = val;
        AmountOfExchangeStar = 0;
        MoneyAmountExchanged = 0;
    }

    public void StarShop(int val)
    {
        if (val == 0)
        {
            AmountOfExchangeStar--;
            MoneyAmountExchanged = ((long)AmountOfExchangeStar * StarPriceUpdate);
        }
        else if (val == 1 || val == 4)
        {
            if (val == 4)
            {
                AmountOfExchangeStar = ValueOfPlayerStar;
                MoneyAmountExchanged = ((long)AmountOfExchangeStar * StarPriceUpdate);
            }
            else
            {
                AmountOfExchangeStar++;
                MoneyAmountExchanged = ((long)AmountOfExchangeStar * StarPriceUpdate);
            }
        }
        else if (val == 2 || val == 5)
        {

            if (ThingsAmount >= Inventorys[rtval])
            {
                audioManager.PlaySFX(audioManager.Pop2);
                alert[8].SetActive(false);
                alert[1].SetActive(true);
                alert[9].SetActive(true);
            }
            else
            {
                if (val == 5)
                {
                    ThingsAmount = Inventorys[rtval];
                    MoneyAmountExchanged = ((long)ThingsAmount * ItemPriceUpdate[rtval]);
                }
                else
                {
                    ThingsAmount++;
                    MoneyAmountExchanged = ((long)ThingsAmount * ItemPriceUpdate[rtval]);
                }
            }

        }
        else if (val == 3)
        {
            ThingsAmount--;
            MoneyAmountExchanged = ((long)ThingsAmount * ItemPriceUpdate[rtval]);
        }
        else
        {
            AmountOfExchangeStar = 0;
            MoneyAmountExchanged = 0;
            ThingsAmount = 0;
        }
    }

    public void AreaPlayerActive(string val)
    {
        if (val == "pond")
        {
            pond = true;
        }
        else if (val == "fence")
        {
            fence = true;
        }
        else
        {
            audioManager.PlaySFX(audioManager.Pop2);
            alert[1].SetActive(true);
            alert[10].SetActive(true);
            StartCoroutine(ResetGame());
        }
    }

    public void SetAnimalsAndPlantBool(int val)
    {
        if (val == 0)
        {
            animals = true;
        }
        else if (val == 1)
        {
            plant = true;
        }
        else if (val == 2)
        {
            animals = false;
            plant = false;
        }
        else
        {
            audioManager.PlaySFX(audioManager.Pop2);
            alert[1].SetActive(true);
            alert[10].SetActive(true);
            StartCoroutine(ResetGame());
        }
    }

    public void ClearValue()
    {
        ThingsAmount = 0;
        MoneyAmountExchanged = 0;
    }

    //For Close Btn and another method
    public void SetTextStatus()
    {
        textstats = !textstats;

    }

    //For only Close Btn
    public void SetShopPrice()
    {
        ShopPriceUpdate = 0;
    }

    //For Shop Icon
    public void SetAreaBool()
    {
        area = !area;
    }

    //For only Close Btn
    public void SetShopBool(bool val)
    {
        shop = val;
    }

    public void SetSoldBool(bool val)
    {
        sold = val;
    }

    //For Icon and Close Btn
    public void ShopStatusInput(string val)
    {
        ShopStatus = val;
    }

    //For Close Btn
    public void ResetSlot()
    {
        for (int i = 0; i < 2; i++)
        {
            Inventorys[cancelSlotA] += QtSlotA;
            Inventorys[cancelSlotB] += QtSlotB;
            QtSlotA = QtSlotB = 0;
            Slot[i].gameObject.name = "Image";
            Slot[i].gameObject.SetActive(false);
            SlotText[i].gameObject.SetActive(false);
        }
    }

    public void Selected()
    {
        for (int i = 0; i < 2; i++)
        {
            QtSlotA = QtSlotB = 0;
            Slot[i].gameObject.name = "Image";
            Slot[i].gameObject.SetActive(false);
            SlotText[i].gameObject.SetActive(false);
        }
    }

    public void CallMethod()
    {
        if (plant != false)
        {
            audioManager.PlaySFX(audioManager.Planting);
            Planting();
        }
        else if (animals != false)
        {
            RaiseAnimals();
        }
        else
        {
            audioManager.PlaySFX(audioManager.Pop2);
            alert[1].SetActive(true);
            alert[10].SetActive(true);
            StartCoroutine(ResetGame());
        }
    }

    //Text for Display the Message
    private void PrepareTextUpdate()
    {
        for (int i = 0; i < StarText.Count; i++)
        {
            if (i == 2 && shop == true && exchange == true)
            {
                if (AmountOfExchangeStar <= 0)
                {
                    StarText[i].gameObject.SetActive(false);
                    MoneyText[i].gameObject.SetActive(false);
                }
                else
                {
                    StarText[i].gameObject.SetActive(true);
                    MoneyText[i].gameObject.SetActive(true);
                    StarText[i].text = "x " + $"<color=yellow>{CurrencyTextFormat((ulong)AmountOfExchangeStar)}</color>";
                    MoneyText[i].text = "+ " + $"<color=green>{CurrencyTextFormat((ulong)MoneyAmountExchanged)}</color>" + " BTH";
                }
            }
            //Alert 9
            else if (sold == true)
            {
                if (ThingsAmount <= 0)
                {
                    SoldText[0].gameObject.SetActive(false);
                    MoneyText[3].gameObject.SetActive(false);
                    MoneyText[1].text = ": " + $"<color=green>{CurrencyTextFormat(ValueOfPlayerMoney)}</color>" + " BTH";
                }
                else
                {
                    SoldText[0].gameObject.SetActive(true);
                    MoneyText[3].gameObject.SetActive(true);
                    SoldText[0].text = "x " + $"<color=white>{CurrencyTextFormat((ulong)thingsAmount)}</color>";
                    MoneyText[3].text = "+ " + $"<color=green>{CurrencyTextFormat((ulong)MoneyAmountExchanged)}</color>" + " BTH";
                    
                }
            }
            else
            {
                MoneyText[i].text = ": " + $"<color=green>{CurrencyTextFormat(ValueOfPlayerMoney)}</color>" + " BTH";
                StarText[i].text = "x " + $"<color=yellow>{CurrencyTextFormat((ulong)ValueOfPlayerStar)}</color>";
                if (i <= 1)
                {
                    LevelText[i].text = "Level " + ValueOfPlayerLevel;
                }
            }
        }
        if (ValueOfPlayerMoney >= (ulong)AreaPriceUpdate) 
        { 
            AlertMessageText[0].text = "Do you want to buy this area for " + $"<color=green>{CurrencyTextFormat((ulong)AreaPriceUpdate)}</color>" + " BTH" + "?";
        }
        else
        {
            AlertMessageText[0].text = "Do you want to buy this area for " + $"<color=red>{CurrencyTextFormat((ulong)AreaPriceUpdate)}</color>" + " BTH" + "?";
        }
        if (textstats == true)
        {
            if (ShopPriceUpdate > 0)
            {
                ExchangeCalculator(ShopPriceUpdate);
            }
            else
            {
                ExchangeCalculator(AreaPriceUpdate);
            }

            AlertMessageText[2].text = "x " + $"<color=yellow>{CurrencyTextFormat((ulong)AmountOfExchangeStar)}</color>";
            AlertMessageText[3].text = "+ " + $"<color=green>{CurrencyTextFormat((ulong)MoneyAmountExchanged)}</color>";
        }
    }

    public void AreaNumbers(int val)
    {
        switch (val)
        {
            case 1:
                areaNumbersStart = 0;
                areaNumbersEnd = 1;
                break;
            case 2: 
                areaNumbersStart = 2;
                areaNumbersEnd = 3;
                break;
            case 3:
                areaNumbersStart = 4;
                areaNumbersEnd = 5;
                break;
            case 4:
                areaNumbersStart = 6;
                areaNumbersEnd = 7;
                break;
            case 5:
                areaNumbersStart = 8;
                areaNumbersEnd = 9;
                break;
            case 6:
                areaNumbersStart = 10;
                areaNumbersEnd = 11;
                break;
            case 7:
                areaNumbersStart = 12;
                areaNumbersEnd = 13;
                break;
            case 8:
                areaNumbersStart = 14;
                areaNumbersEnd = 15;
                break;
            default:
                audioManager.PlaySFX(audioManager.Pop2);
                alert[1].SetActive(true);
                alert[10].SetActive(true);
                StartCoroutine(ResetGame());
                break;
        }
    }

    private enum ListItemsName
    {
        Fish,
        Chicken,
        Pig,
        Buffalo,
        Cow,
        Horse,
        Carrot,
        Corn,
        Radish,
        Turnip,
    }

    private ListItemsName ReObjectName(int val)
    {

        if (val >  9) 
        {
            audioManager.PlaySFX(audioManager.Pop2);
            alert[1].SetActive(true);
            alert[10].SetActive(true);
            StartCoroutine(ResetGame());
        }

        switch (val)
        {
            case 0:
                return ListItemsName.Fish;
            case 1:
                return ListItemsName.Chicken;
            case 2:
                return ListItemsName.Pig;
            case 3:
                return ListItemsName.Buffalo;
            case 4:
                return ListItemsName.Cow;
            case 5:
                return ListItemsName.Horse;
            case 6:
                return ListItemsName.Carrot;
            case 7:
                return ListItemsName.Corn;
            case 8:
                return ListItemsName.Radish;
            case 9:
                return ListItemsName.Turnip;
            default:
                throw new ArgumentOutOfRangeException(nameof(val), val, "Invalid Number");
        }
    }

    private void GetCurrentItemsName(int val)
    {
        if (currentSlotItems == 1)
        {
            cancelSlotA = val;
        }
        else if (currentSlotItems == 2)
        {
            cancelSlotB = val;
        }
        else
        {
            cancelSlotA = val;
        }
    }

    public void CancelItmesSlot(int val)
    {
        if (val == 0)
        {
            if (QtSlotA == 1)
            {
                Inventorys[cancelSlotA]++;
                QtSlotA = 0;
                BtnActive();
                currentSlotItems = 2;
                Slot[val].gameObject.name = "Image";
                Slot[val].gameObject.SetActive(false);
                SlotText[val].gameObject.SetActive(false);
            }
            else
            {

                QtSlotA--;
                SlotText[val].text = QtSlotA + "/" + maxSlotA;
                Inventorys[cancelSlotA]++;
            }
        }
        else if (val == 1)
        {
            if (QtSlotB == 1)
            {
                Inventorys[cancelSlotB]++;
                QtSlotB = 0;
                BtnActive();
                currentSlotItems = 1;
                Slot[val].gameObject.name = "Image";
                Slot[val].gameObject.SetActive(false);
                SlotText[val].gameObject.SetActive(false);

            }
            else
            {
                QtSlotB--;
                SlotText[val].text = QtSlotB + "/" + maxSlotB;
                Inventorys[cancelSlotB]++;
            }
        }
        else
        {
            audioManager.PlaySFX(audioManager.Pop2);
            alert[1].SetActive(true);
            alert[10].SetActive(true);
            StartCoroutine(ResetGame());
        }

    }

    private void BtnActive()
    {
        if (QtSlotA != 0 || QtSlotB != 0)
        {
            GameObject.Find("UI 2/Bag/Close").SetActive(false);
            GameObject.Find("UI 2/Bag/Slot/ButtonSet").SetActive(true);
        }
        else
        {
            GameObject.Find("UI 2/Bag/Slot/ButtonSet").SetActive(false);
            GameObject.Find("UI 2/Bag/Close").SetActive(true);
        }
    }

    public void SoldThings(int val)
    {
        rtval = val;
        if (shoplist != false)
        {
            audioManager.PlaySFX(audioManager.Pop2);
            GameObject.Find("UI 2/Bag").SetActive(false);
            alert[1].SetActive(true);
            alert[8].SetActive(true);
            sold = true;
            things.GetComponent<Image>().sprite = newSprite[val];
        }
        else
        {

            //Select Items     
            if (!Slot[0].gameObject.activeSelf || Slot[0].gameObject.activeSelf || !Slot[1].gameObject.activeSelf || Slot[1].gameObject.activeSelf)
            {
                if (Inventorys[val] > 0)
                {
                    if (QtSlotA < maxSlotA && Slot[0].gameObject.name == ReObjectName(val).ToString() || QtSlotA < maxSlotA && Slot[0].gameObject.name == "Image")
                    {
                        currentSlotItems = 1;
                        GetCurrentItemsName(val);
                        Slot[0].gameObject.SetActive(true);
                        SlotText[0].gameObject.SetActive(true);
                        Slot[0].GetComponent<Image>().sprite = newSprite[val];
                        Inventorys[val]--;
                        QtSlotA++;
                        BtnActive();
                        SlotText[0].text = QtSlotA + "/" + maxSlotA;

                        if (Slot[0].gameObject.name == "Image")
                        {
                            Slot[0].gameObject.name = ReObjectName(val).ToString();
                        }
                    }
                    else if (QtSlotB < maxSlotB && Slot[1].gameObject.name == ReObjectName(val).ToString() || QtSlotB < maxSlotB && Slot[1].gameObject.name == "Image")
                    {
                        currentSlotItems = 2;
                        GetCurrentItemsName(val);
                        Slot[1].gameObject.SetActive(true);
                        SlotText[1].gameObject.SetActive(true);
                        Slot[1].GetComponent<Image>().sprite = newSprite[val];
                        Inventorys[val]--;
                        QtSlotB++;
                        BtnActive();
                        SlotText[1].text = QtSlotB + "/" + maxSlotB;
                        if (Slot[1].gameObject.name == "Image")
                        {
                            Slot[1].gameObject.name = ReObjectName(val).ToString();
                        }
                    }
                    else
                    {
                        audioManager.PlaySFX(audioManager.Pop2);
                        alert[1].SetActive(true);
                        alert[13].SetActive(true);
                    }
                }
                else
                {
                    audioManager.PlaySFX(audioManager.Pop2);
                    alert[1].SetActive(true);
                    alert[12].SetActive(true);
                }
            }
            else
            {
                audioManager.PlaySFX(audioManager.Pop2);
                alert[1].SetActive(true);
                alert[10].SetActive(true);
                StartCoroutine(ResetGame());
            }
        }
    }

    public void Planting()
    {
        for (int q = areaNumbersStart; q <= areaNumbersEnd; q++)
        {
            if (q < areaNumbersEnd)
            {
                switch (Slot[0].gameObject.name)
                {
                    case "Carrot":
                        plants[q].gameObject.SetActive(true);
                        PlantA[q] = ReObjectName(6).ToString();
                        plants[q].GetComponent<Image>().sprite = CarrotGrow[0];
                        plantsGrowLevel[q] = 0;
                        plantQt[q] = QtSlotA;
                        plantarea[q] = true;
                        plantFarmActive = true;
                        break;
                    case "Corn":
                        plants[q].gameObject.SetActive(true);
                        PlantA[q] = ReObjectName(7).ToString();
                        plants[q].GetComponent<Image>().sprite = CornGrow[0];
                        plantsGrowLevel[q] = 0;
                        plantQt[q] = QtSlotA;
                        plantarea[q] = true;
                        plantFarmActive = true;
                        break;
                    case "Radish":
                        plants[q].gameObject.SetActive(true);
                        PlantA [q]= ReObjectName(8).ToString();
                        plants[q].GetComponent<Image>().sprite = RadishGrow[0];
                        plantsGrowLevel[q] = 0;
                        plantQt[q] = QtSlotA;
                        plantarea[q] = true;
                        plantFarmActive = true;
                        break;
                    case "Turnip":
                        plants[q].gameObject.SetActive(true);
                        PlantA[q] = ReObjectName(9).ToString();
                        plants[q].GetComponent<Image>().sprite = TurnipGrow[0];
                        plantsGrowLevel[q] = 0;
                        plantQt[q] = QtSlotA;
                        plantarea[q] = true;
                        plantFarmActive = true;
                        break;
                    default:
                        plants[q].GetComponent<Image>().sprite = null;
                        plants[q].gameObject.SetActive(false);
                        break;
                }
                switch (q)
                {
                    case 0:
                        Block[0].SetActive(true);
                        break;
                    case 2:
                        Block[1].SetActive(true);
                        break;
                    case 4:
                        Block[2].SetActive(true);
                        break;
                    case 6:
                        Block[3].SetActive(true);
                        break;
                    case 8:
                        Block[4].SetActive(true);
                        break;
                    case 10:
                        Block[5].SetActive(true);
                        break;
                    case 12:
                        Block[6].SetActive(true);
                        break;
                    case 14:
                        Block[7].SetActive(true);
                        break;
                }
            }
            else if (q == areaNumbersEnd)
            {
                switch (Slot[1].gameObject.name)
                {
                    case "Carrot":
                        plants[q].gameObject.SetActive(true);
                        PlantB[q] = ReObjectName(6).ToString();
                        plants[q].GetComponent<Image>().sprite = CarrotGrow[0];
                        plantsGrowLevel[q] = 0;
                        plantQt[q] = QtSlotB;
                        plantarea[q] = true;
                        plantFarmActive = true;
                        break;
                    case "Corn":
                        plants[q].gameObject.SetActive(true);
                        PlantB[q] = ReObjectName(7).ToString();
                        plants[q].GetComponent<Image>().sprite = CornGrow[0];
                        plantsGrowLevel[q] = 0;
                        plantQt[q] = QtSlotB;
                        plantarea[q] = true;
                        plantFarmActive = true;
                        break;
                    case "Radish":
                        plants[q].gameObject.SetActive(true);
                        PlantB[q] = ReObjectName(8).ToString();
                        plants[q].GetComponent<Image>().sprite = RadishGrow[0];
                        plantsGrowLevel[q] = 0;
                        plantQt[q] = QtSlotB;
                        plantarea[q] = true;
                        plantFarmActive = true;
                        break;
                    case "Turnip":
                        plants[q].gameObject.SetActive(true);
                        PlantB[q] = ReObjectName(9).ToString();
                        plants[q].GetComponent<Image>().sprite = TurnipGrow[0];
                        plantsGrowLevel[q] = 0;
                        plantQt[q] = QtSlotB;
                        plantarea[q] = true;
                        plantFarmActive = true;
                        break;
                    default:
                        plants[q].GetComponent<Image>().sprite = null;
                        plants[q].gameObject.SetActive(false);
                        break;
                }
            }
            else
            {
                audioManager.PlaySFX(audioManager.Pop2);
                alert[1].SetActive(true);
                alert[10].SetActive(true);
                StartCoroutine(ResetGame());
            }
        }
    }

    private void RaiseAnimals()
    {
        for (int l = areaNumbersStart; l <= areaNumbersEnd; l++)
        {
            if (l < areaNumbersEnd)
            {
                switch (Slot[0].gameObject.name)
                {
                    case "Fish":
                        if (QtSlotA == 1)
                        {
                            audioManager.PlaySFX(audioManager.Fish);
                            animalObj[8].SetActive(true);
                        }
                        else if (QtSlotA == 2)
                        {
                            audioManager.PlaySFX(audioManager.Fish);
                            animalObj[8].SetActive(true);
                            animalObj[9].SetActive(true);
                        }
                        else if (QtSlotA == 3)
                        {
                            audioManager.PlaySFX(audioManager.Fish);
                            animalObj[8].SetActive(true);
                            animalObj[9].SetActive(true);
                            animalObj[10].SetActive(true);
                        }
                        else if (QtSlotA == 4)
                        {
                            audioManager.PlaySFX(audioManager.Fish);
                            animalObj[8].SetActive(true);
                            animalObj[9].SetActive(true);
                            animalObj[10].SetActive(true);
                            animalObj[11].SetActive(true);
                        }
                        else if (QtSlotA >= 5)
                        {
                            audioManager.PlaySFX(audioManager.Fish);
                            animalObj[8].SetActive(true);
                            animalObj[9].SetActive(true);
                            animalObj[10].SetActive(true);
                            animalObj[11].SetActive(true);
                            animalObj[12].SetActive(true);
                        }
                        else
                        {
                            audioManager.PlaySFX(audioManager.Pop2);
                            alert[1].SetActive(true);
                            alert[10].SetActive(true);
                            StartCoroutine(ResetGame());
                        }
                        AnimalA[l] = ReObjectName(0).ToString();
                        animalQt[l] = QtSlotA;
                        animalarea[l] = true;
                        animalFarmActive = true;
                        break;
                    case "Chicken":
                        audioManager.PlaySFX(audioManager.Chicken);
                        animalObj[l].gameObject.SetActive(true);
                        animalObj[l].GetComponent<Animator>().runtimeAnimatorController = animalAnimControl[0];
                        AnimalA[l] = ReObjectName(1).ToString();
                        animalQt[l] = QtSlotA;
                        animalarea[l] = true;
                        animalFarmActive = true;
                        break;
                    case "Pig":
                        audioManager.PlaySFX(audioManager.Pig);
                        animalObj[l].gameObject.SetActive(true);
                        animalObj[l].GetComponent<Animator>().runtimeAnimatorController = animalAnimControl[1];
                        AnimalA[l] = ReObjectName(2).ToString();
                        animalQt[l] = QtSlotA;
                        animalarea[l] = true;
                        animalFarmActive = true;
                        break;
                    case "Buffalo":
                        audioManager.PlaySFX(audioManager.Buffalo);
                        animalObj[l].gameObject.SetActive(true);
                        animalObj[l].GetComponent<Animator>().runtimeAnimatorController = animalAnimControl[2];
                        AnimalA[l] = ReObjectName(3).ToString();
                        animalQt[l] = QtSlotA;
                        animalarea[l] = true;
                        animalFarmActive = true;
                        break;
                    case "Cow":
                        audioManager.PlaySFX(audioManager.Cow);
                        animalObj[l].gameObject.SetActive(true);
                        animalObj[l].GetComponent<Animator>().runtimeAnimatorController = animalAnimControl[3];
                        AnimalA[l] = ReObjectName(4).ToString();
                        animalQt[l] = QtSlotA;
                        animalarea[l] = true;
                        animalFarmActive = true;
                        break;
                    case "Horse":
                        if (l == 0)
                        {
                            audioManager.PlaySFX(audioManager.Horse);
                            animalObj[4].gameObject.SetActive(true);
                        }
                        else if (l == 2)
                        {
                            audioManager.PlaySFX(audioManager.Horse);
                            animalObj[6].gameObject.SetActive(true);
                        }
                        else
                        {
                            audioManager.PlaySFX(audioManager.Pop2);
                            alert[1].SetActive(true);
                            alert[10].SetActive(true);
                            StartCoroutine(ResetGame());
                        }

                        AnimalA[l] = ReObjectName(5).ToString();
                        animalQt[l] = QtSlotA;
                        animalarea[l] = true;
                        animalFarmActive = true;
                        break;
                    default:
                        animalObj[l].gameObject.SetActive(false);
                        break;
                }
            }

            else if (l == areaNumbersEnd)
            {
                switch (Slot[1].gameObject.name)
                {
                    case "Fish":
                        AnimalB[l] = ReObjectName(0).ToString();
                        animalQt[l] = QtSlotB;
                        animalarea[l] = true;
                        animalFarmActive = true;
                        break;
                    case "Chicken":
                        audioManager.PlaySFX(audioManager.Chicken);
                        animalObj[l].gameObject.SetActive(true);
                        animalObj[l].GetComponent<Animator>().runtimeAnimatorController = animalAnimControl[0];
                        AnimalB[l] = ReObjectName(1).ToString();
                        animalQt[l] = QtSlotB;
                        animalarea[l] = true;
                        animalFarmActive = true;
                        break;
                    case "Pig":
                        audioManager.PlaySFX(audioManager.Pig);
                        animalObj[l].gameObject.SetActive(true);
                        animalObj[l].GetComponent<Animator>().runtimeAnimatorController = animalAnimControl[1];
                        AnimalB[l] = ReObjectName(2).ToString();
                        animalQt[l] = QtSlotB;
                        animalarea[l] = true;
                        animalFarmActive = true;
                        break;
                    case "Buffalo":
                        audioManager.PlaySFX(audioManager.Buffalo);
                        animalObj[l].gameObject.SetActive(true);
                        animalObj[l].GetComponent<Animator>().runtimeAnimatorController = animalAnimControl[2];
                        AnimalB[l] = ReObjectName(3).ToString();
                        animalQt[l] = QtSlotB;
                        animalarea[l] = true;
                        animalFarmActive = true;
                        break;
                    case "Cow":
                        audioManager.PlaySFX(audioManager.Cow);
                        animalObj[l].gameObject.SetActive(true);
                        animalObj[l].GetComponent<Animator>().runtimeAnimatorController = animalAnimControl[3];
                        AnimalB[l] = ReObjectName(4).ToString();
                        animalQt[l] = QtSlotB;
                        animalarea[l] = true;
                        animalFarmActive = true;
                        break;
                    case "Horse":
                        if (l == 1)
                        {
                            audioManager.PlaySFX(audioManager.Horse);
                            animalObj[5].gameObject.SetActive(true);
                        }
                        else if (l == 3)
                        {
                            audioManager.PlaySFX(audioManager.Horse);
                            animalObj[7].gameObject.SetActive(true);
                        }
                        else
                        {
                            audioManager.PlaySFX(audioManager.Pop2);
                            alert[1].SetActive(true);
                            alert[10].SetActive(true);
                            StartCoroutine(ResetGame());
                        }
                        AnimalB[l] = ReObjectName(5).ToString();
                        animalQt[l] = QtSlotB;
                        animalarea[l] = true;
                        animalFarmActive = true;
                        break;
                    default:
                        animalQt[l] = 0;
                        animalObj[l].gameObject.SetActive(false);
                        break;
                }
            }
            else
            {
                alert[1].SetActive(true);
                alert[10].SetActive(true);
                StartCoroutine(ResetGame());
            }
        }
    }

    //Formatted the CurrenryText
    private string CurrencyTextFormat(ulong text)
    {
        string TextToFormat = "";

        if (text.ToString().Length == 4)
        {
            if (text.ToString().Contains("000"))
            {
                return TextToFormat = text.ToString().Substring(0, 1) + "K";
            }
            else if (text.ToString().Contains("00"))
            {
                return TextToFormat = text.ToString().Substring(0, 1) + "." + text.ToString().Substring(1, 1) + "K";
            }
            else
            {
                return TextToFormat = text.ToString().Substring(0, 1) + "." + text.ToString().Substring(1, 2) + "K";
            }
        }
        else if (text.ToString().Length == 5)
        {
            if (text.ToString().Contains("0000") || text.ToString().Contains("000"))
            {
                return TextToFormat = text.ToString().Substring(0, 2) + "K";
            }
            else
            {
                return TextToFormat = text.ToString().Substring(0, 2) + "." + text.ToString().Substring(2, 2) + "K";
            }
        }
        else if (text.ToString().Length == 6)
        {
            if (text.ToString().Contains("00000") || text.ToString().Contains("0000") || text.ToString().Contains("000"))
            {
                return TextToFormat = text.ToString().Substring(0, 3) + "K";
            }
            else
            {
                return TextToFormat = text.ToString().Substring(0, 3) + "." + text.ToString().Substring(3, 2) + "K";
            }
        }
        else if (text.ToString().Length == 7)
        {
            if (text.ToString().Contains("000000"))
            {
                return TextToFormat = text.ToString().Substring(0, 1) + "M";
            }
            else if (text.ToString().Contains("00000"))
            {
                return TextToFormat = text.ToString().Substring(0, 1) + "." + text.ToString().Substring(1, 1) + "M";
            }
            else
            {
                return TextToFormat = text.ToString().Substring(0, 1) + "." + text.ToString().Substring(1, 2) + "M";
            }
        }
        else if (text.ToString().Length == 8)
        {
            if (text.ToString().Contains("0000000") || text.ToString().Contains("000000"))
            {
                return TextToFormat = text.ToString().Substring(0, 2) + "M";
            }
            else
            {
                return TextToFormat = text.ToString().Substring(0, 2) + "." + text.ToString().Substring(2, 2) + "M";
            }
        }
        else if (text.ToString().Length == 9)
        {
            if (text.ToString().Contains("00000000") || text.ToString().Contains("0000000"))
            {
                return TextToFormat = text.ToString().Substring(0, 3) + "M";
            }
            else
            {
                return TextToFormat = text.ToString().Substring(0, 3) + "." + text.ToString().Substring(3, 2) + "M";
            }
        }
        else if (text.ToString().Length == 10)
        {
            if (text.ToString().Contains("000000000"))
            {
                return TextToFormat = text.ToString().Substring(0, 1) + "B";
            }
            else if (text.ToString().Contains("00000000"))
            {
                return TextToFormat = text.ToString().Substring(0, 1) + "." + text.ToString().Substring(1, 1) + "B";
            }
            else
            {
                return TextToFormat = text.ToString().Substring(0, 1) + "." + text.ToString().Substring(1, 2) + "B";
            }
        }
        else if (text.ToString().Length == 11)
        {
            if (text.ToString().Contains("0000000000") || text.ToString().Contains("000000000"))
            {
                return TextToFormat = text.ToString().Substring(0, 2) + "B";
            }
            else
            {
                return TextToFormat = text.ToString().Substring(0, 2) + "." + text.ToString().Substring(2, 2) + "B";
            }
        }
        else if (text.ToString().Length == 12)
        {
            if (text.ToString().Contains("00000000000") || text.ToString().Contains("0000000000"))
            {
                return TextToFormat = text.ToString().Substring(0, 3) + "B";
            }
            else
            {
                return TextToFormat = text.ToString().Substring(0, 3) + "." + text.ToString().Substring(3, 2) + "B";
            }
        }
        else if (text.ToString().Length == 13)
        {
            if (text.ToString().Contains("000000000000"))
            {
                return TextToFormat = text.ToString().Substring(0, 1) + "T";
            }
            else if (text.ToString().Contains("00000000000"))
            {
                return TextToFormat = text.ToString().Substring(0, 1) + "." + text.ToString().Substring(1, 1) + "T";
            }
            else
            {
                return TextToFormat = text.ToString().Substring(0, 1) + "." + text.ToString().Substring(1, 2) + "T";
            }
        }
        else if (text.ToString().Length == 14)
        {
            if (text.ToString().Contains("0000000000000") || text.ToString().Contains("000000000000"))
            {
                return TextToFormat = text.ToString().Substring(0, 2) + "T";
            }
            else
            {
                return TextToFormat = text.ToString().Substring(0, 2) + "." + text.ToString().Substring(2, 2) + "T";
            }
        }
        else if (text.ToString().Length == 15)
        {
            if (text.ToString().Contains("00000000000000") || text.ToString().Contains("0000000000000"))
            {
                return TextToFormat = text.ToString().Substring(0, 3) + "T";
            }
            else
            {
                return TextToFormat = text.ToString().Substring(0, 3) + "." + text.ToString().Substring(3, 2) + "T";
            }
        }
        else if (text.ToString().Length == 16)
        {
            if (text.ToString().Contains("000000000000000"))
            {
                return TextToFormat = text.ToString().Substring(0, 1) + "Q";
            }
            else if (text.ToString().Contains("00000000000000"))
            {
                return TextToFormat = text.ToString().Substring(0, 1) + "." + text.ToString().Substring(1, 1) + "Q";
            }
            else
            {
                return TextToFormat = text.ToString().Substring(0, 1) + "." + text.ToString().Substring(1, 2) + "Q";
            }
        }
        else if (text.ToString().Length == 17)
        {
            if (text.ToString().Contains("0000000000000000") || text.ToString().Contains("000000000000000"))
            {
                return TextToFormat = text.ToString().Substring(0, 2) + "Q";
            }
            else
            {
                return TextToFormat = text.ToString().Substring(0, 2) + "." + text.ToString().Substring(2, 2) + "Q";
            }
        }
        else if (text.ToString().Length == 18)
        {
            if (text.ToString().Contains("00000000000000000") || text.ToString().Contains("0000000000000000"))
            {
                return TextToFormat = text.ToString().Substring(0, 3) + "Q";
            }
            else
            {
                return TextToFormat = text.ToString().Substring(0, 3) + "." + text.ToString().Substring(3, 2) + "Q";
            }
        }
        else if (text.ToString().Length == 19)
        {
            if (text.ToString().Contains("000000000000000000"))
            {
                return TextToFormat = text.ToString().Substring(0, 1) + "E";
            }
            else if (text.ToString().Contains("00000000000000000"))
            {
                return TextToFormat = text.ToString().Substring(0, 1) + "." + text.ToString().Substring(1, 1) + "E";
            }
            else
            {
                return TextToFormat = text.ToString().Substring(0, 1) + "." + text.ToString().Substring(1, 2) + "E";
            }
        }
        else if (text.ToString().Length == 20)
        {
            if (text.ToString().Contains("0000000000000000000") || text.ToString().Contains("000000000000000000"))
            {
                return TextToFormat = text.ToString().Substring(0, 2) + "E";
            }
            else
            {
                return TextToFormat = text.ToString().Substring(0, 2) + "." + text.ToString().Substring(2, 2) + "E";
            }
        }
        else
        {
            return TextToFormat = text.ToString();
        }
    }

    private void AnimalsTiming()
    {
        if (animalarea[4] != false && animalQt[4] >= 1 && animalQt[5] == 0)
        {
            time[8] += 0.020f;
            switch (AnimalA[4])
            {
                case "Fish":
                    if (time[8] >= 10.0f && time[8] <= 10.1f)
                    {
                        animalFarmMoney[0] += (ulong)UnityEngine.Random.Range(30 + ValueOfPlayerLevel, 130 + ValueOfPlayerLevel + 1) * (ulong)animalQt[4];
                        Block[10].SetActive(true);
                        time[8] = 0f;
                        audioManager.PlaySFX(audioManager.Pop);
                        Cloud[5].SetActive(true);
                        animalarea[4] = false;
                    }
                    break;
                default:
                    audioManager.PlaySFX(audioManager.Pop2);
                    alert[1].SetActive(true);
                    alert[10].SetActive(true);
                    StartCoroutine(ResetGame());
                    break;
            }
            
        }
        else if (animalObj[8].activeSelf && animalarea[4] != false)
        {
            time[8] += 0.020f;

            if (animalQt[4] >= 1 && animalQt[5] >= 1)
            {
                switch (AnimalA[4])
                {
                    case "Fish":
                        if (time[8] >= 10.0f && time[8] <= 10.1f)
                        {
                            animalFarmMoney[0] += (ulong)UnityEngine.Random.Range(30 + ValueOfPlayerLevel, 130 + ValueOfPlayerLevel + 1) * (ulong)animalQt[4];
                        }
                        break;
                    default:
                        audioManager.PlaySFX(audioManager.Pop2);
                        alert[1].SetActive(true);
                        alert[10].SetActive(true);
                        StartCoroutine(ResetGame());
                        break;
                }

                switch (AnimalB[5])
                {
                    case "Fish":
                        if (time[8] >= 10.0f && time[8] <= 10.1f)
                        {
                            animalFarmMoney[0] += (ulong)UnityEngine.Random.Range(30 + ValueOfPlayerLevel, 130 + ValueOfPlayerLevel + 1) * (ulong)animalQt[5];
                            Block[10].SetActive(true);
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[5].SetActive(true);
                            time[8] = 0f;
                            animalarea[4] = false;
                        }
                        break;
                    default:
                        audioManager.PlaySFX(audioManager.Pop2);
                        alert[1].SetActive(true);
                        alert[10].SetActive(true);
                        StartCoroutine(ResetGame());
                        break;
                }
            }

            else
            {
                audioManager.PlaySFX(audioManager.Pop2);
                alert[1].SetActive(true);
                alert[10].SetActive(true);
                StartCoroutine(ResetGame());
            }
        }

        if (animalObj[0].activeSelf && animalObj[1].activeSelf && !animalObj[4].activeSelf && !animalObj[5].activeSelf && animalarea[0] != false 
            || !animalObj[0].activeSelf && animalObj[4].activeSelf && animalObj[1].activeSelf && !animalObj[5].activeSelf && animalarea[0] != false
            || !animalObj[1].activeSelf && animalObj[0].activeSelf && animalObj[5].activeSelf && !animalObj[4].activeSelf && animalarea[0] != false
            || animalObj[4].activeSelf && animalObj[5].activeSelf && !animalObj[0].activeSelf && !animalObj[1].activeSelf && animalarea[0] != false)
        {
            if (time[9] >= 15.0f && time[9] <= 15.1f)
            {
                audioManager.PlaySFX(audioManager.Pop);

                switch (AnimalA[0])
                {
                    case "Chicken":
                        animalFarmMoney[1] += (ulong)UnityEngine.Random.Range(70 + ValueOfPlayerLevel, 270 + ValueOfPlayerLevel + 1) * (ulong)animalQt[0];
                        break;
                    case "Pig":
                        animalFarmMoney[1] += (ulong)UnityEngine.Random.Range(230 + ValueOfPlayerLevel, 530 + ValueOfPlayerLevel + 1) * (ulong)animalQt[0];
                        break;
                    case "Buffalo":
                        animalFarmMoney[1] += (ulong)UnityEngine.Random.Range(480 + ValueOfPlayerLevel, 1030 + ValueOfPlayerLevel + 1) * (ulong)animalQt[0];
                        break;
                    case "Cow":
                        animalFarmMoney[1] += (ulong)UnityEngine.Random.Range(980 + ValueOfPlayerLevel, 2330 + ValueOfPlayerLevel + 1) * (ulong)animalQt[0];
                        break;
                    case "Horse":
                        animalFarmMoney[1] += (ulong)UnityEngine.Random.Range(1480 + ValueOfPlayerLevel, 4980 + ValueOfPlayerLevel + 1) * (ulong)animalQt[0];
                        break;
                    default:
                        audioManager.PlaySFX(audioManager.Pop2);
                        alert[1].SetActive(true);
                        alert[10].SetActive(true);
                        StartCoroutine(ResetGame());
                        break;
                }

                switch (AnimalB[1])
                {
                    case "Chicken":
                        animalFarmMoney[1] += (ulong)UnityEngine.Random.Range(70 + ValueOfPlayerLevel, 270 + ValueOfPlayerLevel + 1) * (ulong)animalQt[1];
                        Block[9].SetActive(true);
                        time[9] = 0f;
                        Cloud[7].SetActive(true);
                        animalarea[0] = false;
                        break;
                    case "Pig":
                        animalFarmMoney[1] += (ulong)UnityEngine.Random.Range(230 + ValueOfPlayerLevel, 530 + ValueOfPlayerLevel + 1) * (ulong)animalQt[1];
                        Block[9].SetActive(true);
                        time[9] = 0f;
                        Cloud[7].SetActive(true);
                        animalarea[0] = false;
                        break;
                    case "Buffalo":
                        animalFarmMoney[1] += (ulong)UnityEngine.Random.Range(480 + ValueOfPlayerLevel, 1030 + ValueOfPlayerLevel + 1) * (ulong)animalQt[1];
                        Block[9].SetActive(true);
                        time[9] = 0f;
                        Cloud[7].SetActive(true);
                        animalarea[0] = false;
                        break;
                    case "Cow":
                        animalFarmMoney[1] += (ulong)UnityEngine.Random.Range(980 + ValueOfPlayerLevel, 2330 + ValueOfPlayerLevel + 1) * (ulong)animalQt[1];
                        Block[9].SetActive(true);
                        time[9] = 0f;
                        Cloud[7].SetActive(true);
                        animalarea[0] = false;
                        break;
                    case "Horse":
                        animalFarmMoney[1] += (ulong)UnityEngine.Random.Range(1480 + ValueOfPlayerLevel, 4980 + ValueOfPlayerLevel + 1) * (ulong)animalQt[1];
                        Block[9].SetActive(true);
                        time[9] = 0f;
                        Cloud[7].SetActive(true);
                        animalarea[0] = false;
                        break;
                    default:
                        audioManager.PlaySFX(audioManager.Pop2);
                        alert[1].SetActive(true);
                        alert[10].SetActive(true);
                        StartCoroutine(ResetGame());
                        break;
                }
            }
            else
            {
                time[9] += 0.020f;
            }
        }
            
        else if (animalObj[0].activeSelf && !animalObj[1].activeSelf && !animalObj[4].activeSelf && !animalObj[5].activeSelf && animalarea[0] != false
                 || animalObj[4].activeSelf && !animalObj[0].activeSelf && !animalObj[1].activeSelf && !animalObj[5].activeSelf && animalarea[0] != false)
        {
            time[9] += 0.020f;

            if (time[9] >= 15.0f && time[9] <= 15.1f)
            {

                audioManager.PlaySFX(audioManager.Pop);

                switch (AnimalA[0])
                {
                    case "Chicken":
                        animalFarmMoney[1] += (ulong)UnityEngine.Random.Range(70 + ValueOfPlayerLevel, 270 + ValueOfPlayerLevel + 1) * (ulong)animalQt[0];
                        Block[9].SetActive(true);
                        time[9] = 0f;
                        Cloud[7].SetActive(true);
                        animalarea[0] = false;
                        break;
                    case "Pig":
                        animalFarmMoney[1] += (ulong)UnityEngine.Random.Range(230 + ValueOfPlayerLevel, 530 + ValueOfPlayerLevel + 1) * (ulong)animalQt[0];
                        Block[9].SetActive(true);
                        time[9] = 0f;
                        Cloud[7].SetActive(true);
                        animalarea[0] = false;
                        break;
                    case "Buffalo":
                        animalFarmMoney[1] += (ulong)UnityEngine.Random.Range(480 + ValueOfPlayerLevel, 1030 + ValueOfPlayerLevel + 1) * (ulong)animalQt[0];
                        Block[9].SetActive(true);
                        time[9] = 0f;
                        Cloud[7].SetActive(true);
                        animalarea[0] = false;
                        break;
                    case "Cow":
                        animalFarmMoney[1] += (ulong)UnityEngine.Random.Range(980 + ValueOfPlayerLevel, 2330 + ValueOfPlayerLevel + 1) * (ulong)animalQt[0];
                        Block[9].SetActive(true);
                        time[9] = 0f;
                        Cloud[7].SetActive(true);
                        animalarea[0] = false;
                        break;
                    case "Horse":
                        animalFarmMoney[1] += (ulong)UnityEngine.Random.Range(1480 + ValueOfPlayerLevel, 4980 + ValueOfPlayerLevel + 1) * (ulong)animalQt[0];
                        Block[9].SetActive(true);
                        time[9] = 0f;
                        Cloud[7].SetActive(true);
                        animalarea[0] = false;
                        break;
                    default:
                        audioManager.PlaySFX(audioManager.Pop2);
                        alert[1].SetActive(true);
                        alert[10].SetActive(true);
                        StartCoroutine(ResetGame());
                        break;
                }
            }
            else
            {
                time[9] += 0.020f;
            }
        }

        if (animalObj[2].activeSelf && animalObj[3].activeSelf && !animalObj[6].activeSelf && !animalObj[7].activeSelf && animalarea[2] != false
            || !animalObj[2].activeSelf && animalObj[6].activeSelf && animalObj[3].activeSelf && !animalObj[7].activeSelf && animalarea[2] != false
            || !animalObj[3].activeSelf && animalObj[2].activeSelf && animalObj[7].activeSelf && !animalObj[6].activeSelf && animalarea[2] != false
            || animalObj[6].activeSelf && animalObj[7].activeSelf && !animalObj[2].activeSelf && !animalObj[3].activeSelf && animalarea[2] != false)
        {
            time[10] += 0.020f;

            if (time[10] >= 15.0f && time[10] <= 15.1f)
            {

                audioManager.PlaySFX(audioManager.Pop);

                switch (AnimalA[2])
                {
                    case "Chicken":
                        animalFarmMoney[2] += (ulong)UnityEngine.Random.Range(70 + ValueOfPlayerLevel, 270 + ValueOfPlayerLevel + 1) * (ulong)animalQt[2];
                        break;
                    case "Pig":
                        animalFarmMoney[2] += (ulong)UnityEngine.Random.Range(230 + ValueOfPlayerLevel, 530 + ValueOfPlayerLevel + 1) * (ulong)animalQt[2];
                        break;
                    case "Buffalo":
                        animalFarmMoney[2] += (ulong)UnityEngine.Random.Range(480 + ValueOfPlayerLevel, 1030 + ValueOfPlayerLevel + 1) * (ulong)animalQt[2];
                        break;
                    case "Cow":
                        animalFarmMoney[2] += (ulong)UnityEngine.Random.Range(980 + ValueOfPlayerLevel, 2330 + ValueOfPlayerLevel + 1) * (ulong)animalQt[2];
                        break;
                    case "Horse":
                        animalFarmMoney[2] += (ulong)UnityEngine.Random.Range(1480 + ValueOfPlayerLevel, 4980 + ValueOfPlayerLevel + 1) * (ulong)animalQt[2];
                        break;
                    default:
                        audioManager.PlaySFX(audioManager.Pop2);
                        alert[1].SetActive(true);
                        alert[10].SetActive(true);
                        StartCoroutine(ResetGame());
                        break;
                }

                switch (AnimalB[3])
                {
                    case "Chicken":
                        animalFarmMoney[2] += (ulong)UnityEngine.Random.Range(70 + ValueOfPlayerLevel, 270 + ValueOfPlayerLevel + 1) * (ulong)animalQt[3];
                        Block[8].SetActive(true);
                        time[10] = 0f;
                        Cloud[9].SetActive(true);
                        animalarea[2] = false;
                        break;
                    case "Pig":
                        animalFarmMoney[2] += (ulong)UnityEngine.Random.Range(230 + ValueOfPlayerLevel, 530 + ValueOfPlayerLevel + 1) * (ulong)animalQt[3];
                        Block[8].SetActive(true);
                        time[10] = 0f;
                        Cloud[9].SetActive(true);
                        animalarea[2] = false;
                        break;
                    case "Buffalo":
                        animalFarmMoney[2] += (ulong)UnityEngine.Random.Range(480 + ValueOfPlayerLevel, 1030 + ValueOfPlayerLevel + 1) * (ulong)animalQt[3];
                        Block[8].SetActive(true);
                        time[10] = 0f;
                        Cloud[9].SetActive(true);
                        animalarea[2] = false;
                        break;
                    case "Cow":
                        animalFarmMoney[2] += (ulong)UnityEngine.Random.Range(980 + ValueOfPlayerLevel, 2330 + ValueOfPlayerLevel + 1) * (ulong)animalQt[3];
                        Block[8].SetActive(true);
                        time[10] = 0f;
                        Cloud[9].SetActive(true);
                        animalarea[2] = false;
                        break;
                    case "Horse":
                        animalFarmMoney[2] += (ulong)UnityEngine.Random.Range(1480 + ValueOfPlayerLevel, 4980 + ValueOfPlayerLevel + 1) * (ulong)animalQt[3];
                        Block[8].SetActive(true);
                        time[10] = 0f;
                        Cloud[9].SetActive(true);
                        animalarea[2] = false;
                        break;
                    default:
                        audioManager.PlaySFX(audioManager.Pop2);
                        alert[1].SetActive(true);
                        alert[10].SetActive(true);
                        StartCoroutine(ResetGame());
                        break;
                }
            }
            else
            {
                time[10] += 0.020f;
            }
        }
        else if (animalObj[2].activeSelf && !animalObj[3].activeSelf && !animalObj[6].activeSelf && !animalObj[7].activeSelf && animalarea[2] != false
                 || animalObj[6].activeSelf && !animalObj[2].activeSelf && !animalObj[3].activeSelf && !animalObj[7].activeSelf && animalarea[2] != false)
        {

            if (time[10] >= 15.0f && time[10] <= 15.1f)
            {

                audioManager.PlaySFX(audioManager.Pop);

                switch (AnimalA[2])
                {
                    case "Chicken":
                        animalFarmMoney[2] += (ulong)UnityEngine.Random.Range(70 + ValueOfPlayerLevel, 270 + ValueOfPlayerLevel + 1) * (ulong)animalQt[2];
                        Block[8].SetActive(true);
                        time[10] = 0f;
                        Cloud[9].SetActive(true);
                        animalarea[2] = false;
                        break;
                    case "Pig":
                        animalFarmMoney[2] += (ulong)UnityEngine.Random.Range(230 + ValueOfPlayerLevel, 530 + ValueOfPlayerLevel + 1) * (ulong)animalQt[2];
                        Block[8].SetActive(true);
                        time[10] = 0f;
                        Cloud[9].SetActive(true);
                        animalarea[2] = false;
                        break;
                    case "Buffalo":
                        animalFarmMoney[2] += (ulong)UnityEngine.Random.Range(480 + ValueOfPlayerLevel, 1030 + ValueOfPlayerLevel + 1) * (ulong)animalQt[2];
                        Block[8].SetActive(true);
                        time[10] = 0f;
                        Cloud[9].SetActive(true);
                        animalarea[2] = false;
                        break;
                    case "Cow":
                        animalFarmMoney[2] += (ulong)UnityEngine.Random.Range(980 + ValueOfPlayerLevel, 2330 + ValueOfPlayerLevel + 1) * (ulong)animalQt[2];
                        Block[8].SetActive(true);
                        time[10] = 0f;
                        Cloud[9].SetActive(true);
                        animalarea[2] = false;
                        break;
                    case "Horse":
                        animalFarmMoney[2] += (ulong)UnityEngine.Random.Range(1480 + ValueOfPlayerLevel, 4980 + ValueOfPlayerLevel + 1) * (ulong)animalQt[2];
                        Block[8].SetActive(true);
                        time[10] = 0f;
                        Cloud[9].SetActive(true);
                        animalarea[2] = false;
                        break;
                    default:
                        audioManager.PlaySFX(audioManager.Pop2);
                        alert[1].SetActive(true);
                        alert[10].SetActive(true);
                        StartCoroutine(ResetGame());
                        break;
                }
            }
            else
            {
                time[10] += 0.020f;
            }
        }
    }

    private void PlantTiming()
    {
        //Area Level 0

        if (plants[0].gameObject.activeSelf && plants[1].gameObject.activeSelf && plantarea[0] != false)
        {
            time[0] += 0.020f;

            if (time[0] >= 10.0f && time[0] <= 10.1f)
            {
                switch (PlantA[0])
                {
                    case "Carrot":
                        plants[0].GetComponent<Image>().sprite = CarrotGrow[1];
                        plantsGrowLevel[0] = 1;
                        break;
                    case "Corn":
                        plants[0].GetComponent<Image>().sprite = CornGrow[1];
                        plantsGrowLevel[0] = 1;
                        break;
                    case "Radish":
                        plants[0].GetComponent<Image>().sprite = RadishGrow[1];
                        plantsGrowLevel[0] = 1;
                        break;
                    case "Turnip":
                        plants[0].GetComponent<Image>().sprite = TurnipGrow[1];
                        plantsGrowLevel[0] = 1;
                        break;
                }

                switch (PlantB[1])
                {
                    case "Carrot":
                        plants[1].GetComponent<Image>().sprite = CarrotGrow[1];
                        plantsGrowLevel[1] = 1;
                        break;
                    case "Corn":
                        plants[1].GetComponent<Image>().sprite = CornGrow[1];
                        plantsGrowLevel[1] = 1;
                        break;
                    case "Radish":
                        plants[1].GetComponent<Image>().sprite = RadishGrow[1];
                        plantsGrowLevel[1] = 1;
                        break;
                    case "Turnip":
                        plants[1].GetComponent<Image>().sprite = TurnipGrow[1];
                        plantsGrowLevel[1] = 1;
                        break;
                }
            }
            else
            {
                if (time[0] >= 20.0f && time[0] <= 20.1f)
                {
                    switch (PlantA[0])
                    {
                        case "Carrot":
                            plants[0].GetComponent<Image>().sprite = CarrotGrow[2];
                            plantsGrowLevel[0] = 2;
                            break;
                        case "Corn":
                            plants[0].GetComponent<Image>().sprite = CornGrow[2];
                            plantsGrowLevel[0] = 2;
                            break;
                        case "Radish":
                            plants[0].GetComponent<Image>().sprite = RadishGrow[2];
                            plantsGrowLevel[0] = 2;
                            break;
                        case "Turnip":
                            plants[0].GetComponent<Image>().sprite = TurnipGrow[2];
                            plantsGrowLevel[0] = 2;
                            break;
                    }

                    switch (PlantB[1])
                    {
                        case "Carrot":
                            plants[1].GetComponent<Image>().sprite = CarrotGrow[2];
                            plantsGrowLevel[1] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[0].SetActive(true);
                            break;
                        case "Corn":
                            plants[1].GetComponent<Image>().sprite = CornGrow[2];
                            plantsGrowLevel[1] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[0].SetActive(true);
                            break;
                        case "Radish":
                            plants[1].GetComponent<Image>().sprite = RadishGrow[2];
                            plantsGrowLevel[1] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[0].SetActive(true);
                            break;
                        case "Turnip":
                            plants[1].GetComponent<Image>().sprite = TurnipGrow[2];
                            plantsGrowLevel[1] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[0].SetActive(true);
                            break;
                    }
                    time[0] = 0f;
                    plantarea[0] = false;
                }
            }
        }

        else if (plants[0].gameObject.activeSelf && !plants[1].gameObject.activeSelf && plantarea[0] != false)
        {
            time[0] += 0.020f;

            if (time[0] >= 10.0f && time[0] <= 10.1f)
            {
                switch (PlantA[0])
                {
                    case "Carrot":
                        plants[0].GetComponent<Image>().sprite = CarrotGrow[1];
                        plantsGrowLevel[0] = 1;
                        break;
                    case "Corn":
                        plants[0].GetComponent<Image>().sprite = CornGrow[1];
                        plantsGrowLevel[0] = 1;
                        break;
                    case "Radish":
                        plants[0].GetComponent<Image>().sprite = RadishGrow[1];
                        plantsGrowLevel[0] = 1;
                        break;
                    case "Turnip":
                        plants[0].GetComponent<Image>().sprite = TurnipGrow[1];
                        plantsGrowLevel[0] = 1;
                        break;
                }
            }
            else
            {
                if (time[0] >= 20.0f && time[0] <= 20.1f)
                {
                    switch (PlantA[0])
                    {
                        case "Carrot":
                            plants[0].GetComponent<Image>().sprite = CarrotGrow[2];
                            plantsGrowLevel[0] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[0].SetActive(true);
                            break;
                        case "Corn":
                            plants[0].GetComponent<Image>().sprite = CornGrow[2];
                            plantsGrowLevel[0] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[0].SetActive(true);
                            break;
                        case "Radish":
                            plants[0].GetComponent<Image>().sprite = RadishGrow[2];
                            plantsGrowLevel[0] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[0].SetActive(true);
                            break;
                        case "Turnip":
                            plants[0].GetComponent<Image>().sprite = TurnipGrow[2];
                            plantsGrowLevel[0] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[0].SetActive(true);
                            break;
                    }
                    time[0] = 0f;
                    plantarea[0] = false;
                }
            }
        }

        //Area Level 1

        if (plants[2].gameObject.activeSelf && plants[3].gameObject.activeSelf && plantarea[2] != false)
        {
            time[1] += 0.020f;

            if (time[1] >= 10.0f && time[1] <= 10.1f)
            {
                switch (PlantA[2])
                {
                    case "Carrot":
                        plants[2].GetComponent<Image>().sprite = CarrotGrow[1];
                        plantsGrowLevel[2] = 1;
                        break;
                    case "Corn":
                        plants[2].GetComponent<Image>().sprite = CornGrow[1];
                        plantsGrowLevel[2] = 1;
                        break;
                    case "Radish":
                        plants[2].GetComponent<Image>().sprite = RadishGrow[1];
                        plantsGrowLevel[2] = 1;
                        break;
                    case "Turnip":
                        plants[2].GetComponent<Image>().sprite = TurnipGrow[1];
                        plantsGrowLevel[2] = 1;
                        break;
                }

                switch (PlantB[3])
                {
                    case "Carrot":
                        plants[3].GetComponent<Image>().sprite = CarrotGrow[1];
                        plantsGrowLevel[3] = 2;
                        break;
                    case "Corn":
                        plants[3].GetComponent<Image>().sprite = CornGrow[1];
                        plantsGrowLevel[3] = 2;
                        break;
                    case "Radish":
                        plants[3].GetComponent<Image>().sprite = RadishGrow[1];
                        plantsGrowLevel[3] = 2;
                        break;
                    case "Turnip":
                        plants[3].GetComponent<Image>().sprite = TurnipGrow[1];
                        plantsGrowLevel[3] = 2;
                        break;
                }
            }
            else
            {
                if (time[1] >= 20.0f && time[1] <= 20.1f)
                {
                    switch (PlantA[2])
                    {
                        case "Carrot":
                            plants[2].GetComponent<Image>().sprite = CarrotGrow[2];
                            plantsGrowLevel[2] = 2;
                            break;
                        case "Corn":
                            plants[2].GetComponent<Image>().sprite = CornGrow[2];
                            plantsGrowLevel[2] = 2;
                            break;
                        case "Radish":
                            plants[2].GetComponent<Image>().sprite = RadishGrow[2];
                            plantsGrowLevel[2] = 2;
                            break;
                        case "Turnip":
                            plants[2].GetComponent<Image>().sprite = TurnipGrow[2];
                            plantsGrowLevel[2] = 2;
                            break;
                    }

                    switch (PlantB[3])
                    {
                        case "Carrot":
                            plants[3].GetComponent<Image>().sprite = CarrotGrow[2];
                            plantsGrowLevel[3] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[1].SetActive(true);
                            break;
                        case "Corn":
                            plants[3].GetComponent<Image>().sprite = CornGrow[2];
                            plantsGrowLevel[3] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[1].SetActive(true);
                            break;
                        case "Radish":
                            plants[3].GetComponent<Image>().sprite = RadishGrow[2];
                            plantsGrowLevel[3] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[1].SetActive(true);
                            break;
                        case "Turnip":
                            plants[3].GetComponent<Image>().sprite = TurnipGrow[2];
                            plantsGrowLevel[3] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[1].SetActive(true);
                            break;
                    }
                    time[1] = 0f;
                    plantarea[2] = false;
                }
            }
        }

        else if (plants[2].gameObject.activeSelf && !plants[3].gameObject.activeSelf && plantarea[2] != false)
        {
            time[1] += 0.020f;

            if (time[1] >= 10.0f && time[1] <= 10.1f)
            {
                switch (PlantA[2])
                {
                    case "Carrot":
                        plants[2].GetComponent<Image>().sprite = CarrotGrow[1];
                        plantsGrowLevel[2] = 1;
                        break;
                    case "Corn":
                        plants[2].GetComponent<Image>().sprite = CornGrow[1];
                        plantsGrowLevel[2] = 1;
                        break;
                    case "Radish":
                        plants[2].GetComponent<Image>().sprite = RadishGrow[1];
                        plantsGrowLevel[2] = 1;
                        break;
                    case "Turnip":
                        plants[2].GetComponent<Image>().sprite = TurnipGrow[1];
                        plantsGrowLevel[2] = 1;
                        break;
                }
            }
            else
            {
                if (time[1] >= 20.0f && time[1] <= 20.1f)
                {
                    switch (PlantA[2])
                    {
                        case "Carrot":
                            plants[2].GetComponent<Image>().sprite = CarrotGrow[2];
                            plantsGrowLevel[2] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[1].SetActive(true);
                            break;
                        case "Corn":
                            plants[2].GetComponent<Image>().sprite = CornGrow[2];
                            plantsGrowLevel[2] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[1].SetActive(true);
                            break;
                        case "Radish":
                            plants[2].GetComponent<Image>().sprite = RadishGrow[2];
                            plantsGrowLevel[2] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[1].SetActive(true);
                            break;
                        case "Turnip":
                            plants[2].GetComponent<Image>().sprite = TurnipGrow[2];
                            plantsGrowLevel[2] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[1].SetActive(true);
                            break;
                    }
                    time[1] = 0f;
                    plantarea[2] = false;
                }
            }
        }

        //Area Level 2

        if (plants[4].gameObject.activeSelf && plants[5].gameObject.activeSelf && plantarea[4] != false)
        {
            time[2] += 0.020f;

            if (time[2] >= 10.0f && time[2] <= 10.1f)
            {
                switch (PlantA[4])
                {
                    case "Carrot":
                        plants[4].GetComponent<Image>().sprite = CarrotGrow[1];
                        plantsGrowLevel[4] = 1;
                        break;
                    case "Corn":
                        plants[4].GetComponent<Image>().sprite = CornGrow[1];
                        plantsGrowLevel[4] = 1;
                        break;
                    case "Radish":
                        plants[4].GetComponent<Image>().sprite = RadishGrow[1];
                        plantsGrowLevel[4] = 1;
                        break;
                    case "Turnip":
                        plants[4].GetComponent<Image>().sprite = TurnipGrow[1];
                        plantsGrowLevel[4] = 1;
                        break;
                }

                switch (PlantB[5])
                {
                    case "Carrot":
                        plants[5].GetComponent<Image>().sprite = CarrotGrow[1];
                        plantsGrowLevel[5] = 1;
                        break;
                    case "Corn":
                        plants[5].GetComponent<Image>().sprite = CornGrow[1];
                        plantsGrowLevel[5] = 1;
                        break;
                    case "Radish":
                        plants[5].GetComponent<Image>().sprite = RadishGrow[1];
                        plantsGrowLevel[5] = 1;
                        break;
                    case "Turnip":
                        plants[5].GetComponent<Image>().sprite = TurnipGrow[1];
                        plantsGrowLevel[5] = 1;
                        break;
                }
            }
            else
            {
                if (time[2] >= 20.0f && time[2] <= 20.1f)
                {
                    switch (PlantA[4])
                    {
                        case "Carrot":
                            plants[4].GetComponent<Image>().sprite = CarrotGrow[2];
                            plantsGrowLevel[4] = 2;
                            break;
                        case "Corn":
                            plants[4].GetComponent<Image>().sprite = CornGrow[2];
                            plantsGrowLevel[4] = 2;
                            break;
                        case "Radish":
                            plants[4].GetComponent<Image>().sprite = RadishGrow[2];
                            plantsGrowLevel[4] = 2;
                            break;
                        case "Turnip":
                            plants[4].GetComponent<Image>().sprite = TurnipGrow[2];
                            plantsGrowLevel[4] = 2;
                            break;
                    }

                    switch (PlantB[5])
                    {
                        case "Carrot":
                            plants[5].GetComponent<Image>().sprite = CarrotGrow[2];
                            plantsGrowLevel[5] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[2].SetActive(true);
                            break;
                        case "Corn":
                            plants[5].GetComponent<Image>().sprite = CornGrow[2];
                            plantsGrowLevel[5] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[2].SetActive(true);
                            break;
                        case "Radish":
                            plants[5].GetComponent<Image>().sprite = RadishGrow[2];
                            plantsGrowLevel[5] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[2].SetActive(true);
                            break;
                        case "Turnip":
                            plants[5].GetComponent<Image>().sprite = TurnipGrow[2];
                            plantsGrowLevel[5] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[2].SetActive(true);
                            break;
                    }
                    time[2] = 0f;
                    plantarea[4] = false;
                }
            }
        }

        else if (plants[4].gameObject.activeSelf && !plants[5].gameObject.activeSelf && plantarea[4] != false)
        {
            time[2] += 0.020f;

            if (time[2] >= 10.0f && time[2] <= 10.1f)
            {
                switch (PlantA[2])
                {
                    case "Carrot":
                        plants[4].GetComponent<Image>().sprite = CarrotGrow[1];
                        plantsGrowLevel[4] = 1;
                        break;
                    case "Corn":
                        plants[4].GetComponent<Image>().sprite = CornGrow[1];
                        plantsGrowLevel[4] = 1;
                        break;
                    case "Radish":
                        plants[4].GetComponent<Image>().sprite = RadishGrow[1];
                        plantsGrowLevel[4] = 1;
                        break;
                    case "Turnip":
                        plants[4].GetComponent<Image>().sprite = TurnipGrow[1];
                        plantsGrowLevel[4] = 1;
                        break;
                }
            }
            else
            {
                if (time[2] >= 20.0f && time[2] <= 20.1f)
                {
                    switch (PlantA[2])
                    {
                        case "Carrot":
                            plants[4].GetComponent<Image>().sprite = CarrotGrow[2];
                            plantsGrowLevel[4] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[2].SetActive(true);
                            break;
                        case "Corn":
                            plants[4].GetComponent<Image>().sprite = CornGrow[2];
                            plantsGrowLevel[4] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[2].SetActive(true);
                            break;
                        case "Radish":
                            plants[4].GetComponent<Image>().sprite = RadishGrow[2];
                            plantsGrowLevel[4] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[2].SetActive(true);
                            break;
                        case "Turnip":
                            plants[4].GetComponent<Image>().sprite = TurnipGrow[2];
                            plantsGrowLevel[4] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[2].SetActive(true);
                            break;
                    }
                    time[2] = 0f;
                    plantarea[4] = false;
                }
            }
        }

        //Area Level 4

        if (plants[6].gameObject.activeSelf && plants[7].gameObject.activeSelf && plantarea[6] != false)
        {
            time[3] += 0.020f;

            if (time[3] >= 10.0f && time[3] <= 10.1f)
            {
                switch (PlantA[6])
                {
                    case "Carrot":
                        plants[6].GetComponent<Image>().sprite = CarrotGrow[1];
                        plantsGrowLevel[6] = 1;
                        break;
                    case "Corn":
                        plants[6].GetComponent<Image>().sprite = CornGrow[1];
                        plantsGrowLevel[6] = 1;
                        break;
                    case "Radish":
                        plants[6].GetComponent<Image>().sprite = RadishGrow[1];
                        plantsGrowLevel[6] = 1;
                        break;
                    case "Turnip":
                        plants[6].GetComponent<Image>().sprite = TurnipGrow[1];
                        plantsGrowLevel[6] = 1;
                        break;
                }

                switch (PlantB[7])
                {
                    case "Carrot":
                        plants[7].GetComponent<Image>().sprite = CarrotGrow[1];
                        plantsGrowLevel[7] = 1;
                        break;
                    case "Corn":
                        plants[7].GetComponent<Image>().sprite = CornGrow[1];
                        plantsGrowLevel[7] = 1;
                        break;
                    case "Radish":
                        plants[7].GetComponent<Image>().sprite = RadishGrow[1];
                        plantsGrowLevel[7] = 1;
                        break;
                    case "Turnip":
                        plants[7].GetComponent<Image>().sprite = TurnipGrow[1];
                        plantsGrowLevel[7] = 1;
                        break;
                }
            }
            else
            {
                if (time[3] >= 20.0f && time[3] <= 20.1f)
                {
                    switch (PlantA[6])
                    {
                        case "Carrot":
                            plants[6].GetComponent<Image>().sprite = CarrotGrow[2];
                            plantsGrowLevel[6] = 2;
                            break;
                        case "Corn":
                            plants[6].GetComponent<Image>().sprite = CornGrow[2];
                            plantsGrowLevel[6] = 2;
                            break;
                        case "Radish":
                            plants[6].GetComponent<Image>().sprite = RadishGrow[2];
                            plantsGrowLevel[6] = 2;
                            break;
                        case "Turnip":
                            plants[6].GetComponent<Image>().sprite = TurnipGrow[2];
                            plantsGrowLevel[6] = 2;
                            break;
                    }

                    switch (PlantB[7])
                    {
                        case "Carrot":
                            plants[7].GetComponent<Image>().sprite = CarrotGrow[2];
                            plantsGrowLevel[7] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[3].SetActive(true);
                            break;
                        case "Corn":
                            plants[7].GetComponent<Image>().sprite = CornGrow[2];
                            plantsGrowLevel[7] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[3].SetActive(true);
                            break;
                        case "Radish":
                            plants[7].GetComponent<Image>().sprite = RadishGrow[2];
                            plantsGrowLevel[7] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[3].SetActive(true);
                            break;
                        case "Turnip":
                            plants[7].GetComponent<Image>().sprite = TurnipGrow[2];
                            plantsGrowLevel[7] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[3].SetActive(true);
                            break;
                    }
                    time[3] = 0f;
                    plantarea[6] = false;
                }
            }
        }

        else if (plants[6].gameObject.activeSelf && !plants[7].gameObject.activeSelf && plantarea[6] != false)
        {
            time[3] += 0.020f;

            if (time[3] >= 10.0f && time[3] <= 10.1f)
            {
                switch (PlantA[6])
                {
                    case "Carrot":
                        plants[6].GetComponent<Image>().sprite = CarrotGrow[1];
                        plantsGrowLevel[6] = 1;
                        break;
                    case "Corn":
                        plants[6].GetComponent<Image>().sprite = CornGrow[1];
                        plantsGrowLevel[6] = 1;
                        break;
                    case "Radish":
                        plants[6].GetComponent<Image>().sprite = RadishGrow[1];
                        plantsGrowLevel[6] = 1;
                        break;
                    case "Turnip":
                        plants[6].GetComponent<Image>().sprite = TurnipGrow[1];
                        plantsGrowLevel[6] = 1;
                        break;
                }
            }
            else
            {
                if (time[3] >= 20.0f && time[3] <= 20.1f)
                {
                    switch (PlantA[6])
                    {
                        case "Carrot":
                            plants[6].GetComponent<Image>().sprite = CarrotGrow[2];
                            plantsGrowLevel[6] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[3].SetActive(true);
                            break;
                        case "Corn":
                            plants[6].GetComponent<Image>().sprite = CornGrow[2];
                            plantsGrowLevel[6] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[3].SetActive(true);
                            break;
                        case "Radish":
                            plants[6].GetComponent<Image>().sprite = RadishGrow[2];
                            plantsGrowLevel[6] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[3].SetActive(true);
                            break;
                        case "Turnip":
                            plants[6].GetComponent<Image>().sprite = TurnipGrow[2];
                            plantsGrowLevel[6] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[3].SetActive(true);
                            break;
                    }
                    time[3] = 0f;
                    plantarea[6] = false;
                }
            }
        }


        //Area Level 6

        if (plants[8].gameObject.activeSelf && plants[9].gameObject.activeSelf && plantarea[8] != false)
        {
            time[4] += 0.020f;

            if (time[4] >= 10.0f && time[4] <= 10.1f)
            {
                switch (PlantA[8])
                {
                    case "Carrot":
                        plants[8].GetComponent<Image>().sprite = CarrotGrow[1];
                        plantsGrowLevel[8] = 1;
                        break;
                    case "Corn":
                        plants[8].GetComponent<Image>().sprite = CornGrow[1];
                        plantsGrowLevel[8] = 1;
                        break;
                    case "Radish":
                        plants[8].GetComponent<Image>().sprite = RadishGrow[1];
                        plantsGrowLevel[8] = 1;
                        break;
                    case "Turnip":
                        plants[8].GetComponent<Image>().sprite = TurnipGrow[1];
                        plantsGrowLevel[8] = 1;
                        break;
                }

                switch (PlantB[9])
                {
                    case "Carrot":
                        plants[9].GetComponent<Image>().sprite = CarrotGrow[1];
                        plantsGrowLevel[9] = 1;
                        break;
                    case "Corn":
                        plants[9].GetComponent<Image>().sprite = CornGrow[1];
                        plantsGrowLevel[9] = 1;
                        break;
                    case "Radish":
                        plants[9].GetComponent<Image>().sprite = RadishGrow[1];
                        plantsGrowLevel[9] = 1;
                        break;
                    case "Turnip":
                        plants[9].GetComponent<Image>().sprite = TurnipGrow[1];
                        plantsGrowLevel[9] = 1;
                        break;
                }
            }
            else
            {
                if (time[4] >= 20.0f && time[4] <= 20.1f)
                {
                    switch (PlantA[8])
                    {
                        case "Carrot":
                            plants[8].GetComponent<Image>().sprite = CarrotGrow[2];
                            plantsGrowLevel[8] = 2;
                            break;
                        case "Corn":
                            plants[8].GetComponent<Image>().sprite = CornGrow[2];
                            plantsGrowLevel[8] = 2;
                            break;
                        case "Radish":
                            plants[8].GetComponent<Image>().sprite = RadishGrow[2];
                            plantsGrowLevel[8] = 2;
                            break;
                        case "Turnip":
                            plants[8].GetComponent<Image>().sprite = TurnipGrow[2];
                            plantsGrowLevel[8] = 2;
                            break;
                    }

                    switch (PlantB[9])
                    {
                        case "Carrot":
                            plants[9].GetComponent<Image>().sprite = CarrotGrow[2];
                            plantsGrowLevel[9] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[4].SetActive(true);
                            break;
                        case "Corn":
                            plants[9].GetComponent<Image>().sprite = CornGrow[2];
                            plantsGrowLevel[9] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[4].SetActive(true);
                            break;
                        case "Radish":
                            plants[9].GetComponent<Image>().sprite = RadishGrow[2];
                            plantsGrowLevel[9] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[4].SetActive(true);
                            break;
                        case "Turnip":
                            plants[9].GetComponent<Image>().sprite = TurnipGrow[2];
                            plantsGrowLevel[9] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[4].SetActive(true);
                            break;
                    }
                    time[4] = 0f;
                    plantarea[8] = false;
                }
            }
        }

        else if (plants[8].gameObject.activeSelf && !plants[9].gameObject.activeSelf && plantarea[8] != false)
        {
            time[4] += 0.020f;

            if (time[4] >= 10.0f && time[4] <= 10.1f)
            {
                switch (PlantA[8])
                {
                    case "Carrot":
                        plants[8].GetComponent<Image>().sprite = CarrotGrow[1];
                        plantsGrowLevel[8] = 1;
                        break;
                    case "Corn":
                        plants[8].GetComponent<Image>().sprite = CornGrow[1];
                        plantsGrowLevel[8] = 1;
                        break;
                    case "Radish":
                        plants[8].GetComponent<Image>().sprite = RadishGrow[1];
                        plantsGrowLevel[8] = 1;
                        break;
                    case "Turnip":
                        plants[8].GetComponent<Image>().sprite = TurnipGrow[1];
                        plantsGrowLevel[8] = 1;
                        break;
                }
            }
            else
            {
                if (time[4] >= 20.0f && time[4] <= 20.1f)
                {
                    switch (PlantA[8])
                    {
                        case "Carrot":
                            plants[8].GetComponent<Image>().sprite = CarrotGrow[2];
                            plantsGrowLevel[8] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[4].SetActive(true);
                            break;
                        case "Corn":
                            plants[8].GetComponent<Image>().sprite = CornGrow[2];
                            plantsGrowLevel[8] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[4].SetActive(true);
                            break;
                        case "Radish":
                            plants[8].GetComponent<Image>().sprite = RadishGrow[2];
                            plantsGrowLevel[8] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[4].SetActive(true);
                            break;
                        case "Turnip":
                            plants[8].GetComponent<Image>().sprite = TurnipGrow[2];
                            plantsGrowLevel[8] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[4].SetActive(true);
                            break;
                    }
                    time[4] = 0f;
                    plantarea[8] = false;
                }
            }
        }

        //Area Level 9

        if (plants[10].gameObject.activeSelf && plants[11].gameObject.activeSelf && plantarea[10] != false)
        {
            time[5] += 0.020f;

            if (time[5] >= 10.0f && time[5] <= 10.1f)
            {
                switch (PlantA[10])
                {
                    case "Carrot":
                        plants[10].GetComponent<Image>().sprite = CarrotGrow[1];
                        plantsGrowLevel[10] = 1;
                        break;
                    case "Corn":
                        plants[10].GetComponent<Image>().sprite = CornGrow[1];
                        plantsGrowLevel[10] = 1;
                        break;
                    case "Radish":
                        plants[10].GetComponent<Image>().sprite = RadishGrow[1];
                        plantsGrowLevel[10] = 1;
                        break;
                    case "Turnip":
                        plants[10].GetComponent<Image>().sprite = TurnipGrow[1];
                        plantsGrowLevel[10] = 1;
                        break;
                }

                switch (PlantB[11])
                {
                    case "Carrot":
                        plants[11].GetComponent<Image>().sprite = CarrotGrow[1];
                        plantsGrowLevel[11] = 1;
                        break;
                    case "Corn":
                        plants[11].GetComponent<Image>().sprite = CornGrow[1];
                        plantsGrowLevel[11] = 1;
                        break;
                    case "Radish":
                        plants[11].GetComponent<Image>().sprite = RadishGrow[1];
                        plantsGrowLevel[11] = 1;
                        break;
                    case "Turnip":
                        plants[11].GetComponent<Image>().sprite = TurnipGrow[1];
                        plantsGrowLevel[11] = 1;
                        break;
                }
            }
            else
            {
                if (time[5] >= 20.0f && time[5] <= 20.1f)
                {
                    switch (PlantA[10])
                    {
                        case "Carrot":
                            plants[10].GetComponent<Image>().sprite = CarrotGrow[2];
                            plantsGrowLevel[10] = 2;
                            break;
                        case "Corn":
                            plants[10].GetComponent<Image>().sprite = CornGrow[2];
                            plantsGrowLevel[10] = 2;
                            break;
                        case "Radish":
                            plants[10].GetComponent<Image>().sprite = RadishGrow[2];
                            plantsGrowLevel[10] = 2;
                            break;
                        case "Turnip":
                            plants[10].GetComponent<Image>().sprite = TurnipGrow[2];
                            plantsGrowLevel[10] = 2;
                            break;
                    }

                    switch (PlantB[11])
                    {
                        case "Carrot":
                            plants[11].GetComponent<Image>().sprite = CarrotGrow[2];
                            plantsGrowLevel[10] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[6].SetActive(true);
                            break;
                        case "Corn":
                            plants[11].GetComponent<Image>().sprite = CornGrow[2];
                            plantsGrowLevel[10] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[6].SetActive(true);
                            break;
                        case "Radish":
                            plants[11].GetComponent<Image>().sprite = RadishGrow[2];
                            plantsGrowLevel[10] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[6].SetActive(true);
                            break;
                        case "Turnip":
                            plants[11].GetComponent<Image>().sprite = TurnipGrow[2];
                            plantsGrowLevel[10] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[6].SetActive(true);
                            break;
                    }
                    time[5] = 0f;
                    plantarea[10] = false;
                }
            }
        }

        else if (plants[10].gameObject.activeSelf && !plants[11].gameObject.activeSelf && plantarea[10] != false)
        {
            time[5] += 0.020f;

            if (time[5] >= 10.0f && time[5] <= 10.1f)
            {
                switch (PlantA[10])
                {
                    case "Carrot":
                        plants[10].GetComponent<Image>().sprite = CarrotGrow[1];
                        plantsGrowLevel[10] = 1;
                        break;
                    case "Corn":
                        plants[10].GetComponent<Image>().sprite = CornGrow[1];
                        plantsGrowLevel[10] = 1;
                        break;
                    case "Radish":
                        plants[10].GetComponent<Image>().sprite = RadishGrow[1];
                        plantsGrowLevel[10] = 1;
                        break;
                    case "Turnip":
                        plants[10].GetComponent<Image>().sprite = TurnipGrow[1];
                        plantsGrowLevel[10] = 1;
                        break;
                }
            }
            else
            {
                if (time[5] >= 20.0f && time[5] <= 20.1f)
                {
                    switch (PlantA[10])
                    {
                        case "Carrot":
                            plants[10].GetComponent<Image>().sprite = CarrotGrow[2];
                            plantsGrowLevel[10] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[6].SetActive(true);
                            break;
                        case "Corn":
                            plants[10].GetComponent<Image>().sprite = CornGrow[2];
                            plantsGrowLevel[10] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[6].SetActive(true);
                            break;
                        case "Radish":
                            plants[10].GetComponent<Image>().sprite = RadishGrow[2];
                            plantsGrowLevel[10] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[6].SetActive(true);
                            break;
                        case "Turnip":
                            plants[10].GetComponent<Image>().sprite = TurnipGrow[2];
                            plantsGrowLevel[10] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[6].SetActive(true);
                            break;
                    }
                    time[5] = 0f;
                    plantarea[10] = false;
                }
            }
        }

        //Area Level 15

        if (plants[12].gameObject.activeSelf && plants[13].gameObject.activeSelf && plantarea[12] != false)
        {
            time[6] += 0.020f;

            if (time[6] >= 10.0f && time[6] <= 10.1f)
            {
                switch (PlantA[12])
                {
                    case "Carrot":
                        plants[12].GetComponent<Image>().sprite = CarrotGrow[1];
                        plantsGrowLevel[12] = 1;
                        break;
                    case "Corn":
                        plants[12].GetComponent<Image>().sprite = CornGrow[1];
                        plantsGrowLevel[12] = 1;
                        break;
                    case "Radish":
                        plants[12].GetComponent<Image>().sprite = RadishGrow[1];
                        plantsGrowLevel[12] = 1;
                        break;
                    case "Turnip":
                        plants[12].GetComponent<Image>().sprite = TurnipGrow[1];
                        plantsGrowLevel[12] = 1;
                        break;
                }

                switch (PlantB[13])
                {
                    case "Carrot":
                        plants[13].GetComponent<Image>().sprite = CarrotGrow[1];
                        plantsGrowLevel[13] = 1;
                        break;
                    case "Corn":
                        plants[13].GetComponent<Image>().sprite = CornGrow[1];
                        plantsGrowLevel[13] = 1;
                        break;
                    case "Radish":
                        plants[13].GetComponent<Image>().sprite = RadishGrow[1];
                        plantsGrowLevel[13] = 1;
                        break;
                    case "Turnip":
                        plants[13].GetComponent<Image>().sprite = TurnipGrow[1];
                        plantsGrowLevel[13] = 1;
                        break;
                }
            }
            else
            {
                if (time[6] >= 20.0f && time[6] <= 20.1f)
                {
                    switch (PlantA[12])
                    {
                        case "Carrot":
                            plants[12].GetComponent<Image>().sprite = CarrotGrow[2];
                            plantsGrowLevel[12] = 2;
                            break;
                        case "Corn":
                            plants[12].GetComponent<Image>().sprite = CornGrow[2];
                            plantsGrowLevel[12] = 2;
                            break;
                        case "Radish":
                            plants[12].GetComponent<Image>().sprite = RadishGrow[2];
                            plantsGrowLevel[12] = 2;
                            break;
                        case "Turnip":
                            plants[12].GetComponent<Image>().sprite = TurnipGrow[2];
                            plantsGrowLevel[12] = 2;
                            break;
                    }

                    switch (PlantB[13])
                    {
                        case "Carrot":
                            plants[13].GetComponent<Image>().sprite = CarrotGrow[2];
                            plantsGrowLevel[13] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[8].SetActive(true);
                            break;
                        case "Corn":
                            plants[13].GetComponent<Image>().sprite = CornGrow[2];
                            plantsGrowLevel[13] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[8].SetActive(true);
                            break;
                        case "Radish":
                            plants[13].GetComponent<Image>().sprite = RadishGrow[2];
                            plantsGrowLevel[13] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[8].SetActive(true);
                            break;
                        case "Turnip":
                            plants[13].GetComponent<Image>().sprite = TurnipGrow[2];
                            plantsGrowLevel[13] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[8].SetActive(true);
                            break;
                    }
                    time[6] = 0f;
                    plantarea[12] = false;
                }
            }
        }

        else if (plants[12].gameObject.activeSelf && !plants[13].gameObject.activeSelf && plantarea[12] != false)
        {
            time[6] += 0.020f;

            if (time[6] >= 10.0f && time[6] <= 10.1f)
            {
                switch (PlantA[12])
                {
                    case "Carrot":
                        plants[12].GetComponent<Image>().sprite = CarrotGrow[1];
                        plantsGrowLevel[12] = 1;
                        break;
                    case "Corn":
                        plants[12].GetComponent<Image>().sprite = CornGrow[1];
                        plantsGrowLevel[12] = 1;
                        break;
                    case "Radish":
                        plants[12].GetComponent<Image>().sprite = RadishGrow[1];
                        plantsGrowLevel[12] = 1;
                        break;
                    case "Turnip":
                        plants[12].GetComponent<Image>().sprite = TurnipGrow[1];
                        plantsGrowLevel[12] = 1;
                        break;
                }
            }
            else
            {
                if (time[6] >= 20.0f && time[6] <= 20.1f)
                {
                    switch (PlantA[12])
                    {
                        case "Carrot":
                            plants[12].GetComponent<Image>().sprite = CarrotGrow[2];
                            plantsGrowLevel[12] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[8].SetActive(true);
                            break;
                        case "Corn":
                            plants[12].GetComponent<Image>().sprite = CornGrow[2];
                            plantsGrowLevel[12] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[8].SetActive(true);
                            break;
                        case "Radish":
                            plants[12].GetComponent<Image>().sprite = RadishGrow[2];
                            plantsGrowLevel[12] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[8].SetActive(true);
                            break;
                        case "Turnip":
                            plants[12].GetComponent<Image>().sprite = TurnipGrow[2];
                            plantsGrowLevel[12] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[8].SetActive(true);
                            break;
                    }
                    time[6] = 0f;
                    plantarea[12] = false;
                }
            }
        }


        //Area Level 20

        if (plants[14].gameObject.activeSelf && plants[15].gameObject.activeSelf && plantarea[14] != false)
        {
            time[7] += 0.020f;

            if (time[7] >= 10.0f && time[7] <= 10.1f)
            {
                switch (PlantA[14])
                {
                    case "Carrot":
                        plants[14].GetComponent<Image>().sprite = CarrotGrow[1];
                        plantsGrowLevel[14] = 1;
                        break;
                    case "Corn":
                        plants[14].GetComponent<Image>().sprite = CornGrow[1];
                        plantsGrowLevel[14] = 1;
                        break;
                    case "Radish":
                        plants[14].GetComponent<Image>().sprite = RadishGrow[1];
                        plantsGrowLevel[14] = 1;
                        break;
                    case "Turnip":
                        plants[14].GetComponent<Image>().sprite = TurnipGrow[1];
                        plantsGrowLevel[14] = 1;
                        break;
                }

                switch (PlantB[15])
                {
                    case "Carrot":
                        plants[15].GetComponent<Image>().sprite = CarrotGrow[1];
                        plantsGrowLevel[15] = 1;
                        break;
                    case "Corn":
                        plants[15].GetComponent<Image>().sprite = CornGrow[1];
                        plantsGrowLevel[15] = 1;
                        break;
                    case "Radish":
                        plants[15].GetComponent<Image>().sprite = RadishGrow[1];
                        plantsGrowLevel[15] = 1;
                        break;
                    case "Turnip":
                        plants[15].GetComponent<Image>().sprite = TurnipGrow[1];
                        plantsGrowLevel[15] = 1;
                        break;
                }
            }
            else
            {
                if (time[7] >= 20.0f && time[7] <= 20.1f)
                {
                    switch (PlantA[14])
                    {
                        case "Carrot":
                            plants[14].GetComponent<Image>().sprite = CarrotGrow[2];
                            plantsGrowLevel[14] = 2;
                            break;
                        case "Corn":
                            plants[14].GetComponent<Image>().sprite = CornGrow[2];
                            plantsGrowLevel[14] = 2;
                            break;
                        case "Radish":
                            plants[14].GetComponent<Image>().sprite = RadishGrow[2];
                            plantsGrowLevel[14] = 2;
                            break;
                        case "Turnip":
                            plants[14].GetComponent<Image>().sprite = TurnipGrow[2];
                            plantsGrowLevel[14] = 2;
                            break;
                    }

                    switch (PlantB[15])
                    {
                        case "Carrot":
                            plants[15].GetComponent<Image>().sprite = CarrotGrow[2];
                            plantsGrowLevel[15] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[10].SetActive(true);
                            break;
                        case "Corn":
                            plants[15].GetComponent<Image>().sprite = CornGrow[2];
                            plantsGrowLevel[15] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[10].SetActive(true);
                            break;
                        case "Radish":
                            plants[15].GetComponent<Image>().sprite = RadishGrow[2];
                            plantsGrowLevel[15] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[10].SetActive(true);
                            break;
                        case "Turnip":
                            plants[15].GetComponent<Image>().sprite = TurnipGrow[2];
                            plantsGrowLevel[15] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[10].SetActive(true);
                            break;
                    }
                    time[7] = 0f;
                    plantarea[14] = false;
                }
            }
        }

        else if (plants[14].gameObject.activeSelf && !plants[15].gameObject.activeSelf && plantarea[14] != false)
        {
            time[7] += 0.020f;

            if (time[7] >= 10.0f && time[7] <= 10.1f)
            {
                switch (PlantA[14])
                {
                    case "Carrot":
                        plants[14].GetComponent<Image>().sprite = CarrotGrow[1];
                        plantsGrowLevel[14] = 1;
                        break;
                    case "Corn":
                        plants[14].GetComponent<Image>().sprite = CornGrow[1];
                        plantsGrowLevel[14] = 1;
                        break;
                    case "Radish":
                        plants[14].GetComponent<Image>().sprite = RadishGrow[1];
                        plantsGrowLevel[14] = 1;
                        break;
                    case "Turnip":
                        plants[14].GetComponent<Image>().sprite = TurnipGrow[1];
                        plantsGrowLevel[14] = 1;
                        break;
                }
            }
            else
            {
                if (time[7] >= 20.0f && time[7] <= 20.1f)
                {
                    switch (PlantA[14])
                    {
                        case "Carrot":
                            plants[14].GetComponent<Image>().sprite = CarrotGrow[2];
                            plantsGrowLevel[14] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[10].SetActive(true);
                            break;
                        case "Corn":
                            plants[14].GetComponent<Image>().sprite = CornGrow[2];
                            plantsGrowLevel[14] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[10].SetActive(true);
                            break;
                        case "Radish":
                            plants[14].GetComponent<Image>().sprite = RadishGrow[2];
                            plantsGrowLevel[14] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[10].SetActive(true);
                            break;
                        case "Turnip":
                            plants[14].GetComponent<Image>().sprite = TurnipGrow[2];
                            plantsGrowLevel[14] = 2;
                            audioManager.PlaySFX(audioManager.Pop);
                            Cloud[10].SetActive(true);
                            break;
                    }
                    time[7] = 0f;
                    plantarea[14] = false;
                }
            }
        }
    }

    public void Harvest(int val)
    {
        //Area Level 0

        int starpercentage = 0, starQt = 0, expQt = 0;

        if (val == 0)
        {
            if (plantsGrowLevel[0] == 2)
            {
                starpercentage = UnityEngine.Random.Range(1, 101);
                starQt = UnityEngine.Random.Range(1, ValueOfPlayerLevel + 1);

                if (plants[0].gameObject.activeSelf && plants[1].gameObject.activeSelf)
                {
                    switch (PlantA[0])
                    {
                        case "Carrot":
                            plantFarmMoney[0] += (ulong)plantQt[0] * (ulong)UnityEngine.Random.Range(seedsprice[0], 81);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[0], 80 * plantQt[0] + 1);
                            plants[0].gameObject.SetActive(false);
                            break;
                        case "Corn":
                            plantFarmMoney[0] += (ulong)plantQt[0] * (ulong)UnityEngine.Random.Range(seedsprice[1], 161);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[1], 160 * plantQt[0] + 1);
                            plants[0].gameObject.SetActive(false);
                            break;
                        case "Radish":
                            plantFarmMoney[0] += (ulong)plantQt[0] * (ulong)UnityEngine.Random.Range(seedsprice[2], 221);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[2], 220 * plantQt[0] + 1);
                            plants[0].gameObject.SetActive(false);
                            break;
                        case "Turnip":
                            plantFarmMoney[0] += (ulong)plantQt[0] * (ulong)UnityEngine.Random.Range(seedsprice[3], 351);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[3], 350 * plantQt[0] + 1);
                            plants[0].gameObject.SetActive(false);
                            break;
                    }

                    switch (PlantB[1])
                    {
                        case "Carrot":
                            plantFarmMoney[0] += (ulong)plantQt[0] * (ulong)UnityEngine.Random.Range(seedsprice[0], 81);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[0], 80 * plantQt[1] + 1);
                            plants[1].gameObject.SetActive(false);
                            break;
                        case "Corn":
                            plantFarmMoney[0] += (ulong)plantQt[1] * (ulong)UnityEngine.Random.Range(seedsprice[1], 161);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[1], 160 * plantQt[1] + 1);
                            plants[1].gameObject.SetActive(false);
                            break;
                        case "Radish":
                            plantFarmMoney[0] += (ulong)plantQt[1] * (ulong)UnityEngine.Random.Range(seedsprice[2], 221);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[2], 220 * plantQt[1] + 1);
                            plants[1].gameObject.SetActive(false);
                            break;
                        case "Turnip":
                            plantFarmMoney[0] += (ulong)plantQt[1] * (ulong)UnityEngine.Random.Range(seedsprice[3], 351);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[3], 350 * plantQt[1] + 1);
                            plants[1].gameObject.SetActive(false);
                            break;
                    }

                    audioManager.PlaySFX(audioManager.PlantHarvest);
                    audioManager.PlaySFX(audioManager.Money);
                    Block[0].SetActive(false);
                    MoneyUp[0].SetActive(true);
                    MoneyUpText[0].text = $"<color=green>{CurrencyTextFormat((ulong)plantFarmMoney[0])}</color>" + " BTH";
                    ValueOfPlayerMoney += plantFarmMoney[0];
                    Cloud[0].SetActive(false);

                    if (starpercentage <= 50)
                    {
                        switch (plantQt[0])
                        {
                            case 5:
                                plantFarmStar[0] += ((ulong)starQt + 2);
                                break;
                            case 10:
                                plantFarmStar[0] += ((ulong)starQt + 3);
                                break;
                            case 15:
                                plantFarmStar[0] += ((ulong)starQt + 4);
                                break;
                            case 20:
                                plantFarmStar[0] += ((ulong)starQt + 5);
                                break;
                            default:
                                plantFarmStar[0] += (ulong)starQt;
                                break;
                        }

                        switch (plantQt[1])
                        {
                            case 5:
                                plantFarmStar[0] += ((ulong)starQt + 2);
                                break;
                            case 10:
                                plantFarmStar[0] += ((ulong)starQt + 3);
                                break;
                            case 15:
                                plantFarmStar[0] += ((ulong)starQt + 4);
                                break;
                            case 20:
                                plantFarmStar[0] += ((ulong)starQt + 5);
                                break;
                            default:
                                plantFarmStar[0] += (ulong)starQt;
                                break;
                        }

                        audioManager.PlaySFX(audioManager.Star);
                        StarUp[0].SetActive(true);
                        StarUpText[0].gameObject.SetActive(true);
                        StarUpText[0].text = $"<color=yellow>{CurrencyTextFormat(plantFarmStar[0])}</color>";
                        ValueOfPlayerStar += (long)plantFarmStar[0];
                        plantFarmStar[0] = 0;
                    }
                }
                else
                {
                    if (plants[0].gameObject.activeSelf && !plants[1].gameObject.activeSelf)
                    {
                        switch (PlantA[0])
                        {
                            case "Carrot":
                                plantFarmMoney[0] += (ulong)plantQt[0] * (ulong)UnityEngine.Random.Range(seedsprice[0], 81);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[0], 80 * plantQt[0] + 1);
                                Block[0].SetActive(false);
                                plants[0].gameObject.SetActive(false);
                                break;
                            case "Corn":
                                plantFarmMoney[0] += (ulong)plantQt[0] * (ulong)UnityEngine.Random.Range(seedsprice[1], 161);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[1], 160 * plantQt[0] + 1);
                                Block[0].SetActive(false);
                                plants[0].gameObject.SetActive(false);
                                break;
                            case "Radish":
                                plantFarmMoney[0] += (ulong)plantQt[0] * (ulong)UnityEngine.Random.Range(seedsprice[2], 221);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[2], 220 * plantQt[0] + 1);
                                Block[0].SetActive(false);
                                plants[0].gameObject.SetActive(false);
                                break;
                            case "Turnip":
                                plantFarmMoney[0] += (ulong)plantQt[0] * (ulong)UnityEngine.Random.Range(seedsprice[3], 351);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[3], 350 * plantQt[0] + 1);
                                Block[0].SetActive(false);
                                plants[0].gameObject.SetActive(false);
                                break;
                        }

                        audioManager.PlaySFX(audioManager.PlantHarvest);
                        audioManager.PlaySFX(audioManager.Money);
                        MoneyUp[0].SetActive(true);
                        MoneyUpText[0].text = $"<color=green>{CurrencyTextFormat((ulong)plantFarmMoney[0])}</color>" + " BTH";
                        ValueOfPlayerMoney += plantFarmMoney[0];
                        Block[0].SetActive(false);
                        Cloud[0].SetActive(false);

                        if (starpercentage <= 30)
                        {
                            switch (plantQt[0])
                            {
                                case 5:
                                    ValueOfPlayerStar += (starQt + 2);
                                    break;
                                case 10:
                                    ValueOfPlayerStar += (starQt + 3);
                                    break;
                                case 15:
                                    ValueOfPlayerStar += (starQt + 4);
                                    break;
                                case 20:
                                    ValueOfPlayerStar += (starQt + 5);
                                    break;
                                default:
                                    ValueOfPlayerStar += starQt;
                                    break;
                            }

                            audioManager.PlaySFX(audioManager.Star);
                            StarUp[0].SetActive(true);
                            StarUpText[0].gameObject.SetActive(true);
                            StarUpText[0].text = $"<color=yellow>{CurrencyTextFormat((ulong)plantFarmStar[0])}</color>";
                            ValueOfPlayerStar += (long)plantFarmStar[0];
                            plantFarmStar[0] = 0;
                        }
                    }
                }
            }
            else
            {
                audioManager.PlaySFX(audioManager.Denied);
                alert[14].SetActive(true);
            }
        }

        //Area Level 1

        else if (val == 1)
        {
            if (plantsGrowLevel[2] == 2)
            {
                starpercentage = UnityEngine.Random.Range(1, 101);
                starQt = UnityEngine.Random.Range(1, ValueOfPlayerLevel + 1);
                if (plants[2].gameObject.activeSelf && plants[3].gameObject.activeSelf)
                {
                    switch (PlantA[2])
                    {
                        case "Carrot":
                            plantFarmMoney[1] += (ulong)plantQt[2] * (ulong)UnityEngine.Random.Range(seedsprice[0], 81);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[0], 80 * plantQt[2] + 1);
                            plants[2].gameObject.SetActive(false);
                            break;
                        case "Corn":
                            plantFarmMoney[1] += (ulong)plantQt[2] * (ulong)UnityEngine.Random.Range(seedsprice[1], 161);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[1], 160 * plantQt[2] + 1);
                            plants[2].gameObject.SetActive(false);
                            break;
                        case "Radish":
                            plantFarmMoney[1] += (ulong)plantQt[2] * (ulong)UnityEngine.Random.Range(seedsprice[2], 221);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[2], 220 * plantQt[2] + 1);
                            plants[2].gameObject.SetActive(false);
                            break;
                        case "Turnip":
                            plantFarmMoney[1] += (ulong)plantQt[2] * (ulong)UnityEngine.Random.Range(seedsprice[3], 351);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[3], 350 * plantQt[2] + 1);
                            plants[2].gameObject.SetActive(false);
                            break;
                    }

                    switch (PlantB[3])
                    {
                        case "Carrot":
                            plantFarmMoney[1] += (ulong)plantQt[3] * (ulong)UnityEngine.Random.Range(seedsprice[0], 81);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[0], 80 * plantQt[3] + 1);
                            plants[3].gameObject.SetActive(false);
                            break;
                        case "Corn":
                            plantFarmMoney[1] += (ulong)plantQt[3] * (ulong)UnityEngine.Random.Range(seedsprice[1], 161);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[1], 160 * plantQt[3] + 1);
                            plants[3].gameObject.SetActive(false);
                            break;
                        case "Radish":
                            plantFarmMoney[1] += (ulong)plantQt[3] * (ulong)UnityEngine.Random.Range(seedsprice[2], 221);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[2], 220 * plantQt[3] + 1);
                            plants[3].gameObject.SetActive(false);
                            break;
                        case "Turnip":
                            plantFarmMoney[1] += (ulong)plantQt[3] * (ulong)UnityEngine.Random.Range(seedsprice[3], 351);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[3], 350 * plantQt[3] + 1);
                            plants[3].gameObject.SetActive(false);
                            break;
                    }

                    audioManager.PlaySFX(audioManager.PlantHarvest);
                    audioManager.PlaySFX(audioManager.Money);
                    MoneyUp[1].SetActive(true);
                    MoneyUpText[1].text = $"<color=green>{CurrencyTextFormat((ulong)plantFarmMoney[1])}</color>" + " BTH";
                    ValueOfPlayerMoney += plantFarmMoney[1];
                    Block[1].SetActive(false);
                    Cloud[1].SetActive(false);

                    if (starpercentage <= 50)
                    {
                        switch (plantQt[2])
                        {
                            case 5:
                                plantFarmStar[1] += ((ulong)starQt + 2);
                                break;
                            case 10:
                                plantFarmStar[1] += ((ulong)starQt + 3);
                                break;
                            case 15:
                                plantFarmStar[1] += ((ulong)starQt + 4);
                                break;
                            case 20:
                                plantFarmStar[1] += ((ulong)starQt + 5);
                                break;
                            default:
                                plantFarmStar[1] += (ulong)starQt;
                                break;
                        }

                        switch (plantQt[3])
                        {
                            case 5:
                                plantFarmStar[1] += ((ulong)starQt + 2);
                                break;
                            case 10:
                                plantFarmStar[1] += ((ulong)starQt + 3);
                                break;
                            case 15:
                                plantFarmStar[1] += ((ulong)starQt + 4);
                                break;
                            case 20:
                                plantFarmStar[1] += ((ulong)starQt + 5);
                                break;
                            default:
                                plantFarmStar[1] += (ulong)starQt;
                                break;
                        }

                        audioManager.PlaySFX(audioManager.Star);
                        StarUp[1].SetActive(true);
                        StarUpText[1].gameObject.SetActive(true);
                        StarUpText[1].text = $"<color=yellow>{CurrencyTextFormat((ulong)plantFarmStar[1])}</color>";
                        ValueOfPlayerStar += (long)plantFarmStar[1];
                        plantFarmStar[1] = 0;
                    }
                }
                else
                {
                    if (plants[2].gameObject.activeSelf && !plants[3].gameObject.activeSelf)
                    {
                        switch (PlantA[2])
                        {
                            case "Carrot":
                                plantFarmMoney[1] += (ulong)plantQt[2] * (ulong)UnityEngine.Random.Range(seedsprice[0], 81);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[0], 80 * plantQt[2] + 1);
                                plants[2].gameObject.SetActive(false);
                                break;
                            case "Corn":
                                plantFarmMoney[1] += (ulong)plantQt[2] * (ulong)UnityEngine.Random.Range(seedsprice[1], 161);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[1], 160 * plantQt[2] + 1);
                                plants[2].gameObject.SetActive(false);
                                break;
                            case "Radish":
                                plantFarmMoney[1] += (ulong)plantQt[2] * (ulong)UnityEngine.Random.Range(seedsprice[2], 221);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[2], 220 * plantQt[2] + 1);
                                plants[2].gameObject.SetActive(false);
                                break;
                            case "Turnip":
                                plantFarmMoney[1] += (ulong)plantQt[2] * (ulong)UnityEngine.Random.Range(seedsprice[3], 351);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[3], 350 * plantQt[2] + 1);
                                plants[2].gameObject.SetActive(false);
                                break;
                        }

                        audioManager.PlaySFX(audioManager.PlantHarvest);
                        audioManager.PlaySFX(audioManager.Money);
                        MoneyUp[1].SetActive(true);
                        MoneyUpText[1].text = $"<color=green>{CurrencyTextFormat((ulong)plantFarmMoney[1])}</color>" + " BTH";
                        ValueOfPlayerMoney += plantFarmMoney[1];
                        Block[1].SetActive(false);
                        Cloud[1].SetActive(false);

                        if (starpercentage <= 30)
                        {
                            switch (plantQt[2])
                            {
                                case 5:
                                    plantFarmStar[1] += ((ulong)starQt + 2);
                                    break;
                                case 10:
                                    plantFarmStar[1] += ((ulong)starQt + 3);
                                    break;
                                case 15:
                                    plantFarmStar[1] += ((ulong)starQt + 4);
                                    break;
                                case 20:
                                    plantFarmStar[1] += ((ulong)starQt + 5);
                                    break;
                                default:
                                    plantFarmStar[1] += (ulong)starQt;
                                    break;
                            }

                            audioManager.PlaySFX(audioManager.Star);
                            StarUp[1].SetActive(true);
                            StarUpText[1].gameObject.SetActive(true);
                            StarUpText[1].text = $"<color=yellow>{CurrencyTextFormat((ulong)plantFarmStar[1])}</color>";
                            ValueOfPlayerStar += (long)plantFarmStar[1];
                            plantFarmStar[1] = 0;
                        }
                    }
                }
            }
            else
            {
                audioManager.PlaySFX(audioManager.Denied);
                alert[14].SetActive(true);
            }
        }

        //Area Level 2

        else if (val == 2)
        {
            if (plantsGrowLevel[4] == 2)
            {
                starpercentage = UnityEngine.Random.Range(1, 101);
                starQt = UnityEngine.Random.Range(1, ValueOfPlayerLevel + 1);
                if (plants[4].gameObject.activeSelf && plants[5].gameObject.activeSelf)
                {
                    switch (PlantA[4])
                    {
                        case "Carrot":
                            plantFarmMoney[2] += (ulong)plantQt[4] * (ulong)UnityEngine.Random.Range(seedsprice[0], 81);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[0], 80 * plantQt[4] + 1);
                            plants[4].gameObject.SetActive(false);
                            
                            break;
                        case "Corn":
                            plantFarmMoney[2] += (ulong)plantQt[4] * (ulong)UnityEngine.Random.Range(seedsprice[1], 161);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[1], 160 * plantQt[4] + 1);
                            plants[4].gameObject.SetActive(false);
                            Block[2].SetActive(false);
                            break;
                        case "Radish":
                            plantFarmMoney[2] += (ulong)plantQt[4] * (ulong)UnityEngine.Random.Range(seedsprice[2], 221);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[2], 220 * plantQt[4] + 1);
                            plants[4].gameObject.SetActive(false);
                            Block[2].SetActive(false);
                            break;
                        case "Turnip":
                            plantFarmMoney[2] += (ulong)plantQt[4] * (ulong)UnityEngine.Random.Range(seedsprice[3], 351);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[3], 350 * plantQt[4] + 1);
                            plants[4].gameObject.SetActive(false);
                            Block[2].SetActive(false);
                            break;
                    }

                    switch (PlantB[5])
                    {
                        case "Carrot":
                            plantFarmMoney[2] += (ulong)plantQt[5] * (ulong)UnityEngine.Random.Range(seedsprice[0], 81);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[0], 80 * plantQt[5] + 1);
                            plants[5].gameObject.SetActive(false);
                            break;
                        case "Corn":
                            plantFarmMoney[2] += (ulong)plantQt[5] * (ulong)UnityEngine.Random.Range(seedsprice[1], 161);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[1], 160 * plantQt[5] + 1);
                            plants[5].gameObject.SetActive(false);
                            break;
                        case "Radish":
                            plantFarmMoney[2] += (ulong)plantQt[5] * (ulong)UnityEngine.Random.Range(seedsprice[2], 221);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[2], 220 * plantQt[5] + 1);
                            plants[5].gameObject.SetActive(false);
                            break;
                        case "Turnip":
                            plantFarmMoney[2] += (ulong)plantQt[5] * (ulong)UnityEngine.Random.Range(seedsprice[3], 351);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[3], 350 * plantQt[5] + 1);
                            plants[5].gameObject.SetActive(false);
                            break;
                    }

                    audioManager.PlaySFX(audioManager.PlantHarvest);
                    audioManager.PlaySFX(audioManager.Money);
                    MoneyUp[2].SetActive(true);
                    MoneyUpText[2].text = $"<color=green>{CurrencyTextFormat((ulong)plantFarmMoney[2])}</color>" + " BTH";
                    ValueOfPlayerMoney += plantFarmMoney[2];
                    Block[2].SetActive(false);
                    Cloud[2].SetActive(false);

                    if (starpercentage <= 50)
                    {
                        switch (plantQt[4])
                        {
                            case 5:
                                plantFarmStar[2] += ((ulong)starQt + 2);
                                break;
                            case 10:
                                plantFarmStar[2] += ((ulong)starQt + 3);
                                break;
                            case 15:
                                plantFarmStar[2] += ((ulong)starQt + 4);
                                break;
                            case 20:
                                plantFarmStar[2] += ((ulong)starQt + 5);
                                break;
                            default:
                                plantFarmStar[2] += (ulong)starQt;
                                break;
                        }

                        switch (plantQt[5])
                        {
                            case 5:
                                plantFarmStar[2] += ((ulong)starQt + 2);
                                break;
                            case 10:
                                plantFarmStar[2] += ((ulong)starQt + 3);
                                break;
                            case 15:
                                plantFarmStar[2] += ((ulong)starQt + 4);
                                break;
                            case 20:
                                plantFarmStar[2] += ((ulong)starQt + 5);
                                break;
                            default:
                                plantFarmStar[2] += (ulong)starQt;
                                break;
                        }

                        audioManager.PlaySFX(audioManager.Star);
                        StarUp[2].SetActive(true);
                        StarUpText[2].gameObject.SetActive(true);
                        StarUpText[2].text = $"<color=yellow>{CurrencyTextFormat((ulong)plantFarmStar[2])}</color>";
                        ValueOfPlayerStar += (long)plantFarmStar[2];
                        plantFarmStar[2] = 0;
                    }
                }
                else
                {
                    if (plants[4].gameObject.activeSelf && !plants[5].gameObject.activeSelf)
                    {
                        switch (PlantA[4])
                        {
                            case "Carrot":
                                plantFarmMoney[2] += (ulong)plantQt[4] * (ulong)UnityEngine.Random.Range(seedsprice[0], 81);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[0], 80 * plantQt[4] + 1);
                                plants[4].gameObject.SetActive(false);
                                break;
                            case "Corn":
                                plantFarmMoney[2] += (ulong)plantQt[4] * (ulong)UnityEngine.Random.Range(seedsprice[1], 161);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[1], 160 * plantQt[4] + 1);
                                plants[4].gameObject.SetActive(false);
                                break;
                            case "Radish":
                                plantFarmMoney[2] += (ulong)plantQt[4] * (ulong)UnityEngine.Random.Range(seedsprice[2], 221);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[2], 220 * plantQt[4] + 1);
                                plants[4].gameObject.SetActive(false);
                                break;
                            case "Turnip":
                                plantFarmMoney[2] += (ulong)plantQt[4] * (ulong)UnityEngine.Random.Range(seedsprice[3], 351);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[3], 350 * plantQt[4] + 1);
                                plants[4].gameObject.SetActive(false);
                                break;
                        }

                        audioManager.PlaySFX(audioManager.PlantHarvest);
                        audioManager.PlaySFX(audioManager.Money);
                        MoneyUp[2].SetActive(true);
                        MoneyUpText[2].text = $"<color=green>{CurrencyTextFormat((ulong)plantFarmMoney[2])}</color>" + " BTH";
                        ValueOfPlayerMoney += plantFarmMoney[2];
                        Block[2].SetActive(false);
                        Cloud[2].SetActive(false);

                        if (starpercentage <= 30)
                        {
                            switch (plantQt[4])
                            {
                                case 5:
                                    plantFarmStar[2] += ((ulong)starQt + 2);
                                    break;
                                case 10:
                                    plantFarmStar[2] += ((ulong)starQt + 3);
                                    break;
                                case 15:
                                    plantFarmStar[2] += ((ulong)starQt + 4);
                                    break;
                                case 20:
                                    plantFarmStar[2] += ((ulong)starQt + 5);
                                    break;
                                default:
                                    plantFarmStar[2] += (ulong)starQt;
                                    break;
                            }

                            audioManager.PlaySFX(audioManager.Star);
                            StarUp[2].SetActive(true);
                            StarUpText[2].gameObject.SetActive(true);
                            StarUpText[2].text = $"<color=yellow>{CurrencyTextFormat((ulong)plantFarmStar[2])}</color>";
                            ValueOfPlayerStar += (long)plantFarmStar[2];
                            plantFarmStar[2] = 0;
                        }
                    }
                }
            }
            else
            {
                audioManager.PlaySFX(audioManager.Denied);
                alert[14].SetActive(true);
            }
        }

        //Area Level 4

        else if (val == 3)
        {
            if (plantsGrowLevel[6] == 2)
            {
                starpercentage = UnityEngine.Random.Range(1, 101);
                starQt = UnityEngine.Random.Range(1, ValueOfPlayerLevel + 1);
                if (plants[6].gameObject.activeSelf && plants[7].gameObject.activeSelf)
                {
                    switch (PlantA[6])
                    {
                        case "Carrot":
                            plantFarmMoney[3] += (ulong)plantQt[6] * (ulong)UnityEngine.Random.Range(seedsprice[0], 81);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[0], 80 * plantQt[6] + 1);
                            plants[6].gameObject.SetActive(false);
                            break;
                        case "Corn":
                            plantFarmMoney[3] += (ulong)plantQt[6] * (ulong)UnityEngine.Random.Range(seedsprice[1], 161);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[1], 160 * plantQt[6] + 1);
                            plants[6].gameObject.SetActive(false);
                            break;
                        case "Radish":
                            plantFarmMoney[3] += (ulong)plantQt[6] * (ulong)UnityEngine.Random.Range(seedsprice[2], 221);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[2], 220 * plantQt[6] + 1);
                            plants[6].gameObject.SetActive(false);
                            break;
                        case "Turnip":
                            plantFarmMoney[3] += (ulong)plantQt[6] * (ulong)UnityEngine.Random.Range(seedsprice[3], 351);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[3], 350 * plantQt[6] + 1);
                            plants[6].gameObject.SetActive(false);
                            break;
                    }

                    switch (PlantB[7])
                    {
                        case "Carrot":
                            plantFarmMoney[3] += (ulong)plantQt[7] * (ulong)UnityEngine.Random.Range(seedsprice[0], 81);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[0], 80 * plantQt[7] + 1);
                            plants[7].gameObject.SetActive(false);
                            break;
                        case "Corn":
                            plantFarmMoney[3] += (ulong)plantQt[7] * (ulong)UnityEngine.Random.Range(seedsprice[1], 161);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[1], 160 * plantQt[7] + 1);
                            plants[7].gameObject.SetActive(false);
                            break;
                        case "Radish":
                            plantFarmMoney[3] += (ulong)plantQt[7] * (ulong)UnityEngine.Random.Range(seedsprice[2], 221);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[2], 220 * plantQt[7] + 1);
                            plants[7].gameObject.SetActive(false);
                            break;
                        case "Turnip":
                            plantFarmMoney[3] += (ulong)plantQt[7] * (ulong)UnityEngine.Random.Range(seedsprice[3], 351);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[3], 350 * plantQt[7] + 1);
                            plants[7].gameObject.SetActive(false);
                            break;
                    }

                    audioManager.PlaySFX(audioManager.PlantHarvest);
                    audioManager.PlaySFX(audioManager.Money);
                    MoneyUp[3].SetActive(true);
                    MoneyUpText[3].text = $"<color=green>{CurrencyTextFormat((ulong)plantFarmMoney[3])}</color>" + " BTH";
                    ValueOfPlayerMoney += plantFarmMoney[3];
                    Block[3].SetActive(false);
                    Cloud[3].SetActive(false);

                    if (starpercentage <= 50)
                    {
                        switch (plantQt[6])
                        {
                            case 5:
                                plantFarmStar[3] += ((ulong)starQt + 2);
                                break;
                            case 10:
                                plantFarmStar[3] += ((ulong)starQt + 3);
                                break;
                            case 15:
                                plantFarmStar[3] += ((ulong)starQt + 4);
                                break;
                            case 20:
                                plantFarmStar[3] += ((ulong)starQt + 5);
                                break;
                            default:
                                plantFarmStar[3] += (ulong)starQt;
                                break;
                        }

                        switch (plantQt[7])
                        {
                            case 5:
                                plantFarmStar[3] += ((ulong)starQt + 2);
                                break;
                            case 10:
                                plantFarmStar[3] += ((ulong)starQt + 3);
                                break;
                            case 15:
                                plantFarmStar[3] += ((ulong)starQt + 4);
                                break;
                            case 20:
                                plantFarmStar[3] += ((ulong)starQt + 5);
                                break;
                            default:
                                plantFarmStar[3] += (ulong)starQt;
                                break;
                        }

                        audioManager.PlaySFX(audioManager.Star);
                        StarUp[3].SetActive(true);
                        StarUpText[3].gameObject.SetActive(true);
                        StarUpText[3].text = $"<color=yellow>{CurrencyTextFormat((ulong)plantFarmStar[3])}</color>";
                        ValueOfPlayerStar += (long)plantFarmStar[3];
                        plantFarmStar[3] = 0;
                    }
                }
                else
                {
                    if (plants[6].gameObject.activeSelf && !plants[7].gameObject.activeSelf)
                    {
                        switch (PlantA[6])
                        {
                            case "Carrot":
                                plantFarmMoney[3] += (ulong)plantQt[6] * (ulong)UnityEngine.Random.Range(seedsprice[0], 81);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[0], 80 * plantQt[6] + 1);
                                plants[6].gameObject.SetActive(false);
                                break;
                            case "Corn":
                                plantFarmMoney[3] += (ulong)plantQt[6] * (ulong)UnityEngine.Random.Range(seedsprice[1], 161);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[1], 160 * plantQt[6] + 1);
                                plants[6].gameObject.SetActive(false);
                                break;
                            case "Radish":
                                plantFarmMoney[3] += (ulong)plantQt[6] * (ulong)UnityEngine.Random.Range(seedsprice[2], 221);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[2], 220 * plantQt[6] + 1);
                                plants[6].gameObject.SetActive(false);
                                break;
                            case "Turnip":
                                plantFarmMoney[3] += (ulong)plantQt[6] * (ulong)UnityEngine.Random.Range(seedsprice[3], 351);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[3], 350 * plantQt[6] + 1);
                                plants[6].gameObject.SetActive(false);
                                break;
                        }

                        audioManager.PlaySFX(audioManager.PlantHarvest);
                        audioManager.PlaySFX(audioManager.Money);
                        MoneyUp[3].SetActive(true);
                        MoneyUpText[3].text = $"<color=green>{CurrencyTextFormat((ulong)plantFarmMoney[3])}</color>" + " BTH";
                        ValueOfPlayerMoney += plantFarmMoney[3];
                        Block[3].SetActive(false);
                        Cloud[3].SetActive(false);

                        if (starpercentage <= 30)
                        {
                            switch (plantQt[6])
                            {
                                case 5:
                                    plantFarmStar[3] += ((ulong)starQt + 2);
                                    break;
                                case 10:
                                    plantFarmStar[3] += ((ulong)starQt + 3);
                                    break;
                                case 15:
                                    plantFarmStar[3] += ((ulong)starQt + 4);
                                    break;
                                case 20:
                                    plantFarmStar[3] += ((ulong)starQt + 5);
                                    break;
                                default:
                                    plantFarmStar[3] += (ulong)starQt;
                                    break;
                            }

                            audioManager.PlaySFX(audioManager.Star);
                            StarUp[3].SetActive(true);
                            StarUpText[3].gameObject.SetActive(true);
                            StarUpText[3].text = $"<color=yellow>{CurrencyTextFormat((ulong)plantFarmStar[3])}</color>";
                            ValueOfPlayerStar += (long)plantFarmStar[3];
                            plantFarmStar[3] = 0;
                        }
                    }
                }
            }
            else
            {
                audioManager.PlaySFX(audioManager.Denied);
                alert[14].SetActive(true);
            }
        }

        //Area Level 6

        else if (val == 4)
        {
            if (plantsGrowLevel[8] == 2)
            {
                starpercentage = UnityEngine.Random.Range(1, 101);
                starQt = UnityEngine.Random.Range(1, ValueOfPlayerLevel + 1);
                if (plants[8].gameObject.activeSelf && plants[9].gameObject.activeSelf)
                {
                    switch (PlantA[8])
                    {
                        case "Carrot":
                            plantFarmMoney[4] += (ulong)plantQt[8] * (ulong)UnityEngine.Random.Range(seedsprice[0], 81);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[0], 80 * plantQt[8] + 1);
                            plants[8].gameObject.SetActive(false);
                            break;
                        case "Corn":
                            plantFarmMoney[4] += (ulong)plantQt[8] * (ulong)UnityEngine.Random.Range(seedsprice[1], 161);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[1], 160 * plantQt[8] + 1);
                            plants[8].gameObject.SetActive(false);
                            break;
                        case "Radish":
                            plantFarmMoney[4] += (ulong)plantQt[8] * (ulong)UnityEngine.Random.Range(seedsprice[2], 221);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[2], 220 * plantQt[8] + 1);
                            plants[8].gameObject.SetActive(false);
                            break;
                        case "Turnip":
                            plantFarmMoney[4] += (ulong)plantQt[8] * (ulong)UnityEngine.Random.Range(seedsprice[3], 351);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[3], 350 * plantQt[8] + 1);
                            plants[8].gameObject.SetActive(false);
                            break;
                    }

                    switch (PlantB[9])
                    {
                        case "Carrot":
                            plantFarmMoney[4] += (ulong)plantQt[9] * (ulong)UnityEngine.Random.Range(seedsprice[0], 81);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[0], 80 * plantQt[9] + 1);
                            plants[9].gameObject.SetActive(false);
                            break;
                        case "Corn":
                            plantFarmMoney[4] += (ulong)plantQt[9] * (ulong)UnityEngine.Random.Range(seedsprice[1], 161);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[1], 160 * plantQt[9] + 1);
                            plants[9].gameObject.SetActive(false);
                            break;
                        case "Radish":
                            plantFarmMoney[4] += (ulong)plantQt[9] * (ulong)UnityEngine.Random.Range(seedsprice[2], 221);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[2], 220 * plantQt[9] + 1);
                            plants[9].gameObject.SetActive(false);
                            break;
                        case "Turnip":
                            plantFarmMoney[4] += (ulong)plantQt[9] * (ulong)UnityEngine.Random.Range(seedsprice[3], 351);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[3], 350 * plantQt[9] + 1);
                            plants[9].gameObject.SetActive(false);
                            break;
                    }

                    audioManager.PlaySFX(audioManager.PlantHarvest);
                    audioManager.PlaySFX(audioManager.Money);
                    MoneyUp[4].SetActive(true);
                    MoneyUpText[4].text = $"<color=green>{CurrencyTextFormat((ulong)plantFarmMoney[4])}</color>" + " BTH";
                    ValueOfPlayerMoney += plantFarmMoney[4];
                    Block[4].SetActive(false);
                    Cloud[4].SetActive(false);

                    if (starpercentage <= 50)
                    {
                        switch (plantQt[8])
                        {
                            case 5:
                                plantFarmStar[4] += ((ulong)starQt + 2);
                                break;
                            case 10:
                                plantFarmStar[4] += ((ulong)starQt + 3);
                                break;
                            case 15:
                                plantFarmStar[4] += ((ulong)starQt + 4);
                                break;
                            case 20:
                                plantFarmStar[4] += ((ulong)starQt + 5);
                                break;
                            default:
                                plantFarmStar[4] += (ulong)starQt;
                                break;
                        }

                        switch (plantQt[9])
                        {
                            case 5:
                                plantFarmStar[4] += ((ulong)starQt + 2);
                                break;
                            case 10:
                                plantFarmStar[4] += ((ulong)starQt + 3);
                                break;
                            case 15:
                                plantFarmStar[4] += ((ulong)starQt + 4);
                                break;
                            case 20:
                                plantFarmStar[4] += ((ulong)starQt + 5);
                                break;
                            default:
                                plantFarmStar[4] += (ulong)starQt;
                                break;
                        }

                        audioManager.PlaySFX(audioManager.Star);
                        StarUp[4].SetActive(true);
                        StarUpText[4].gameObject.SetActive(true);
                        StarUpText[4].text = $"<color=yellow>{CurrencyTextFormat((ulong)plantFarmStar[4])}</color>";
                        ValueOfPlayerStar += (long)plantFarmStar[4];
                        plantFarmStar[4] = 0;
                    }
                }
                else
                {
                    if (plants[8].gameObject.activeSelf && !plants[9].gameObject.activeSelf)
                    {
                        switch (PlantA[8])
                        {
                            case "Carrot":
                                plantFarmMoney[4] += (ulong)plantQt[8] * (ulong)UnityEngine.Random.Range(seedsprice[0], 81);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[0], 80 * plantQt[8] + 1);
                                plants[8].gameObject.SetActive(false);
                                break;
                            case "Corn":
                                plantFarmMoney[4] += (ulong)plantQt[8] * (ulong)UnityEngine.Random.Range(seedsprice[1], 161);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[1], 160 * plantQt[8] + 1);
                                plants[8].gameObject.SetActive(false);
                                break;
                            case "Radish":
                                plantFarmMoney[4] += (ulong)plantQt[8] * (ulong)UnityEngine.Random.Range(seedsprice[2], 221);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[2], 220 * plantQt[8] + 1);
                                plants[8].gameObject.SetActive(false);
                                break;
                            case "Turnip":
                                plantFarmMoney[4] += (ulong)plantQt[8] * (ulong)UnityEngine.Random.Range(seedsprice[3], 351);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[3], 350 * plantQt[8] + 1);
                                plants[8].gameObject.SetActive(false);
                                break;
                        }

                        audioManager.PlaySFX(audioManager.PlantHarvest);
                        audioManager.PlaySFX(audioManager.Money);
                        MoneyUp[4].SetActive(true);
                        MoneyUpText[4].text = $"<color=green>{CurrencyTextFormat((ulong)plantFarmMoney[4])}</color>" + " BTH";
                        ValueOfPlayerMoney += plantFarmMoney[4];
                        Block[4].SetActive(false);
                        Cloud[4].SetActive(false);

                        if (starpercentage <= 30)
                        {
                            switch (plantQt[8])
                            {
                                case 5:
                                    plantFarmStar[4] += ((ulong)starQt + 2);
                                    break;
                                case 10:
                                    plantFarmStar[4] += ((ulong)starQt + 3);
                                    break;
                                case 15:
                                    plantFarmStar[4] += ((ulong)starQt + 4);
                                    break;
                                case 20:
                                    plantFarmStar[4] += ((ulong)starQt + 5);
                                    break;
                                default:
                                    plantFarmStar[4] += (ulong)starQt;
                                    break;
                            }

                            audioManager.PlaySFX(audioManager.Star);
                            StarUp[4].SetActive(true);
                            StarUpText[4].gameObject.SetActive(true);
                            StarUpText[4].text = $"<color=yellow>{CurrencyTextFormat((ulong)plantFarmStar[4])}</color>";
                            ValueOfPlayerStar += (long)plantFarmStar[4];
                            plantFarmStar[4] = 0;
                        }
                    }
                }
            }
            else
            {
                audioManager.PlaySFX(audioManager.Denied);
                alert[14].SetActive(true);
            }
        }

        //Area Level 9

        else if (val == 5)
        {
            if (plantsGrowLevel[10] == 2)
            {
                starpercentage = UnityEngine.Random.Range(1, 101);
                starQt = UnityEngine.Random.Range(1, ValueOfPlayerLevel + 1);
                if (plants[10].gameObject.activeSelf && plants[11].gameObject.activeSelf)
                {
                    switch (PlantA[10])
                    {
                        case "Carrot":
                            plantFarmMoney[5] += (ulong)plantQt[10] * (ulong)UnityEngine.Random.Range(seedsprice[0], 81);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[0], 80 * plantQt[10] + 1);
                            plants[10].gameObject.SetActive(false);
                            break;
                        case "Corn":
                            plantFarmMoney[5] += (ulong)plantQt[10] * (ulong)UnityEngine.Random.Range(seedsprice[1], 161);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[1], 160 * plantQt[10] + 1);
                            plants[10].gameObject.SetActive(false);
                            break;
                        case "Radish":
                            plantFarmMoney[5] += (ulong)plantQt[10] * (ulong)UnityEngine.Random.Range(seedsprice[2], 221);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[2], 220 * plantQt[10] + 1);
                            plants[10].gameObject.SetActive(false);
                            break;
                        case "Turnip":
                            plantFarmMoney[5] += (ulong)plantQt[10] * (ulong)UnityEngine.Random.Range(seedsprice[3], 351);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[3], 350 * plantQt[10] + 1);
                            plants[10].gameObject.SetActive(false);
                            break;
                    }

                    switch (PlantB[11])
                    {
                        case "Carrot":
                            plantFarmMoney[5] += (ulong)plantQt[11] * (ulong)UnityEngine.Random.Range(seedsprice[0], 81);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[0], 80 * plantQt[11] + 1);
                            plants[11].gameObject.SetActive(false);
                            break;
                        case "Corn":
                            plantFarmMoney[5] += (ulong)plantQt[11] * (ulong)UnityEngine.Random.Range(seedsprice[1], 161);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[1], 160 * plantQt[11] + 1);
                            plants[11].gameObject.SetActive(false);
                            break;
                        case "Radish":
                            plantFarmMoney[5] += (ulong)plantQt[11] * (ulong)UnityEngine.Random.Range(seedsprice[2], 221);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[2], 220 * plantQt[11] + 1);
                            plants[11].gameObject.SetActive(false);
                            break;
                        case "Turnip":
                            plantFarmMoney[5] += (ulong)plantQt[11] * (ulong)UnityEngine.Random.Range(seedsprice[3], 351);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[3], 350 * plantQt[11] + 1);
                            plants[11].gameObject.SetActive(false);
                            break;
                    }

                    audioManager.PlaySFX(audioManager.PlantHarvest);
                    audioManager.PlaySFX(audioManager.Money);
                    MoneyUp[6].SetActive(true);
                    MoneyUpText[6].text = $"<color=green>{CurrencyTextFormat((ulong)plantFarmMoney[5])}</color>" + " BTH";
                    ValueOfPlayerMoney += plantFarmMoney[5];
                    Block[5].SetActive(false);
                    Cloud[6].SetActive(false);

                    if (starpercentage <= 50)
                    {
                        switch (plantQt[10])
                        {
                            case 5:
                               plantFarmStar[5] += ((ulong)starQt + 2);
                                break;
                            case 10:
                               plantFarmStar[5] += ((ulong)starQt + 3);
                                break;
                            case 15:
                               plantFarmStar[5] += ((ulong)starQt + 4);
                                break;
                            case 20:
                               plantFarmStar[5] += ((ulong)starQt + 5);
                                break;
                            default:
                               plantFarmStar[5] += (ulong)starQt;
                                break;
                        }

                        switch (plantQt[11])
                        {
                            case 5:
                               plantFarmStar[5] += ((ulong)starQt + 2);
                                break;
                            case 10:
                               plantFarmStar[5] += ((ulong)starQt + 3);
                                break;
                            case 15:
                               plantFarmStar[5] += ((ulong)starQt + 4);
                                break;
                            case 20:
                               plantFarmStar[5] += ((ulong)starQt + 5);
                                break;
                            default:
                               plantFarmStar[5] += (ulong)starQt;
                                break;
                        }

                        audioManager.PlaySFX(audioManager.Star);
                        StarUp[6].SetActive(true);
                        StarUpText[6].gameObject.SetActive(true);
                        StarUpText[6].text = $"<color=yellow>{CurrencyTextFormat((ulong)plantFarmStar[5])}</color>";
                        ValueOfPlayerStar += (long)plantFarmStar[5];
                        plantFarmStar[5] = 0;
                    }
                }
                else
                {
                    if (plants[10].gameObject.activeSelf && !plants[11].gameObject.activeSelf)
                    {
                        switch (PlantA[10])
                        {
                            case "Carrot":
                                plantFarmMoney[5] += (ulong)plantQt[10] * (ulong)UnityEngine.Random.Range(seedsprice[0], 81);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[0], 80 * plantQt[10] + 1);
                                plants[10].gameObject.SetActive(false);
                                break;
                            case "Corn":
                                plantFarmMoney[5] += (ulong)plantQt[10] * (ulong)UnityEngine.Random.Range(seedsprice[1], 161);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[1], 160 * plantQt[10] + 1);
                                plants[10].gameObject.SetActive(false);
                                break;
                            case "Radish":
                                plantFarmMoney[5] += (ulong)plantQt[10] * (ulong)UnityEngine.Random.Range(seedsprice[2], 221);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[2], 220 * plantQt[10] + 1);
                                plants[10].gameObject.SetActive(false);
                                break;
                            case "Turnip":
                                plantFarmMoney[5] += (ulong)plantQt[10] * (ulong)UnityEngine.Random.Range(seedsprice[3], 351);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[3], 350 * plantQt[10] + 1);
                                plants[10].gameObject.SetActive(false);
                                break;
                        }

                        audioManager.PlaySFX(audioManager.PlantHarvest);
                        audioManager.PlaySFX(audioManager.Money);
                        MoneyUp[6].SetActive(true);
                        MoneyUpText[6].text = $"<color=green>{CurrencyTextFormat((ulong)plantFarmMoney[5])}</color>" + " BTH";
                        ValueOfPlayerMoney += plantFarmMoney[5];
                        Block[5].SetActive(false);
                        Cloud[6].SetActive(false);

                        if (starpercentage <= 30)
                        {
                            switch (plantQt[10])
                            {
                                case 5:
                                   plantFarmStar[5] += ((ulong)starQt + 2);
                                    break;
                                case 10:
                                   plantFarmStar[5] += ((ulong)starQt + 3);
                                    break;
                                case 15:
                                   plantFarmStar[5] += ((ulong)starQt + 4);
                                    break;
                                case 20:
                                   plantFarmStar[5] += ((ulong)starQt + 5);
                                    break;
                                default:
                                   plantFarmStar[5] += (ulong)starQt;
                                    break;
                            }

                            audioManager.PlaySFX(audioManager.Star);
                            StarUp[6].SetActive(true);
                            StarUpText[6].gameObject.SetActive(true);
                            StarUpText[6].text = $"<color=yellow>{CurrencyTextFormat((ulong)plantFarmStar[5])}</color>";
                            ValueOfPlayerStar += (long)plantFarmStar[5];
                            plantFarmStar[5] = 0;
                        }
                    }
                }
            }
            else
            {
                audioManager.PlaySFX(audioManager.Denied);
                alert[14].SetActive(true);
            }
        }

        //Area Level 15

        else if (val == 6)
        {
            if (plantsGrowLevel[12] == 2)
            {
                starpercentage = UnityEngine.Random.Range(1, 101);
                starQt = UnityEngine.Random.Range(1, ValueOfPlayerLevel + 1);
                if (plants[12].gameObject.activeSelf && plants[13].gameObject.activeSelf)
                {
                    switch (PlantA[12])
                    {
                        case "Carrot":
                            plantFarmMoney[6] += (ulong)plantQt[12] * (ulong)UnityEngine.Random.Range(seedsprice[0], 81);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[0], 80 * plantQt[12] + 1);
                            plants[12].gameObject.SetActive(false);
                            break;
                        case "Corn":
                            plantFarmMoney[6] += (ulong)plantQt[12] * (ulong)UnityEngine.Random.Range(seedsprice[1], 161);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[1], 160 * plantQt[12] + 1);
                            plants[12].gameObject.SetActive(false);
                            break;
                        case "Radish":
                            plantFarmMoney[6] += (ulong)plantQt[12] * (ulong)UnityEngine.Random.Range(seedsprice[2], 221);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[2], 220 * plantQt[12] + 1);
                            plants[12].gameObject.SetActive(false);
                            break;
                        case "Turnip":
                            plantFarmMoney[6] += (ulong)plantQt[12] * (ulong)UnityEngine.Random.Range(seedsprice[3], 351);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[3], 350 * plantQt[12] + 1);
                            plants[12].gameObject.SetActive(false);
                            break;
                    }

                    switch (PlantB[13])
                    {
                        case "Carrot":
                            plantFarmMoney[6] += (ulong)plantQt[13] * (ulong)UnityEngine.Random.Range(seedsprice[0], 81);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[0], 80 * plantQt[13] + 1);
                            plants[13].gameObject.SetActive(false);
                            break;
                        case "Corn":
                            plantFarmMoney[6] += (ulong)plantQt[13] * (ulong)UnityEngine.Random.Range(seedsprice[1], 161);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[1], 160 * plantQt[13] + 1);
                            plants[13].gameObject.SetActive(false);
                            break;
                        case "Radish":
                            plantFarmMoney[6] += (ulong)plantQt[13] * (ulong)UnityEngine.Random.Range(seedsprice[2], 221);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[2], 220 * plantQt[13] + 1);
                            plants[13].gameObject.SetActive(false);
                            break;
                        case "Turnip":
                            plantFarmMoney[6] += (ulong)plantQt[13] * (ulong)UnityEngine.Random.Range(seedsprice[3], 351);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[3], 350 * plantQt[13] + 1);
                            plants[13].gameObject.SetActive(false);
                            break;
                    }

                    audioManager.PlaySFX(audioManager.PlantHarvest);
                    audioManager.PlaySFX(audioManager.Money);
                    MoneyUp[8].SetActive(true);
                    MoneyUpText[8].text = $"<color=green>{CurrencyTextFormat((ulong)plantFarmMoney[6])}</color>" + " BTH";
                    ValueOfPlayerMoney += plantFarmMoney[6];
                    Block[6].SetActive(false);
                    Cloud[8].SetActive(false);

                    if (starpercentage <= 50)
                    {
                        switch (plantQt[12])
                        {
                            case 5:
                               plantFarmStar[6] += ((ulong)starQt + 2);
                                break;
                            case 10:
                               plantFarmStar[6] += ((ulong)starQt + 3);
                                break;
                            case 15:
                               plantFarmStar[6] += ((ulong)starQt + 4);
                                break;
                            case 20:
                               plantFarmStar[6] += ((ulong)starQt + 5);
                                break;
                            default:
                               plantFarmStar[6] += (ulong)starQt;
                                break;
                        }

                        switch (plantQt[13])
                        {
                            case 5:
                               plantFarmStar[6] += ((ulong)starQt + 2);
                                break;
                            case 10:
                               plantFarmStar[6] += ((ulong)starQt + 3);
                                break;
                            case 15:
                               plantFarmStar[6] += ((ulong)starQt + 4);
                                break;
                            case 20:
                               plantFarmStar[6] += ((ulong)starQt + 5);
                                break;
                            default:
                               plantFarmStar[6] += (ulong)starQt;
                                break;
                        }

                        audioManager.PlaySFX(audioManager.Star);
                        StarUp[8].SetActive(true);
                        StarUpText[8].gameObject.SetActive(true);
                        StarUpText[8].text = $"<color=yellow>{CurrencyTextFormat((ulong)plantFarmStar[6])}</color>";
                        ValueOfPlayerStar += (long)plantFarmStar[6];
                        plantFarmStar[6] = 0;
                    }
                }
                else
                {
                    if (plants[12].gameObject.activeSelf && !plants[13].gameObject.activeSelf)
                    {
                        switch (PlantA[12])
                        {
                            case "Carrot":
                                plantFarmMoney[6] += (ulong)plantQt[12] * (ulong)UnityEngine.Random.Range(seedsprice[0], 81);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[0], 80 * plantQt[12] + 1);
                                plants[12].gameObject.SetActive(false);
                                break;
                            case "Corn":
                                plantFarmMoney[6] += (ulong)plantQt[12] * (ulong)UnityEngine.Random.Range(seedsprice[1], 161);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[1], 160 * plantQt[12] + 1);
                                plants[12].gameObject.SetActive(false);
                                break;
                            case "Radish":
                                plantFarmMoney[6] += (ulong)plantQt[12] * (ulong)UnityEngine.Random.Range(seedsprice[2], 221);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[2], 220 * plantQt[12] + 1);
                                plants[12].gameObject.SetActive(false);
                                break;
                            case "Turnip":
                                plantFarmMoney[6] += (ulong)plantQt[12] * (ulong)UnityEngine.Random.Range(seedsprice[3], 351);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[3], 350 * plantQt[12] + 1);
                                plants[12].gameObject.SetActive(false);
                                break;
                        }

                        audioManager.PlaySFX(audioManager.PlantHarvest);
                        audioManager.PlaySFX(audioManager.Money);
                        MoneyUp[8].SetActive(true);
                        MoneyUpText[8].text = $"<color=green>{CurrencyTextFormat((ulong)plantFarmMoney[6])}</color>" + " BTH";
                        ValueOfPlayerMoney += plantFarmMoney[6];
                        Block[6].SetActive(false);
                        Cloud[8].SetActive(false);

                        if (starpercentage <= 30)
                        {
                            switch (plantQt[12])
                            {
                                case 5:
                                   plantFarmStar[6] += ((ulong)starQt + 2);
                                    break;
                                case 10:
                                   plantFarmStar[6] += ((ulong)starQt + 3);
                                    break;
                                case 15:
                                   plantFarmStar[6] += ((ulong)starQt + 4);
                                    break;
                                case 20:
                                   plantFarmStar[6] += ((ulong)starQt + 5);
                                    break;
                                default:
                                   plantFarmStar[6] += (ulong)starQt;
                                    break;
                            }

                            audioManager.PlaySFX(audioManager.Star);
                            StarUp[8].SetActive(true);
                            StarUpText[8].gameObject.SetActive(true);
                            StarUpText[8].text = $"<color=yellow>{CurrencyTextFormat((ulong)plantFarmStar[6])}</color>";
                            ValueOfPlayerStar += (long)plantFarmStar[6];
                            plantFarmStar[6] = 0;
                        }
                    }
                }
            }
            else
            {
                audioManager.PlaySFX(audioManager.Denied);
                alert[14].SetActive(true);
            }
        }

        //Area Level 20

        else if (val == 7)
        {
            if (plantsGrowLevel[14] == 2)
            {
                starpercentage = UnityEngine.Random.Range(1, 101);
                starQt = UnityEngine.Random.Range(1, ValueOfPlayerLevel + 1);
                if (plants[14].gameObject.activeSelf && plants[15].gameObject.activeSelf)
                {
                    switch (PlantA[14])
                    {
                        case "Carrot":
                            plantFarmMoney[7] += (ulong)plantQt[14] * (ulong)UnityEngine.Random.Range(seedsprice[0], 81);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[0], 80 * plantQt[14] + 1);
                            plants[14].gameObject.SetActive(false);
                            break;
                        case "Corn":
                            plantFarmMoney[7] += (ulong)plantQt[14] * (ulong)UnityEngine.Random.Range(seedsprice[1], 161);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[1], 160 * plantQt[14] + 1);
                            plants[14].gameObject.SetActive(false);
                            break;
                        case "Radish":
                            plantFarmMoney[7] += (ulong)plantQt[14] * (ulong)UnityEngine.Random.Range(seedsprice[2], 221);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[2], 220 * plantQt[14] + 1);
                            plants[14].gameObject.SetActive(false);
                            break;
                        case "Turnip":
                            plantFarmMoney[7] += (ulong)plantQt[14] * (ulong)UnityEngine.Random.Range(seedsprice[3], 351);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[3], 350 * plantQt[14] + 1);
                            plants[14].gameObject.SetActive(false);
                            break;
                    }

                    switch (PlantB[15])
                    {
                        case "Carrot":
                            plantFarmMoney[7] += (ulong)plantQt[15] * (ulong)UnityEngine.Random.Range(seedsprice[0], 81);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[0], 80 * plantQt[15] + 1);
                            plants[15].gameObject.SetActive(false);
                            break;
                        case "Corn":
                            plantFarmMoney[7] += (ulong)plantQt[15] * (ulong)UnityEngine.Random.Range(seedsprice[1], 161);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[1], 160 * plantQt[15] + 1);
                            plants[15].gameObject.SetActive(false);
                            break;
                        case "Radish":
                            plantFarmMoney[7] += (ulong)plantQt[15] * (ulong)UnityEngine.Random.Range(seedsprice[2], 221);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[2], 220 * plantQt[15] + 1);
                            plants[15].gameObject.SetActive(false);
                            break;
                        case "Turnip":
                            plantFarmMoney[7] += (ulong)plantQt[15] * (ulong)UnityEngine.Random.Range(seedsprice[3], 351);
                            ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[3], 350 * plantQt[15] + 1);
                            plants[15].gameObject.SetActive(false);
                            break;
                    }

                    audioManager.PlaySFX(audioManager.PlantHarvest);
                    audioManager.PlaySFX(audioManager.Money);
                    MoneyUp[10].SetActive(true);
                    MoneyUpText[10].text = $"<color=green>{CurrencyTextFormat((ulong)plantFarmMoney[7])}</color>" + " BTH";
                    ValueOfPlayerMoney += plantFarmMoney[7];
                    Block[7].SetActive(false);
                    Cloud[10].SetActive(false);

                    if (starpercentage <= 50)
                    {
                        switch (plantQt[14])
                        {
                            case 5:
                                plantFarmStar[7] += ((ulong)starQt + 2);
                                break;
                            case 10:
                                plantFarmStar[7] += ((ulong)starQt + 3);
                                break;
                            case 15:
                                plantFarmStar[7] += ((ulong)starQt + 4);
                                break;
                            case 20:
                                plantFarmStar[7] += ((ulong)starQt + 5);
                                break;
                            default:
                                plantFarmStar[7] += (ulong)starQt;
                                break;
                        }

                        switch (plantQt[15])
                        {
                            case 5:
                                plantFarmStar[7] += ((ulong)starQt + 2);
                                break;
                            case 10:
                                plantFarmStar[7] += ((ulong)starQt + 3);
                                break;
                            case 15:
                                plantFarmStar[7] += ((ulong)starQt + 4);
                                break;
                            case 20:
                                plantFarmStar[7] += ((ulong)starQt + 5);
                                break;
                            default:
                                plantFarmStar[7] += (ulong)starQt;
                                break;
                        }

                        audioManager.PlaySFX(audioManager.Star);
                        StarUp[10].SetActive(true);
                        StarUpText[10].gameObject.SetActive(true);
                        StarUpText[10].text = $"<color=yellow>{CurrencyTextFormat((ulong)plantFarmStar[7])}</color>";
                        ValueOfPlayerStar += (long)plantFarmStar[7];
                        plantFarmStar[7] = 0;
                    }
                }
                else
                {
                    if (plants[14].gameObject.activeSelf && !plants[15].gameObject.activeSelf)
                    {
                        switch (PlantA[14])
                        {
                            case "Carrot":
                                plantFarmMoney[7] += (ulong)plantQt[14] * (ulong)UnityEngine.Random.Range(seedsprice[0], 81);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[0], 80 * plantQt[14] + 1);
                                plants[14].gameObject.SetActive(false);
                                break;
                            case "Corn":
                                plantFarmMoney[7] += (ulong)plantQt[14] * (ulong)UnityEngine.Random.Range(seedsprice[1], 161);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[1], 160 * plantQt[14] + 1);
                                plants[14].gameObject.SetActive(false);
                                break;
                            case "Radish":
                                plantFarmMoney[7] += (ulong)plantQt[14] * (ulong)UnityEngine.Random.Range(seedsprice[2], 221);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[2], 220 * plantQt[14] + 1);
                                plants[14].gameObject.SetActive(false);
                                break;
                            case "Turnip":
                                plantFarmMoney[7] += (ulong)plantQt[14] * (ulong)UnityEngine.Random.Range(seedsprice[3], 351);
                                ValueOfPlayerExperience += expQt = UnityEngine.Random.Range(seedsprice[3], 350 * plantQt[14] + 1);
                                plants[14].gameObject.SetActive(false);
                                break;
                        }

                        audioManager.PlaySFX(audioManager.PlantHarvest);
                        audioManager.PlaySFX(audioManager.Money);
                        MoneyUp[10].SetActive(true);
                        MoneyUpText[10].text = $"<color=green>{CurrencyTextFormat((ulong)plantFarmMoney[7])}</color>" + " BTH";
                        ValueOfPlayerMoney += plantFarmMoney[7];
                        Block[7].SetActive(false);
                        Cloud[10].SetActive(false);

                        if (starpercentage <= 30)
                        {
                            switch (plantQt[14])
                            {
                                case 5:
                                    plantFarmStar[7] += ((ulong)starQt + 2);
                                    break;
                                case 10:
                                    plantFarmStar[7] += ((ulong)starQt + 3);
                                    break;
                                case 15:
                                    plantFarmStar[7] += ((ulong)starQt + 4);
                                    break;
                                case 20:
                                    plantFarmStar[7] += ((ulong)starQt + 5);
                                    break;
                                default:
                                    plantFarmStar[7] += (ulong)starQt;
                                    break;
                            }

                            audioManager.PlaySFX(audioManager.Star);
                            StarUp[10].SetActive(true);
                            StarUpText[10].gameObject.SetActive(true);
                            StarUpText[10].text = $"<color=yellow>{CurrencyTextFormat((ulong)plantFarmStar[7])}</color>";
                            ValueOfPlayerStar += (long)plantFarmStar[7];
                            plantFarmStar[7] = 0;
                        }
                    }
                }
            }
            else
            {
                audioManager.PlaySFX(audioManager.Denied);
                alert[14].SetActive(true);
            }
        }

        //Area Level 7

        else if (val == 8)
        {
            starpercentage = UnityEngine.Random.Range(1, 101);
            starQt = UnityEngine.Random.Range(1, ValueOfPlayerLevel + 1);
            if (Block[10].activeSelf)
            {
                for (int f = 8; f < animalObj.Count; f++)
                {
                    if (animalObj[f] == null || !animalObj[f].activeSelf)
                    {
                        continue;
                    }
                    else
                    {
                        animalObj[f].SetActive(false);
                    }
                }

                audioManager.PlaySFX(audioManager.FishHarvest);
                audioManager.PlaySFX(audioManager.Money);
                MoneyUp[5].SetActive(true);
                ValueOfPlayerMoney += (ulong)animalFarmMoney[0];
                ValueOfPlayerExperience += (int)animalFarmMoney[0];
                MoneyUpText[5].text = $"<color=green>{CurrencyTextFormat((ulong)animalFarmMoney[0])}</color>" + " BTH";
                Cloud[5].SetActive(false);

                if (starpercentage <= 50)
                {
                    switch (animalQt[4])
                    {
                        case 5:
                            animalFarmStar[0] += ((ulong)starQt + 2);
                            break;
                        case 10:
                            animalFarmStar[0] += ((ulong)starQt + 3);
                            break;
                        case 15:
                            animalFarmStar[0] += ((ulong)starQt + 4);
                            break;
                        case 20:
                            animalFarmStar[0] += ((ulong)starQt + 5);
                            break;
                        default:
                            animalFarmStar[0] += (ulong)starQt;
                            break;
                    }

                    if (animalQt[5] >= 1)
                    {
                        switch (animalQt[5])
                        {
                            case 5:
                                animalFarmStar[0] += ((ulong)starQt + 2);
                                break;
                            case 10:
                                animalFarmStar[0] += ((ulong)starQt + 3);
                                break;
                            case 15:
                                animalFarmStar[0] += ((ulong)starQt + 4);
                                break;
                            case 20:
                                animalFarmStar[0] += ((ulong)starQt + 5);
                                break;
                            default:
                                animalFarmStar[0] += (ulong)starQt;
                                break;
                        }

                        audioManager.PlaySFX(audioManager.Star);
                        StarUp[5].SetActive(true);
                        StarUpText[5].gameObject.SetActive(true);
                        StarUpText[5].text = $"<color=yellow>{CurrencyTextFormat((ulong)animalFarmStar[0])}</color>";
                        ValueOfPlayerStar += (long)animalFarmStar[0];
                        animalFarmStar[0] = 0;
                    }
                }

                Block[10].SetActive(false);
            }
        }

        //Area Level 10

        else if (val == 9)
        {
            starpercentage = UnityEngine.Random.Range(1, 101);
            starQt = UnityEngine.Random.Range(1, ValueOfPlayerLevel + 1);

            if (animalObj[0].activeSelf && animalObj[1].activeSelf && !animalObj[4].activeSelf && !animalObj[5].activeSelf && Block[9].activeSelf
                || !animalObj[0].activeSelf && animalObj[4].activeSelf && animalObj[1].activeSelf && !animalObj[5].activeSelf && Block[9].activeSelf
                || !animalObj[1].activeSelf && animalObj[0].activeSelf && animalObj[5].activeSelf && !animalObj[4].activeSelf && Block[9].activeSelf
                || animalObj[4].activeSelf && animalObj[5].activeSelf && !animalObj[0].activeSelf && !animalObj[1].activeSelf && Block[9].activeSelf)
            {

                switch (AnimalA[0])
                {
                    case "Chicken":
                        animalObj[0].SetActive(false);
                        break;
                    case "Pig":
                        animalObj[0].SetActive(false);
                        break;
                    case "Buffalo":
                        animalObj[0].SetActive(false);
                        break;
                    case "Cow":
                        animalObj[0].SetActive(false);
                        break;
                    case "Horse":
                        animalObj[4].SetActive(false);
                        break;
                    default:
                        audioManager.PlaySFX(audioManager.Pop2);
                        alert[1].SetActive(true);
                        alert[10].SetActive(true);
                        StartCoroutine(ResetGame());
                        break;
                }

                switch (AnimalB[1])
                {
                    case "Chicken":
                        animalObj[1].SetActive(false);
                        break;
                    case "Pig":
                        animalObj[1].SetActive(false);
                        break;
                    case "Buffalo":
                        animalObj[1].SetActive(false);
                        break;
                    case "Cow":
                        animalObj[1].SetActive(false);
                        break;
                    case "Horse":
                        animalObj[5].SetActive(false);
                        break;
                    default:
                        audioManager.PlaySFX(audioManager.Pop2);
                        alert[1].SetActive(true);
                        alert[10].SetActive(true);
                        StartCoroutine(ResetGame());
                        break;
                }

                audioManager.PlaySFX(audioManager.AnimalHarvest);
                audioManager.PlaySFX(audioManager.Money);
                MoneyUp[7].SetActive(true);
                ValueOfPlayerMoney += (ulong)animalFarmMoney[1];
                ValueOfPlayerExperience += (int)animalFarmMoney[1];
                MoneyUpText[7].text = $"<color=green>{CurrencyTextFormat((ulong)animalFarmMoney[1])}</color>" + " BTH";
                Cloud[7].SetActive(false);

                if (starpercentage <= 50)
                {
                    switch (animalQt[0])
                    {
                        case 5:
                           animalFarmStar[1] += ((ulong)starQt + 2);
                            break;
                        case 10:
                           animalFarmStar[1] += ((ulong)starQt + 3);
                            break;
                        case 15:
                           animalFarmStar[1] += ((ulong)starQt + 4);
                            break;
                        case 20:
                           animalFarmStar[1] += ((ulong)starQt + 5);
                            break;
                        default:
                           animalFarmStar[1] += (ulong)starQt;
                            break;
                    }

                    switch (animalQt[1])
                    {
                        case 5:
                           animalFarmStar[1] += ((ulong)starQt + 2);
                            break;
                        case 10:
                           animalFarmStar[1] += ((ulong)starQt + 3);
                            break;
                        case 15:
                           animalFarmStar[1] += ((ulong)starQt + 4);
                            break;
                        case 20:
                           animalFarmStar[1] += ((ulong)starQt + 5);
                            break;
                        default:
                           animalFarmStar[1] += (ulong)starQt;
                            break;
                    }

                    audioManager.PlaySFX(audioManager.Star);
                    StarUp[7].SetActive(true);
                    StarUpText[7].gameObject.SetActive(true);
                    StarUpText[7].text = $"<color=yellow>{CurrencyTextFormat((ulong)animalFarmStar[1])}</color>";
                    ValueOfPlayerStar += (long)animalFarmStar[1];
                    animalFarmStar[1] = 0;
                }
            }
            else
            {
                if (animalObj[0].activeSelf && !animalObj[1].activeSelf && !animalObj[4].activeSelf && !animalObj[5].activeSelf && Block[9].activeSelf
                    || animalObj[4].activeSelf && !animalObj[0].activeSelf && !animalObj[1].activeSelf && !animalObj[5].activeSelf && Block[9].activeSelf)
                {

                    switch (AnimalA[0])
                    {
                        case "Chicken":
                            animalObj[0].SetActive(false);
                            break;
                        case "Pig":
                            animalObj[0].SetActive(false);
                            break;
                        case "Buffalo":
                            animalObj[0].SetActive(false);
                            break;
                        case "Cow":
                            animalObj[0].SetActive(false);
                            break;
                        case "Horse":
                            animalObj[4].SetActive(false);
                            break;
                        default:
                            audioManager.PlaySFX(audioManager.Pop2);
                            alert[1].SetActive(true);
                            alert[10].SetActive(true);
                            StartCoroutine(ResetGame());
                            break;
                    }

                    audioManager.PlaySFX(audioManager.AnimalHarvest);
                    audioManager.PlaySFX(audioManager.Money);
                    MoneyUp[7].SetActive(true);
                    ValueOfPlayerMoney += (ulong)animalFarmMoney[1];
                    ValueOfPlayerExperience += (int)animalFarmMoney[1];
                    MoneyUpText[7].text = $"<color=green>{CurrencyTextFormat((ulong)animalFarmMoney[1])}</color>" + " BTH";
                    Cloud[7].SetActive(false);

                    if (starpercentage <= 30)
                    {
                        switch (animalQt[0])
                        {
                            case 5:
                               animalFarmStar[1] += ((ulong)starQt + 2);
                                break;
                            case 10:
                               animalFarmStar[1] += ((ulong)starQt + 3);
                                break;
                            case 15:
                               animalFarmStar[1] += ((ulong)starQt + 4);
                                break;
                            case 20:
                               animalFarmStar[1] += ((ulong)starQt + 5);
                                break;
                            default:
                               animalFarmStar[1] += (ulong)starQt;
                                break;
                        }

                        audioManager.PlaySFX(audioManager.Star);
                        StarUp[7].SetActive(true);
                        StarUpText[7].gameObject.SetActive(true);
                        StarUpText[7].text = $"<color=yellow>{CurrencyTextFormat((ulong)animalFarmStar[1])}</color>";
                        ValueOfPlayerStar += (long)animalFarmStar[1];
                        animalFarmStar[1] = 0;
                    }
                }
            } 

            Block[9].SetActive(false);
        }

        //Area Level 18

        else if (val == 10) 
        {
            starpercentage = UnityEngine.Random.Range(1, 101);
            starQt = UnityEngine.Random.Range(1, ValueOfPlayerLevel + 1);

            if (animalObj[2].activeSelf && animalObj[3].activeSelf && !animalObj[6].activeSelf && !animalObj[7].activeSelf && Block[8].activeSelf
                || !animalObj[2].activeSelf && animalObj[6].activeSelf && animalObj[3].activeSelf && !animalObj[7].activeSelf && Block[8].activeSelf
                || !animalObj[3].activeSelf && animalObj[2].activeSelf && animalObj[7].activeSelf && !animalObj[6].activeSelf && Block[8].activeSelf
                || animalObj[6].activeSelf && animalObj[7].activeSelf && !animalObj[2].activeSelf && !animalObj[3].activeSelf && Block[8].activeSelf)
            {
                switch (AnimalA[2])
                {
                    case "Chicken":
                        animalObj[2].SetActive(false);
                        break;
                    case "Pig":
                        animalObj[2].SetActive(false);
                        break;
                    case "Buffalo":
                        animalObj[2].SetActive(false);
                        break;
                    case "Cow":
                        animalObj[2].SetActive(false);
                        break;
                    case "Horse":
                        animalObj[6].SetActive(false);
                        break;
                    default:
                        audioManager.PlaySFX(audioManager.Pop2);
                        alert[1].SetActive(true);
                        alert[10].SetActive(true);
                        StartCoroutine(ResetGame());
                        break;
                }

                switch (AnimalB[3])
                {
                    case "Chicken":
                        animalObj[3].SetActive(false);
                        break;
                    case "Pig":
                        animalObj[3].SetActive(false);
                        break;
                    case "Buffalo":
                        animalObj[3].SetActive(false);
                        break;
                    case "Cow":
                        animalObj[3].SetActive(false);
                        break;
                    case "Horse":
                        animalObj[7].SetActive(false);
                        break;
                    default:
                        audioManager.PlaySFX(audioManager.Pop2);
                        alert[1].SetActive(true);
                        alert[10].SetActive(true);
                        StartCoroutine(ResetGame());
                        break;
                }

                audioManager.PlaySFX(audioManager.AnimalHarvest);
                audioManager.PlaySFX(audioManager.Money);
                MoneyUp[9].SetActive(true);
                ValueOfPlayerMoney += (ulong)animalFarmMoney[2];
                ValueOfPlayerExperience += (int)animalFarmMoney[2];
                MoneyUpText[9].text = $"<color=green>{CurrencyTextFormat((ulong)animalFarmMoney[2])}</color>" + " BTH";
                Cloud[9].SetActive(false);

                if (starpercentage <= 50)
                {
                    switch (animalQt[2])
                    {
                        case 5:
                            animalFarmStar[2] += ((ulong)starQt + 2);
                            break;
                        case 10:
                            animalFarmStar[2] += ((ulong)starQt + 3);
                            break;
                        case 15:
                            animalFarmStar[2] += ((ulong)starQt + 4);
                            break;
                        case 20:
                            animalFarmStar[2] += ((ulong)starQt + 5);
                            break;
                        default:
                            animalFarmStar[2] += (ulong)starQt;
                            break;
                    }

                    switch (animalQt[3])
                    {
                        case 5:
                            animalFarmStar[2] += ((ulong)starQt + 2);
                            break;
                        case 10:
                            animalFarmStar[2] += ((ulong)starQt + 3);
                            break;
                        case 15:
                            animalFarmStar[2] += ((ulong)starQt + 4);
                            break;
                        case 20:
                            animalFarmStar[2] += ((ulong)starQt + 5);
                            break;
                        default:
                            animalFarmStar[2] += (ulong)starQt;
                            break;
                    }

                    audioManager.PlaySFX(audioManager.Star);
                    StarUp[9].SetActive(true);
                    StarUpText[9].gameObject.SetActive(true);
                    StarUpText[9].text = $"<color=yellow>{CurrencyTextFormat((ulong)animalFarmStar[2])}</color>";
                    ValueOfPlayerStar += (long)animalFarmStar[2];
                    animalFarmStar[2] = 0;
                }
            }
            else
            {
                if (animalObj[2].activeSelf && !animalObj[3].activeSelf && !animalObj[6].activeSelf && !animalObj[7].activeSelf && Block[8].activeSelf
                    || animalObj[6].activeSelf && !animalObj[2].activeSelf && !animalObj[3].activeSelf && !animalObj[7].activeSelf && Block[8].activeSelf)
                {
                    switch (AnimalA[2])
                    {
                        case "Chicken":
                            animalObj[2].SetActive(false);
                            break;
                        case "Pig":
                            animalObj[2].SetActive(false);
                            break;
                        case "Buffalo":
                            animalObj[2].SetActive(false);
                            break;
                        case "Cow":
                            animalObj[2].SetActive(false);
                            break;
                        case "Horse":
                            animalObj[6].SetActive(false);
                            break;
                        default:
                            audioManager.PlaySFX(audioManager.Pop2);
                            alert[1].SetActive(true);
                            alert[10].SetActive(true);
                            StartCoroutine(ResetGame());
                            break;
                    }

                    audioManager.PlaySFX(audioManager.AnimalHarvest);
                    audioManager.PlaySFX(audioManager.Money);
                    MoneyUp[9].SetActive(true);
                    ValueOfPlayerMoney += (ulong)animalFarmMoney[2];
                    ValueOfPlayerExperience += (int)animalFarmMoney[2];
                    MoneyUpText[9].text = $"<color=green>{CurrencyTextFormat((ulong)animalFarmMoney[2])}</color>" + " BTH";
                    Cloud[9].SetActive(false);

                    if (starpercentage <= 30)
                    {
                        switch (animalQt[2])
                        {
                            case 5:
                                animalFarmStar[2] += ((ulong)starQt + 2);
                                break;
                            case 10:
                                animalFarmStar[2] += ((ulong)starQt + 3);
                                break;
                            case 15:
                                animalFarmStar[2] += ((ulong)starQt + 4);
                                break;
                            case 20:
                                animalFarmStar[2] += ((ulong)starQt + 5);
                                break;
                            default:
                                animalFarmStar[2] += (ulong)starQt;
                                break;
                        }

                        audioManager.PlaySFX(audioManager.Star);
                        StarUp[9].SetActive(true);
                        StarUpText[9].gameObject.SetActive(true);
                        StarUpText[9].text = $"<color=yellow>{CurrencyTextFormat((ulong)animalFarmStar[2])}</color>";
                        ValueOfPlayerStar += (long)animalFarmStar[2];
                        animalFarmStar[2] = 0;
                    }
                }
            }

            Block[8].SetActive(false);
        }
        else
        {
            audioManager.PlaySFX(audioManager.Pop2);
            alert[1].SetActive(true);
            alert[10].SetActive(true);
            StartCoroutine(ResetGame());
        }
    }
}