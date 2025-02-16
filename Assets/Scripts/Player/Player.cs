using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private PlayerAim playerAim;
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private float moveSpeed, maxHealth, damage, attackTime, projectileSpeed;

    private Rigidbody2D rb;
    private PlayerControls controls;
    private float currentHealth;
    private float currentFireTimer;

    private Vector2 moveDirection, lookPosition;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start() {
        controls = GetComponent<PlayerControls>();
        currentHealth = maxHealth;
        currentFireTimer = 0f;
    }

    private void Update() {
        moveDirection = controls.MoveDirection;
        lookPosition = controls.LookPosition;

        playerAim.UpdateAim(lookPosition);
    }

    private void FixedUpdate() {
        rb.velocity = new Vector2(
            moveDirection.x * moveSpeed * Time.deltaTime, 
            moveDirection.y * moveSpeed * Time.deltaTime);

        currentFireTimer += Time.deltaTime;
    }

    public void Fire() {
        if(currentFireTimer >= attackTime) {
            // Reset timer
            currentFireTimer = 0f;

            Vector3 projectilePosition = transform.position + playerAim.Direction;
            GameObject projectile = Instantiate(projectilePrefab, projectilePosition, Quaternion.identity, GameManager.instance.projectilesParent);
            // Give the projectile damage
            projectile.GetComponent<Projectile>().damage = damage;
            // Apply a force to the projectile (in the direction it was fired)
            Vector2 projectileForce = playerAim.Direction;
            projectileForce.Normalize();
            projectileForce *= projectileSpeed;
            projectile.GetComponent<Rigidbody2D>().AddForce(projectileForce);
        }
    }

    public void TakeDamage(float amount) {
        if(amount < 0) {
            return;
        }

        currentHealth -= amount;

        if(currentHealth < 0) {
            Debug.Log("Player Killed. Game Over!");
            Destroy(gameObject);
        }
    }

    public void Heal() {
        currentHealth = maxHealth;
    }
}
