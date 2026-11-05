
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SlotReel : MonoBehaviour
{
    [Header("Reel Settings")]
    [SerializeField] private Image symbolImage;
    [SerializeField] private Sprite[] symbols;

    [Header("Spin Settings")]
    [SerializeField] private float spinSpeed = 0.08f;

    public Sprite CurrentSymbol => symbolImage.sprite;

    // Changes symbols rapidly to create a spinning effect.
    public IEnumerator Spin(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            int randomIndex = Random.Range(0, symbols.Length);

            symbolImage.sprite = symbols[randomIndex];

            yield return new WaitForSeconds(spinSpeed);

            elapsed += spinSpeed;
        }

        // Select the final random symbol.
        int finalIndex = Random.Range(0, symbols.Length);
        symbolImage.sprite = symbols[finalIndex];
    }
}
