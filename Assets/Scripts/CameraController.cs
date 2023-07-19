using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    private Vector3 position;
    private float cameraPositionZ = -1f;
    private void Awake()
    {
        if (!player)
        {
            player = FindObjectOfType<Hero>().transform;
        }
    }

    private void Update()
    {
        position = player.position;
        position.z = cameraPositionZ;
        position.y += 3f;
        transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime);
        
    }
}
