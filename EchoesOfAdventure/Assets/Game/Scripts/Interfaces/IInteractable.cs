using Interactable;
using Player;

namespace Interface
{
    public interface IInteractable
    {
        public InteractableType GetInteractableType();
        public bool IsInteractionAllowed(CharacterType type);

        public void StartInteracting();
        public void StopInteracting();
    }

    public interface IFireJumpable
    {
        public void SetFireJumpValue(bool isInFire);
    }
}

