using Oxide.Core.Plugins;
using Oxide.Game.Rust.Cui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using WebSocketSharp;

namespace Oxide.Plugins
{
    [Info("Terminal Hack Plugin", "Mooli", "1.0.0")]
    public class TerminalHackPlugin : RustPlugin
    {
        [PluginReference]
        private Plugin ImageLibrary;

        private static int numberOfLines = 14;
        private string[] terminalUsedCommands = new string[numberOfLines];
        private string terminalPath = @"C:\User\Brinda>";

        private const string imagePath = "https://cdn.discordapp.com/attachments/1268169780024840202/1278667833131270255/Group_3_1.png";

        void Init()
        {
            if (ImageLibrary == null)
            {
                PrintError("ImageLibrary plugin is not loaded. Please install ImageLibrary plugin.");
                return;
            }

            ImageLibrary.AddImage(imagePath, "scannerUi");
        }


        //helpers
        private void clearLastCommands()
        {
            terminalUsedCommands = new string[numberOfLines];
        }

        private void updateLastCommand(string cmd)
        {
            for (int i = 0; i < numberOfLines; i++)
            {
                (cmd, terminalUsedCommands[i]) = (terminalUsedCommands[i], cmd);
            }
        }

        void runTerminalCommand(string command)
        {
            switch (command)
            {
                case "dir":
                    updateLastCommand("hack.exe");
                    break;

                case "run":
                    ConsoleSystem.Run(ConsoleSystem.Option.Server.Quiet(), "scanner.open");
                    break;

                case "clear":
                    clearLastCommands();
                    break;

                default:
                    updateLastCommand("Invalid Command");
                    break;
            }
        }

        //UI helpers
        private void CreateScannerUI(BasePlayer player)
        {
            Puts("Created Scanner");
            var uiElements = new CuiElementContainer();
            string imageId = ImageLibrary.GetImage("scannerUi");

            var mainPanel = uiElements.Add(new CuiPanel
            {
                Image = { Color = "0 0 0 0" },
                RectTransform = { AnchorMin = "0.3178574 0.3257154", AnchorMax = "0.6714277 0.7152363" },
                CursorEnabled = true
            }, "Overlay", "ScannerPlan");

            uiElements.Add(new CuiButton
            {
                Button = { Close = mainPanel, Color = "0 0 0 0" },
                Text = { Text = "X", FontSize = 14 },
                RectTransform = { AnchorMin = "0.915827 0.9022043", AnchorMax = "0.9798004 0.9798004" }

            }, mainPanel);

            uiElements.Add(new CuiElement
            {
                Name = "Scanner",
                Parent = mainPanel,
                Components =
                {
                    new CuiRawImageComponent { Png = imageId, Sprite = "assets/content/ui/uibackgroundblur.mat" },
                    new CuiRectTransformComponent { AnchorMin = "0.3714287 0.2285714", AnchorMax = "0.5797616 0.7523809" }
                }
            });

            CuiHelper.AddUi(player, uiElements);
        }

        private void CreateTerminalUI(BasePlayer player)
        {
            Puts("Created Terminal UI");
            var uiElements = new CuiElementContainer();

            var mainPanel = uiElements.Add(new CuiPanel
            {
                Image = { Color = "0 0 0 1" },
                RectTransform = { AnchorMin = "0.3178574 0.3257154", AnchorMax = "0.6714277 0.7152363" },
                CursorEnabled = true
            }, "Overlay", "TerminalPanel");

            double[,] commandanchors = { { 0.03030238, 0.1149155, 1.210441, 0.1809276 },
                                         { 0.03030238, 0.1687056, 1.210441, 0.2347180 },
                                         { 0.03030238, 0.2224961, 1.210441, 0.2885089} };

            double[] commandanchor = { 0.1149155, 0.1809276 };

            for (int i = 0; i < numberOfLines; i++)
            {
                var lastcommand = terminalUsedCommands[i];

                if (lastcommand == null)
                {
                    lastcommand = "";
                }

                uiElements.Add(new CuiLabel
                {

                    Text = { Text = lastcommand, FontSize = 14 },
                    RectTransform = {
                        AnchorMin = $"0.03030238 {commandanchor[0] + i*0.0537905}",
                        AnchorMax = $"1.21044100 {commandanchor[1] + i*0.0537905}"
                    }
                }, mainPanel);
            };

            uiElements.Add(new CuiLabel
            {

                Text = { Text = $"{terminalPath}", FontSize = 14 },
                RectTransform = {
                        AnchorMin = "0.03030238 0.069155",
                        AnchorMax = "1.210441 0.1269276"
                    }
            }, mainPanel);

            uiElements.Add(new CuiElement
            {
                Parent = mainPanel,
                Components =
                {
                    new CuiInputFieldComponent { Command = "terminal.enter", FontSize = 14, Text = "" },
                    new CuiRectTransformComponent { AnchorMin = "0.25930238 0.069155", AnchorMax = "1.502441 0.1269276" },
                    new CuiNeedsCursorComponent()
                }
            });

            uiElements.Add(new CuiButton
            {
                Button = { Close = mainPanel, Color = "1 1 1 1" },
                Text = { Text = "X", FontSize = 14 },
                RectTransform = { AnchorMin = "0.915827 0.9022043", AnchorMax = "0.9798004 0.9798004" }

            }, mainPanel);

            CuiHelper.AddUi(player, uiElements);
        }


        //chat commands
        [ChatCommand("laptop")]
        private void OpenLaptop(BasePlayer player)
        {
            clearLastCommands();
            CreateTerminalUI(player);
        }

        [ChatCommand("scanner")]
        private void OpenScanner(BasePlayer player)
        {
            CreateScannerUI(player);
        }


        [ChatCommand("test")]
        private void test(BasePlayer player)
        {
            player.ChatMessage("Hi");
        }

        //console commands
        [ConsoleCommand("terminal.enter")]
        private void TerminalEnter(ConsoleSystem.Arg arg)
        {
            string userInput = arg.GetString(0, "");
            updateLastCommand(terminalPath + userInput);
            CuiHelper.DestroyUi(arg.Player(), "TerminalPanel");
            runTerminalCommand(userInput);
            CreateTerminalUI(arg.Player());
        }


        [ConsoleCommand("scanner.open")]
        private void ScannerOpen(ConsoleSystem.Arg arg)
        {
            updateLastCommand("Now opening Scanner");
            timer.Once(3f, () =>
            {

            });
            CreateScannerUI(arg.Player());
        }



    }
}
