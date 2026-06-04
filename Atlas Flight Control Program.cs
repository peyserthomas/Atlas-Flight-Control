using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Blocks;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.EntityComponents;
using Sandbox.Game.GameSystems.Electricity;
using Sandbox.Graphics.GUI;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.Entities.Blocks;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game.VisualScripting;
using VRageMath;
using VRageRender;
using Color = VRageMath.Color;



namespace IngameScript
{
    public partial class Program : MyGridProgram
    {

        Color ESA_blue = new Color(0x23, 0x85, 0xE8);
        bool Initialized = false;
        float TextHeight;
        float Padding = 12f;
        double Altitude;

        int AtlasFlightStage = 0;
        bool AtlasStage1Attached;
        bool AtlasStage3Attached;

        List<string> MenuElements = new List<string> {"Flight Plan", "Hardware", "Warnings"};
        List<string> QuickInfoElements = new List<string> {"SPD:", "ALT:", "GRV:"};
        List<string> QuickInfoDataElements = new List<string> {"N/A", "N/A", "N/A"};

        List<string> FlightPlanElements = new List<string> {"Next Stage", "Prev Stage", "Sel Stage", "Reset Plan"};
        List<string> FlightPlanOverviewElements = new List<string> {""};

        List<string> WarningElements = new List<string>();

        string DisplayA_Content = "Main Menu";
        string DisplayB_Content = "Main Menu";
        int SelectedIndexA = 0;
        int SelectedIndexB = 0;
        char DisplayIdentifierA = 'A';
        char DisplayIdentifierB = 'B';
        bool SelectedA = false;
        bool SelectedB = false;

        int AtlasTotalWarningCount = 0;
        int AtlasLVWarningCount = 0;
        int AtlasLLWarningCount = 0;
        int AtlasSCWarningCount = 0;

        // Necessary Blocks
        IMyProgrammableBlock Atlas_LL_Programmable_Block;
        IMyTextPanel Atlas_LL_AFC_Display_A1;
        IMyTextPanel Atlas_LL_AFC_Display_A2;
        IMyTextPanel Atlas_LL_AFC_Display_B1;
        IMyTextPanel Atlas_LL_AFC_Display_B2;
        IMyShipController Atlas_LL_Control_Seat_A;
        IMyShipController Atlas_LL_Control_Seat_B;

        // Launch Vehicle Blocks
        IMyGasTank Atlas_LV_Hydrogen_Tank_A;
        IMyGasTank Atlas_LV_Hydrogen_Tank_B;
        IMyBatteryBlock Atlas_LV_Small_Battery_1;
        IMyBatteryBlock Atlas_LV_Small_Battery_2;
        IMyBatteryBlock Atlas_LV_Small_Battery_3;
        IMyBatteryBlock Atlas_LV_Small_Battery_4;
        IMyParachute Atlas_LV_Parachute_Hatch_1;
        IMyParachute Atlas_LV_Parachute_Hatch_2;
        IMyParachute Atlas_LV_Parachute_Hatch_3;
        IMyParachute Atlas_LV_Parachute_Hatch_4;
        IMyThrust Atlas_LV_Primary_Launch_Thruster_A;
        IMyThrust Atlas_LV_Primary_Launch_Thruster_B;
        IMyThrust Atlas_LV_Primary_Launch_Thruster_C;
        IMyThrust Atlas_LV_Primary_Launch_Thruster_D;
        IMyShipMergeBlock Atlas_LV_Stage_2_Merge_Block_A;
        IMyShipMergeBlock Atlas_LV_Stage_2_Merge_Block_B;
        IMyShipMergeBlock Atlas_LV_Stage_2_Merge_Block_C;
        IMyShipMergeBlock Atlas_LV_Stage_2_Merge_Block_D;
        IMyShipConnector Atlas_LV_Fuel_Connector;
        IMyShipConnector Atlas_LV_Stage_1_Connector;
        IMyBeacon Atlas_LV_Reacquisition_Beacon;
        IMyConveyorSorter Atlas_LV_Parachute_Conveyor_Sorter_1;
        IMyConveyorSorter Atlas_LV_Parachute_Conveyor_Sorter_2;
        IMyConveyorSorter Atlas_LV_Parachute_Conveyor_Sorter_3;
        IMyConveyorSorter Atlas_LV_Parachute_Conveyor_Sorter_4;
        IMyPistonBase Atlas_LV_Landing_Piston_1;
        IMyPistonBase Atlas_LV_Landing_Piston_2;
        IMyPistonBase Atlas_LV_Landing_Piston_3;
        IMyPistonBase Atlas_LV_Landing_Piston_4;
        IMyLandingGear Atlas_LV_Magnetic_Plate_1;
        IMyLandingGear Atlas_LV_Magnetic_Plate_2;
        IMyLandingGear Atlas_LV_Magnetic_Plate_3;
        IMyLandingGear Atlas_LV_Magnetic_Plate_4;
        List<IMyTerminalBlock> LVBlocks = new List<IMyTerminalBlock>();
        string[] LVBlockNames;

        // Lunar Lander Blocks
        IMyGasTank Atlas_LL_Small_Hydrogen_Tank_A;
        IMyGasTank Atlas_LL_Small_Hydrogen_Tank_B;
        IMyGasTank Atlas_LL_Small_Hydrogen_Tank_C;
        IMyGyro Atlas_LL_Gyroscope_A;
        IMyAssembler Atlas_LL_Survival_Kit;
        IMyBatteryBlock Atlas_LL_Battery_A;
        IMyShipMergeBlock Atlas_LL_Stage_1_Merge_Block_A;
        IMyShipMergeBlock Atlas_LL_Stage_1_Merge_Block_B;
        IMyShipMergeBlock Atlas_LL_Stage_1_Merge_Block_C;
        IMyShipMergeBlock Atlas_LL_Stage_1_Merge_Block_D;
        IMyShipMergeBlock Atlas_LL_Stage_3_Merge_Block_A;
        IMyShipMergeBlock Atlas_LL_Stage_3_Merge_Block_B;
        IMyShipMergeBlock Atlas_LL_Stage_3_Merge_Block_C;
        IMyShipMergeBlock Atlas_LL_Stage_3_Merge_Block_D;
        IMyShipConnector Atlas_LL_Stage_1_Connector;
        IMyShipConnector Atlas_LL_Stage_3_Connector;
        IMyShipConnector Atlas_LL_Lunar_Rover_Connector;
        IMyThrust Atlas_LL_Hydrogen_Thruster_1;
        IMyThrust Atlas_LL_Hydrogen_Thruster_2;
        IMyThrust Atlas_LL_Hydrogen_Thruster_3;
        IMyThrust Atlas_LL_Hydrogen_Thruster_4;
        IMyThrust Atlas_LL_Hydrogen_Thruster_5;
        IMyThrust Atlas_LL_Hydrogen_Thruster_6;
        IMyThrust Atlas_LL_Hydrogen_Thruster_7;
        IMyThrust Atlas_LL_Hydrogen_Thruster_8;
        IMyThrust Atlas_LL_Hydrogen_Thruster_9;
        IMyRadioAntenna Atlas_LL_Antenna;
        IMyCargoContainer Atlas_LL_Medium_Cargo_Container_A;
        IMyGasTank Atlas_LL_Oxygen_Tank_A;
        IMyGasGenerator Atlas_LL_O2_H2_Generator;
        IMyPowerProducer Atlas_LL_Hydrogen_Engine;
        IMyLandingGear Atlas_LL_Magnetic_Plate_1;
        IMyLandingGear Atlas_LL_Magnetic_Plate_2;
        IMyLandingGear Atlas_LL_Magnetic_Plate_3;
        IMyLandingGear Atlas_LL_Magnetic_Plate_4;
        List<IMyTerminalBlock> LLBlocks = new List<IMyTerminalBlock>();
        string[] LLBlockNames;

