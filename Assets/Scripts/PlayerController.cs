using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.XR;
using UnityEngine.VFX;
using UnityEngine.Windows;
using Game.Cheats;
using Zenject;
using Assets.Scripts.Game.Cheats;
using NUnit.Framework.Constraints;
using UnityEditor.ShortcutManagement;
using Game.Levels.Enemies;
using Game.Levels.Obstacles;
using Cysharp.Threading.Tasks.Triggers;
using Assets.Scripts.Game;

[SelectionBase]
[RequireComponent(typeof(CharacterController2D))]

public class PlayerController : MonoBehaviour, ICheatable, IHatWearing, IBowWearing
{
    [Inject] private readonly CheatManager cheatManager;
    [Inject] private readonly CameraController cameraController;

    [Inject] SFXManager sfxManager;
    [Inject] HealthManager healthManager;
    [SerializeField] Projectile[] projectilePrefab;
    [SerializeField] Transform launchOffsetLeft;
    [SerializeField] Transform launchOffsetRight;
    [SerializeField] int loadLevelID;

    [Header("Jump")]
    [SerializeField] public AdjustableNumber jumpCountMax = new(1f);
    [SerializeField] public AdjustableNumber jumpHeight = new(2f);
    [SerializeField] public AdjustableNumber wallJumpHeightMultiplier = new(1f);
    [SerializeField] public AdjustableNumber jumpHangTime = new(2f);
    [SerializeField] public AdjustableNumber gravityDownwardsMultiplier = new(1f);
    [SerializeField] public AdjustableNumber variableJumpMultiplier = new(0.5f);
    [SerializeField] public AdjustableNumber coyoteTime = new(0.1f);
    [SerializeField] public AdjustableNumber horizontalJumpDist = new(5f);
    [SerializeField] public AdjustableNumber horizontalWallOffsetDist = new(5f);

    [Header("Ground Movement")]
    [SerializeField] public AdjustableNumber moveSpeed = new(2f);
    [SerializeField] public AdjustableNumber dashSpeedMultiplier = new(3f);
    [SerializeField] public AdjustableNumber dashSpeed = new(6f);
    [SerializeField] public AdjustableNumber acceleration = new(2f);
    [SerializeField] public AdjustableNumber deceleration = new(2f);
    [SerializeField] public AdjustableNumber knockbackDist = new(5f);
    [SerializeField] public AdjustableNumber dashDist = new(100f);
    [SerializeField] public AdjustableBoolean dashEnabled = new(false);
	[SerializeField] public AdjustableBoolean wallJumpEnabled = new(false); // TODO: Implement

	[Header("Wall Slide")]
    [SerializeField] public AdjustableNumber wallSlideGravityMultiplier = new(0.5f);
    [SerializeField] public AdjustableNumber wallSlideMaxTime = new(2f);
    [SerializeField] public AdjustableNumber wallJumpCoyoteTime = new(0.1f);

    [Header("Player Properties")]
    [SerializeField] public AdjustableNumber iframeSeconds = new(1.0f);
    [SerializeField] public AdjustableBoolean invincible = new(false);
    public bool controlEnabled = true;

    [Header("Player Attacking")]
    [SerializeField] public AdjustableNumber projectileSpeed = new(30f);
    [SerializeField] public AdjustableNumber timeBetweenProjectile = new(0.5f);
    [SerializeField] public AdjustableNumber timeSwordHitbox = new(0.5f);
    [SerializeField] private bool projectileOut = false;

    [Header("Visual Effects")]
    [SerializeField] public AdjustableBoolean enableBow = new(false); // TODO
    [SerializeField] public AdjustableBoolean enableHat = new(false); // TODO

    [SerializeField] private GameObject bowGameObject;
    [SerializeField] private GameObject hatGameObject;

	AdjustableBoolean IBowWearing.wearingBow => enableBow;
	AdjustableBoolean IHatWearing.wearingHat => enableHat;

