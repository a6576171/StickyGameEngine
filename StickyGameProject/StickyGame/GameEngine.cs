using System;
using System.Text;
using System.Threading;

namespace StickyGame
{
    public class GameEngine
    {
        static Random R = new Random();

        // up 0, down 1, left 2, right 3, jump 4. Use wisely.
        static int CurrentCommand = -1;
        static string[] Man_Walk1 = new string[] { " 0 ", "!T!", " | " }; // boneco template 1
        static string[] Man_Walk2 = new string[] { " 0 ", "!T!", "/ \\" }; // bpneco template 2
        static string[] Man_Jump = new string[] { " 0 ", "<T/", "/ >" }; // bpneco template 3 (jump)
        static bool WalkSwitch; // animation switch for boneco walking
        static int ManPositionOnFloor = Console.BufferWidth / 6; // how far from left boneco is
        static int BaseFloorHeight = 20; // ground position on Y
        static int BonecoFloorHeight = 20; // boneco position on Y
        static string[] WholeFloor = new string[1024 * 16];
        static int MovementOffset = 0; // how far from starting point is boneco
        static bool ShowMessage = false; // message switch
        static string Message = string.Empty; // which message will show must be attached here
        static bool IsBonecoJumping = false; // switch for jump draw
        static int BonecoJumpHeight = 0; // up and down

