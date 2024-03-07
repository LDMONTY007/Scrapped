using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

//https://forum.unity.com/threads/progress-bar-slider-gradient-fill.706544/

[RequireComponent(typeof(Image))]
public class ImageGradient : MonoBehaviour
{
    public bool manuallyAssignFields = false;

    [SerializeField] private Gradient _gradient = null;
    [SerializeField] private Image _image = null;
    public float totalTime = 1f;
    private float _currentTime = 0f;
    public float currentTime { get { return _currentTime; } set { _currentTime = Mathf.Clamp(value, 0f, totalTime); } }

    public UnityEvent OnGradientAnimationEnd;

    private void Awake()
    {
        if (!manuallyAssignFields)
        {
            _image = GetComponent<Image>();
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
            _image.color = _gradient.Evaluate(currentTime / totalTime);
            currentTime += Time.deltaTime;
            yield return null;
        }
        OnGradientAnimationEnd.Invoke();
    }

}
