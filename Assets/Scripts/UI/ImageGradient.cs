using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

//Jamming but also trying not to steal code so I'm just kinda linking things where I found stuff.
//https://forum.unity.com/threads/progress-bar-slider-gradient-fill.706544/

[RequireComponent(typeof(Image))]
public class ImageGradient : MonoBehaviour
{
    public bool playOnStart = false;
    public bool loopGradient = false;
    public float delayBetweenLoops = 5f;
    public bool manuallyAssignFields = false;
    public bool useCanvasGroupAlpha = false;

    public CanvasGroup canvasGroup;

    [SerializeField] private Gradient _gradient = null;
    [SerializeField] private Image _image = null;
    public float totalTime = 1f;
    private float _currentTime = 0f;
    public float currentTime { get { return _currentTime; } set { _currentTime = Mathf.Clamp(value, 0f, totalTime); } }

    public UnityEvent onGradientAnimationStart;
    public UnityEvent OnGradientAnimationEnd;



    private void Awake()
    {
        if (!manuallyAssignFields)
        {
            _image = GetComponent<Image>();
        }
        //currentTime = 0f;
    }

    private void Start()
    {
        if (useCanvasGroupAlpha)
        {
            canvasGroup.alpha = _gradient.Evaluate(currentTime / totalTime).a;
        }
        _image.color = _gradient.Evaluate(currentTime / totalTime);

        if (playOnStart)
        {
            StartAnimatingGradient();
        }
    }

    private void Update()
    {
        
    }

    public IEnumerator delayCoroutine(float delay, IEnumerator coroutine)
    {
        while (delay > 0f)
        {
            delay -= Time.deltaTime;
            yield return null;
        }
        StartCoroutine(coroutine);
    }

    public void StartAnimatingGradient()
    {
        Debug.Log("START ANIMATING GRADIENT");
        StartCoroutine(AnimateGradientCoroutine());
    }


    //just animates from one end of the gradient to the other end.
    public IEnumerator AnimateGradientCoroutine()
    {
        onGradientAnimationStart.Invoke();
        Debug.Log("ANIMATE COROUTINE");
        Debug.Log(currentTime + " " + totalTime);
        while (currentTime < totalTime)
        {
            Debug.Log("ANIMATE COROUTINE 1");
            Debug.Log(currentTime + " " + totalTime);
            if (currentTime >= totalTime - 0.01f)
            {
                Debug.Log("ANIMATE COROUTINE 2");
                currentTime = totalTime;
            }
            if (useCanvasGroupAlpha)
            {
                canvasGroup.alpha = _gradient.Evaluate(currentTime / totalTime).a;
            }
            _image.color = _gradient.Evaluate(currentTime / totalTime);
            currentTime += Time.deltaTime;
            yield return null;
        }
        currentTime = 0f;
        OnGradientAnimationEnd.Invoke();
        if (loopGradient)
        {
            StartCoroutine(delayCoroutine(delayBetweenLoops, AnimateGradientCoroutine()));
        }
    }

}
