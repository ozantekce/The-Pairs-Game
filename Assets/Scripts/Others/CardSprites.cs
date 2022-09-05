using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardSprites", menuName = "ScriptableObjects/CardSprites", order = 1)]
public class CardSprites : ScriptableObject
{

    public Sprite[] cardFrontSprites;

    public Sprite[] cardBackSprites;

}