        //Space Capsule Blocks

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
            Atlas_LL_Programmable_Block = Me;
            AssignBlockListNames();
        }

        public void Save() { Storage = $"{SelectedIndexA};{SelectedIndexB};{DisplayA_Content};{DisplayB_Content}"; }
        public void Load()
        {
            if (string.IsNullOrEmpty(Storage)) return;
            var parts = Storage.Split(';');
            if (parts.Length == 4)
            {
                int.TryParse(parts[0], out SelectedIndexA);
                int.TryParse(parts[1], out SelectedIndexB);
                DisplayA_Content = parts[2];
                DisplayB_Content = parts[3];
            }
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (!Initialized)
            {
                Initialized = InitializeSystems();
                return;
            }

            argument = argument.ToLower().Trim();

            ProcessArgumentsA(argument);
            ProcessArgumentsB(argument);

            if (argument == "swap")
            {
                string PlaceHolderContent = DisplayB_Content;
                DisplayB_Content = DisplayA_Content;
                DisplayA_Content = PlaceHolderContent;
                int IndexPlaceHolder = SelectedIndexB;
                SelectedIndexB = SelectedIndexA;
                SelectedIndexA = IndexPlaceHolder;
            }

            HardwareCheck();
            UpdateQuickInfo();

            DrawUIDisplay1(Atlas_LL_AFC_Display_A1, DisplayA_Content, SelectedIndexA, SelectedA, DisplayIdentifierA);
            DrawUIDisplay1(Atlas_LL_AFC_Display_B1, DisplayB_Content, SelectedIndexB, SelectedB, DisplayIdentifierB);

            DrawUIDisplay2(Atlas_LL_AFC_Display_A2, DisplayIdentifierA);
            DrawUIDisplay2(Atlas_LL_AFC_Display_B2, DisplayIdentifierB);
        }// Ends Main.

        void UpdateQuickInfo()
        {
            // Right column data
            QuickInfoDataElements[0] = $"{Atlas_LL_Control_Seat_A.GetShipSpeed():0} m/s";
            QuickInfoDataElements[2] = $"{(Atlas_LL_Control_Seat_A.GetNaturalGravity().Length() / 9.81):0.00} g";

            if (Atlas_LL_Control_Seat_A.TryGetPlanetElevation(MyPlanetElevation.Surface, out Altitude))
                QuickInfoDataElements[1] = $"{Altitude:0} m"; 
            else
                QuickInfoDataElements[1] = "N/A";
        }// Ends UpdateQuickInfo.

        void ProcessArgumentsA(string argument)
        {
            if (DisplayA_Content != "Main Menu")
            {
                if (argument == "returna") DisplayA_Content = "Main Menu";
            }

            if (argument == "upa")
            {
                SelectedIndexA--;
                if (SelectedIndexA < 0) SelectedIndexA = MenuElements.Count - 1;
            }
            else if (argument == "downa")
            {
                SelectedIndexA++;
                if (SelectedIndexA >= MenuElements.Count) SelectedIndexA = 0;
            }
            else if (argument == "selecta")
            {
                if (DisplayA_Content == "Main Menu")
                    DisplayA_Content = MenuElements[SelectedIndexA];
                else
                    SelectedA = true;
            }
        }// Ends ProcessArgumentsA.

        void ProcessArgumentsB(string argument)
        {
            if (DisplayB_Content != "Main Menu")
            {
                if (argument == "returnb") DisplayB_Content = "Main Menu";
            }

            if (argument == "upb")
            {
                SelectedIndexB--;
                if (SelectedIndexB < 0) SelectedIndexB = MenuElements.Count - 1;
            }
            else if (argument == "downb")
            {
                SelectedIndexB++;
                if (SelectedIndexB >= MenuElements.Count) SelectedIndexB = 0;
            }
            else if (argument == "selectb")
            {
                if (DisplayB_Content == "Main Menu")
                    DisplayB_Content = MenuElements[SelectedIndexB];
                else
                    SelectedB = true;
            }
        }// Ends ProcessArgumentsB

        void DrawUIDisplay2(IMyTextPanel Display, char DisplayIdentifier)
        {
            var surface = Display;
            surface.ContentType = ContentType.SCRIPT;
            surface.Script = "";

            Vector2 TextureSize = surface.TextureSize;
            Vector2 CanvasSize = surface.SurfaceSize;
            Vector2 ViewPortOffset = (TextureSize - CanvasSize) / 2;

            using (var frame = surface.DrawFrame())
            {

                float FontScale = 1.2f;
                Vector2 textSize = surface.MeasureStringInPixels(
                new StringBuilder("TextHeightScaleText"),
                "White",   // font name
                FontScale   // font scale
                );

                TextHeight = textSize.Y;
                float Padding = 12f;
                float LineCenterX = ViewPortOffset.X + (CanvasSize.X / 2f);
                float BoxWidth = 5f;
                float StartY = ViewPortOffset.Y + Padding; // begin at padding

                float ContentStartY = StartY + (TextHeight) + Padding + BoxWidth; //begin next line after text, with padding, including linewidth
                float VertLineHeight = (ViewPortOffset.Y + CanvasSize.Y) - ContentStartY; //Subtract page title line Y from whole canvas size
                float MiddleLineY = ContentStartY + (VertLineHeight / 2f);

                DrawUIStructureDisplay2(frame, ViewPortOffset, CanvasSize, ContentStartY, MiddleLineY, DisplayIdentifier);

            }
        }//Ends DrawUI.

        void DrawUIDisplay1(IMyTextPanel Display, string DisplayContent, int SelectedIndex, bool Selected, char DisplayIdentifier)
        {
            var surface = Display;
            surface.ContentType = ContentType.SCRIPT;
            surface.Script = "";

            Vector2 TextureSize = surface.TextureSize;
            Vector2 CanvasSize = surface.SurfaceSize;
            Vector2 ViewPortOffset = (TextureSize - CanvasSize) / 2;

            using (var frame = surface.DrawFrame())
            {

                float FontScale = 1.2f;
                Vector2 textSize = surface.MeasureStringInPixels(
                new StringBuilder(DisplayContent),
                "White",   // font name
                FontScale   // font scale
                );

                TextHeight = textSize.Y;
                float Padding = 12f;
                float LineCenterX = ViewPortOffset.X + (CanvasSize.X / 2f);
                float BoxWidth = 5f;
                float StartY = ViewPortOffset.Y + Padding; // begin at padding
                
                float ContentStartY = StartY + (TextHeight) + Padding + BoxWidth; //begin next line after text, with padding, including linewidth
                float VertLineHeight = (ViewPortOffset.Y + CanvasSize.Y) - ContentStartY; //Subtract page title line Y from whole canvas size
                float MiddleLineY = ContentStartY + (VertLineHeight / 2f);

                DrawUIStructureDisplay1(frame, ViewPortOffset, CanvasSize, DisplayContent, ContentStartY, MiddleLineY, DisplayIdentifier);

                DrawUIPanel2(frame, ViewPortOffset, CanvasSize, ContentStartY, MiddleLineY, SelectedIndex); //Quick Info Panel
                DrawUIPanel1(frame, ViewPortOffset, CanvasSize, DisplayContent, SelectedIndex, MiddleLineY, ContentStartY);
            }
        }//Ends DrawUIDisplay1.

