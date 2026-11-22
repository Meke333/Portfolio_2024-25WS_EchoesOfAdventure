using Helper;
using Interface;
using NaughtyAttributes;
using System;
using UnityEngine;
using Zenject;

namespace ObjectPool
{
    [Obsolete("Old version of GameObjectPool, not used because GameObject could not be deactivated by themself")]
    public class GameObjectPoolBaseDeprecated<T> where T : MonoBehaviour, IPoolObject
	{
        [Inject] private FactoryHelper _factoryHelper;

        [field: SerializeField, ReadOnly] protected T[] pool { get; private set; }
		[SerializeField, ReadOnly] private int _startIndex;
		public int StartIndex
		{
			get => _startIndex;
			set
			{
				_startIndex = (value + pool.Length) % pool.Length;
			}
		}


		[SerializeField, ReadOnly] private int _endIndex;
        public int EndIndex
        {
			get => _endIndex;
			set
			{
				_endIndex = (value + pool.Length) % pool.Length;
			}
        }

		[SerializeField, ReadOnly] private int _activeObjectAmount;
        public int ActiveObjectAmount
		{
			get => _activeObjectAmount;
			set
			{
				_activeObjectAmount = (int) Math.Clamp(value, 0, pool.Length);
			}
		}

		protected Transform objectParent;

		protected GameObject elementPrefab;


        //Pseudo Constructors
        public void Construct(Transform parent)
        {
            pool = parent.GetComponentsInChildren<T>();

            StartIndex = 0;
            EndIndex = 0;
            ActiveObjectAmount = 0;

            objectParent = parent;
        }

        public void Construct(Transform parent, GameObject prefab, int length)
        {
            pool = new T[length];

            objectParent = parent;
            elementPrefab = prefab;

            FillInPool();

            StartIndex = 0;
            EndIndex = 0;
            ActiveObjectAmount = 0;

        }

        public virtual void Initialize()
		{
			//Events
		}

        public virtual void Dispose()
		{
			//Events
		}

        private void FillInPool()
        {
            for (int i = 0; i < pool.Length; i++)
            {
                pool[i] = _factoryHelper.InstantiatePrefab(elementPrefab, objectParent).GetComponent<T>();
                SetElementActive(i, false);
            }
        }

        public void ResetPool()
		{
			pool = new T[pool.Length];
			FillInPool();

            StartIndex = 0;
            EndIndex = 0;
			ActiveObjectAmount = 0;
        }

        public void ResetAndDeletePool()
		{
			for (int i = 0; i < pool.Length; i++)
			{
				T temp = pool[i];
				pool[i] = null;

                UnityEngine.Object.Destroy(temp.gameObject);
			}

            pool = new T[pool.Length];
            FillInPool();

            StartIndex = 0;
            EndIndex = 0;
            ActiveObjectAmount = 0;
        }

        public int GetActiveAmount()
		{
			return ActiveObjectAmount;

        }

        public bool IsFull()
		{
			return ActiveObjectAmount >= pool.Length || StartIndex == EndIndex;

        }

        public bool IsEmpty()
		{
			return ActiveObjectAmount <= 0;
		}

        public bool IsIndexActive(int index)
		{
			if (index < 0 || index >= pool.Length)
				throw new IndexOutOfRangeException("Index out of bounds from Object Pool!");

			return pool[index].gameObject.activeSelf;
		}

        public T GetFirstObject()
        {
            return pool[StartIndex];
        }

        public T GetLastObject()
		{
			return pool[EndIndex];
		}

		private void SetElementActive(int index, bool value)
		{
            pool[index].gameObject.SetActive(value);
            pool[index].SetActivity(value);
        }


        /// <summary>
        /// Remove an Object with an index, which starts from the queue.
        /// </summary>
        /// <param name="index">Index starting from the currentIndex</param>
        /// <returns>
        /// true: Object at Index exists and is removed from pool
        /// false: Object at Index does not exist and therefore can not be removed
        /// </returns>
        public bool ResetAtIndex(int index)
        {
            if (IsEmpty() || pool[index] == null)
                return false;

			bool isActive = pool[index].gameObject.activeSelf;

            pool[index] = _factoryHelper.InstantiatePrefab(elementPrefab, objectParent).GetComponent<T>();
            SetElementActive(index, isActive);

            return true;
        }

        public bool ResetAndDeleteAtIndex(int index)
		{
            if (IsEmpty() || pool[index] == null)
                return false;

            bool isActive = pool[index].gameObject.activeSelf;

            UnityEngine.Object.Destroy(pool[index].gameObject);
            pool[index] = _factoryHelper.InstantiatePrefab(elementPrefab, objectParent).GetComponent<T>();
            SetElementActive(index, isActive);

            return true;
        }

        public bool ResetObject(T objectToRemove)
		{
			for (int i = 0; i < pool.Length; i++)
			{
				if (pool[i] != objectToRemove)
                    continue;

                bool isActive = pool[i].gameObject.activeSelf;
                pool[i] = _factoryHelper.InstantiatePrefab(elementPrefab, objectParent).GetComponent<T>();
                SetElementActive(i, isActive);


                ActiveObjectAmount--;
				return true;
			}
			return false;
		}

        public bool ResetAndDeleteObject(T objectToRemove)
		{
            for (int i = 0; i < pool.Length; i++)
            {
                if (pool[i] != objectToRemove)
                    continue;

                bool isActive = pool[i].gameObject.activeSelf;
                UnityEngine.Object.Destroy(pool[i].gameObject);
                pool[i] = _factoryHelper.InstantiatePrefab(elementPrefab, objectParent).GetComponent<T>();
                SetElementActive(i, isActive);


                ActiveObjectAmount--;
                return true;
            }
            return false;
        }

        public T ActivateNext()
		{
			if (IsFull()) 
				return null;

			SetElementActive(EndIndex, true);
            T result = pool[EndIndex];

            EndIndex++;
			ActiveObjectAmount++;

			return result;
		}

        public T DeactivateLast()
		{
			if (IsEmpty())
				return null;

			SetElementActive(StartIndex, false);
            T result = pool[StartIndex];

            StartIndex++;
			ActiveObjectAmount--;

			return result;
		}

	}
}