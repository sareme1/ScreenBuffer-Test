using System.Diagnostics;
using System.Drawing;

using System.Runtime.InteropServices;

namespace View
{
    struct COORD {
        public short X;
        public short Y;
        public COORD(short x, short y) {
            X = x; Y = y; 
        }
    }
    internal class Program
    {
        static ulong GENERIC_READ = 0x80000000L;
        static ulong GENERIC_WRITE = 0x40000000L;

        static uint FILE_SHARE_READ = 0x00000001;
        static uint FILE_SHARE_WRITE = 0x00000002;

        static uint CONSOLE_TEXTMODE_BUFFER = 1;

        static int STD_OUTPUT_HANDLE = -11;

        private void simple() {
            int winHeight = Console.WindowHeight;
            int winWidth = Console.WindowWidth;
            char[,] buffer = new char[winHeight, winWidth];

            int row = winHeight / 2;
            int col = winWidth / 2;
            short dir = 1;
            while (true) {

                for (int y = 0; y < winHeight; y++)
                    for (int x = 0; x < winWidth; x++)
                        buffer[y, x] = ' ';

                buffer[row, col] = 'H';
                buffer[row, col + 1] = 'E';
                buffer[row, col + 2] = 'L';
                buffer[row, col + 3] = 'L';
                buffer[row, col + 4] = '0';

                Console.CursorVisible = false;
                Console.SetCursorPosition(0, 0);
                for (int y = 0; y < winHeight; y++)
                    for (int x = 0; x < winWidth; x++)
                        Console.Write(buffer[y, x]);

                row += dir;
                if (row >= winHeight - 1 || row <= 1) {
                    dir *= -1;
                }
            }
        }

        // Ref: https://learn.microsoft.com/en-us/windows/console/createconsolescreenbuffer
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateConsoleScreenBuffer(
            ulong dwDesiredAccess,
            uint dwSharedMode,
            IntPtr securityAttributes,
            uint flags,
            IntPtr screenBufferData
        );

        // Ref: https://learn.microsoft.com/en-us/windows/console/getstdhandle
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int handle);

        // Ref: https://learn.microsoft.com/en-us/windows/console/setconsoleactivescreenbuffer
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleActiveScreenBuffer(IntPtr handle);

