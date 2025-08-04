using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float _transitionTime = 1f;
    
    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void LoadLevel()
    {
        StartCoroutine(LoadLevelCoroutine());
    }
    
    private IEnumerator LoadLevelCoroutine()
    {
        _animator.SetTrigger("Start");
        yield return new WaitForSeconds(_transitionTime);
    }
}
