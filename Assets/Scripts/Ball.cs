using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour
{
    private Vector3 _velocity;
    [SerializeField] private float speed;
    
    // Start is called before the first frame update
    private void OnEnable()
    {
        if (!GameManager.Instance.isCreator)
            speed = 0f;
        SetVelocity();
    }

    public void SetVelocity()
    {
        var temp = Random.Range(1, 5);
        switch (temp)
        {
            case 1:
                _velocity = new Vector3(1, 1, 0);
                break;
            case 2:
                _velocity = new Vector3(1, -1, 0);
                break;
            case 3:
                _velocity = new Vector3(-1, 1, 0);
                break;
            case 4:
                _velocity = new Vector3(-1, -1, 0);
                break;
        }
        _velocity = Vector3.Normalize(_velocity) * speed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += _velocity * Time.deltaTime;
    }

    public void SetVelocity(Vector3 velocity)
    {
        _velocity = velocity;
    }
    public Vector3 GetVelocity()
    {
        return _velocity;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!GameManager.Instance.isCreator)
            return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _velocity = new Vector3(-_velocity.x, _velocity.y, _velocity.z);
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            _velocity = new Vector3(_velocity.x, -_velocity.y, _velocity.z);
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Goal1"))
        {
            GameManager.Instance.player2Score++;
            GameManager.Instance.room.GetMatch().SaveState("P2", GameManager.Instance.player2Score.ToString());
            if (GameManager.Instance.player2Score >= 5)
            {
                GameManager.Instance.FinishGame(1);
                return;
            }
            StartCoroutine(ResetBall());
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Goal2"))
        {
            GameManager.Instance.player1Score++;
            GameManager.Instance.room.GetMatch().SaveState("P1", GameManager.Instance.player1Score.ToString());
            if (GameManager.Instance.player1Score >= 5)
            {
                GameManager.Instance.FinishGame(0);
                return;
            }
            StartCoroutine(ResetBall());
        }
    }

    private IEnumerator ResetBall()
    {
        transform.position = Vector3.zero;
        _velocity = Vector3.zero;
        yield return new WaitForSeconds(3f);
        SetVelocity();
    }
}
