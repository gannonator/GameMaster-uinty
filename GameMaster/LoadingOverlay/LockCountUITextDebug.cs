using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LockCountUITextDebug : MonoBehaviour
{
    public Text text;
    // Use this for initialization
    void Start()
    {
        text =  GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = GameMaster.LoadingLocks.LockCount.ToString(); 
    }
}
