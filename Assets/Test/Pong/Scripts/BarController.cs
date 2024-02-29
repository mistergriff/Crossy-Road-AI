using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarController : MonoBehaviour
{
    [SerializeField] private Transform ball;

    // Update is called once per frame
    void Update()
    {
        Vector3 ballPosition = ball.localPosition;

        Vector3 pos = new Vector3(9f, ballPosition.y, 0f);

        transform.localPosition = pos;
    }
}
