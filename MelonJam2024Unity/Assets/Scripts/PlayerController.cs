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

    public List<Bullet> Scrap = new List<Bullet>(); 

    [SerializeField] 
    private float speed = 10f;
    public float Speed { get { return speed; } set { speed = value; } }

    [SerializeField] 
    private Animator _animator;

    private Vector2 _movementDirection;
    
    private Vector3 _targetPosition;
    private Direction _direction;
    private static readonly int AnimDirection = Animator.StringToHash("Direction");
    private int layer;

    private bool isMoving = false;

    private void Awake()
    {
        layer = LayerMask.GetMask("Interactable");
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
            RaycastHit2D hit = Physics2D.Raycast(transform.position, DirectionToDir(), 1f, layer);
            

            if (hit.collider) 
            {
                if (hit.collider.gameObject.TryGetComponent(out ScrapPile scrapPile) && Scrap.Count < MaxCarringCapacity) 
                {
                    scrapPile.DealDmg(harvestDamage);
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
            if (isMoving)
            {
                isMoving = false;
                MusicSoundManagement.Instance.StopSFXLoop(MusicSoundManagement.AUDIOTYPE.FOOT_STEP);
            }
            _animator.SetInteger(AnimDirection, (int) _direction + 10);
            //TODO STOP SOUND
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
            if (!isMoving)
            {
                isMoving = true;
                MusicSoundManagement.Instance.PlaySfx(MusicSoundManagement.AUDIOTYPE.FOOT_STEP,true);
            }
        }
        else
        {
            _animator.SetInteger(AnimDirection, (int) _direction + 10); 
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
            RaycastHit2D hit = Physics2D.CircleCast(transform.position,0.6f,Vector2.one,0, layer);

            if (hit.collider)
            {
                if (hit.collider.gameObject.TryGetComponent(out InteractableButton button))
                {
                    button.InvokeButton(gameObject);
                }
            }

        }
    }

    public bool CanTakeMoreLoot() 
    {
        return Scrap.Count + 1 <= MaxCarringCapacity;
    }

    public void GiveLoot(Bullet newScrap) 
    {
        Scrap.Add(newScrap);
        newScrap.transform.SetParent(transform);
        newScrap.transform.localPosition = new Vector3(0,Scrap.Count,0);
    }

    public List<Bullet> TryGetScrap(int amount)
    {
        List<Bullet> returnList = new List<Bullet>();

        if (amount < Scrap.Count)
        {
            returnList = Scrap.GetRange(0, amount);
            Scrap.RemoveRange(0, amount);
        }
        else
        {
            returnList = Scrap.GetRange(0, Scrap.Count);
            Scrap.Clear();
        }

        return returnList;
    }

    public enum Direction
    {
        NORTH, EAST, SOUTH, WEST
    }
}
