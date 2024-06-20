using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public BoardManager boardManager;

    public TextManager textManager;

    private void Awake()
    {
        if (null == instance )
        {
            instance = this;
        }
    }

}
