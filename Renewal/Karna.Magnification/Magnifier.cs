using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using Gma.UserActivityMonitor;
using System.Collections;


namespace Karna.Magnification
{

    public class Magnifier : IDisposable
    {


        private Form form;
        private IntPtr hwndMag;
        private float magnification;
        private bool initialized;
        private RECT magWindowRect = new RECT();
        private System.Windows.Forms.Timer timer;
        public bool isLens;
        private bool oldIsLens;
        private int initialStyle;
        private RECT MagRECT;

        // Variables that store the mouse position at the moment the magnifier is activated 
        //                      and the area around that position.
        private POINT mousePoint = new POINT();
        private RECT sourceRect = new RECT();

        public Magnifier(Form form, bool isLens)
        {
            this.isLens = isLens;
            oldIsLens = isLens;//Used to track changes of the isLens variable because changing the cursor settings of the magnifier requiers recreating the magnifier
            magnification = 2.0f;
            this.form = form;

            if (form == null)
                throw new ArgumentNullException("form");

            initialized = NativeMethods.MagInitialize();
            NativeMethods.MagShowSystemCursor(false);

            if (initialized)
            {
                SetupMagnifier();

                this.form.Resize += new EventHandler(form_Resize);
                this.form.FormClosing += new FormClosingEventHandler(form_FormClosing);
                timer = new Timer();
                timer.Tick += new EventHandler(timer_Tick);
                timer.Interval = NativeMethods.USER_TIMER_MINIMUM;
                timer.Enabled = true;

                
                //HookManager.MouseMove += new MouseEventHandler(mouseMove); 
            }
            else
            {
                form.Close();
            }
        }
      
        private void mouseMove(Object sender, MouseEventArgs e)
        {
            UpdateMaginifier();
        }


        public void recreateSelf()
        {
            //Used to apply the changed settings on the magnifier which can only happen if the magnifier is recreated
            RemoveMagnifier();//Remove the magnifier objects by calling windows api
            NativeMethods.SendMessage(hwndMag, NativeMethods.WM_SYSCOMMAND, NativeMethods.SC_CLOSE, 0);//close the inner magnifier window
            initialized = NativeMethods.MagInitialize();//Initialize the magnifier again by calling windows api
            if(initialized)
            {
                SetupMagnifier();//apply settings
            }
        }

        void form_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer.Enabled = false;
            Dispose();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            UpdateMaginifier();
        }

        void form_Resize(object sender, EventArgs e)
        {
            ResizeMagnifier();
        }

        ~Magnifier()
        {
            Dispose(false);
        }

        protected virtual void ResizeMagnifier()
        {
            if ( initialized && (hwndMag != IntPtr.Zero))
            {
                NativeMethods.GetClientRect(form.Handle, ref magWindowRect);
                // Resize the control to fill the window.
                NativeMethods.SetWindowPos(hwndMag, IntPtr.Zero,
                    magWindowRect.left, magWindowRect.top, magWindowRect.right, magWindowRect.bottom, 0);
            }
        }

        public virtual void UpdateMaginifier()
        {
            if ((!initialized) || (hwndMag == IntPtr.Zero))
                return;

              /*  
              --Each time you update, you set a new zoom area around the mouse.--

              POINT mousePoint = new POINT();
              RECT sourceRect = new RECT();

              NativeMethods.GetCursorPos(ref mousePoint);

              int width = (int)((magWindowRect.right - magWindowRect.left) / magnification);
              int height = (int)((magWindowRect.bottom - magWindowRect.top) / magnification);

              sourceRect.left = mousePoint.x - width / 2;
              sourceRect.top = mousePoint.y - height / 2;
              */
            //the width and height of sourceRect
            int width = (int)((magWindowRect.right - magWindowRect.left) / magnification);
            int height = (int)((magWindowRect.bottom - magWindowRect.top) / magnification);



            // Don't scroll outside desktop area.
            if(isLens)
            {
                if (sourceRect.left < 0)
                {
                    sourceRect.left = 0;
                }
                if (sourceRect.left > NativeMethods.GetSystemMetrics(NativeMethods.SM_CXSCREEN) - width)
                {
                    sourceRect.left = NativeMethods.GetSystemMetrics(NativeMethods.SM_CXSCREEN) - width;
                }
                sourceRect.right = sourceRect.left + width;

                if (sourceRect.top < 0)
                {
                    sourceRect.top = 0;
                }
                if (sourceRect.top > NativeMethods.GetSystemMetrics(NativeMethods.SM_CYSCREEN) - height)
                {
                    sourceRect.top = NativeMethods.GetSystemMetrics(NativeMethods.SM_CYSCREEN) - height;
                }
                sourceRect.bottom = sourceRect.top + height;
            }
            if (this.form == null)
            {
                timer.Enabled = false;
                return;
            }

            if (this.form.IsDisposed)
            {
                timer.Enabled = false;
                return;
            }

            // Set the source rectangle for the magnifier control.
            NativeMethods.MagSetWindowSource(hwndMag, sourceRect);

            POINT mouse = new POINT();
            NativeMethods.GetCursorPos(ref mouse);

            /*when the position of mouse is in the magnification window region */
            if (mouse.x > form.Left && mouse.x < form.Right && mouse.y < form.Bottom && mouse.y > form.Top)
                NativeMethods.MagShowSystemCursor(false); // show only magnified cursor 
            else
                NativeMethods.MagShowSystemCursor(true); // show both real cursor and magnified cursor

            /*
            // Reclaim topmost status, to prevent unmagnified menus from remaining in view. 
            if (isLens)// If the magnifier is a lens let it follow the cursor and stay on top
            {
                POINT mouse = new POINT();
                NativeMethods.GetCursorPos(ref mouse);

                NativeMethods.SetWindowPos(form.Handle, NativeMethods.HWND_TOPMOST, mouse.x - (int)(magnification * width / 2), mouse.y - (int)(magnification * height / 2), width, height,
                (int)SetWindowPosFlags.SWP_NOACTIVATE | (int)SetWindowPosFlags.SWP_NOSIZE);
            }
            else// if the magnifier is not a lens don't move it and keep it on top of all non-topmost windows
            */
            NativeMethods.SetWindowPos(form.Handle, new IntPtr(0), 0, 0, 0, 0,
                (int)SetWindowPosFlags.SWP_NOACTIVATE | (int)SetWindowPosFlags.SWP_NOZORDER | (int)SetWindowPosFlags.SWP_NOREDRAW | (int)SetWindowPosFlags.SWP_NOMOVE | (int)SetWindowPosFlags.SWP_NOSIZE);

            

            // Force redraw.
            NativeMethods.InvalidateRect(hwndMag, IntPtr.Zero, true);
        }

