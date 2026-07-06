using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float lifetime = 0.25f;
    private Coroutine destroyCoroutine;

    void OnEnable()
    {
        if (destroyCoroutine != null) StopCoroutine(destroyCoroutine);
        destroyCoroutine = StartCoroutine(ReturnToPoolAfterTime());
    }

    private System.Collections.IEnumerator ReturnToPoolAfterTime()
    {
        yield return new WaitForSeconds(lifetime);
        if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.Instance.ReturnToPool(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}