using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class ADCardsView : MonoBehaviour
{
    // [Back(Hidden), List0, List1, ..., Front(Hidden)]
    //  -> Forward
    // Backward <-
    [SerializeField] private List<AdCardConfig> ListConfigs;

    [SerializeField] private List<ADCard> Cards;

    private List<ADCardData> ListDatas;

    [ContextMenu("Load Configs From Transforms")]
    public void LoadConfigsFromTransforms()
    {
        ListConfigs.Clear();
        foreach (var card in Cards)
        {
            card.RectTransform.pivot = new Vector2(0.5f, 0f);
            var config = new AdCardConfig
            {
                Position = card.RectTransform.anchoredPosition,
                Scale = card.transform.localScale.x,
                Color = card.CardImage.color,
                AnimDuration = 0.5f,
            };
            ListConfigs.Add(config);
        }
    }

    [ContextMenu("Set Cards With Configs")]
    public void SetCardsWithConfigs()
    {
        for (int i = 0; i < Cards.Count; i++)
        {
            Cards[i].SetConfig(ListConfigs[i]);
            Cards[i].SetData(GetData(i));
        }
    }

    private ADCardData GetData(int index)
    {
        if (index < 0)
        {
            index += ListDatas.Count;
        }
        if (index >= ListDatas.Count)
        {
            index %= ListDatas.Count;
        }
        return ListDatas[index];
    }

    public void InitAndStart(
        List<ADCardData> datas,
        float timeDistance,
        ADCardAnimType animType)
    {
        ListDatas = datas;
        for (int i = 0; i < ListDatas.Count; i++)
        {
            ListDatas[i].Index = i;
        }

        SetCardsWithConfigs();
        StartCoroutine(RoutineAnimNextCard(timeDistance, animType));
    }

    private bool IsAnimRunning = false;
    private IEnumerator RoutineAnimNextCard(
        float timeDistance,
        ADCardAnimType animType)
    {
        var wait = new WaitForSeconds(timeDistance);

        while (true)
        {
            IsAnimRunning = true;

            switch (animType)
            {
                case ADCardAnimType.SerialForward:
                    yield return RoutineAnimSerialForward();
                    break;
                case ADCardAnimType.SerialBackward:
                    yield return RoutineAnimSerialBackward();
                    break;
                case ADCardAnimType.ParallelForward:
                    ShiftForward();
                    yield return RoutineAnimParallel();
                    break;
                case ADCardAnimType.ParallelBackward:
                    ShiftBackward();
                    yield return RoutineAnimParallel();
                    break;
                default:
                    break;
            }

            yield return wait;
        }
    }

    private void ShiftBackward()
    {
        // Show Range [1, Count - 2]
        // Shift Backward Means:
        // Cards[1] => Move To Configs[0] And Hide 
        // Cards[0] => Move To Configs[Count - 1], Always Hide
        // Cards[Count - 1] => Move To Configs[Count - 2] And Show Cards[Count - 2].Id + 1 Data

        // Change Data
        var frontCard = Cards[Cards.Count - 1];
        var markCard = Cards[Cards.Count - 2];
        var markId = markCard.Data.Index;
        var data = GetData(markId + 1);
        frontCard.SetData(data);

        // Change Card
        var backCard = Cards[0];
        Cards.RemoveAt(0);
        Cards.Add(backCard);
        backCard.RectTransform.SetAsLastSibling();
    }

    private void ShiftForward()
    {
        // Show Range [1, Count - 2]
        // Shift Forward Means:
        // Cards[Count - 2] => Move To Configs[Count - 1] And Hide 
        // Cards[Count - 1] => Move To Configs[0], Always Hide
        // Cards[0] => Move To Configs[1] And Show Cards[1].Id - 1 Data

        // Change Data
        var backCard = Cards[0];
        var markCard = Cards[1];
        var markId = markCard.Data.Index;
        var data = GetData(markId - 1);
        backCard.SetData(data);

        // Change Card
        var frontCard = Cards[Cards.Count - 1];
        Cards.RemoveAt(Cards.Count - 1);
        Cards.Insert(0, frontCard);
        frontCard.RectTransform.SetAsFirstSibling();
    }

    private IEnumerator RoutineAnimSerialForward()
    {
        ShiftForward();
        // anim forward
        for (int i = 0; i < Cards.Count; i++)
        {
            if (i == Cards.Count - 1)
            {
                IsAnimRunning = false;
            }
            yield return Cards[i].RoutineAnimToConfig(ListConfigs[i]);
        }
    }

    private IEnumerator RoutineAnimSerialBackward()
    {
        ShiftBackward();
        for (int i = Cards.Count - 1; i >= 0; i--)
        {
            if (i == Cards.Count - 2)
            {
                IsAnimRunning = false;
            }
            yield return Cards[i].RoutineAnimToConfig(ListConfigs[i]);
        }
    }

    private List<IEnumerator> enumerators = new List<IEnumerator>();
    private IEnumerator RoutineAnimParallel()
    {
        enumerators.Clear();

        for (int i = 0; i < Cards.Count; i++)
        {
            enumerators.Add(Cards[i].RoutineAnimToConfig(ListConfigs[i]));
        }

        bool hasNext = true;
        while (hasNext)
        {
            hasNext = false;
            for (int i = 0; i < enumerators.Count; i++)
            {
                if (enumerators[i].MoveNext())
                {
                    hasNext = true;
                }
            }
            yield return null;
        }

        IsAnimRunning = false;
    }

    public void OnClicked()
    {
        if (!IsAnimRunning)
        {
            var CurrentData = Cards[Cards.Count - 2].Data;
            CurrentData?.Action?.Invoke();
        }
    }
}