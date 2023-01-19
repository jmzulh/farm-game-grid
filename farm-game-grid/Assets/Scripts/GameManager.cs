using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager current;
    public GameObject canvas;

    void Awake(){
        current = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetXP(int amount){
        XPAddedGameEvent info = new XPAddedGameEvent(amount);
        EventManager.Instance.QueueEvent(info);
    }

    public void GetCoins(int amount){
        Debug.Log(CurrencyType.Coins);
        CurrencyChangeGameEvent info = new CurrencyChangeGameEvent(amount, CurrencyType.Coins);
        EventManager.Instance.QueueEvent(info);
    }
}
