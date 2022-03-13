using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Player : MonoBehaviour {

    public float forwardVelocity = 5f;
    public float jumpForce = 5f;
    public float maxFallVelocity = 10f;
    private new Rigidbody2D rigidbody;
    private bool inAir = false;
    private bool canDoubleJump = true;
    private bool alive = true;

    private Chunk activeChunk;
    private float lastActiveChunkTime = 0f;
    private float activeChunkTime = 0f;

    private PlayerMesh mesh;

    public void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        mesh = GetComponentInChildren<PlayerMesh>();
    }

    // Start is called before the first frame update
    public void Start() {
        Debug.Log("Start!");
    }

    // Update is called once per frame
    public void Update() {
        if (alive) {
            SetVelocityX(forwardVelocity);
            if (rigidbody.velocity.y < -maxFallVelocity) {
                SetVelocityY(-maxFallVelocity);
            }
            if (rigidbody.velocity.y < -0.01f) {
                inAir = true;
            }
            SetVelocityX(forwardVelocity);
            HandleInput();
            if (rigidbody.velocity.y < -50f) {
                Die();
            }
            GameCamera.Instance.UpdateForPlayer(this);
            lastActiveChunkTime = activeChunkTime;
            activeChunkTime += Time.deltaTime;
        }
        else {
            SetVelocityX(0f);
            SetVelocityY(0f);
        }
    }

    public void HandleInput() {
        if(GameStateHolder.Instance.State == GameState.MENU) {
            HandleAIInput();
        }
        else {
            HandlePlayerInput();
        }
    }

    public void HandleAIInput() {
        if(activeChunk != null) {
            foreach (float t in activeChunk.Simulation.JumpTimes) {
                if (t > lastActiveChunkTime && t < activeChunkTime) {
                    OnSpace();
                }
            }
        }
    }

    public void HandlePlayerInput() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            OnSpace();
        }
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        if (collision.enabled) {
            Collider2D collider2D = collision.collider;
            if (collider2D.GetComponent<Solid>() != null) {
                inAir = false;
                canDoubleJump = true;
                mesh.StopRotation();
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collider2D) {
        if (collider2D.GetComponent<Bomb>() != null) {
            if (GameStateHolder.Instance.State == GameState.LOADING) {
                Destroy(collider2D.gameObject);
            }
            else {
                Die();
            }
        }
    }

    public void SetActiveChunk(Chunk chunk) {
        activeChunk = chunk;
        activeChunkTime = 0f;
        lastActiveChunkTime = 0f;
    }

    private void Die() {
        mesh.Die();
        alive = false;
        LeanTween.delayedCall(0.5f, () => {
            GameStateHolder.Instance.State = GameState.GAME_OVER;
        });
    }

    public void OnSpace() {
        if (!inAir) {
            Jump();
        }
        else if (canDoubleJump) {
            Jump();
            canDoubleJump = false;
        }
    }

    public void SetVelocityX(float v) {
        Vector2 velocityVector = rigidbody.velocity;
        velocityVector.x = v;
        rigidbody.velocity = velocityVector;
    }

    public void SetVelocityY(float v) {
        Vector2 velocityVector = rigidbody.velocity;
        velocityVector.y = v;
        rigidbody.velocity = velocityVector;
    }

    public void AddVelocityY(float v) {
        Vector2 velocityVector = rigidbody.velocity;
        velocityVector.y += v;
        rigidbody.velocity = velocityVector;
    }

    private void Jump() {
        mesh.StopRotation();
        SetVelocityY(jumpForce);
        inAir = true;
        mesh.StartRotation();
    }
}