    //Components
    private CharacterController2D characterController;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] public Animator anim;
    //[SerializeField] private GhostTrail ghostTrail;
    //private PlatformMovement pM;

    //Arm Child
    [SerializeField] ArmGetAnim arm;

    //movement
    private Vector2 velocity = Vector2.zero;

    //inputs
    private Vector2 moveInput = Vector2.zero;
    private bool holdingJump = false;
    public bool dash = false;
    public bool attacking = false;

    //timers
    private float wallSlideTimeCurrent = 0f;
    private float wallJumpCoyoteTimeCurrent = 0f;
    private float coyoteTimeCurrent = 0f;
    private float jumpBufferTimeCurrent = 0f;

    //Wallslide/jump
    private int currentJumpCount = 0;
    private float lastWallSlideInputDirection = 0f;

    // Convenience properties
    private float jumpForce => Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(jumpGravity));
    private float jumpGravity => jumpHeight / (2f * Mathf.Pow(jumpHangTime / 4, 2));
    private bool isOnWall => characterController.collisionInfo.onWall;
    private bool isOnGround => characterController.collisionInfo.onGround;
    private bool isOnCeiling => characterController.collisionInfo.onCeiling;
    private bool isWallSliding => velocity.y <= 0.0f && !isOnGround && isOnWall && wallSlideTimeCurrent > 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cheatManager.Register(this);
        characterController = GetComponent<CharacterController2D>();
        //ghostTrail = GetComponent<GhostTrail>();
        dashSpeed.Value = moveSpeed * dashSpeedMultiplier;
    }

    private void Update()
    {
        //if (hatGameObject) hatGameObject.SetActive(enableHat);
        //if (bowGameObject) bowGameObject.SetActive(enableBow);
        coyoteTimeCurrent = Mathf.Max(0.0f, coyoteTimeCurrent - Time.deltaTime);
        wallJumpCoyoteTimeCurrent = Mathf.Max(0.0f, wallJumpCoyoteTimeCurrent - Time.deltaTime);
        jumpBufferTimeCurrent = Mathf.Max(0.0f, jumpBufferTimeCurrent - Time.deltaTime);

        if (isWallSliding)
        {
            wallSlideTimeCurrent = Mathf.Max(0.0f, wallSlideTimeCurrent - Time.deltaTime);
        }

        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        if (characterController == null) return;

        var wasOnGroundBefore = isOnGround;
        var wasWallSlidingBefore = isWallSliding;

        ProcessMoveInput();
        ApplyGravity();

        characterController.Move(velocity * Time.deltaTime);

        if (isOnGround || isOnCeiling)
        {
            velocity.y = 0f;
        }

        if (isOnGround)
        {
            wallSlideTimeCurrent = wallSlideMaxTime;
        }

        if (!wasWallSlidingBefore && isWallSliding)
        {
            velocity.y = 0.0f;
            lastWallSlideInputDirection = Mathf.Sign(velocity.x);
        }

        if (wasOnGroundBefore && !isOnGround && velocity.y <= 0.0f)
        {
            coyoteTimeCurrent = coyoteTime;
        }

        if (wasWallSlidingBefore && !isWallSliding && wallSlideTimeCurrent > 0.0f)
        {
            wallJumpCoyoteTimeCurrent = wallJumpCoyoteTime;
        }
    }

	private void OnDestroy()
    {
        cheatManager.Unregister(this);
	}

	private void ProcessMoveInput()
    {
        var rate = moveInput.x != 0f ? acceleration : deceleration;
        var inputX = moveInput.x != 0.0f ? Mathf.Sign(moveInput.x) : 0f;

        if (dash)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, inputX * dashSpeed, rate * Time.deltaTime);
        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, inputX * moveSpeed, rate * Time.deltaTime);
        }
    }

    private void ApplyGravity()
    {
        var gravity = -jumpGravity;

        if (velocity.y < 0 && !isWallSliding)
        {
            gravity *= gravityDownwardsMultiplier;
        }
        else if (velocity.y <= 0.0f && isWallSliding)
        {
            gravity *= wallSlideGravityMultiplier;
        }

        velocity.y += gravity * Time.deltaTime;
    }

    private void UpdateAnimations()
    {
        if (anim == null) return;
        arm.SetGround(isOnGround);
        arm.SetIdle(velocity.x != 0 && isOnGround);
        anim.SetBool("IsGrounded", isOnGround);
        anim.SetBool("IsMoving", velocity.x != 0 && isOnGround);
        anim.SetBool("Jump", velocity.y > 0 && !isOnGround);
        //anim.SetBool("IsFalling", velocity.y <= 0 && !isOnGround);// && !isWallSliding);
        //anim.SetBool("IsRising", velocity.y >= 0 && !isOnGround);// && !isWallSliding);
        anim.SetBool("IsWallSliding", isWallSliding);
        anim.SetBool("IsDash", dash);

        if (spriteRenderer == null) return;

        if (velocity.x != 0f)
        {
            spriteRenderer.flipX = velocity.x < 0;
        }
        arm.ChangeOrientation(spriteRenderer.flipX);
    }

    public void OnJumpPressed(InputAction.CallbackContext context)
    {
        if (cheatManager.IsCheating)
        {
            holdingJump = false;
            return;
        }
        bool groundedJumpCondition = isOnGround || coyoteTimeCurrent > 0.0f || isWallSliding || wallJumpCoyoteTimeCurrent > 0.0f;
        bool airborneJumpCondition = !holdingJump && currentJumpCount < jumpCountMax.Floor();
		if (context.performed && (groundedJumpCondition || airborneJumpCondition))
        {
			if (sfxManager) sfxManager.PlaySFX("Jump", 0);
            coyoteTimeCurrent = 0.0f;
            if (isWallSliding || wallJumpCoyoteTimeCurrent > 0.0f)
            {
                velocity.x = lastWallSlideInputDirection * -horizontalWallOffsetDist * horizontalJumpDist;
                velocity.y = (jumpForce + Mathf.Abs(jumpGravity * Time.deltaTime)) * wallJumpHeightMultiplier;
                wallSlideTimeCurrent = wallSlideMaxTime;
            }
            else
            {
                velocity.y = jumpForce + Mathf.Abs(jumpGravity * Time.deltaTime);
            }
            holdingJump = true;
            if (!groundedJumpCondition && airborneJumpCondition)
            {
                currentJumpCount++;
            }
            else
            {
                currentJumpCount = 1;
			}
		}

        if (context.canceled && holdingJump && velocity.y > 0.0f)
        {
            holdingJump = false;
            velocity.y *= variableJumpMultiplier;
        }
    }

    public void OnAttackPressed(InputAction.CallbackContext context)
    {
        if (cheatManager.IsCheating)
        {
            return;
        }
		if (context.performed)//&& (isOnGround || coyoteTimeCurrent > 0.0f || isWallSliding || wallJumpCoyoteTimeCurrent > 0.0f))
        {
            StartCoroutine("PlayerAttack");
        }
    }

    public void PlayerKnockedback()
    {
        //sfxManager.PlaySFX("Jump", 0);
        if (spriteRenderer.flipX)
        {
            velocity.x = -knockbackDist;
        }
        else
        {
            velocity.x = knockbackDist;
        }
        velocity.y = (jumpForce + Mathf.Abs(jumpGravity * Time.deltaTime)) * 0.55f;
    }

    public void PlayerDash(InputAction.CallbackContext context)
    {
        if (cheatManager.IsCheating)
        {
            return;
        }
		//sfxManager.PlaySFX("Jump", 0);
		if (context.performed && !dash && (isOnGround || isOnWall))
        {
            StartCoroutine("PlayerSetDash");
        }
    }

    public IEnumerator PlayerSetDash()
    {
        if (!dashEnabled)
        {
            yield break;
        }
        //sfxManager.PlaySFX("Jump", 0);
        dash = true;
        //ghostTrail.enabled = true;
        yield return new WaitForSeconds(0.5f);
        if (isOnGround)
        {
            dash = false;
            //ghostTrail.enabled = false;
        }
        else
        {
            while (!isOnGround)
            {
                yield return null;
            }
            dash = false;
            //ghostTrail.enabled = false;
        }
    }

    public IEnumerator PlayerAttack()
    {
        //sfxManager.PlaySFX("Jump", 0);
        attacking = true;
        anim.SetBool("IsShooting", true);
        arm.SetShoot(true);
        //hitbox becomes active
        StartCoroutine("CheckForProjectile");
        yield return new WaitForSeconds(timeSwordHitbox);
        anim.SetBool("IsShooting", false);
        arm.SetShoot(false);
        attacking = false;
        //hitbox dies
    }

    public IEnumerator DamagedCoroutine(float waitTime)
    {
        anim.SetBool("Damaged", true);
        var playerController = GetComponent<PlayerController>();
        playerController.PlayerKnockedback();
		if (sfxManager) sfxManager.PlaySFX("KuhoDamage", 0);
        yield return new WaitForSeconds(1);
        anim.SetBool("Damaged", false);
    }
    public IEnumerator iframeCoroutine()
    {
        //invincible = true;
        Physics2D.IgnoreLayerCollision(0, 7, true);
        yield return new WaitForSeconds(iframeSeconds);
        Physics2D.IgnoreLayerCollision(0, 7, false);
        //invincible = false;
    }

    public IEnumerator EnableProjectileCoroutine(float waittime)
    {
        yield return new WaitForSeconds(waittime);
        projectileOut = false;
    }

    public IEnumerator LoadSceneAsync()
    {

        //overlayImage.DOFade(0, 1.5f);
        //GameTimer.StopTimer();
        yield return new WaitForSeconds(0.5f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(loadLevelID);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
    public void CheckForProjectile()
    {
        //if (!projectileOut)
        //{
            int randomProjectile = Random.Range(0, projectilePrefab.Length);
            if (!spriteRenderer.flipX)
            {
                var obj = Instantiate(projectilePrefab[randomProjectile], launchOffsetRight.position, transform.rotation);
                var rbP = obj.GetComponent<Rigidbody2D>();
                rbP.linearVelocity = new Vector2(Mathf.Cos(transform.rotation.z), Mathf.Sin(transform.rotation.z)) * projectileSpeed;
            }
            else
            {
                var obj = Instantiate(projectilePrefab[randomProjectile], launchOffsetLeft.position, transform.rotation);
                var rbP = obj.GetComponent<Rigidbody2D>();
                rbP.linearVelocity = new Vector2(Mathf.Cos(transform.rotation.z + 180), Mathf.Sin(transform.rotation.z)) * projectileSpeed;
            }
            if (sfxManager) sfxManager.PlaySFX("kuhoProjectileSFX", randomProjectile);
            projectileOut = true;
            StartCoroutine(EnableProjectileCoroutine(timeBetweenProjectile));
        //}
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        if (cheatManager.IsCheating)
        {
            moveInput = Vector2.zero;
            return;
        }
		moveInput = context.ReadValue<Vector2>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
		CheckForDamageDealers(collision.gameObject);
        if (collision.transform.CompareTag("DeathZone"))
        {
			if (sfxManager) sfxManager.PlaySFX("GameOver", 0);
            //uiController.ShowGameOverScreen();
        }
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
		CheckForDamageDealers(collision.gameObject);
        
        var cameraLimiter = collision.gameObject.GetComponent<CameraControllerLimiter>();
        if (cameraLimiter != null)
        {
            cameraController.EnterLimiter(cameraLimiter);
        }

		/*if (collision.CompareTag("Collectable"))
        {
            collectedCoins++;
            sfxManager.PlaySFX("Coin");
            Destroy(collision.gameObject);
        }*/

		//if (collision.CompareTag("NextLevel"))
		//{
		//    StartCoroutine("LoadSceneAsync");
		//}

		/*if (collision.CompareTag("Shield"))
        {
            if (hasShield == false)
            {
                sfxManager.PlaySFX("Shield");
                lastCreatedShield = Instantiate(Shield, transform.position, Quaternion.identity);
                Destroy(collision.gameObject);
                hasShield = true;
            }
        }*/
	}

	private void OnTriggerExit2D(Collider2D collision) {
		var cameraLimiter = collision.gameObject.GetComponent<CameraControllerLimiter>();
		if (cameraLimiter != null) {
			cameraController.ExitLimiter(cameraLimiter);
		}
	}

	private void CheckForDamageDealers(GameObject target) {
		if (invincible) {
			return;
		}
		bool damaged = false;
		float damage = 0f;
		if (target.transform.CompareTag("Enemy")) {
			var enemy = target.GetComponent<Enemy>();
			if (enemy) {
				damage += enemy.GetContactDamage(this, healthManager.healthAmount, out bool hit);
				damaged = damaged || hit;
			}
		}
		if (target.transform.CompareTag("Obstacle")) {
			var obstacle = target.GetComponent<Obstacle>();
			if (obstacle) {
				damage += obstacle.GetContactDamage(this, healthManager.healthAmount, out bool hit);
				damaged = damaged || hit;
			}
		}
		if (target.transform.CompareTag("EnemyProjectile")) {
			damage += 25;
			damaged = true;
		}
		if (!damaged) {
			return;
		}
		DealDamage(damage);
	}

	public void DealDamage(float damage) {
		healthManager.TakeDamage(damage);
		if (healthManager.healthAmount > 0) {
			StartCoroutine("DamagedCoroutine", 1);
			StartCoroutine("iframeCoroutine");
		} else {
			if (sfxManager) sfxManager.PlaySFX("GameOver", 0);
			//uiController.ShowGameOverScreen();
		}
	}

    public void GiveHealing(float healing) {
        healthManager.GiveHealing(healing);
    }
}
