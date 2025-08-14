using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float skinWidth = 1f / 32f;
    [SerializeField] private int verticalRaycasts = 3;
    [SerializeField] private int horizontalRaycasts = 10;

    public CollisionInfo collisionInfo;



    private BoxCollider2D boxCollider;
    private Rigidbody2D rb;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }
    public void Move(Vector2 velocity)
    {
        collisionInfo.Reset();

        var currentVelocity = velocity;
        if (currentVelocity.x != 0) currentVelocity = PerformHorizontalCollisions(currentVelocity);
        if (currentVelocity.y != 0) currentVelocity = PerformVerticalCollisions(currentVelocity);

        rb.MovePosition(rb.position + currentVelocity);
    }

    private Vector2 PerformHorizontalCollisions(Vector2 velocity)
    {
        var currentVelocity = velocity;
        var horizontalRaySpacing = (boxCollider.bounds.size.y - (skinWidth * 2)) / (float)(horizontalRaycasts - 1);
        var rayLength = Mathf.Abs(velocity.x) + skinWidth;
        var direction = new Vector2(Mathf.Sign(currentVelocity.x), 0);
        var originx = velocity.x < 0 ? boxCollider.bounds.min.x + skinWidth : boxCollider.bounds.max.x - skinWidth;

        for (int i = 0; i < horizontalRaycasts; i++)
        {
            var originy = boxCollider.bounds.min.y + skinWidth + (i * horizontalRaySpacing);
            Debug.DrawRay(new Vector2(originx, originy), direction * rayLength, Color.red);

            var result = Physics2D.Raycast(new Vector2(originx, originy), direction, rayLength, boxCollider.includeLayers);
            if (result)
            {
                currentVelocity.x = direction.x * (result.distance - skinWidth);
                rayLength = result.distance;

                collisionInfo.onWall = true;
            }
        }

        return currentVelocity;
    }
    private Vector2 PerformVerticalCollisions(Vector2 velocity)
    {
        var currentVelocity = velocity;
        var verticalRaySpacing = (boxCollider.bounds.size.x - (skinWidth * 2)) / (float)(verticalRaycasts - 1);
        var rayLength = Mathf.Abs(velocity.y) + skinWidth;
        var direction = new Vector2(0, Mathf.Sign(currentVelocity.y));
        var originy = velocity.y < 0 ? boxCollider.bounds.min.y + skinWidth : boxCollider.bounds.max.y - skinWidth;

        for (int i = 0; i < verticalRaycasts; i++)
        {
            var originx = boxCollider.bounds.min.x + skinWidth + (i * verticalRaySpacing);
            Debug.DrawRay(new Vector2(originx, originy), direction * rayLength, Color.red);

            var result = Physics2D.Raycast(new Vector2(originx, originy), direction, rayLength, boxCollider.includeLayers);
            if (result)
            {
                currentVelocity.y = direction.y * (result.distance - skinWidth);
                rayLength = result.distance;

                collisionInfo.onGround = direction.y < 0;
                collisionInfo.onCeiling = direction.y > 0;
            }
        }

        return currentVelocity;
    }
    public struct CollisionInfo
    {
        public bool onGround;
        public bool onCeiling;
        public bool onWall;

        public void Reset()
        {
            onGround = false;
            onCeiling = false;
            onWall = false;
        }
    }
}
