using System;
using Events;
using Interface;
using UnityEngine;
using Zenject;

public class TriggerBoxTutorialText : MonoBehaviour
{

    [Inject]private ProjectEvents _projectEvents;
    [SerializeField] private GameObject _tutorialText;
    [SerializeField] private Transform _tutorialTrigger;

    private Vector3 _triggerPosition;
    private Vector3 _triggerScale;

    private bool _canActivate = false;
    private bool _inCollider = false;

    private void Awake()
    {
        _triggerPosition = new Vector3(_tutorialTrigger.position.x, _tutorialTrigger.position.y + _tutorialTrigger.localScale.y / 2);
        _triggerScale = _tutorialTrigger.localScale;
    }

    private void OnEnable()
    {
        _projectEvents.TutorialBoxTurnToStone += CanActivate;
        
    }

    private void OnDisable()
    {
        _projectEvents.TutorialBoxTurnToStone -= CanActivate;
        _canActivate = false;
    }

    private void FixedUpdate()
    {
        CheckForPlayer();
    }

    private void CheckForPlayer()
    {
        // Ensure the _hitbox object is active
        if (_tutorialTrigger == null || !_tutorialTrigger.gameObject.activeInHierarchy)
        {
            return;
        }

        // Use Collider2DHelper to detect the player within the hitbox
        IHurtBox[] playerHits = Collider2DHelper.GetHitArray<IHurtBox>(_triggerPosition, _triggerScale, Constants.Layers.Player);

        if (playerHits.Length > 0)
        {
            ActivateText();
            _inCollider = true;
        }
        else
        {
            _inCollider = false;
        }
    }

    private void ActivateText()
    {
        if (_canActivate)
        {
            _tutorialText.SetActive(true);
        }
    }

    private void CanActivate()
    {
        if (_inCollider) 
            _canActivate = true;
    }
    
    private void OnDrawGizmos()
    {
        // Check if _textTrigger is assigned
        if (_tutorialTrigger != null)
        {
            // Calculate the trigger position and scale
            Vector3 triggerPosition = new Vector3(
                _tutorialTrigger.position.x, 
                _tutorialTrigger.position.y + _tutorialTrigger.localScale.y / 2, 
                _tutorialTrigger.position.z
            );
            Vector3 triggerScale = _tutorialTrigger.localScale;

            // Save the original Gizmos color
            Color originalColor = Gizmos.color;

            // Set the Gizmos color (you can change the color as needed)
            Gizmos.color = new Color(0, 0, 1, 0.3f); // Red with transparency for detection area

            // Draw the detection area as a cube
            Gizmos.DrawCube(triggerPosition, triggerScale);

            // Draw the outline of the cube
            Gizmos.color = Color.red; // Solid red for the outline
            Gizmos.DrawWireCube(triggerPosition, triggerScale);

            // Restore the original Gizmos color
            Gizmos.color = originalColor;
        }
    }
}
