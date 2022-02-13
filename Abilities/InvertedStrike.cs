﻿using APIPlugin;
using DiskCardGame;
using HarmonyLib;

namespace GrimoraMod;

public class InvertedStrike : AbilityBehaviour
{
	public static Ability ability;
	public override Ability Ability => ability;

	public static NewAbility Create()
	{
		const string rulebookDescription =
			"[creature] will strike the opposing slot as if the board was flipped. " +
			"A card in the far left slot will attack the opposing far right slot.";

		return ApiUtils.CreateAbility<InvertedStrike>(rulebookDescription);
	}
}

[HarmonyPatch]
public class PatchesForInvertedStrike
{
	[HarmonyPostfix, HarmonyPatch(typeof(PlayableCard), nameof(PlayableCard.GetOpposingSlots))]
	public static void InvertedStrikeGetOpposingSlots(PlayableCard __instance, ref List<CardSlot> __result)
	{
		if (__instance.HasAbility(InvertedStrike.ability))
		{
			List<CardSlot> slotsToCheck = __instance.OpponentCard
				? BoardManager.Instance.PlayerSlotsCopy
				: BoardManager.Instance.OpponentSlotsCopy;

			int slotIndex = __instance.Slot.Index;
			// 3 - 0 (card slot) == 3 (opposing slot)
			// 3 - 1 (card slot) == 2 (opposing slot)
			// 3 - 2 (card slot) == 1 (opposing slot)
			// 3 - 3 (card slot) == 0 (opposing slot)
			// if for whatever reason we increase the number of card slots in the mod, don't hardcode to 3
			int slotToAttack = (BoardManager.Instance.playerSlots.Count - 1) - slotIndex;

			__result = new List<CardSlot>() { slotsToCheck[slotToAttack] };
		}
	}
}