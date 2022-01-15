﻿using DiskCardGame;
using GrimoraMod.Properties;

namespace GrimoraMod
{
	public partial class GrimoraPlugin
	{
		public const string NameDeadHand = "ara_DeadHand";

		private void AddAra_DeadHand()
		{
			ApiUtils.Add(
				NameDeadHand, "Dead Hand",
				"Cut off from an ancient God, the Dead Hand took on its own Life.",
				1, 1, 5, Resources.DeadHand, Ability.DrawNewHand, complexity: CardComplexity.Advanced);
		}
	}
}