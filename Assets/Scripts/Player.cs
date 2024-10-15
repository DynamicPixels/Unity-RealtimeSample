using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private bool mainPlayer;
    [SerializeField] private float speed;
    public int direction;
    
    private void Update()
    {
        if (!GameManager.Instance.move)
            return;
        if (mainPlayer && GameManager.Instance.isCreator || !mainPlayer && !GameManager.Instance.isCreator)
        {
            if (Input.GetKey(KeyCode.S))
                MoveDown();
            else if (Input.GetKey(KeyCode.W))
                MoveUp();
            else direction = 0;
        }
        else
        {
            switch (direction)
            {
                case 1:
                    MoveUp();
                    break;
                case -1:
                    MoveDown();
                    break;
            }

            StartCoroutine(ResetDirection());
        }
    }

    private IEnumerator ResetDirection()
    {
        yield return new WaitForSeconds(0.1f);
        direction = 0;
    }

    private void MoveUp()
    {
        transform.position += new Vector3(0f, speed * Time.deltaTime, 0f);
        direction = 1;
    }

    private void MoveDown()
    {
        transform.position += new Vector3(0f, -speed * Time.deltaTime, 0f);
        direction = -1;
    }

    public void SetDirection(int dir)
    {
        direction = dir;
        StopAllCoroutines();
    }

    public float GetSpeed()
    {
        return speed;
    }
}
