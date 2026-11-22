using NaughtyAttributes;
using Zenject;

namespace Base
{
    [System.Serializable]
    public abstract class StateBase
    {

        public enum EVENT
        {
            ENTER   = 0,
            UPDATE  = 1,
            EXIT    = 2
        }

        [ShowNonSerializedField] protected EVENT stage;
        [ShowNonSerializedField] protected StateBase nextState;

        public StateBase()
        {

        }

        public virtual void Enter()
        {
            stage = EVENT.UPDATE;
        }

        public virtual void Update()
        {
            stage = EVENT.UPDATE;
        }

        public virtual void Exit()
        {
            stage = EVENT.EXIT;
        }

        public StateBase Process()
        {
            switch (stage)
            {
                case EVENT.ENTER:
                    Enter();
                    break;
                case EVENT.UPDATE:
                    Update();
                    break;
                case EVENT.EXIT:
                    Exit();
                    return nextState;
            }

            return this;
        }

    }
}

