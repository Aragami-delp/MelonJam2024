using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Player; 

    [SerializeField]
    private bool interacting;

    public float harvestCooldown { get; set; } = 1.5f; 
    private float cooldownPassed;


    public int harvestDamage { get; set; } = 10;

    public int MaxCarringCapacity { get; set; } = 5;

    List<Transform> Scrap = new List<Transform>(); 

    [SerializeField] 
    private float speed = 10f;
    public float Speed { get { return speed; } set { speed = value; } }

    [SerializeField] 
    private Animator _animator;

    private Vector2 _movementDirection;
    
    private Vector3 _targetPosition;
    private Direction _direction;
    private static readonly int AnimDirection = Animator.StringToHash("Direction");

    private void Awake()
    {
        Player = this;
    }

    void Start()
    {  
        _targetPosition = transform.position;
        _direction = Direction.SOUTH;
    }

    // Update is called once per frame
    void Update()
    {
        // harvest cooldown time
        cooldownPassed += Time.deltaTime;
        if (harvestCooldown <= cooldownPassed && interacting) 
        {
            cooldownPassed = 0; 
            RaycastHit2D hit = Physics2D.Raycast(transform.position, DirectionToDir());
            

            if (hit.collider) 
            {
                if (hit.collider.gameObject.TryGetComponent(out ScrapPile scrapPile) && Scrap.Count < MaxCarringCapacity) 
                {
                    scrapPile.DealDmg(harvestDamage);
                }
                else if(hit.collider.gameObject.TryGetComponent(out InteractableButton button))
                {
                    button.InvokeButton();
                }
            }
        }

        if (_movementDirection != Vector2.zero)
        {
            if (_targetPosition == transform.position)
            {
                if(Mathf.Abs(_movementDirection.x) > Mathf.Abs(_movementDirection.y))
                {

                    if(_movementDirection.x > 0)
                    {
                        _direction = Direction.EAST;
                        NewDirection(Vector3.right);

                    }
                    else
                    {
                        _direction = Direction.WEST;
                        NewDirection(Vector3.left);
                    }
                }
                else
                {
                    if (_movementDirection.y > 0)
                    {
                        _direction = Direction.NORTH;
                        NewDirection(Vector3.up);
                    }
                    else
                    {
                        _direction = Direction.SOUTH;
                        NewDirection(Vector3.down);
                    }
                }
            }
        } 
        else
        {
            _animator.SetInteger(AnimDirection, (int) _direction + 10);
        }
        
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, speed * Time.deltaTime);
    }

    private void NewDirection(Vector3 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction);

        if (!hit || hit.distance > 1)
        {
            _targetPosition += direction;
            _animator.SetInteger(AnimDirection, (int) _direction); 
        }
    }

    private Vector2 DirectionToDir() 
    {
        switch (_direction) 
        {
            case Direction.NORTH:
                return Vector2.up;

            case Direction.EAST:
                return Vector2.right;

            case Direction.SOUTH:
                return Vector2.down;

            case Direction.WEST:
                return Vector2.left;
        }

        return Vector2.up;
    }

    public void OnMove(InputAction.CallbackContext callback)
    {
        _movementDirection = callback.action.ReadValue<Vector2>();
    }

    public void OnInteractPressed(InputAction.CallbackContext callback)
    {
        interacting = !callback.canceled;

        if (callback.started) 
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, DirectionToDir());
            
            if (hit.collider.gameObject.TryGetComponent(out InteractableButton button))
            {
                button.InvokeButton();
            }

        }
    }

    public bool CanTakeMoreLoot() 
    {
        return Scrap.Count + 1 <= MaxCarringCapacity;
    }

    public void GiveLoot(Transform newScrap) 
    {
        Scrap.Add(newScrap);
        newScrap.SetParent(transform);
        newScrap.localPosition = new Vector3(0,Scrap.Count,0);
    }

    public enum Direction
    {
        NORTH, EAST, SOUTH, WEST
    }
}
