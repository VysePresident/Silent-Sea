/*
 Function coded by Isaac Burns
 Mostly followed a Brackeys tutorial:
 https://www.youtube.com/watch?v=_nRzoTzeyxU
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string name;
    [TextArea(5,10)]
    public string[] sentences;

}