        private static void debug_ShowHUD()
        {
            int hudX = 60;
            int hudY = 0;

            int x = Console.CursorLeft;
            int y = Console.CursorTop;

            string commandText = string.Empty;

            if (CurrentCommand == 0) commandText = "Up";
            if (CurrentCommand == 1) commandText = "Down";
            if (CurrentCommand == 2) commandText = "Left";
            if (CurrentCommand == 3) commandText = "Right";
            if (CurrentCommand == 4) commandText = "Space bar";
            if (CurrentCommand == -1) commandText = "Idle";

            Console.SetCursorPosition(hudX, hudY);
            Console.Write(new string(' ', 22));
            Console.SetCursorPosition(hudX, hudY);
            Console.Write($"Command: {CurrentCommand} ({commandText})");

            Console.SetCursorPosition(x, y);
        }
        public static void Run()
        {
            Console.CursorVisible = false; 
            
            generateWholeGround();
            drawBoneco_Walking();
            Message = "Run, Forest!";
            int messageCycles = 0;
            ShowMessage = true;

            bool switchJump = false;

            while (true)
            {
                getCommand();
                debug_ShowHUD();
                drawGround();
                milestones();

                if (IsBonecoJumping)
                {
                    CurrentCommand = 4;

                    if (!switchJump)
                    {
                        BonecoJumpHeight++;
                        BonecoFloorHeight--;
                    }
                    if (switchJump)
                    {
                        BonecoJumpHeight--;
                        BonecoFloorHeight++;
                    }

                    if (BonecoJumpHeight == 4) switchJump = !switchJump;

                    if (BonecoJumpHeight <= 0)
                    {
                        IsBonecoJumping = false;
                        CurrentCommand = -1;
                        BonecoJumpHeight = 0;
                        switchJump = !switchJump;
                    }
                }

                switch (CurrentCommand)
                {
                    case 4:
                        if (IsBonecoJumping) drawBoneco_Jumping();
                        IsBonecoJumping = true;
                        break;
                    default:
                        if(!IsBonecoJumping) drawBoneco_Walking();
                        break;
                }

                if(ShowMessage)
                {
                    messageCycles++;
                    if (messageCycles >= 30)
                    {
                        messageCycles = 0;
                        ShowMessage = false;
                    }
                }

                MovementOffset++;
                draw_Message(Message);
                Thread.Sleep(50);
            }
        }
        private static void milestones()
        {
            if(MovementOffset % 100 == 0 && MovementOffset > 0)
            {
                ShowMessage = true;
                Message = $"You've reached {MovementOffset} meters!";
            }
        }
        private static void draw_Message(string message)
        {
            Console.SetCursorPosition(ManPositionOnFloor, BaseFloorHeight - 9);

            if (!ShowMessage) message = new string(' ', Console.BufferWidth);
            Console.Write(message);
        }
        private static void generateWholeGround()
        {
            var currentProgress = 0;
            int total = WholeFloor.Length;

            for (int i = 0; i < WholeFloor.Length; i++)
            {
                currentProgress++;
                double value = ((double)currentProgress / (double)total);

                Console.SetCursorPosition(0, 0);
                Console.Write($"Generating scenery... {value.ToString("P1")}");

                int val = R.Next(0, 3);

                string char0 = "-";
                string char1 = ".";
                string char2 = "º";
                string char3 = " ";

                string selected = "*";
                if (val == 0) selected = char0;
                if (val == 1) selected = char1;
                if (val == 2) selected = char2;
                if (val == 3) selected = char3;

                WholeFloor[i] = selected;
            }

            Console.Clear();
        }
        private static void drawGround()
        {
            if (MovementOffset < 0)
            {
                MovementOffset = 0;
            }

            StringBuilder sb = new StringBuilder();

            for (int i = MovementOffset; i < Console.BufferWidth + MovementOffset; i++)
            {
                sb.Append(WholeFloor[i]);
            }

            string currentFloor = sb.ToString();

            Console.SetCursorPosition(0, BaseFloorHeight);
            Console.Write(currentFloor);
        }
        private static void drawBoneco_Jumping()
        {
            for (int i = 0; i < 6; i++)
            {
                Console.SetCursorPosition(ManPositionOnFloor, BonecoFloorHeight - i);
                Console.Write(new string(' ', 3));
            }

            Console.SetCursorPosition(ManPositionOnFloor, BonecoFloorHeight - 3);
            Console.WriteLine(Man_Jump[0]);
            Console.Write(new string(' ', ManPositionOnFloor));
            Console.WriteLine(Man_Jump[1]);
            Console.Write(new string(' ', ManPositionOnFloor));
            Console.WriteLine(Man_Jump[2]);
            Console.Write(new string(' ', ManPositionOnFloor));
        }
        private static void drawBoneco_Walking()
        {
            for (int i = 1; i < 5; i++)
            {  
                Console.SetCursorPosition(ManPositionOnFloor, BonecoFloorHeight - i);
                Console.Write(new string(' ', 3));
            }

            Console.SetCursorPosition(ManPositionOnFloor, BonecoFloorHeight - 3);
            if (WalkSwitch)
            {
                Console.WriteLine(Man_Walk1[0]);
                Console.Write(new string(' ', ManPositionOnFloor));
                Console.WriteLine(Man_Walk1[1]);
                Console.Write(new string(' ', ManPositionOnFloor));
                Console.WriteLine(Man_Walk1[2]);
            }
            else
            {
                Console.WriteLine(Man_Walk2[0]);
                Console.Write(new string(' ', ManPositionOnFloor));
                Console.WriteLine(Man_Walk2[1]);
                Console.Write(new string(' ', ManPositionOnFloor));
                Console.WriteLine(Man_Walk2[2]);
            }

            WalkSwitch = !WalkSwitch;
        }
        private static void getCommand()
        {
            CurrentCommand = -1;

            if (Console.KeyAvailable)
            {
                var keyPressed = Console.ReadKey(true).Key;

                switch (keyPressed)
                {
                    case ConsoleKey.UpArrow:
                        CurrentCommand = 0;
                        break;
                    case ConsoleKey.DownArrow:
                        CurrentCommand = 1;
                        break;
                    case ConsoleKey.LeftArrow:
                        CurrentCommand = 2;
                        break;
                    case ConsoleKey.RightArrow:
                        CurrentCommand = 3;
                        break;
                    case ConsoleKey.Spacebar:
                        CurrentCommand = 4;
                        break;
                }
            }
        }
    }
}
