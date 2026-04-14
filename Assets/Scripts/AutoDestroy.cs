using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float lifetime = 0.25f;
    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}