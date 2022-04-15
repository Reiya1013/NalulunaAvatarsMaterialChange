using System;
using HarmonyLib;
using UnityEngine;
using VRM;
//using NalulunaAvatars;

namespace NalulunaAvatarsMaterialChange
{
    [HarmonyPatch(typeof(UniGLTF.ImporterContext), "ShowMeshes", MethodType.Normal)]
    public static class VRMHarmony
    {
        /// <summary>
        /// ロード済のアバターを表示するUniVRMのメソッドをHarmonyで割り込んでマテリアルチェンジする
        /// </summary>
        /// <param name="__instance"></param>
        public static void Prefix(UniGLTF.ImporterContext __instance)
        {
            try
            {
                Logger.log.Debug($"Harmony ShowMeshes");

                //マテリアルチェンジOFF時は処理を飛ばす
                //if (Settings.instance.MaterialChange_value == MaterialChange.OFF) return;
                Logger.log.Debug($"Harmony ChangeMaterial Start");

                //マテリアル変更
                SharedCoroutineStarter.instance.StartCoroutine(NalulunaMaterialChange.instance.ChangeMaterial(__instance));

                Logger.log.Debug($"Harmony ChangeMaterial End");
            }
            catch (System.Exception e)
            {
                Logger.log.Debug($"Harmony ShowMeshes Err {e.Message}");
            }
        }
    }

}
