using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 0.25f);
    }
}