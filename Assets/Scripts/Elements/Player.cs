using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Player : MonoBehaviour {

    public float forwardVelocity = 10f;
    public float jumpForce = 30f;
    public float maxFallVelocity = 10f;
    private new Rigidbody2D rigidbody;
    private bool inAir = false;
    private bool canDoubleJump = true;
    private bool alive = true;

    public Chunk ActiveChunk { get; set; }
    private float lastChunkX = 0f;

    private PlayerMesh mesh;

    public void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        mesh = GetComponentInChildren<PlayerMesh>();
    }

    // Start is called before the first frame update
    public void Start() {
        if(!IsInSimulation()) {
            GameCamera.Instance.CenterOnPlayer(this);
        }
        Physics2D.simulationMode = SimulationMode2D.Script;
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
            if (rigidbody.velocity.y < -50f) {
                Difficulty cause = Difficulty.Initial(0f);
                cause.Set(DifficultyType.PLATFORM_SIZE, 1f);
                if(!canDoubleJump) {
                    cause.Set(DifficultyType.DOUBLE_JUMP, 1f);
                }
                Die(cause);
            }
            if(!IsInSimulation()) {
                GameCamera.Instance.UpdateForPlayer(this);
                HandleInput();
            }
            lastChunkX = GetXInActiveChunk();
        }
        else {
            SetVelocityX(0f);
            SetVelocityY(0f);
        }
    }

    public void FixedUpdate() {
        if(!IsInSimulation()) {
            PhysicsScene2D physicsScene2D = gameObject.scene.GetPhysicsScene2D();
            physicsScene2D.Simulate(Time.fixedDeltaTime);
        }
    }

    private void HandleInput() {
        if(GameManager.Instance.State == GameState.MENU) {
            HandleAIInput();
        }
        else {
            HandlePlayerInput();
        }
    }

    public void HandleAIInput() {
        if(ActiveChunk != null) {
            foreach (float x in ActiveChunk.Simulation.JumpPositions) {
                if (x > lastChunkX && x <= GetXInActiveChunk()) {
                    OnSpace();
                }
            }
        }
    }

    public float GetXInActiveChunk() {
        if (ActiveChunk == null) {
            return 0f;
        }
        return (transform.position - ActiveChunk.transform.position).x;
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
            if (IsInSimulation()) {
                //Destroy(collider2D.gameObject);
                Vector3 step = collider2D.transform.localPosition - transform.localPosition;
                ActiveChunk.Grid.MoveToEmpty(collider2D.gameObject, step);
            }
            else {
                Difficulty cause = Difficulty.Initial(0f);
                cause.Set(DifficultyType.BOMB_DENSITY, 1f);
                cause.Set(DifficultyType.BOMB_DENSITY, 1f);
                Die(cause);
            }
        }
    }

    public void SetActiveChunk(Chunk chunk) {
        ActiveChunk = chunk;
        lastChunkX = 0f;
    }

    private void Die(Difficulty cause) {
        if(!IsInSimulation() && GameManager.Instance.State == GameState.PLAY) {
            mesh.Die();
            alive = false;
            if (ActiveChunk != null) {
                PlayerProfile.Instance.PlayerSkill.DecreaseTo(ActiveChunk.Difficulty, cause);
            }
            LeanTween.delayedCall(0.5f, () => {
                GameManager.Instance.State = GameState.GAME_OVER;
            });
        }
    }

    public void OnSpace() {
        if (GameManager.Instance.State == GameState.MENU) {
            Jump();
        }
        else {
            if (!inAir) {
                Jump();
            } else if (canDoubleJump) {
                Jump();
                canDoubleJump = false;
            }
        }
    }

    private bool IsInSimulation() {
        return transform.root.GetComponent<ChunkLibrary>() != null;
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
