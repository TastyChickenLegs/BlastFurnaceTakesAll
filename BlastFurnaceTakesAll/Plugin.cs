using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using HarmonyLib;

namespace BlastFurnaceFix
{

    [BepInPlugin("tastychickenlegs.BlastFurnaceTakesAll", "BlastFurnaceTakesAll", "1.0.9")]
    [BepInProcess("valheim.exe")]
    [BepInProcess("valheim_server.exe")]
    [HarmonyPatch]
    public class Plugin : BaseUnityPlugin
    {
        private const string GUID = "BlastFurnaceTakesAll";
        private const string VERSION = "1.0.9";

        static Dictionary<string, ItemDrop> metals = new Dictionary<string, ItemDrop>
        {
            { "$item_copperore", null },
            { "$item_copper", null },
            { "$item_ironscrap", null },
            { "$item_ironore", null },
            { "$item_iron", null },
            { "$item_tinore", null },
            { "$item_tin", null },
            { "$item_silverore", null },
            { "$item_silver", null },
            { "$item_copperscrap", null },
            { "$item_bronzescrap", null },
             { "$item_bronze", null }
        };

        void Awake()
        {
            Harmony harmony = new Harmony(GUID);
            harmony.PatchAll();
        }
        void destroy()
        {
            
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Smelter), "Awake")]
        static void BlastFurnacePatch(ref Smelter __instance)
        {
            if(__instance.m_name != "$piece_blastfurnace")
            {
                UnityEngine.Debug.Log("Ignored non-blast furnace smelter.");
                return;
            }
            UnityEngine.Debug.Log("Found a blast furnace! Applying fix.");

            ObjectDB instance = ObjectDB.instance;
            List<ItemDrop> materials = instance.GetAllItems(ItemDrop.ItemData.ItemType.Material, "");

            foreach (ItemDrop material in materials)
            {
                if (Plugin.metals.Keys.Contains(material.m_itemData.m_shared.m_name))
                {
                    UnityEngine.Debug.Log("Adding " + material.m_itemData.m_shared.m_name + " to list of materials.");
                    Plugin.metals[material.m_itemData.m_shared.m_name] = material;
                }
            }

            List<Smelter.ItemConversion> conversions = new List<Smelter.ItemConversion>()
            {
                new Smelter.ItemConversion{ m_from = Plugin.metals["$item_copperore"], m_to = Plugin.metals["$item_copper"]},
                new Smelter.ItemConversion{ m_from = Plugin.metals["$item_tinore"], m_to = Plugin.metals["$item_tin"]},
                new Smelter.ItemConversion{ m_from = Plugin.metals["$item_ironscrap"], m_to = Plugin.metals["$item_iron"]},
                new Smelter.ItemConversion{ m_from = Plugin.metals["$item_silverore"], m_to = Plugin.metals["$item_silver"]},
               new Smelter.ItemConversion{ m_from = Plugin.metals["$item_copperscrap"], m_to = Plugin.metals["$item_copper"]},
               new Smelter.ItemConversion{ m_from = Plugin.metals["$item_bronzescrap"], m_to = Plugin.metals["$item_bronze"]},
               new Smelter.ItemConversion{ m_from = Plugin.metals["$item_ironore"], m_to = Plugin.metals["$item_iron"]},
            };
                
            foreach(Smelter.ItemConversion conversion in conversions)
            {
                __instance.m_conversion.Add(conversion);
            }
        }
    }
}
