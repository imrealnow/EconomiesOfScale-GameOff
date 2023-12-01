using TMPro;
using UnityEngine;

public class ScaleText : MonoBehaviour
{
    public ScaleManager scaleManager;
    private TextMeshProUGUI text;
    public string template = "Scale: {0}";

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        text.text = string.Format(template, scaleManager.Scale.ToString());
    }
}
