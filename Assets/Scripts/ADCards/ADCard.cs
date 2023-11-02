using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ADCard : MonoBehaviour
{
    public Image CardImage;

    private RectTransform m_RectTransform = null;
    public RectTransform RectTransform
    {
        get
        {
            if (m_RectTransform == null)
            {
                m_RectTransform = GetComponent<RectTransform>();
            }
            return m_RectTransform;
        }
    }

    private AdCardConfig CurrentConfig = null;

    public ADCardData Data = null;
    public void SetData(ADCardData data)
    {
        CardImage.sprite = data.Sprite;
        CardImage.SetNativeSize();
        Data = data;
    }

    public void SetConfig(AdCardConfig config)
    {
        CurrentConfig = config;
        RectTransform.anchoredPosition = config.Position;
        RectTransform.localScale = Vector3.one * config.Scale;
        CardImage.color = config.Color;
    }

    public void AnimToConfig(AdCardConfig config)
    {
        StartCoroutine(RoutineAnimToConfig(config));
    }

    public IEnumerator RoutineAnimToConfig(AdCardConfig config)
    {
        var posFrom = CurrentConfig.Position;
        var posTo = config.Position;
        var scaleFrom = CurrentConfig.Scale;
        var scaleTo = config.Scale;
        var colorFrom = CurrentConfig.Color;
        var colorTo = config.Color;

        var animDuration = config.AnimDuration;

        float t = 0f;
        while (t < animDuration)
        {
            t += Time.deltaTime;
            var p = t / animDuration;

            var pos = Vector2.Lerp(posFrom, posTo, p);
            var scale = Mathf.Lerp(scaleFrom, scaleTo, p);
            var color = Color.Lerp(colorFrom, colorTo, p);

            RectTransform.anchoredPosition = pos;
            RectTransform.localScale = Vector3.one * scale;
            CardImage.color = color;

            yield return null;
        }

        SetConfig(config);
    }
}