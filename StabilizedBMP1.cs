﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MelonLoader;
using StabilizedBMP1;
using GHPC.Vehicle;
using GHPC;
using System.Reflection;
using GHPC.Weapons;

[assembly: MelonInfo(typeof(StabilizedBMP1Mod), "Stabilized BMP-1", "1.0.0", "ATLAS")]
[assembly: MelonGame("Radian Simulations LLC", "GHPC")]


namespace StabilizedBMP1
{
    public class StabilizedBMP1Mod : MelonMod
    {
        private GameObject[] vic_gos;
        private string[] invalid_scenes = new string[] { "MainMenu2_Scene", "LOADER_MENU", "LOADER_INITIAL", "t64_menu" };

        public override async void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (invalid_scenes.Contains(sceneName)) return;
            vic_gos = GameObject.FindGameObjectsWithTag("Vehicle");

            while (vic_gos.Length == 0)
            {
                vic_gos = GameObject.FindGameObjectsWithTag("Vehicle");
                await Task.Delay(1);
            }

            await Task.Delay(3000);

            foreach (GameObject vic_go in vic_gos)
            {
                Vehicle vic = vic_go.GetComponent<Vehicle>();

                if (vic == null) continue;

                string name = vic.FriendlyName;

                if (name == "BMP-1" || name == "BMP-1P")
                {
                    AimablePlatform[] aimables = vic.AimablePlatforms;
                    FieldInfo stab_mode = typeof(AimablePlatform).GetField("_stabMode", BindingFlags.Instance | BindingFlags.NonPublic);
                    FieldInfo stab_active = typeof(AimablePlatform).GetField("_stabActive", BindingFlags.Instance | BindingFlags.NonPublic);
                    PropertyInfo stab_FCS_active = typeof(FireControlSystem).GetProperty("StabsActive", BindingFlags.Instance | BindingFlags.Public);

                    WeaponsManager weapons_manager = vic.GetComponent<WeaponsManager>();
                    WeaponSystemInfo main_gun_info = weapons_manager.Weapons[0];
                    stab_FCS_active.SetValue(main_gun_info.FCS, true);
                    main_gun_info.FCS.CurrentStabMode = StabilizationMode.Vector;

                    aimables[0].Stabilized = true;
                    stab_active.SetValue(aimables[0], true);
                    stab_mode.SetValue(aimables[0], StabilizationMode.Vector);

                    aimables[3].Stabilized = true;
                    stab_active.SetValue(aimables[3], true);
                    stab_mode.SetValue(aimables[3], StabilizationMode.Vector);
                }
            }
        }
    }
}
