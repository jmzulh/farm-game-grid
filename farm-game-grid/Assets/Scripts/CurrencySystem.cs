using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class CurrencySystem : MonoBehaviour
{
    private static Dictionary<CurrencyType, int> CurrencyAmounts  = new Dictionary<CurrencyType, int>();
    [SerializeField] private List<GameObject> texts;
    private Dictionary<CurrencyType, TextMeshProUGUI> currencyTexts = new Dictionary<CurrencyType, TextMeshProUGUI>();

    private void Awake(){
        for (int i = 0; i < this.texts.Count; i++)
        {
            print((CurrencyType)i);
            CurrencyAmounts.Add((CurrencyType)i, 0);
            print(texts[i].name);
            currencyTexts.Add((CurrencyType)i, texts[i].GetComponent<TextMeshProUGUI>());
        }
    }

    private void Start(){
        EventManager.Instance.AddListener<CurrencyChangeGameEvent>(OnCurrencyChange);
        EventManager.Instance.AddListener<NotEnoughCurrencyGameEvent>(OnNotEnough);
    }

    private void OnCurrencyChange(CurrencyChangeGameEvent info)
    {
        CurrencyAmounts[info.currencyType] += info.amount;
        currencyTexts[info.currencyType].text = CurrencyAmounts[info.currencyType].ToString();
    }

    private void OnNotEnough(NotEnoughCurrencyGameEvent info){
        Debug.Log(message:$"You don't have enough of {info.amount} {info.currencyType}");
    }
}

public enum CurrencyType{
    Coins,
    Diamonds
}