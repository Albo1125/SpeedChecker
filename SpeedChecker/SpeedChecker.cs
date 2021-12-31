using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedChecker
{
    //data_file 'WEAPONINFO_FILE_PATCH' 'weaponmarksmanpistol.meta'

    public class SpeedChecker : BaseScript
    {
        private static WeaponAsset speedGunWeapon = new WeaponAsset(WeaponHash.MarksmanPistol);
        
        private bool speedgunActive = false;
        private string speedgunInfo = "";
        private string speedInfo = "";
        public SpeedChecker()
        {
            EventHandlers["fivemskillsreset"] += new Action<dynamic>((dynamic) =>
            {
                AverageSpeedCheck.authorized = null;
                AverageSpeedCheck.Showing = false;
            });

            EventHandlers["SpeedChecker:ToggleSpeedgun"] += new Action<dynamic>((dynamic d) =>
            {
                if (Game.Player != null && Game.PlayerPed != null && Game.PlayerPed.Exists())
                {
                    WeaponHash speedhash = (WeaponHash)speedGunWeapon.Hash;
                    if (speedgunActive)
                    {
                        Game.Player.Character.Weapons.Remove(speedhash);
                        
                    }
                    else
                    {
                        Game.Player.Character.Weapons.Give(speedhash, -1, true, true);
                    }
                }

            });

            EventHandlers["SpeedChecker:GetSpeedCl"] += new Action<int>((int requester) =>
            {
                if (Game.Player != null && Game.PlayerPed != null && Game.PlayerPed.Exists() && Game.PlayerPed.IsInVehicle() && Game.PlayerPed.CurrentVehicle != null && Game.PlayerPed.CurrentVehicle.Exists())
                {
                    float speedmph = Game.PlayerPed.CurrentVehicle.Speed * 2.236936f;
                    TriggerServerEvent("SpeedChecker:PushSpeed", requester, Math.Max(0, Math.Round(speedmph, 1)));
                }
            });

            EventHandlers["SpeedChecker:PushSpeedCl"] += new Action<double>((double speed) =>
            {
                if (Game.Player != null && Game.PlayerPed != null && Game.PlayerPed.Exists())
                {
                    speedInfo = "~g~" + speed;
                }

            });
            mainLogic();
        }

        private async Task mainLogic()
        {
            while (true)
            {
                await Delay(0);
                drawing();
                speedgunActive = false;
                
                if (Game.PlayerPed.Weapons.Current.Hash == speedGunWeapon)
                {
                    if (AverageSpeedCheck.authorized == null)
                    {
                        Debug.WriteLine("Requesting server for authorization.");
                        TriggerServerEvent("SpeedChecker:svProvidaAuth", LocalPlayer.ServerId);
                    }
                    else if (AverageSpeedCheck.authorized == false)
                    {
                        await Delay(500);
                    }
                    else if (AverageSpeedCheck.authorized == true)
                    {
                        if (!Game.PlayerPed.IsSittingInVehicle() || Game.PlayerPed.CurrentVehicle.Speed < 3f)
                        {
                            speedgunActive = true;
                            Game.DisableControlThisFrame(0, Control.Attack);
                            Game.DisableControlThisFrame(0, Control.Attack2);
                            Game.DisableControlThisFrame(0, Control.MeleeAttack1);
                            Game.DisableControlThisFrame(0, Control.MeleeAttack2);

                            if (Game.IsDisabledControlJustPressed(0, Control.Attack))
                            {
                                Vehicle veh = null;
                                try
                                {
                                    int entityhandle = 0;
                                    if (CitizenFX.Core.Native.API.GetEntityPlayerIsFreeAimingAt(Game.Player.Handle, ref entityhandle))
                                    {

                                        Entity ent = Entity.FromHandle(entityhandle);
                                        if (ent is Ped)
                                        {
                                            veh = ((Ped)ent).CurrentVehicle;
                                        }
                                        else if (ent is Vehicle)
                                        {
                                            veh = (Vehicle)ent;
                                        }
                                    }
                                }
                                catch (Exception e) { }



                                if (veh != null && veh.Exists())
                                {
                                    string plate = CitizenFX.Core.Native.Function.Call<string>(CitizenFX.Core.Native.Hash.GET_VEHICLE_NUMBER_PLATE_TEXT, veh);
                                    float speedmph = veh.Speed * 2.236936f;
                                    if (veh.Driver.IsPlayer)
                                    {
                                        foreach (Player p in Players)
                                        {
                                            if (p != null && p.Character != null && p.Character == veh.Driver)
                                            {
                                                speedgunInfo = "~b~Plate:~w~ " + plate + "~n~~y~MPH: ";
                                                TriggerServerEvent("SpeedChecker:GetSpeed", Game.Player.ServerId, p.ServerId);
                                                break;
                                            }
                                        }

                                    }
                                    else
                                    {
                                        speedgunInfo = "~b~Plate:~w~ " + plate + "~n~~y~MPH: ";
                                        speedInfo = "~w~" + Math.Max(0, Math.Round(speedmph, 1));
                                    }

                                }

                            }
                        }
                    }
                }
            }
        }

        private void drawing()
        {
            if (AverageSpeedCheck.authorized == true)
            {
                CitizenFX.Core.UI.Rectangle rect = new CitizenFX.Core.UI.Rectangle(new System.Drawing.PointF(0.1f, 139), new System.Drawing.SizeF(150, 50), System.Drawing.Color.FromArgb(210, 0, 10, 28));
                CitizenFX.Core.UI.Text txt = new CitizenFX.Core.UI.Text("", new System.Drawing.PointF(1.1f, 140), 0.40f, System.Drawing.Color.FromArgb(255, 255, 255, 255));
                rect.Enabled = speedgunActive && !AverageSpeedCheck.Showing;
                txt.Enabled = speedgunActive && !AverageSpeedCheck.Showing;
                txt.Caption = speedgunInfo + speedInfo;
                rect.Draw();
                txt.Draw();
            }
        }
    }
}
