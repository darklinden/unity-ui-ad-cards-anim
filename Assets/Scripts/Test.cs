using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public ADCardAnimType animType = ADCardAnimType.None;
    public int dataCount = 5;
    public float timeDistance = 2f;
    public Sprite[] sprites;
    public ADCardsView cardView;

    void Start()
    {
        var list = new List<ADCardData>();

        for (int i = 0; i < dataCount; i++)
        {
            var sprite = sprites[i % sprites.Length];
            var index = i;
            list.Add(new ADCardData
            {
                Sprite = sprite,
                Action = () => OnClick(index),
            });
        }

        cardView.InitAndStart(list, timeDistance, animType);
    }

    void OnClick(int index)
    {
        Debug.Log("OnClick: " + index);
    }
}
