using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    [SerializeField] private Waypoints waypoints;
    [SerializeField] private float minMoveSpeed = 1f;
    [SerializeField] private float maxMoveSpeed = 5f;
    [SerializeField] private float distanceThreshold = 0.1f;
    
    private Transform _currentWaypoint;
    private Transform _previousWaypoint;
    private bool _isInsideGate = false;
    private float moveSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        // Set random move speed
        moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
        
        // Set initial position to the first waypoint
        _currentWaypoint = waypoints.GetNextWaypoint(_currentWaypoint);
        transform.position = _currentWaypoint.position;
        
        // Set the next waypoint as the target
        _previousWaypoint = _currentWaypoint;
        _currentWaypoint = waypoints.GetNextWaypoint(_currentWaypoint);
        
        StartCoroutine(UpdateMoveSpeed());
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        //if (transform == null || _currentWaypoint == null) return;
        if (transform == null || _isInsideGate) return;
        
        if (_currentWaypoint == null || _currentWaypoint.gameObject == null)
        {
            _currentWaypoint = _previousWaypoint ?? waypoints.GetNextWaypoint(_currentWaypoint);
        }
        
        if (_currentWaypoint == null) return;
        
        transform.position = Vector3.MoveTowards(transform.position, _currentWaypoint.position, moveSpeed * Time.deltaTime);
        //Vector3 targetPosition = Vector3.MoveTowards(transform.position, _currentWaypoint.position, moveSpeed * Time.deltaTime);
        //_rigidbody.MovePosition(targetPosition);
        
        if (Vector3.Distance(transform.position, _currentWaypoint.position) < distanceThreshold)
        {
            _previousWaypoint = _currentWaypoint;
            _currentWaypoint = waypoints.GetNextWaypoint(_currentWaypoint);
        }
    }
    
    private IEnumerator UpdateMoveSpeed()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Gate"))
        {
            Debug.Log("_isInsideGate: " + _isInsideGate);
            _isInsideGate = true;
           // _rigidbody.velocity = Vector3.zero; // Stop the Rigidbody's movement
           // transform.position = transform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Gate"))
        {
            Debug.Log("_isInsideGate: " + _isInsideGate);
            _isInsideGate = false;
        }
    }
}
