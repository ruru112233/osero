using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    Vector2 pos = Vector2.zero;

    public Vector2 BoardPositon { get { return pos; } set { pos = value; } }
}
