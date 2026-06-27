using UnityEngine;

public class FlashEffect : MonoBehaviour
{
    public float blinkDecaySpeed = 10f;
    private SpriteRenderer[] renderers;
    private MaterialPropertyBlock propertyBlock;
    private float blinkFactor = 0f;

    private void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
        propertyBlock = new MaterialPropertyBlock();
        ApplyBlinkFactor();
    }

    private void Update()
    {
        if (blinkFactor > 0f)
        {
            blinkFactor = Mathf.Lerp(blinkFactor, 0f, Time.deltaTime * blinkDecaySpeed);
            if (blinkFactor < 0.01f) blinkFactor = 0f;
            ApplyBlinkFactor();
        }
    }

    private void ApplyBlinkFactor()
    {
        foreach (var renderer in renderers)
        {
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat("_BlinkFactor", blinkFactor);
            renderer.SetPropertyBlock(propertyBlock);
        }
    }

    public void TriggerFlash()
    {
        blinkFactor = 1f;
        ApplyBlinkFactor();
    }
}