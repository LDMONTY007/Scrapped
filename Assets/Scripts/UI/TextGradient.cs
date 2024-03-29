using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

//https://forum.unity.com/threads/progress-bar-slider-gradient-fill.706544/

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextGradient : MonoBehaviour
{
    public bool manuallyAssignFields = false;

    [SerializeField] private Gradient _gradient = null;
    [SerializeField] private TextMeshProUGUI _text = null;
    public float totalTime = 1f;
    private float _currentTime = 0f;
    public float currentTime { get { return _currentTime; } set { _currentTime = Mathf.Clamp(value, 0f, totalTime); } }

    public UnityEvent OnGradientAnimationEnd;

    private void Awake()
    {
        if (!manuallyAssignFields)
        {
            _text = GetComponent<TextMeshProUGUI>();
        }
    }

    private void Update()
    {
        
    }

    public void StartAnimatingGradient()
    {
        StartCoroutine(AnimateGradientCoroutine());
    }

    //just animates from one end of the gradient to the other end.
    public IEnumerator AnimateGradientCoroutine()
    {
        while (currentTime < totalTime)
        {
            if (currentTime >= totalTime - 0.01f)
            {
                currentTime = totalTime;
            }
            _text.color = _gradient.Evaluate(currentTime / totalTime);
            currentTime += Time.deltaTime;
            yield return null;
        }
        currentTime = 0f;
        OnGradientAnimationEnd.Invoke();
    }

    public void StartAnimatingGradient(float time, Gradient g)
    {
        StartCoroutine(AnimateGradientCoroutine(time, g));
    }

    //just animates from one end of the gradient to the other end.
    public IEnumerator AnimateGradientCoroutine(float totTime, Gradient g)
    {
        while (currentTime < totTime)
        {
            if (currentTime >= totTime - 0.01f)
            {
                currentTime = totTime;
            }
            _text.color = g.Evaluate(currentTime / totTime);
            currentTime += Time.deltaTime;
            yield return null;
        }
        currentTime = 0f;
        OnGradientAnimationEnd.Invoke();
    }

}
