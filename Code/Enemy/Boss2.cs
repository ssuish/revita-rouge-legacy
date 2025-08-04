using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnemyAnimationController : MonoBehaviour
{
    private Animator _animator;
    private bool _isPlayerDetected;
    private Vector3 _previousPosition;
    private Vector3 _currentPosition;
    [SerializeField] private GameObject trappedSound;
    [SerializeField] private GameObject loopSound;
    [SerializeField] private GameObject itemToDropPrefab;
    private CircleCollider2D _collider;
    private const float StationaryThreshold = 2.0f;
    private float _stationaryTime;
    private Vector3 direction;
    private Transform _dropPoint;
    private bool isItemDropped = false;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _previousPosition = transform.position;
        _collider = GetComponent<CircleCollider2D>();
        //trappedSound.SetActive(false);
        //loopSound.SetActive(false);
        
        // Get the drop point game object in children
        _dropPoint = transform.Find("DropPoint");
    }

    private void OnEnable()
    {
        GameEventsManagerSO.instance.miscEvents.OnGateClosedStopBoss += HandleBossMovementGateClosed;
    }
    
    private void OnDisable()
    {
        GameEventsManagerSO.instance.miscEvents.OnGateClosedStopBoss -= HandleBossMovementGateClosed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _currentPosition = transform.position;
        direction = (_currentPosition - _previousPosition).normalized;
        
        if (direction.magnitude <= 0f)
        {
            _stationaryTime += Time.fixedDeltaTime;
        }
        else
        {
            _stationaryTime = 0f;
        }
        
        HandleMovementAnimations(direction);
        UpdateHeadlightDirection(direction);
        
        _previousPosition = _currentPosition;
    }

    private void HandleMovementAnimations(Vector3 direction)
    {
        // Edit this when waypoint logic is implemented
        float horizontal = direction.x;
        float vertical = direction.y;
        
        if (_stationaryTime >= StationaryThreshold)
        {
            _animator.SetBool("isIdle", true);
            _collider.enabled = true;
            return;
        }
        
        _animator.SetFloat("Horizontal", horizontal);
        _animator.SetFloat("Vertical", vertical);
        _collider.enabled = false;
    }
    
    private void HandleBossMovementGateClosed()
    {
        _animator.SetBool("isIdle", true);
    }
    
  
    private void UpdateHeadlightDirection(Vector3 movement)
    {
        Vector3 lockedYPosition = transform.localPosition;
        Light2D headlight = GetComponentInChildren<Light2D>();

        // Smoothly interpolate the headlight's position
        Vector3 targetPosition = new Vector3(movement.x * 0.2f, -0.2f, lockedYPosition.z);
        headlight.transform.localPosition = Vector3.Lerp(headlight.transform.localPosition, targetPosition, Time.deltaTime * 5f);

        // For 2D rotation, rotate the flashlight to face the movement direction
        float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg - 90f;

        // Smoothly interpolate the headlight's rotation
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        headlight.transform.rotation = Quaternion.Lerp(headlight.transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    private void DropItem()
    {
        // TODO - Implement drop item logic
        if (_isPlayerDetected && _dropPoint != null && isItemDropped == false)
        {
            Instantiate(itemToDropPrefab, _dropPoint.transform.position, Quaternion.identity);
            isItemDropped = true;
        }
        else
        {
            //Debug.Log("Boss2 position not detected. Cannot drop item.");
        }
    }

    private void DialogueTriggered()
    {
        GameEventsManagerSO.instance.miscEvents.DialogueRequested(true, "Defeat");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            
            _isPlayerDetected = true;
            Debug.Log("Player detected");
            
            DialogueTriggered();

            DropItem();
            
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerDetected = false;
        }
    }
}