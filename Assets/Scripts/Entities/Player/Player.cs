using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Player : MonoBehaviour {

    public float forwardVelocity = 10f;
    public float jumpForce = 30f;
    public float maxFallVelocity = 10f;
    public bool saveSkillChanges;
    public PlaySpeed playSpeed = PlaySpeed.REAL_TIME;
    public Action OnDeath;
    

    public Action<Collider2D> OnTriggerEnter;
    public Action<Chunk> OnActiveChunkChange;

    public Chunk ActiveChunk { get; set; }
    public PlayerSkill Skill { get; set; }
    public int Score { get; set; } = 0;

    private new Rigidbody2D rigidbody;

    private bool InAir { 
        get {
            return _inAir;
        }
        set {
            if (value && !_inAir) {
                mesh.StopRotation();
                mesh.StartRotation();
            }
            _inAir = value;
        }
    }
    private bool _inAir = false;

    private bool canDoubleJump = true;
    private bool alive = true;

    private PlayerMesh mesh;
    private IEnumerator simulationCoroutine;

    public void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        mesh = GetComponentInChildren<PlayerMesh>();
        simulationCoroutine = SimulationCoroutine();
    }

    // Start is called before the first frame update
    public void Start() {
        if(!IsGeneratingChunk()) {
            GameCamera.Instance.CenterOnPlayer(this);
        }
        Physics2D.simulationMode = SimulationMode2D.Script;

        if(Skill == null) {
            Skill = new PlayerSkill(PlayerProfile.Instance.PlayerSkill);
        }

        if (playSpeed == PlaySpeed.FASTEST) {
            StartCoroutine(simulationCoroutine);
        }
    }

    public void OnDestroy() {
        StopCoroutine(simulationCoroutine);
    }

    // Update is called once per frame
    public void Update() {
        if (alive) {
            SetVelocityX(forwardVelocity);
            if (rigidbody.velocity.y < -maxFallVelocity) {
                SetVelocityY(-maxFallVelocity);
            }
            if (Mathf.Abs(rigidbody.velocity.y) > 0.1f) {
                InAir = true;
            }
            SetVelocityX(forwardVelocity);
            CheckFallDeath();
            if (!IsGeneratingChunk()) {
                GameCamera.Instance.UpdateForPlayer(this);
            }
        }
        else {
            SetVelocityX(0f);
            SetVelocityY(0f);
        }
    }

    private void CheckFallDeath() {
        if (rigidbody.velocity.y < -50f) {
            Difficulty cause = Difficulty.Initial(0f);
            cause.Set(DifficultyType.PLATFORM_LENGTH, 1f);
            if (!canDoubleJump) {
                cause.Set(DifficultyType.DOUBLE_JUMP, 1f);
            }
            Die(cause);
        }
    }

    public void FixedUpdate() {
        if(playSpeed == PlaySpeed.REAL_TIME && !IsGeneratingChunk()) {
            PhysicsScene2D physicsScene2D = gameObject.scene.GetPhysicsScene2D();
            physicsScene2D.Simulate(Time.fixedDeltaTime);
        }
    }

    private IEnumerator SimulationCoroutine() {
        while(true) {
            PhysicsScene2D physicsScene2D = gameObject.scene.GetPhysicsScene2D();
            physicsScene2D.Simulate(ChunkGenerator.Instance.deltaTime);
            yield return null;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        if (collision.enabled) {
            Collider2D collider2D = collision.collider;
            if (collider2D.GetComponent<Solid>() != null) {
                InAir = false;
                canDoubleJump = true;
                mesh.StopRotation();
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collider2D) {
        OnTriggerEnter?.Invoke(collider2D);
        if (!IsGeneratingChunk()) {
            if(collider2D.GetComponent<Hazard>() != null) {
                Difficulty cause = Difficulty.Initial(0f);
                cause.Set(DifficultyType.BOMB_DENSITY, 1f);
                cause.Set(DifficultyType.BOMB_DENSITY, 1f);
                Die(cause);
            }
        }
    }

    public void SetActiveChunk(Chunk chunk) {
        ActiveChunk = chunk;
        OnActiveChunkChange?.Invoke(chunk);
    }

    private void Die(Difficulty cause) {
        if(alive && !IsGeneratingChunk()) {
            mesh.Die();
            alive = false;
            float delay = playSpeed == PlaySpeed.REAL_TIME ? 0.5f : 0;
            LeanTween.delayedCall(delay, () => {
                Skill.DecreaseTo(ActiveChunk.Difficulty, cause);
                if (saveSkillChanges) {
                    Skill.Save();
                }
                OnDeath?.Invoke();
            });
        }
    }

    public void OnSpace() {
        if (!InAir) {
            Jump();
        } else if (canDoubleJump) {
            Jump();
            canDoubleJump = false;
        }
    }

    private bool IsGeneratingChunk() {
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
        InAir = true;
        SetVelocityY(jumpForce);
    }
}
