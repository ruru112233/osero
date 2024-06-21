using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    public Text turnText;
    public Text whiteStoneCounterText;
    public Text blackStoneCounterText;

    [SerializeField] private GameObject pathText;

    private int blackStoneCounter = 0;
    private int whiteStoneCounter = 0;
    public int BlackStoneCounter { 
        get { return blackStoneCounter; }
        set { blackStoneCounter = value; } 
    }

    public int WhiteStoneCounter {
        get { return whiteStoneCounter; }
        set { whiteStoneCounter = value; } 
    }

    private string BLACK_STR = "��";
    private string WHITE_STR = "��";

    // Start is called before the first frame update
    void Start()
    {
        turnText.text = "��ԁF�@" + BLACK_STR;

        blackStoneCounterText.text = BLACK_STR + ":" + BlackStoneCounter;
        whiteStoneCounterText.text = WHITE_STR + ":" + WhiteStoneCounter;

        pathText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.boardManager.BlackTurn)
        {
            turnText.text = "��ԁF�@" + BLACK_STR;
        }
        else
        {
            turnText.text = "��ԁF�@" + WHITE_STR;
        }

        blackStoneCounterText.text = BLACK_STR + ":" + BlackStoneCounter;
        whiteStoneCounterText.text = WHITE_STR + ":" + WhiteStoneCounter;
    }

    public void ValidPathText()
    {
        pathText.SetActive(true);
    }

    public void InvalidPathText()
    {
        pathText.SetActive(false);
    }
}
