using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace NJackOS.Implementation
{
    internal class JackStreenInternal
    {
        public bool Started { get; private set; } = false;

        public JackStreenInternal()
        {

        }

        public void StartNewThread()
        {
            var window = CreateAndShowMainWindow();
            app = new Application();
            Started = true;
            app.Run(window);
        }

        private Window CreateAndShowMainWindow()
        {
            // Create the application's main window
            var mainWindow = new Window();

            // Create a canvas sized to fill the window
            Canvas myCanvas = new Canvas();
            myCanvas.Background = Brushes.White;

            mainWindow.Height = 256;
            mainWindow.Width = 512;

            writeableBitmap = BitmapFactory.New(512, 256);
            Image img = new Image();
            img.Source = writeableBitmap;

            myCanvas.Children.Add(img);

            myCanvas.Focusable = true;
            using (writeableBitmap.GetBitmapContext())
            {
                writeableBitmap.Clear(Colors.White);
            }

            mainWindow.Content = myCanvas;
            myCanvas.Focus();
            mainWindow.Title = "Canvas Sample";
            return mainWindow;
        }

        public void clearScreen()
        {
            app.Dispatcher.BeginInvoke(new Action(() =>
            {
                using (writeableBitmap.GetBitmapContext())
                {
                    writeableBitmap.Clear(Colors.White);
                }
            }));
        }

        public void drawCircle(short cx, short cy, short r, bool color)
        {
            app.Dispatcher.BeginInvoke(new Action(() =>
            {
                using (writeableBitmap.GetBitmapContext())
                {
                    writeableBitmap.FillEllipseCentered(cx, cy, r, r, color ? Colors.Black : Colors.White);
                }
            }));
        }

        public void drawPixel(short x, short y, bool color)
        {
            app.Dispatcher.BeginInvoke(new Action(() =>
            {
                using (writeableBitmap.GetBitmapContext())
                {
                    writeableBitmap.SetPixel(x, y, color ? Colors.Black : Colors.White);
                }
            }));
        }

        public void drawLine(short x1, short x2, short y1, short y2, bool color)
        {
            app.Dispatcher.BeginInvoke(new Action(() =>
            {
                using (writeableBitmap.GetBitmapContext())
                {
                    writeableBitmap.DrawLine(x1, x2, y1, y2, color ? Colors.Black : Colors.White);
                }
            }));
        }

        public void drawRectangle(short x1, short y1, short x2, short y2, bool color)
        {
            Console.WriteLine($"{x1} {x2} {y1} {y2}");
            app.Dispatcher.BeginInvoke(new Action(() =>
            {
                using (writeableBitmap.GetBitmapContext())
                {
                    writeableBitmap.FillRectangle(x1, y1, x2, y2, color ? Colors.Black : Colors.White);
                }
            }));
        }

        private WriteableBitmap writeableBitmap;
        private Application app;
        
    }
}