        void DrawUIStructureDisplay1(MySpriteDrawFrame frame, Vector2 offset, Vector2 size, string DisplayContent, float ContentStartY, float MiddleLineY, char DisplayIdentifier)
        {
            float VertLineHeight = (offset.Y + size.Y) - ContentStartY; //Subtract page title line Y from whole canvas size
            float BoxWidth = 5f;
            float CanvasCenterX = offset.X + (size.X / 2f);
            Vector2 TextPosition = new Vector2(CanvasCenterX, offset.Y + Padding);
            float FontScale = 1.2f;


            frame.Add(new MySprite() // Vertical Divider
            {
                Type = SpriteType.TEXTURE,
                Data = "SquareSimple",
                Position = new Vector2((size.X / 2f) - 40f, MiddleLineY),
                Size = new Vector2(BoxWidth, VertLineHeight),
                Color = Color.White
            });

            // Horizontal center line
            frame.Add(new MySprite()
            {
                Type = SpriteType.TEXTURE,
                Data = "SquareSimple",
                Position = new Vector2(offset.X, MiddleLineY),
                Size = new Vector2((size.X / 2f) - 40f, BoxWidth),
                Color = Color.White
            });

            DrawUIStructureTitle(frame, offset, size, TextPosition, DisplayIdentifier, ContentStartY, DisplayContent, 1);
        }//Ends DrawUIStructure.

        void DrawUIStructureDisplay2(MySpriteDrawFrame frame, Vector2 offset, Vector2 size, float ContentStartY, float MiddleLineY, char DisplayIdentifier)
        {
            float VertLineHeight = (offset.Y + size.Y) - ContentStartY; //Subtract page title line Y from whole canvas size
            float CanvasCenterX = offset.X + (size.X / 2f);
            Vector2 TextPosition = new Vector2(CanvasCenterX, offset.Y + Padding);

            DrawUIStructureTitle(frame, offset, size, TextPosition, DisplayIdentifier, ContentStartY, ($"Status Page {DisplayIdentifier}"), 2);
        }//Ends DrawUIStructure.

        void DrawUIStructureTitle(MySpriteDrawFrame frame, Vector2 offset, Vector2 size, Vector2 TextPosition, char DisplayIdentifier, float ContentStartY, string TitleText, int DisplayNumber) {

            float BoxWidth = 5f;
            float FontScale = 1.2f;

            // Background fill
            frame.Add(new MySprite()
            {
                Type = SpriteType.TEXTURE,
                Data = "SquareSimple",
                Position = offset + (size / 2f),
                Size = size,
                Color = Color.Black
            });

            frame.Add(new MySprite() // Page Title
            {
                Type = SpriteType.TEXT,
                Data = TitleText,
                Position = TextPosition,
                RotationOrScale = FontScale,
                Color = Color.White,
                Alignment = TextAlignment.CENTER,
                FontId = "White"
            });

            frame.Add(new MySprite() // Page Display Identifier Right
            {
                Type = SpriteType.TEXT,
                Data = $"{DisplayIdentifier}{DisplayNumber}",
                Position = new Vector2((offset.X + size.X - (Padding * 3f)), TextPosition.Y),
                RotationOrScale = FontScale,
                Color = Color.White,
                Alignment = TextAlignment.CENTER,
                FontId = "White"
            });

            frame.Add(new MySprite() // Title Horizontal Line
            {
                Type = SpriteType.TEXTURE,
                Data = "SquareSimple",
                Position = new Vector2(offset.X, ContentStartY),
                Size = new Vector2(size.X, BoxWidth),
                Color = Color.White
            });
        }//Ends DrawUIStructureTitle.

