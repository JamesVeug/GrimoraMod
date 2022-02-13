﻿using DiskCardGame;
using HarmonyLib;
using UnityEngine;
using static GrimoraMod.GrimoraPlugin;

namespace GrimoraMod;

public class GrimoraItemsManagerExt : ItemsManager
{
	[SerializeField] internal HammerItemSlot hammerSlot;

	public new static GrimoraItemsManagerExt Instance => ItemsManager.Instance as GrimoraItemsManagerExt;

	public override List<string> SaveDataItemsList => Part3SaveData.Data.items;

	public override void OnBattleStart()
	{
		hammerSlot.InitializeHammer();
	}

	public override void OnBattleEnd()
	{
		hammerSlot.CleanupHammer();
	}

	public static void AddHammer()
	{
		GrimoraItemsManager currentItemsManager = GrimoraItemsManager.Instance.GetComponent<GrimoraItemsManager>();

		GrimoraItemsManagerExt ext = GrimoraItemsManager.Instance.GetComponent<GrimoraItemsManagerExt>();

		if (ext is null)
		{
			Log.LogDebug($"[AddHammer] Creating hammer and GrimoraItemsManagerExt");

			ext = GrimoraItemsManager.Instance.gameObject.AddComponent<GrimoraItemsManagerExt>();
			ext.consumableSlots = currentItemsManager.consumableSlots;
			Log.LogDebug($"[AddHammer] Destroying old manager");
			Destroy(currentItemsManager);

			Part3ItemsManager part3ItemsManager = Instantiate(
				ResourceBank.Get<Part3ItemsManager>("Prefabs/Items/ItemsManager_Part3")
			);

			ext.hammerSlot = part3ItemsManager.hammerSlot;
			part3ItemsManager.hammerSlot.transform.SetParent(ext.transform);

			float xVal = Harmony.HasAnyPatches("julianperge.inscryption.act1.increaseCardSlots") ? -8.75f : -7.5f;
			ext.hammerSlot.gameObject.transform.localPosition = new Vector3(xVal, 0.81f, -0.48f);
			ext.hammerSlot.gameObject.transform.rotation = Quaternion.Euler(270f, 315f, 0f);
		}

		if (FindObjectOfType<Part3ItemsManager>() is not null)
		{
			Log.LogDebug($"[AddHammer] Destroying existing part3ItemsManager");
			Destroy(FindObjectOfType<Part3ItemsManager>().gameObject);
		}

		Log.LogDebug($"[AddHammer] Finished adding hammer");
	}
}

[HarmonyPatch(typeof(ItemSlot), nameof(ItemSlot.CreateItem))]
public class AddNewHammerExt
{
	[HarmonyPrefix]
	public static bool InitHammerExtAfter(ItemSlot __instance, ItemData data, bool skipDropAnimation = false)
	{
		if (GrimoraSaveUtil.isGrimora && data.prefabId.Equals("HammerItem"))
		{
			if (__instance.Item != null)
			{
				Object.Destroy(__instance.Item.gameObject);
			}

			GameObject gameObject = Object.Instantiate(
				ResourceBank.Get<GameObject>("Prefabs/Items/" + data.PrefabId),
				__instance.transform
			);
			gameObject.transform.localPosition = Vector3.zero;
			var oldHammer = gameObject.GetComponent<Item>();
			HammerItemExt ext = gameObject.AddComponent<HammerItemExt>();
			ext.Data = oldHammer.Data;
			__instance.Item = ext;
			__instance.Item.SetData(data);
			__instance.Item.PlayEnterAnimation(true);

			Log.LogDebug($"Destroying old HammerItem");
			Object.Destroy(oldHammer);
			return false;
		}

		return true;
	}
}