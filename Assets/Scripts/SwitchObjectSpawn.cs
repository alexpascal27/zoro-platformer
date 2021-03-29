using System;
using UnityEngine;

public class SwitchObjectSpawn : MonoBehaviour
{
    [SerializeField] private bool changeRotation = false;
    [SerializeField] private bool changeScale = false;
    [SerializeField] private Vector3 objectPosition=Vector3.zero;
    [SerializeField] private Vector3 objectRotation=Vector3.zero;
    [SerializeField] private Vector3 objectScale=Vector3.one;
    [SerializeField] private GameObject objectPrefab;
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        GameObject collisionGameObject = other.gameObject;
        if (collisionGameObject.CompareTag("Bullet"))
        {
            Trigger();
            Destroy(gameObject);
        }
    }

    private void Trigger()
    {
        if (changeRotation)
        {
            objectPrefab.transform.rotation = Quaternion.Euler(objectRotation);
        }

        if (changeScale)
        {
            Transform transformParent= objectPrefab.transform.parent;
            objectPrefab.transform.parent = null;
            objectPrefab.transform.localScale = objectScale;
            objectPrefab.transform.parent = transformParent;
        }

        objectPrefab.transform.position = objectPosition;
        
        Instantiate(objectPrefab);
    }
}
