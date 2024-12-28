using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [SerializeField] 
    private float speed = 10f;

    [SerializeField] 
    private Animator _animator;

    private Vector2 _movementDirection;
    
    private Vector3 _targetPosition;
    private Direction _direction;
    private static readonly int AnimDirection = Animator.StringToHash("Direction");

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _targetPosition = transform.position;
        _direction = Direction.SOUTH;
    }

    // Update is called once per frame
    void Update()
    {
        if(_movementDirection != Vector2.zero)
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

    public void OnMove(InputAction.CallbackContext callback)
    {
        _movementDirection = callback.action.ReadValue<Vector2>();
    }
    
    public enum Direction
    {
        NORTH, EAST, SOUTH, WEST
    }
}
