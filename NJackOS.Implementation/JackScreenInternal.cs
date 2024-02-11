using NJackOS.Interface;
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
    internal class JackScreenInternal : ICurrentKeyObservable
    {
        public bool Started { get; private set; } = false;

        public JackScreenInternal()
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

            myCanvas.KeyDown += new KeyEventHandler(OnKeyDownHandler);
            myCanvas.KeyUp += new KeyEventHandler(OnKeyUpHandler);

            mainWindow.Content = myCanvas;
            myCanvas.Focus();
            mainWindow.Title = "Hack System Display";
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
            app.Dispatcher.BeginInvoke(new Action(() =>
            {
                using (writeableBitmap.GetBitmapContext())
                {
                    writeableBitmap.FillRectangle(x1, y1, x2, y2, color ? Colors.Black : Colors.White);
                }
            }));
        }

        public void Subscribe(ICurrentKeyObserver observer)
        {
            this.observers.Add(observer);
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            char c;
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
                c = (e.Key - Key.D0).ToString()[0];
            else if (e.Key.Equals(Key.OemMinus))
                c = '-';
            else if (e.Key.Equals(Key.Enter))
                c = '\n';
            else if (e.Key.Equals(Key.Back))
                c = (char)8;
            else if (e.Key.Equals(Key.Left))
                c = (char)130;
            else if (e.Key.Equals(Key.Right))
                c = (char)132;
            else if (e.Key.Equals(Key.Up))
                c = (char)131;
            else if (e.Key.Equals(Key.Down))
                c = (char)133;
            else if (e.Key.Equals(Key.Home))
                c = (char)134;
            else if (e.Key.Equals(Key.End))
                c = (char)135;
            else if (e.Key.Equals(Key.PageUp))
                c = (char)136;
            else if (e.Key.Equals(Key.PageDown))
                c = (char)137;
            else if (e.Key.Equals(Key.Insert))
                c = (char)138;
            else if (e.Key.Equals(Key.Delete))
                c = (char)139;
            else if (e.Key.Equals(Key.Escape))
                c = (char)140;
            else if (e.Key.Equals(Key.F1))
                c = (char)141;
            else if (e.Key.Equals(Key.F2))
                c = (char)142;
            else if (e.Key.Equals(Key.F3))
                c = (char)143;
            else if (e.Key.Equals(Key.F4))
                c = (char)144;
            else if (e.Key.Equals(Key.F5))
                c = (char)145;
            else if (e.Key.Equals(Key.F6))
                c = (char)146;
            else if (e.Key.Equals(Key.F7))
                c = (char)147;
            else if (e.Key.Equals(Key.F8))
                c = (char)148;
            else if (e.Key.Equals(Key.F9))
                c = (char)149;
            else if (e.Key.Equals(Key.F10))
                c = (char)150;
            else if (e.Key.Equals(Key.F11))
                c = (char)151;
            else if (e.Key.Equals(Key.F12))
                c = (char)151;
            else
                c = e.Key.ToString()[0];
            foreach (var observer in observers)
            {
                observer.OnCurrentKeyChanged(c);
            };
        }

        private void OnKeyUpHandler(object sender, KeyEventArgs e)
        {
            foreach (var observer in observers)
            {
                observer.OnCurrentKeyChanged('\0');
            };
        }

        private WriteableBitmap writeableBitmap;
        private Application app;
        private HashSet<ICurrentKeyObserver> observers = new HashSet<ICurrentKeyObserver>();
    }
}