        // Ref: https://learn.microsoft.com/en-us/windows/console/writeconsole
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteConsole(
            IntPtr handle,
            string data,
            int nrOfCharToWrite,
            out int nrOfCharactersWritten,
            IntPtr reserverd
        );
        // Ref: https://learn.microsoft.com/en-us/windows/console/setconsolecursorposition
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleCursorPosition(
            IntPtr handle,
            COORD cursorPos
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool FillConsoleOutputCharacter(
            IntPtr handle,
            char c,
            int count,
            COORD startPos,
            out int charWritten
            );

        //[DllImport("kernel32.dll", SetLastError = true)]
        //static extern bool SetStdHandle(int stdHandle, IntPtr handle);

        public void fancy() {
            int winHeight = Console.WindowHeight;
            int winWidth = Console.WindowWidth;

            IntPtr[] buffers = new IntPtr[2];
            char[] buffer = new char[winHeight * winWidth];

            IntPtr ptr = CreateConsoleScreenBuffer(
                GENERIC_READ | GENERIC_WRITE,
                FILE_SHARE_READ | FILE_SHARE_WRITE,
                IntPtr.Zero,
                CONSOLE_TEXTMODE_BUFFER,
                IntPtr.Zero
            );

            Console.SetBufferSize(winWidth, winHeight);

            IntPtr stdPtr = GetStdHandle(STD_OUTPUT_HANDLE);

            buffers[0] = stdPtr;
            buffers[1] = ptr;

            IntPtr backBuffer = buffers[1];

            //bool d = SetStdHandle(STD_OUTPUT_HANDLE, ptr);
            //string data = "test\n";
            //WriteConsole(ptr, data, data.Length, out int _, IntPtr.Zero);

            Stopwatch stopwatch = new();

            int row = winHeight / 2;
            int col = winWidth / 2;
            short dir = 1;

            int fps = 90;
            double ms = 1000.0 / fps;

            COORD topLeft = new COORD(0, 0);

            while (true) {
                stopwatch.Restart();

                for (int y = 0; y < winHeight; y++)
                    for (int x = 0; x < winWidth; x++)
                        buffer[y * winWidth + x] = ' ';

                buffer[row * winWidth + col] = 'H';
                buffer[row * winWidth + col + 1] = 'E';
                buffer[row * winWidth + col + 2] = 'L';
                buffer[row * winWidth + col + 3] = 'L';
                buffer[row * winWidth + col + 4] = '0';

                SetConsoleCursorPosition(backBuffer, topLeft);
                string val = new string(buffer);
                WriteConsole(backBuffer, val, val.Length, out int _, IntPtr.Zero);
                                
                row += dir;
                if (row >= winHeight - 1 || row <= 1) {
                    dir *= -1;
                }

                if (stopwatch.ElapsedMilliseconds < ms) {
                    Thread.Sleep((int)Math.Max(0.0, (ms - stopwatch.ElapsedMilliseconds)));
                }

                SetConsoleActiveScreenBuffer(backBuffer);
                int newBackBuffIdx = (Array.FindIndex(buffers, m => m.Equals(backBuffer)) + 1) % 2;
                backBuffer = buffers[newBackBuffIdx];

            }

            //Console.WriteLine(ptr);
            //Console.WriteLine(stdPtr);
            //Console.WriteLine($"Change? {success}");
            Console.ReadKey();
        }


        public void fancier() {
            int winHeight = Console.WindowHeight;
            int winWidth = Console.WindowWidth;

            IntPtr[] buffers = new IntPtr[2];
            char[] buffer = new char[winHeight * winWidth];

            IntPtr ptr = CreateConsoleScreenBuffer(
                GENERIC_READ | GENERIC_WRITE,
                FILE_SHARE_READ | FILE_SHARE_WRITE,
                IntPtr.Zero,
                CONSOLE_TEXTMODE_BUFFER,
                IntPtr.Zero
            );

            Console.SetBufferSize(winWidth, winHeight);

            IntPtr stdPtr = GetStdHandle(STD_OUTPUT_HANDLE);

            buffers[0] = stdPtr;
            buffers[1] = ptr;

            IntPtr backBuffer = buffers[1];

            //bool d = SetStdHandle(STD_OUTPUT_HANDLE, ptr);
            //string data = "test\n";
            //WriteConsole(ptr, data, data.Length, out int _, IntPtr.Zero);

            Stopwatch stopwatch = new();

            int row = winHeight / 2;
            int col = winWidth / 2;
            short dir = 1;

            int fps = 60;
            double ms = 1000.0 / fps;

            COORD topLeft = new COORD(0, 0);
            COORD cursorPos = new COORD(0, 0);
            while (true) {
                stopwatch.Restart();
                FillConsoleOutputCharacter(backBuffer, ' ', buffer.Length, topLeft, out int _);

                //for (int y = 0; y < winHeight; y++)
                //    for (int x = 0; x < winWidth; x++)
                //        buffer[y * winWidth + x] = ' ';

                cursorPos.X = (short)col;
                cursorPos.Y = (short)row;
                SetConsoleCursorPosition(backBuffer, cursorPos);
                WriteConsole(backBuffer, "Hello", 5, out int _, IntPtr.Zero);

                row += dir;
                if (row >= winHeight - 1 || row <= 1) {
                    dir *= -1;
                }

                if (stopwatch.ElapsedMilliseconds < ms) {
                    Thread.Sleep((int)Math.Max(0.0, (ms - stopwatch.ElapsedMilliseconds)));
                }

                SetConsoleActiveScreenBuffer(backBuffer);
                int newBackBuffIdx = (Array.FindIndex(buffers, m => m.Equals(backBuffer)) + 1) % 2;
                backBuffer = buffers[newBackBuffIdx];

            }

            //Console.WriteLine(ptr);
            //Console.WriteLine(stdPtr);
            //Console.WriteLine($"Change? {success}");
            Console.ReadKey();

        }
            static void Main(string[] args) {
            new Program().fancier();
        }




    }
}