        public float Magnification
        {
            get { return magnification; }
            set
            {
                if (magnification != value)
                {
                    magnification = value;
                    // Set the magnification factor.
                    Transformation matrix = new Transformation(magnification);
                    NativeMethods.MagSetWindowTransform(hwndMag, ref matrix);
                }
            }
        }

        public void setMagnificationFactor()
        {
            Transformation matrix = new Transformation(magnification);
            NativeMethods.MagSetWindowTransform(hwndMag, ref matrix);
        }

        private void SetupMagnifier()
        {
            if (!initialized)
                return;

            IntPtr hInst;

            hInst = NativeMethods.GetModuleHandle(null);


            // Make the window opaque. Which has been dealt with in the magnifier class
            //form.AllowTransparency = true;
            //form.TransparencyKey = Color.Empty;
            //form.Opacity = 255;

            // Create a magnifier control that fills the client area.
            NativeMethods.GetClientRect(form.Handle, ref magWindowRect);
            /*
            if (isLens)// if the magnifier is a lens don't show magnifier cursor
            {
                hwndMag = NativeMethods.CreateWindow((int)ExtendedWindowStyles.WS_EX_CLIENTEDGE, NativeMethods.WC_MAGNIFIER,
                    "MagnifierWindow", (int)WindowStyles.WS_CHILD | 
                    (int)WindowStyles.WS_VISIBLE,
                    magWindowRect.left, magWindowRect.top, magWindowRect.right, magWindowRect.bottom, form.Handle, IntPtr.Zero, hInst, IntPtr.Zero);
            }
            else// if the magnifier is not a lens show the maginified cursor*/
            //{

            // show mainified cursor
                hwndMag = NativeMethods.CreateWindow((int)ExtendedWindowStyles.WS_EX_CLIENTEDGE , NativeMethods.WC_MAGNIFIER,
                    "MagnifierWindow", (int)WindowStyles.WS_CHILD | (int)MagnifierStyle.MS_SHOWMAGNIFIEDCURSOR |
                    (int)WindowStyles.WS_VISIBLE,
                    magWindowRect.left, magWindowRect.top, magWindowRect.right, magWindowRect.bottom, form.Handle, IntPtr.Zero, hInst, IntPtr.Zero);
           // }


            if (hwndMag == IntPtr.Zero)
            {
                return;
            }

            // Set the magnification factor.
            Transformation matrix = new Transformation(magnification);
            NativeMethods.MagSetWindowTransform(hwndMag, ref matrix);

            NativeMethods.GetCursorPos(ref mousePoint);

            //int width = (int)((magWindowRect.right - magWindowRect.left) / magnification);
            //int height = (int)((magWindowRect.bottom - magWindowRect.top) / magnification);

            // the size of the sourceRect
            int width = 250;
            int height = 250;

            //Set the area around the mouse to sourceRect.  
            //and adjust this sourceRect if the sourceRect is over the region of the screen.
            if (mousePoint.x - width / 2 < 0)
                sourceRect.left = 0;
            else
                sourceRect.left = mousePoint.x - width/2;
            if (mousePoint.y - height / 2 < 0)
                sourceRect.top = 0;
            else
                sourceRect.top = mousePoint.y - height / 2;
            sourceRect.right = mousePoint.x + width / 2;
            sourceRect.bottom = mousePoint.y + height / 2;
            
            

        }

        public void RemoveMagnifier()
        {
            if (initialized)
                NativeMethods.MagUninitialize();
        }

        protected virtual void Dispose(bool disposing)
        {//Safely dispose the magnifier
            timer.Enabled = false;
            if (disposing)
                timer.Dispose();
            timer = null;
            form.Resize -= form_Resize; 
            this.form.FormClosing -= new FormClosingEventHandler(form_FormClosing);
            
            RemoveMagnifier();
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
