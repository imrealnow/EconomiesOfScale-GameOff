using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class TextPopup : MonoBehaviour
{
    public Vector2 offset = Vector2.zero;
    public float showDuration;
    public bool graduallyReveal;
    [Tooltip("How long to wait between each character showing up")]
    public float characterRevealStepDelay;

    private string targetText = "";
    private TextMeshPro textMesh;
    private float endTime;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    private void FixedUpdate()
    {
        if (Time.time >= endTime) gameObject.SetActive(false);
    }

    public void AttachToTransform(Transform parent)
    {
        transform.SetParent(parent);
    }

    public void SetText(string text)
    {
        targetText = text;
        endTime = Time.time + showDuration;
        if (!graduallyReveal)
        {
            if (text == null) return;
            textMesh.text = text;
        }
        else
        {
            StartCoroutine(GraduallyRevealCharacters());
        }
    }

    private IEnumerator GraduallyRevealCharacters()
    {
        char[] characters = targetText.ToCharArray();
        endTime += characters.Length * characterRevealStepDelay;
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < characters.Length; i++)
        {
            stringBuilder.Append(characters[i]);
            textMesh.text = stringBuilder.ToString();
            yield return new WaitForSeconds(characterRevealStepDelay);
        }
    }
}
