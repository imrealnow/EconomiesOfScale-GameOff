using TMPro;
using UnityEngine;

public class SIntToText : MonoBehaviour
{
    public SInt gold;
    [TextArea]
    public string template = "Gold: {0}";
    private TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        text.text = string.Format(template, gold.Value);
    }
}