        void DrawWarningsMenu(MySpriteDrawFrame frame, Vector2 offset, Vector2 size)
        {
            float Padding = 12f;
            float StartX1 = offset.X + Padding;
            float StartY = offset.Y + Padding;

            for (int i = 0; i < WarningElements.Count; i++)
            {
                float ItemTop = StartY + (i * TextHeight);
                float ItemCenterY = ItemTop + (TextHeight / 2f);
                float FontScale = 1.2f;
                Vector2 TextPosition = new Vector2(StartX1 + 20f, ItemCenterY - (FontScale * 8f));

                frame.Add(new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = WarningElements[i],
                    Position = TextPosition,
                    RotationOrScale = FontScale,
                    Color = AtlasTotalWarningCount > 0 ? Color.Red : Color.White,
                    Alignment = TextAlignment.LEFT,
                    FontId = "White"
                });
            }
        }//Ends Draw Warnings.

        void DrawFlightPlanMenu(MySpriteDrawFrame frame, Vector2 offset, Vector2 size, int SelectedIndex, float ContentStartY, float MiddleLineY)
        {
            float ItemPadding = 6f;
            float StartX = offset.X + Padding;
            //float MiddleLineY = offset.Y + size.Y / 2f;
            float TotalMenuHeight = ((4 * TextHeight) + (3 * ItemPadding));
            float MenuStartY = ContentStartY + ((MiddleLineY - ContentStartY - TotalMenuHeight) / 2);
            Vector2 StartPosition = new Vector2(StartX, MenuStartY);

            DrawUIPanel1List(frame, FlightPlanElements, StartPosition, SelectedIndex);

        }//Ends DrawFlightPlan.

        void DrawMainMenu(MySpriteDrawFrame frame, Vector2 offset, Vector2 size, int SelectedIndex, float ContentStartY, float MiddleLineY)
        {
            float ItemPadding = 6f;
            float StartX = offset.X + Padding;
            //float MiddleLineY = offset.Y + size.Y / 2f;
            float TotalMenuHeight = ((4 * TextHeight) + (3 * ItemPadding));
            float MenuStartY = ContentStartY + ((MiddleLineY - ContentStartY - TotalMenuHeight) / 2);
            Vector2 StartPosition = new Vector2(StartX, MenuStartY);

            DrawUIPanel1List(frame, MenuElements, StartPosition, SelectedIndex);
            Vector2[] DiagramListPositions = DrawRocketDiagram(frame, offset, size, ContentStartY, MiddleLineY);
            DrawUIPanel3ListContent(frame, offset, size, DiagramListPositions, SelectedIndex);
        } //Ends DrawMainMenu.

        Vector2[] DrawRocketDiagram(MySpriteDrawFrame frame, Vector2 offset, Vector2 size, float ContentStartY, float MiddleLineY)
        {
            // Lunar Lander Box Dimension Info
            Vector2 LLDiagramBoxDim = new Vector2(70f, 90f);

            // Space Capsule Box Dimension Info
            Vector2 SCDiagramBoxDim = new Vector2(50f, 100f);

            // Launch Vehicle Box Dimension Info
            Vector2 LVDiagramBoxDim = new Vector2(70f, 140f);

            // Thruster Boxes Dimension Info
            Vector2 ThrusterDiagramBoxDim = new Vector2(25f, 25f);

            // Diagram Spacing Math
            float TotalDiagramHeight = SCDiagramBoxDim.Y + LLDiagramBoxDim.Y + LVDiagramBoxDim.Y + ThrusterDiagramBoxDim.Y + 30f;
            float DiagramSpacing = ((size.Y - ContentStartY) - TotalDiagramHeight) /2f;
            float DiagramStartY = ContentStartY + DiagramSpacing;
            float HorizontalLineX = offset.X + (size.X / 2f) - 40f;
            float DiagramStartX = HorizontalLineX + (2f * Padding);

            // Space Capsule Box Position Info
            float SCDiagramBoxX = DiagramStartX + (LLDiagramBoxDim.X - SCDiagramBoxDim.X) / 2f;
            float SCDiagramBoxY = (DiagramStartY + (SCDiagramBoxDim.Y / 2f));
            Vector2 SCDiagramBoxPos = new Vector2(SCDiagramBoxX, SCDiagramBoxY);

            // Lunar Lander Box Position Info
            float LLDiagramBoxY = (SCDiagramBoxY + (SCDiagramBoxDim.Y / 2f) + (LLDiagramBoxDim.Y / 2f)) + 10f;
            Vector2 LLDiagramBoxPos = new Vector2(DiagramStartX, LLDiagramBoxY);

            // Launch Vehicle Box Position Info
            float LVDiagramBoxY = (LLDiagramBoxY + (LLDiagramBoxDim.Y / 2f) + (LVDiagramBoxDim.Y / 2f)) + 10f;
            Vector2 LVDiagramBoxPos = new Vector2(DiagramStartX, LVDiagramBoxY);

            // Thruster Boxes Position Info
            float ThrusterDiagramBoxSpacer = (LLDiagramBoxDim.X - (2f * ThrusterDiagramBoxDim.X)) / 3;
            float ThrusterDiagramBoxX1 = DiagramStartX + ThrusterDiagramBoxSpacer;
            float ThrusterDiagramBoxX2 = DiagramStartX + LLDiagramBoxDim.X - (ThrusterDiagramBoxSpacer + ThrusterDiagramBoxDim.X);
            float ThrusterDiagramBoxY = (LVDiagramBoxY + (LVDiagramBoxDim.Y / 2f) + (ThrusterDiagramBoxDim.Y / 2f)) + 10f;
            Vector2 ThrusterDiagramBoxPos1 = new Vector2(ThrusterDiagramBoxX1, ThrusterDiagramBoxY);
            Vector2 ThrusterDiagramBoxPos2 = new Vector2(ThrusterDiagramBoxX2, ThrusterDiagramBoxY);

            // Lunar Lander Diagram Box
            frame.Add(new MySprite()
            {
                Type = SpriteType.TEXTURE,
                Data = "SquareSimple",
                Position = LLDiagramBoxPos,
                Size = LLDiagramBoxDim,
                Color = (AtlasLLWarningCount > 0 ) ? Color.Red : Color.White,
            });

            // Space Capsule Diagram Box
            frame.Add(new MySprite()
            {
                Type = SpriteType.TEXTURE,
                Data = "SquareSimple",
                Position = SCDiagramBoxPos,
                Size = SCDiagramBoxDim,
                Color = (AtlasSCWarningCount > 0) ? Color.Red : Color.White,
            });

            // Launch Vehicle Diagram Box
            frame.Add(new MySprite()
            {
                Type = SpriteType.TEXTURE,
                Data = "SquareSimple",
                Position = LVDiagramBoxPos,
                Size = LVDiagramBoxDim,
                Color = (AtlasLVWarningCount > 0) ? Color.Red : Color.White,
            });

            // Thruster 1 Diagram Box
            frame.Add(new MySprite()
            {
                Type = SpriteType.TEXTURE,
                Data = "SquareSimple",
                Position = ThrusterDiagramBoxPos1,
                Size = ThrusterDiagramBoxDim,
                Color = (AtlasLVWarningCount > 0) ? Color.Red : Color.White
            });

            // Thruster 2 Diagram Box
            frame.Add(new MySprite()
            {
                Type = SpriteType.TEXTURE,
                Data = "SquareSimple",
                Position = ThrusterDiagramBoxPos2,
                Size = ThrusterDiagramBoxDim,
                Color = (AtlasLVWarningCount > 0) ? Color.Red : Color.White
            });

            float DiagramTextX = DiagramStartX + LLDiagramBoxDim.X + (Padding * 2f);
            Vector2[] DiagramListHeightValues = new Vector2[] { 
            new Vector2(DiagramTextX, DiagramStartY), //Space Capsule Text Pos
            new Vector2(DiagramTextX, SCDiagramBoxY + (SCDiagramBoxDim.Y / 2f)), //Lunar Lander Text Pos
            new Vector2(DiagramTextX, LLDiagramBoxY + (LLDiagramBoxDim.Y / 2f)), //Launch Vehicle Text Pos
            new Vector2(DiagramTextX, LVDiagramBoxY + (LVDiagramBoxDim.Y / 2f) ), //Thruster Text Pos
            };

            return DiagramListHeightValues;
        }//Ends DrawRocketDiagram

        void DrawUIPanel3ListContent(MySpriteDrawFrame frame, Vector2 offset, Vector2 size, Vector2[] DiagramListPositions, int SelectedIndex)
        {
            Vector2 DiagramBoxDim = new Vector2(20f, 20f);

            for (int i = 0; i < 3; i++) {
                // Power Label
                frame.Add(new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = "PWR",
                    Position = DiagramListPositions[i],
                    Alignment = TextAlignment.LEFT,
                    FontId = "White",
                    RotationOrScale = 1.1f,
                    Color = Color.White,
                });

                // Fuel Label
                frame.Add(new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = "FUEL",
                    Position = new Vector2(DiagramListPositions[i].X, DiagramListPositions[i].Y + TextHeight + 5f),
                    Alignment = TextAlignment.LEFT,
                    FontId = "White",
                    RotationOrScale = 1.1f,
                    Color = Color.White,
                });
            }

            float DiagramBoxX = offset.X + size.X - Padding - DiagramBoxDim.X;

            for (int i = 0; i < 3; i++)
            {
                float DiagramBoxPosY = DiagramListPositions[i].Y + (DiagramBoxDim.Y / 2f);

                // Power Box Right
                frame.Add(new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "SquareSimple",
                    Position = new Vector2(DiagramBoxX, (DiagramBoxPosY + 2.5f)),
                    Size = DiagramBoxDim,
                    Color = Color.Green,
                });

                // Fuel Box Right
                frame.Add(new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "SquareSimple",
                    Position = new Vector2(DiagramBoxX, (DiagramBoxPosY + TextHeight + 7.5f)),
                    Size = DiagramBoxDim,
                    Color = Color.Green,
                });
            }

        }//Ends DrawList.

        void DrawUIPanel2(MySpriteDrawFrame frame, Vector2 offset, Vector2 size, float ContentStartY, float MiddleLineY, int SelectedIndex)
        {
            float Padding = 12f;
            // Altitude, Speed, and warning indicator
            float ItemPadding = 6f;
            float QuickInfoStartX = offset.X + Padding;
            //float MiddleLineY = offset.Y + size.Y / 2f;
            float TotalMenuHeight = ((4f * TextHeight) + (3f * ItemPadding));
            float MenuStartY = ContentStartY + ((MiddleLineY - ContentStartY - TotalMenuHeight) / 2f);
            float VertLineHeight = (offset.Y + size.Y) - ContentStartY; //Subtract page title line Y from whole canvas size
            Vector2 StartPosition = new Vector2(QuickInfoStartX, MenuStartY + (VertLineHeight / 2f));

            float FontScale = 1.1f;

            // Quick Information
            for (int i = 0; i < QuickInfoElements.Count; i++)
            {
                float ItemTop = StartPosition.Y + (i * (TextHeight + ItemPadding));
                float ItemCenterY = ItemTop + (TextHeight / 2f);
                Vector2 TextPosition = new Vector2(StartPosition.X, ItemTop);

                //Quick Information Text
                frame.Add(new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = QuickInfoElements[i],
                    Position = new Vector2((TextPosition.X + Padding), TextPosition.Y),
                    RotationOrScale = FontScale,
                    Color = Color.White,
                    Alignment = TextAlignment.LEFT,
                    FontId = "White"
                });

                //Quick Information Data
                frame.Add(new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = QuickInfoDataElements[i],
                    Position = new Vector2((offset.X + (size.X / 2f) - (2 * Padding)) - 40f, TextPosition.Y),
                    RotationOrScale = FontScale,
                    Color = Color.White,
                    Alignment = TextAlignment.RIGHT,
                    FontId = "White"
                });
            }
        }//Ends DrawQuickInformation.

        void DrawUIPanel1(MySpriteDrawFrame frame, Vector2 offset, Vector2 size, string DisplayContent, int SelectedIndex, float MiddleLineY, float ContentStartY)
        {
            if (DisplayContent == "Main Menu")
                DrawMainMenu(frame, offset, size, SelectedIndex, ContentStartY, MiddleLineY);
            else if (DisplayContent == "Flight Plan")
                DrawFlightPlanMenu(frame, offset, size, SelectedIndex, ContentStartY, MiddleLineY);
            else if (DisplayContent == "Warnings")
                DrawWarningsMenu(frame, offset, size);
        }//Ends DrawUIPanel1.

        void DrawUIPanel1List(MySpriteDrawFrame frame, List<string> list, Vector2 StartPosition, int SelectedIndex) {

            float BoxWidth = 5f;
            float Padding = 12f;
            float FontScale = 1.2f;
            float ItemPadding = 6f;

            // Menu items
            for (int i = 0; i < list.Count; i++)
            {
                float ItemTop = StartPosition.Y + (i * (TextHeight + ItemPadding));
                float ItemCenterY = ItemTop + (TextHeight / 2f);
                bool isSelected = (i == SelectedIndex);
                Vector2 TextPosition = new Vector2(StartPosition.X, ItemTop);

                // Selection Cursor
                if (isSelected)
                {
                    frame.Add(new MySprite()
                    {
                        Type = SpriteType.TEXTURE,
                        Data = "SquareSimple",
                        Position = new Vector2(TextPosition.X, ItemCenterY),
                        Size = new Vector2(BoxWidth, TextHeight),
                        Color = Color.White
                    });
                }

                //Menu Text
                frame.Add(new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = (list[i] == "Warnings" && AtlasTotalWarningCount > 0)
                        ? list[i] + $" [{AtlasTotalWarningCount}]"
                        : list[i],
                    Position = new Vector2((TextPosition.X + Padding), TextPosition.Y),
                    RotationOrScale = FontScale,
                    Color = (list[i] == "Warnings" && AtlasTotalWarningCount > 0) ? Color.Red : Color.White,
                    Alignment = TextAlignment.LEFT,
                    FontId = "White"
                });
            }
        }//Ends DrawList.

        public bool InitializeSystems()
        {
            bool setupComplete = false;

            if (Atlas_LL_AFC_Display_A1 == null)
            {
                Echo("Missing Display A1");
                Atlas_LL_AFC_Display_A1 = GridTerminalSystem.GetBlockWithName("Atlas LL AFC Display A1") as IMyTextPanel;
            }

            if (Atlas_LL_AFC_Display_B1 == null)
            {
                Echo("Missing Display B1");
                Atlas_LL_AFC_Display_B1 = GridTerminalSystem.GetBlockWithName("Atlas LL AFC Display B1") as IMyTextPanel;
            }

            if (Atlas_LL_AFC_Display_A2 == null)
            {
                Echo("Missing Display A2");
                Atlas_LL_AFC_Display_A2 = GridTerminalSystem.GetBlockWithName("Atlas LL AFC Display A2") as IMyTextPanel;
            }

            if (Atlas_LL_AFC_Display_B2 == null)
            {
                Echo("Missing Display B2");
                Atlas_LL_AFC_Display_B2 = GridTerminalSystem.GetBlockWithName("Atlas LL AFC Display B2") as IMyTextPanel;
            }

            if (Atlas_LL_Control_Seat_A == null)
            {
                Echo("Missing Control Seat A");
                Atlas_LL_Control_Seat_A = GridTerminalSystem.GetBlockWithName("Atlas LL Control Seat A") as IMyShipController;
            }

            if (Atlas_LL_Control_Seat_B == null)
            {
                Echo("Missing Control Seat B");
                Atlas_LL_Control_Seat_B = GridTerminalSystem.GetBlockWithName("Atlas LL Control Seat B") as IMyShipController;
            }

            if (Atlas_LL_AFC_Display_A1 != null && Atlas_LL_AFC_Display_B1 != null && Atlas_LL_AFC_Display_A2 != null && Atlas_LL_AFC_Display_B2 != null && Atlas_LL_Control_Seat_A != null && Atlas_LL_Control_Seat_B != null)
                setupComplete = true;

            return setupComplete;
        } //Ends InitializeSystemss.

        public void HardwareCheck()
        {
            WarningElements.Clear();

            // Launch Vehicle Blocks
            Atlas_LV_Hydrogen_Tank_A = GridTerminalSystem.GetBlockWithName("Atlas LV Hydrogen Tank A") as IMyGasTank;
            Atlas_LV_Hydrogen_Tank_B = GridTerminalSystem.GetBlockWithName("Atlas LV Hydrogen Tank B") as IMyGasTank;
            Atlas_LV_Small_Battery_1 = GridTerminalSystem.GetBlockWithName("Atlas LV Small Battery 1") as IMyBatteryBlock;
            Atlas_LV_Small_Battery_2 = GridTerminalSystem.GetBlockWithName("Atlas LV Small Battery 2") as IMyBatteryBlock;
            Atlas_LV_Small_Battery_3 = GridTerminalSystem.GetBlockWithName("Atlas LV Small Battery 3") as IMyBatteryBlock;
            Atlas_LV_Small_Battery_4 = GridTerminalSystem.GetBlockWithName("Atlas LV Small Battery 4") as IMyBatteryBlock;
            Atlas_LV_Parachute_Hatch_1 = GridTerminalSystem.GetBlockWithName("Atlas LV Parachute Hatch 1") as IMyParachute;
            Atlas_LV_Parachute_Hatch_2 = GridTerminalSystem.GetBlockWithName("Atlas LV Parachute Hatch 2") as IMyParachute;
            Atlas_LV_Parachute_Hatch_3 = GridTerminalSystem.GetBlockWithName("Atlas LV Parachute Hatch 3") as IMyParachute;
            Atlas_LV_Parachute_Hatch_4 = GridTerminalSystem.GetBlockWithName("Atlas LV Parachute Hatch 4") as IMyParachute;
            Atlas_LV_Primary_Launch_Thruster_A = GridTerminalSystem.GetBlockWithName("Atlas LV Primary Launch Thruster A") as IMyThrust;
            Atlas_LV_Primary_Launch_Thruster_B = GridTerminalSystem.GetBlockWithName("Atlas LV Primary Launch Thruster B") as IMyThrust;
            Atlas_LV_Primary_Launch_Thruster_C = GridTerminalSystem.GetBlockWithName("Atlas LV Primary Launch Thruster C") as IMyThrust;
            Atlas_LV_Primary_Launch_Thruster_D = GridTerminalSystem.GetBlockWithName("Atlas LV Primary Launch Thruster D") as IMyThrust;
            Atlas_LV_Stage_2_Merge_Block_A = GridTerminalSystem.GetBlockWithName("Atlas LV Stage 2 Merge Block A") as IMyShipMergeBlock;
            Atlas_LV_Stage_2_Merge_Block_B = GridTerminalSystem.GetBlockWithName("Atlas LV Stage 2 Merge Block B") as IMyShipMergeBlock;
            Atlas_LV_Stage_2_Merge_Block_C = GridTerminalSystem.GetBlockWithName("Atlas LV Stage 2 Merge Block C") as IMyShipMergeBlock;
            Atlas_LV_Stage_2_Merge_Block_D = GridTerminalSystem.GetBlockWithName("Atlas LV Stage 2 Merge Block D") as IMyShipMergeBlock;
            Atlas_LV_Fuel_Connector = GridTerminalSystem.GetBlockWithName("Atlas LV Fuel Connector") as IMyShipConnector;
            Atlas_LV_Stage_1_Connector = GridTerminalSystem.GetBlockWithName("Atlas LV Stage 1 Connector") as IMyShipConnector;
            Atlas_LV_Reacquisition_Beacon = GridTerminalSystem.GetBlockWithName("Atlas LV Reaquisition Beacon") as IMyBeacon;
            Atlas_LV_Parachute_Conveyor_Sorter_1 = GridTerminalSystem.GetBlockWithName("Atlas LV Parachute Conveyor Sorter 1") as IMyConveyorSorter;
            Atlas_LV_Parachute_Conveyor_Sorter_2 = GridTerminalSystem.GetBlockWithName("Atlas LV Parachute Conveyor Sorter 2") as IMyConveyorSorter;
            Atlas_LV_Parachute_Conveyor_Sorter_3 = GridTerminalSystem.GetBlockWithName("Atlas LV Parachute Conveyor Sorter 3") as IMyConveyorSorter;
            Atlas_LV_Parachute_Conveyor_Sorter_4 = GridTerminalSystem.GetBlockWithName("Atlas LV Parachute Conveyor Sorter 4") as IMyConveyorSorter;
            Atlas_LV_Landing_Piston_1 = GridTerminalSystem.GetBlockWithName("Atlas LV Landing Piston 1") as IMyPistonBase;
            Atlas_LV_Landing_Piston_2 = GridTerminalSystem.GetBlockWithName("Atlas LV Landing Piston 2") as IMyPistonBase;
            Atlas_LV_Landing_Piston_3 = GridTerminalSystem.GetBlockWithName("Atlas LV Landing Piston 3") as IMyPistonBase;
            Atlas_LV_Landing_Piston_4 = GridTerminalSystem.GetBlockWithName("Atlas LV Landing Piston 4") as IMyPistonBase;
            Atlas_LV_Magnetic_Plate_1 = GridTerminalSystem.GetBlockWithName("Atlas LV Magnetic Plate 1") as IMyLandingGear;
            Atlas_LV_Magnetic_Plate_2 = GridTerminalSystem.GetBlockWithName("Atlas LV Magnetic Plate 2") as IMyLandingGear;
            Atlas_LV_Magnetic_Plate_3 = GridTerminalSystem.GetBlockWithName("Atlas LV Magnetic Plate 3") as IMyLandingGear;
            Atlas_LV_Magnetic_Plate_4 = GridTerminalSystem.GetBlockWithName("Atlas LV Magnetic Plate 4") as IMyLandingGear;

            // Launch Vehicle Block List
            LVBlocks.Clear();
            LVBlocks.Add(Atlas_LV_Hydrogen_Tank_A);
            LVBlocks.Add(Atlas_LV_Hydrogen_Tank_B);
            LVBlocks.Add(Atlas_LV_Small_Battery_1);
            LVBlocks.Add(Atlas_LV_Small_Battery_2);
            LVBlocks.Add(Atlas_LV_Small_Battery_3);
            LVBlocks.Add(Atlas_LV_Small_Battery_4);
            LVBlocks.Add(Atlas_LV_Parachute_Hatch_1);
            LVBlocks.Add(Atlas_LV_Parachute_Hatch_2);
            LVBlocks.Add(Atlas_LV_Parachute_Hatch_3);
            LVBlocks.Add(Atlas_LV_Parachute_Hatch_4);
            LVBlocks.Add(Atlas_LV_Primary_Launch_Thruster_A);
            LVBlocks.Add(Atlas_LV_Primary_Launch_Thruster_B);
            LVBlocks.Add(Atlas_LV_Primary_Launch_Thruster_C);
            LVBlocks.Add(Atlas_LV_Primary_Launch_Thruster_D);
            LVBlocks.Add(Atlas_LV_Stage_2_Merge_Block_A);
            LVBlocks.Add(Atlas_LV_Stage_2_Merge_Block_B);
            LVBlocks.Add(Atlas_LV_Stage_2_Merge_Block_C);
            LVBlocks.Add(Atlas_LV_Stage_2_Merge_Block_D);
            LVBlocks.Add(Atlas_LV_Fuel_Connector);
            LVBlocks.Add(Atlas_LV_Stage_1_Connector);
            LVBlocks.Add(Atlas_LV_Reacquisition_Beacon);
            LVBlocks.Add(Atlas_LV_Parachute_Conveyor_Sorter_1);
            LVBlocks.Add(Atlas_LV_Parachute_Conveyor_Sorter_2);
            LVBlocks.Add(Atlas_LV_Parachute_Conveyor_Sorter_3);
            LVBlocks.Add(Atlas_LV_Parachute_Conveyor_Sorter_4);
            LVBlocks.Add(Atlas_LV_Landing_Piston_1);
            LVBlocks.Add(Atlas_LV_Landing_Piston_2);
            LVBlocks.Add(Atlas_LV_Landing_Piston_3);
            LVBlocks.Add(Atlas_LV_Landing_Piston_4);
            LVBlocks.Add(Atlas_LV_Magnetic_Plate_1);
            LVBlocks.Add(Atlas_LV_Magnetic_Plate_2);
            LVBlocks.Add(Atlas_LV_Magnetic_Plate_3);
            LVBlocks.Add(Atlas_LV_Magnetic_Plate_4);
            CheckFunctionality(LVBlocks, 1);

            // Lunar Lander Blocks
            Atlas_LL_Small_Hydrogen_Tank_A = GridTerminalSystem.GetBlockWithName("Atlas LL Small Hydrogen Tank A") as IMyGasTank;
            Atlas_LL_Small_Hydrogen_Tank_B = GridTerminalSystem.GetBlockWithName("Atlas LL Small Hydrogen Tank B") as IMyGasTank;
            Atlas_LL_Small_Hydrogen_Tank_C = GridTerminalSystem.GetBlockWithName("Atlas LL Small Hydrogen Tank C") as IMyGasTank;
            Atlas_LL_Gyroscope_A = GridTerminalSystem.GetBlockWithName("Atlas LL Gyroscope A") as IMyGyro;
            Atlas_LL_Survival_Kit = GridTerminalSystem.GetBlockWithName("Atlas LL Survival Kit") as IMyAssembler;
            Atlas_LL_Battery_A = GridTerminalSystem.GetBlockWithName("Atlas LL Battery A") as IMyBatteryBlock;
            Atlas_LL_Stage_1_Merge_Block_A = GridTerminalSystem.GetBlockWithName("Atlas LL Stage 1 Merge Block A") as IMyShipMergeBlock;
            Atlas_LL_Stage_1_Merge_Block_B = GridTerminalSystem.GetBlockWithName("Atlas LL Stage 1 Merge Block B") as IMyShipMergeBlock;
            Atlas_LL_Stage_1_Merge_Block_C = GridTerminalSystem.GetBlockWithName("Atlas LL Stage 1 Merge Block C") as IMyShipMergeBlock;
            Atlas_LL_Stage_1_Merge_Block_D = GridTerminalSystem.GetBlockWithName("Atlas LL Stage 1 Merge Block D") as IMyShipMergeBlock;
            Atlas_LL_Stage_3_Merge_Block_A = GridTerminalSystem.GetBlockWithName("Atlas LL Stage 3 Merge Block A") as IMyShipMergeBlock;
            Atlas_LL_Stage_3_Merge_Block_B = GridTerminalSystem.GetBlockWithName("Atlas LL Stage 3 Merge Block B") as IMyShipMergeBlock;
            Atlas_LL_Stage_3_Merge_Block_C = GridTerminalSystem.GetBlockWithName("Atlas LL Stage 3 Merge Block C") as IMyShipMergeBlock;
            Atlas_LL_Stage_3_Merge_Block_D = GridTerminalSystem.GetBlockWithName("Atlas LL Stage 3 Merge Block D") as IMyShipMergeBlock;
            Atlas_LL_Stage_1_Connector = GridTerminalSystem.GetBlockWithName("Atlas LL Stage 1 Connector") as IMyShipConnector;
            Atlas_LL_Stage_3_Connector = GridTerminalSystem.GetBlockWithName("Atlas LL Stage 3 Connector") as IMyShipConnector;
            Atlas_LL_Lunar_Rover_Connector = GridTerminalSystem.GetBlockWithName("Atlas LL Lunar Rover Connector") as IMyShipConnector;
            Atlas_LL_Hydrogen_Thruster_1 = GridTerminalSystem.GetBlockWithName("Atlas LL Hydrogen Thruster 1") as IMyThrust;
            Atlas_LL_Hydrogen_Thruster_2 = GridTerminalSystem.GetBlockWithName("Atlas LL Hydrogen Thruster 2") as IMyThrust;
            Atlas_LL_Hydrogen_Thruster_3 = GridTerminalSystem.GetBlockWithName("Atlas LL Hydrogen Thruster 3") as IMyThrust;
            Atlas_LL_Hydrogen_Thruster_4 = GridTerminalSystem.GetBlockWithName("Atlas LL Hydrogen Thruster 4") as IMyThrust;
            Atlas_LL_Hydrogen_Thruster_5 = GridTerminalSystem.GetBlockWithName("Atlas LL Hydrogen Thruster 5") as IMyThrust;
            Atlas_LL_Hydrogen_Thruster_6 = GridTerminalSystem.GetBlockWithName("Atlas LL Hydrogen Thruster 6") as IMyThrust;
            Atlas_LL_Hydrogen_Thruster_7 = GridTerminalSystem.GetBlockWithName("Atlas LL Hydrogen Thruster 7") as IMyThrust;
            Atlas_LL_Hydrogen_Thruster_8 = GridTerminalSystem.GetBlockWithName("Atlas LL Hydrogen Thruster 8") as IMyThrust;
            Atlas_LL_Hydrogen_Thruster_9 = GridTerminalSystem.GetBlockWithName("Atlas LL Hydrogen Thruster 9") as IMyThrust;
            Atlas_LL_Antenna = GridTerminalSystem.GetBlockWithName("Atlas LL Antenna") as IMyRadioAntenna;
            Atlas_LL_Medium_Cargo_Container_A = GridTerminalSystem.GetBlockWithName("Atlas LL Medium Cargo Container A") as IMyCargoContainer;
            Atlas_LL_Oxygen_Tank_A = GridTerminalSystem.GetBlockWithName("Atlas LL Oxygen Tank A") as IMyGasTank;
            Atlas_LL_O2_H2_Generator = GridTerminalSystem.GetBlockWithName("Atlas LL O2/H2 Generator") as IMyGasGenerator;
            Atlas_LL_Hydrogen_Engine = GridTerminalSystem.GetBlockWithName("Atlas LL Hydrogen Engine") as IMyPowerProducer;
            Atlas_LL_Magnetic_Plate_1 = GridTerminalSystem.GetBlockWithName("Atlas LL Magnetic Plate 1") as IMyLandingGear;
            Atlas_LL_Magnetic_Plate_2 = GridTerminalSystem.GetBlockWithName("Atlas LL Magnetic Plate 2") as IMyLandingGear;
            Atlas_LL_Magnetic_Plate_3 = GridTerminalSystem.GetBlockWithName("Atlas LL Magnetic Plate 3") as IMyLandingGear;
            Atlas_LL_Magnetic_Plate_4 = GridTerminalSystem.GetBlockWithName("Atlas LL Magnetic Plate 4") as IMyLandingGear;

            // Lunar Lander Block List
            LLBlocks.Clear();
            LLBlocks.Add(Atlas_LL_Small_Hydrogen_Tank_A);
            LLBlocks.Add(Atlas_LL_Small_Hydrogen_Tank_B);
            LLBlocks.Add(Atlas_LL_Small_Hydrogen_Tank_C);
            LLBlocks.Add(Atlas_LL_Gyroscope_A);
            LLBlocks.Add(Atlas_LL_Survival_Kit);
            LLBlocks.Add(Atlas_LL_Battery_A);
            LLBlocks.Add(Atlas_LL_Stage_1_Merge_Block_A);
            LLBlocks.Add(Atlas_LL_Stage_1_Merge_Block_B);
            LLBlocks.Add(Atlas_LL_Stage_1_Merge_Block_C);
            LLBlocks.Add(Atlas_LL_Stage_1_Merge_Block_D);
            LLBlocks.Add(Atlas_LL_Stage_3_Merge_Block_A);
            LLBlocks.Add(Atlas_LL_Stage_3_Merge_Block_B);
            LLBlocks.Add(Atlas_LL_Stage_3_Merge_Block_C);
            LLBlocks.Add(Atlas_LL_Stage_3_Merge_Block_D);
            LLBlocks.Add(Atlas_LL_Stage_1_Connector);
            LLBlocks.Add(Atlas_LL_Stage_3_Connector);
            LLBlocks.Add(Atlas_LL_Lunar_Rover_Connector);
            LLBlocks.Add(Atlas_LL_Hydrogen_Thruster_1);
            LLBlocks.Add(Atlas_LL_Hydrogen_Thruster_2);
            LLBlocks.Add(Atlas_LL_Hydrogen_Thruster_3);
            LLBlocks.Add(Atlas_LL_Hydrogen_Thruster_4);
            LLBlocks.Add(Atlas_LL_Hydrogen_Thruster_5);
            LLBlocks.Add(Atlas_LL_Hydrogen_Thruster_6);
            LLBlocks.Add(Atlas_LL_Hydrogen_Thruster_7);
            LLBlocks.Add(Atlas_LL_Hydrogen_Thruster_8);
            LLBlocks.Add(Atlas_LL_Hydrogen_Thruster_9);
            LLBlocks.Add(Atlas_LL_Antenna);
            LLBlocks.Add(Atlas_LL_Medium_Cargo_Container_A);
            LLBlocks.Add(Atlas_LL_Oxygen_Tank_A);
            LLBlocks.Add(Atlas_LL_O2_H2_Generator);
            LLBlocks.Add(Atlas_LL_Hydrogen_Engine);
            LLBlocks.Add(Atlas_LL_Magnetic_Plate_1);
            LLBlocks.Add(Atlas_LL_Magnetic_Plate_2);
            LLBlocks.Add(Atlas_LL_Magnetic_Plate_3);
            LLBlocks.Add(Atlas_LL_Magnetic_Plate_4);
            CheckFunctionality(LLBlocks, 2);

            AtlasTotalWarningCount = AtlasLVWarningCount + AtlasLLWarningCount + AtlasSCWarningCount;
        }//Ends HardwareCheck.

        void AssignBlockListNames() {
            LLBlockNames = new string[] {
                "Atlas LL Small Hydrogen Tank A",
                "Atlas LL Small Hydrogen Tank B",
                "Atlas LL Small Hydrogen Tank C",
                "Atlas LL Gyroscope A",
                "Atlas LL Survival Kit",
                "Atlas LL Battery A",
                "Atlas LL Stage 1 Merge Block A",
                "Atlas LL Stage 1 Merge Block B",
                "Atlas LL Stage 1 Merge Block C",
                "Atlas LL Stage 1 Merge Block D",
                "Atlas LL Stage 3 Merge Block A",
                "Atlas LL Stage 3 Merge Block B",
                "Atlas LL Stage 3 Merge Block C",
                "Atlas LL Stage 3 Merge Block D",
                "Atlas LL Stage 1 Connector",
                "Atlas LL Stage 3 Connector",
                "Atlas LL Lunar Rover Connector",
                "Atlas LL Hydrogen Thruster 1",
                "Atlas LL Hydrogen Thruster 2",
                "Atlas LL Hydrogen Thruster 3",
                "Atlas LL Hydrogen Thruster 4",
                "Atlas LL Hydrogen Thruster 5",
                "Atlas LL Hydrogen Thruster 6",
                "Atlas LL Hydrogen Thruster 7",
                "Atlas LL Hydrogen Thruster 8",
                "Atlas LL Hydrogen Thruster 9",
                "Atlas LL Antenna",
                "Atlas LL Medium Cargo Container A",
                "Atlas LL Oxygen Tank A",
                "Atlas LL O2 H2 Generator",
                "Atlas LL Hydrogen Engine",
                "Atlas LL Magnetic Plate 1",
                "Atlas LL Magnetic Plate 2",
                "Atlas LL Magnetic Plate 3",
                "Atlas LL Magnetic Plate 4"
            };

            LVBlockNames = new string[] {
                "Atlas LV Hydrogen Tank A",
                "Atlas LV Hydrogen Tank B",
                "Atlas LV Small Battery 1",
                "Atlas LV Small Battery 2",
                "Atlas LV Small Battery 3",
                "Atlas LV Small Battery 4",
                "Atlas LV Parachute Hatch 1",
                "Atlas LV Parachute Hatch 2",
                "Atlas LV Parachute Hatch 3",
                "Atlas LV Parachute Hatch 4",
                "Atlas LV Primary Launch Thruster A",
                "Atlas LV Primary Launch Thruster B",
                "Atlas LV Primary Launch Thruster C",
                "Atlas LV Primary Launch Thruster D",
                "Atlas LV Stage 2 Merge Block A",
                "Atlas LV Stage 2 Merge Block B",
                "Atlas LV Stage 2 Merge Block C",
                "Atlas LV Stage 2 Merge Block D",
                "Atlas LV Fuel Connector",
                "Atlas LV Stage 1 Connector",
                "Atlas LV Reacquisition Beacon",
                "Atlas LV Parachute Conveyor Sorter 1",
                "Atlas LV Parachute Conveyor Sorter 2",
                "Atlas LV Parachute Conveyor Sorter 3",
                "Atlas LV Parachute Conveyor Sorter 4",
                "Atlas LV Landing Piston 1",
                "Atlas LV Landing Piston 2",
                "Atlas LV Landing Piston 3",
                "Atlas LV Landing Piston 4",
                "Atlas LV Magnetic Plate 1",
                "Atlas LV Magnetic Plate 2",
                "Atlas LV Magnetic Plate 3",
                "Atlas LV Magnetic Plate 4"
            };
        }// Ends AssignBlockNames.

        void CheckFunctionality(List<IMyTerminalBlock> Blocks, int StageNum) {
            int ErrorCount = 0;

            string[] BlockNames = null;

            if (StageNum == 1)
            {
                BlockNames = LVBlockNames;
            }
            else if (StageNum == 2)
            {
                BlockNames = LLBlockNames;
            }
            else if (StageNum == 3) {
                BlockNames = LLBlockNames;
            }

                for (int i = 0; i < Blocks.Count; i++)
                {
                    if (Blocks[i] == null)
                    {
                        ErrorCount++;
                        WarningElements.Add($"{BlockNames[i]} is Missing.");
                    }
                    else if (!Blocks[i].IsFunctional)
                    {
                        ErrorCount++;
                        WarningElements.Add($"{BlockNames[i]} Critical Failure.");
                    }
                }

            if (StageNum == 1)
            {
                AtlasLVWarningCount = ErrorCount;
            }
            else if (StageNum == 2)
            {
                AtlasLLWarningCount = ErrorCount;
            }
            else if (StageNum == 3){
                AtlasSCWarningCount = ErrorCount;
            }

        }//Ends CheckFunctionality.
    }
}