using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using System;
using System.Threading.Tasks;

namespace SpeedChecker
{
    public class AverageSpeedCheck : BaseScript
    {
        public static bool Showing = false;
        private bool MeasuringDistance = false;
        private bool MeasuringTime = false;
        private double Distance = 0;
        private Vector3 LastDistanceCheckReferencePoint = Vector3.Zero;
        private double SecondsPassed = 0;
        private DateTime timer = DateTime.Now;
        private double AverageSpeed = 0;
        private System.Drawing.Color SpeedColour = System.Drawing.Color.FromArgb(255, 255, 255, 255);
        public static bool? authorized = null;

        public static bool lastInputWasController => !Function.Call<bool>(Hash._IS_INPUT_DISABLED, 0);

        public AverageSpeedCheck()
        {
            EventHandlers["SpeedChecker:ProvidaAuth"] += new Action<bool>((bool auth) =>
            {
                if (Game.Player != null && Game.PlayerPed != null && Game.PlayerPed.Exists())
                {
                    authorized = auth;
                    
                    if (!Showing && auth)
                    {
                        Showing = true;
                    }
                }

            });
            Main();
            LowPriority();
        }

        private async Task Main()
        {
            while (true)
            {
                await Delay(0);
                drawing();
                if (Game.Player != null && Game.PlayerPed != null && Game.PlayerPed.Exists() && Game.PlayerPed.CurrentVehicle != null && Game.PlayerPed.CurrentVehicle.Exists()
                    && !lastInputWasController)
                {
                    if (Game.IsControlJustPressed(0, Control.ScriptedFlyZUp) || Game.IsDisabledControlJustPressed(0, Control.ScriptedFlyZUp))
                    {
                        if (authorized == null)
                        {
                            Debug.WriteLine("Requesting server for authorization.");
                            TriggerServerEvent("SpeedChecker:svProvidaAuth", LocalPlayer.ServerId);
                        }
                        else if (authorized == true)
                        {
                            if (!Showing)
                            {
                                Showing = true;
                            }
                            else
                            {
                                HandleProvida();
                            }
                        }
                    }
                    else if (!(Game.IsControlJustPressed(0, Control.ScriptedFlyZUp) || Game.IsDisabledControlJustPressed(0, Control.ScriptedFlyZUp)) && (Game.IsControlJustPressed(0, Control.ScriptedFlyZDown) || Game.IsDisabledControlJustPressed(0, Control.ScriptedFlyZDown)))
                    {
                        Showing = false;
                    }
                }
            }
        }

        private async Task LowPriority()
        {
            while (true)
            {
                await Delay(10);
                if (Game.Player != null && Game.PlayerPed != null && Game.PlayerPed.Exists() && Game.PlayerPed.CurrentVehicle != null && Game.PlayerPed.CurrentVehicle.Exists())
                {
                    
                    if (Showing)
                    {
                        if (MeasuringDistance)
                        {
                            Distance += Vector3.Distance(LastDistanceCheckReferencePoint, Game.PlayerPed.CurrentVehicle.Position);
                            LastDistanceCheckReferencePoint = Game.PlayerPed.CurrentVehicle.Position;                           
                        }
                        if (MeasuringTime)
                        {
                            SecondsPassed = (DateTime.Now - timer).TotalSeconds;
                        }
                    }
                }
                else if (Showing)
                {
                    Showing = false;                   
                }
            }
        }

        private void HandleProvida()
        {
            if (!MeasuringDistance && !MeasuringTime)
            {
                if (SecondsPassed > 0)
                {
                    ResetProvida();
                }
                else
                {
                    StartTimeMeasuring();
                }
            }
            else if (MeasuringTime && !MeasuringDistance)
            {
                StartDistanceMeasuring();
            }
            else if (MeasuringTime && MeasuringDistance)
            {
                StopTimeMeasuring();
            }
            else if (!MeasuringTime && MeasuringDistance)
            {
                StopDistanceMeasuring();
                CalculateAverageSpeed();
            }
        }

        private void drawing()
        {
            Rectangle rect = new Rectangle(new System.Drawing.PointF(0.1f, 169), new System.Drawing.SizeF(130, 70), System.Drawing.Color.FromArgb(210, 0, 10, 28));
            Text txt = new Text("", new System.Drawing.PointF(1.1f, 170), 0.40f, SpeedColour);
            rect.Enabled = Showing;
            txt.Enabled = Showing;
            txt.Caption = "d: " + Math.Round(Distance, 4) + " m" + "~n~t: " + Math.Round(SecondsPassed, 4) + " s" + "~n~s: " + Math.Round(AverageSpeed, 1) + " MPH";
            rect.Draw();
            txt.Draw();
        }

        private void ResetProvida()
        {
            timer = DateTime.Now;
            AverageSpeed = 0f;
            Distance = 0f;
            SecondsPassed = 0f;
            MeasuringDistance = false;
            MeasuringTime = false;
            SpeedColour = System.Drawing.Color.FromArgb(255, 255, 255, 255);
        }

        private void StartDistanceMeasuring()
        {
            Distance = 0f;
            LastDistanceCheckReferencePoint = Game.PlayerPed.CurrentVehicle.Position;
            MeasuringDistance = true;
            SpeedColour = System.Drawing.Color.FromArgb(255, 255, 255, 0);
            Screen.ShowSubtitle("Started distance measuring", 1500);
        }

        private void StartTimeMeasuring()
        {
            timer = DateTime.Now;
            MeasuringTime = true;
            SpeedColour = System.Drawing.Color.FromArgb(255, 255, 255, 0);
            Screen.ShowSubtitle("Started time measuring", 1500);
        }

        private void StopDistanceMeasuring()
        {
            MeasuringDistance = false;
            Distance += Vector3.Distance(LastDistanceCheckReferencePoint, Game.PlayerPed.CurrentVehicle.Position);
            LastDistanceCheckReferencePoint = Game.PlayerPed.CurrentVehicle.Position;
            SpeedColour = System.Drawing.Color.FromArgb(255, 255, 255, 0);
            Screen.ShowSubtitle("Stopped distance measuring", 1500);
        }

        private void StopTimeMeasuring()
        {
            MeasuringTime = false;
            SecondsPassed = (DateTime.Now - timer).TotalSeconds;
            SpeedColour = System.Drawing.Color.FromArgb(255, 255, 255, 0);
            Screen.ShowSubtitle("Stopped time measuring", 1500);
        }

        private void CalculateAverageSpeed()
        {
            AverageSpeed = Distance * 0.000621371f / (SecondsPassed / 3600);
            SpeedColour = System.Drawing.Color.FromArgb(255, 173, 216, 230);
            TriggerEvent("chatMessage", "Provida Speed Check", new int[] { 0, 191, 255 }, "d: " + Math.Round(Distance, 4) + " m - t: " + Math.Round(SecondsPassed, 4) + " s - s: " + Math.Round(AverageSpeed, 1) + " MPH");
        }
        
    }
}
