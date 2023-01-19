using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSystem : MonoBehaviour
{
    private int XPNow;
    private int level;
    private int xpToNext;

    [SerializeField] private GameObject levelPanel;
    [SerializeField] private GameObject lvlWindowPrefab;

    private Slider slider;
    private TextMeshProUGUI xpText;
    private TextMeshProUGUI lvlText;
    private Image starimage;

    public static bool initialized;
    private static Dictionary<int, int> xpToNextLevel = new Dictionary<int, int>();
    private static Dictionary<int, int[]> lvlReward = new Dictionary<int, int[]>();

    void Awake(){
        this.slider = levelPanel.transform.Find("Slider").GetComponent<Slider>();
        this.xpText = slider.transform.Find("xp_text").GetComponent<TextMeshProUGUI>();
        this.starimage = levelPanel.transform.Find("star_icon").GetComponent<Image>();
        this.lvlText = levelPanel.transform.Find("level").GetComponent<TextMeshProUGUI>();

        if(!initialized){
            Initialize();
        }

        xpToNextLevel.TryGetValue(level, out xpToNext);
    }

    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.AddListener<XPAddedGameEvent>(OnXpAdded);
        EventManager.Instance.AddListener<LevelChangedGameEvent>(OnLevelChanged);

        this.UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private static void Initialize(){
        try{
            string path = "levelsXP";
            TextAsset textAsset = Resources.Load<TextAsset>(path);
            string[] lines = textAsset.text.Split('\n');

            xpToNextLevel = new Dictionary<int, int>(lines.Length - 1);

            for (int i = 0; i < lines.Length - 1; i++)
            {
                string[] columns = lines[i].Split(',');

                int lvl = -1;
                int xp = -1;
                int curr1 = -1;
                int curr2 = -1;

                int.TryParse(columns[0], out lvl);
                int.TryParse(columns[1], out xp);
                int.TryParse(columns[2], out curr1);
                int.TryParse(columns[3], out curr2);

                if(lvl >= 0 && xp >= 0){
                    if(!xpToNextLevel.ContainsKey(lvl)){
                        xpToNextLevel.Add(lvl, xp);
                        lvlReward.Add(lvl, new []{curr1, curr2});
                    }
                }
            }
        }catch(Exception e){
            Debug.LogError(e.Message);
        }

        initialized = true;
    }

    private void UpdateUI(){
        float fill = (float)XPNow / xpToNext;
        slider.value = fill;
        xpText.text = XPNow + "/" + xpToNext;
    }

    private void OnXpAdded(XPAddedGameEvent info){
        XPNow += info.amount;

        this.UpdateUI();

        if(XPNow >= xpToNext){
            level++;
            LevelChangedGameEvent levelChange = new LevelChangedGameEvent(level);
            EventManager.Instance.QueueEvent(levelChange);
        }
    }

    private void OnLevelChanged(LevelChangedGameEvent info){
        XPNow -= xpToNext;
        xpToNext = xpToNextLevel[info.newlvl];
        lvlText.text = (info.newlvl +1).ToString();
        this.UpdateUI();

        GameObject window = Instantiate(lvlWindowPrefab, GameManager.current.canvas.transform);

        window.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate {Destroy(window);});

        CurrencyChangeGameEvent currencyInfo = new CurrencyChangeGameEvent(lvlReward[info.newlvl][0], CurrencyType.Coins);
        EventManager.Instance.QueueEvent(currencyInfo);

        currencyInfo = new CurrencyChangeGameEvent(lvlReward[info.newlvl][1], CurrencyType.Coins);
        EventManager.Instance.QueueEvent(currencyInfo);
    }

}
