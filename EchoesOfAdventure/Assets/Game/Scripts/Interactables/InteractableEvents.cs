
using System;
using UnityEngine;

namespace Interactable
{
	public class InteractableEvents
	{
		public Action<Vector3> WoodenBox_Breaks;
		public Action<bool> WoodenBox_Drag;
		public Action<Vector3> WoodenBox_Lands;

		public Action<Vector3> Collectible_Collected;

		public Action<Vector3> Fire_Burning;
	}
}