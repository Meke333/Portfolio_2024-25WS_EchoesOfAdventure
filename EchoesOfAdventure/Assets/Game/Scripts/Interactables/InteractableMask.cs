
using System;

namespace Interactable
{
    [Flags]
	public enum InteractableMask
	{
		Woodbox = 1 << 1,
		PlantSprout = 1 << 2,
		PlantStandAlone = 1 << 3,
		Ladder = 1 << 4,

	}
}