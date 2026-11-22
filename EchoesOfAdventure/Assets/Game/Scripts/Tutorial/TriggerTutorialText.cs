using System;
using Interface;
using NaughtyAttributes;
using Player;
using UnityEngine;

public class TriggerTutorialText : MonoBehaviour
{

    [SerializeField] private GameObject _tutorialText;
    [SerializeField] private Transform _tutorialTrigger;
    [SerializeField] private bool _shouldTriggerForOneCharacterOnly;
    [SerializeField, ShowIf(nameof(_shouldTriggerForOneCharacterOnly))] private CharacterMask _chosenCharacterType; 

    private Vector3 _triggerPosition;
    private Vector3 _triggerScale;

    private bool _isActive = false;

    private void Awake()
    {
        _triggerPosition = new Vector3(_tutorialTrigger.position.x, _tutorialTrigger.position.y + _tutorialTrigger.localScale.y / 2);
        _triggerScale = _tutorialTrigger.localScale;
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
            if (!_shouldTriggerForOneCharacterOnly)
            {
                ActivateText();
                return;
            }

            for (int i = 0; i < playerHits.Length; i++)
            {
                if (!playerHits[i].CanGetHurt(_chosenCharacterType))
                    continue;

                ActivateText();
            }
        }
        else
        {
            DeactivateText();
        }
        
    
    }

    private void ActivateText()
    {
        if (!_isActive)
        {
            _tutorialText.SetActive(true);
            _isActive = true;
        }

    }
    
    private void DeactivateText()
    {
        if (_isActive)
        {
            _tutorialText.SetActive(false);
            _isActive = false;
        }
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
            Gizmos.color = new Color(1, 0, 0, 0.3f); // Red with transparency for detection area

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
