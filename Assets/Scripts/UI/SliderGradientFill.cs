using UnityEngine;
using UnityEngine.UI;

//https://forum.unity.com/threads/progress-bar-slider-gradient-fill.706544/

[RequireComponent(typeof(Image))]
public class SliderGradientFill : MonoBehaviour
{
    public bool manuallyAssignFields = false;

    [SerializeField] private Gradient _gradient = null;
    [SerializeField] private Image _image = null;
    [SerializeField] private Slider _slider = null;

    private void Awake()
    {
        if (!manuallyAssignFields)
        {
            _slider = GetComponentInParent<Slider>();
            _image = GetComponent<Image>();
        }
    }

    private void Update()
    {
        _image.color = _gradient.Evaluate(_slider.value / _slider.maxValue);
    }

}
