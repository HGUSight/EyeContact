using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using static Renewal.MainWindow;
using System.Diagnostics;
using System.Security.Permissions;

namespace Renewal
{
    public partial class magnifier : Form
    {
        int defaultStyle;
        bool isLens;
        private int initialStyle;

        public const int LEFTDOWN = 0x0002;
        public const int LEFTUP = 0x0004;
        public const int RIGHTDOWN = 0x0008;
        public const int RIGHTUP = 0x0010;
        public const int MOUSEWHEEL = 0x0800;
        //GlobalMouseHandler mouseHandler;
       

        public magnifier(bool isLens)
        {
           // Application.AddMessageFilter(new GlobalMouseHandler());
            //GlobalMouseHandler.MouseMovedEvent += GlobalMouseHandler_MouseMovedEvent;

            // mouseHandler = new GlobalMouseHandler();
            InitializeComponent();
            this.isLens = isLens;
            
            
            // Setting a position of Magnifier where is the point of cursor
            this.StartPosition = FormStartPosition.Manual;

            int left = Control.MousePosition.X - this.Bounds.Width / 2; 
            int top = Control.MousePosition.Y - this.Bounds.Height / 2;
            
            if (left < 0)
                left = 0;
            if (top < 0)
                top = 0;
            int right = left + this.Bounds.Width;
            if (right > Screen.PrimaryScreen.Bounds.Right)
                left = Screen.PrimaryScreen.Bounds.Right - this.Bounds.Width;
            int bottom = top + this.Bounds.Height;
            if (bottom > Screen.PrimaryScreen.Bounds.Bottom)
                top = Screen.PrimaryScreen.Bounds.Bottom - this.Bounds.Height;
            this.Location = new Point(left, top);

        }

        #region Native Methods
        //Native functions needed---------------------------------------------------------------

        [DllImport("user32.dll")]
        static extern int ShowCursor(bool bShow);

        [DllImport("Magnification.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "MagShowSystemCursor")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool MagShowSystemCursor([MarshalAs(UnmanagedType.Bool)]bool fShowCursor);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte vk, byte scan, int flags, ref int extrainfo);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        static extern int GetWindowLong(IntPtr hWnd, int index);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        static extern int SetWindowLong(IntPtr hWnd, int index, int val);

        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, int crKey, byte bAlpha, LayeredWindowAttributeFlags dwFlags);
        [FlagsAttribute]
        [Description("Layered window flags")]

        
        //-----------------------------------------------------------------------------------------
        public enum LayeredWindowAttributeFlags : int
        {
            /// <summary>
            /// Use key as a transparency color
            /// </summary>
            LWA_COLORKEY = 0x00000001,
            /// <summary>
            /// Use Alpha to determine the opacity of the layered window.
            /// </summary>
            LWA_ALPHA = 0x00000002
        }

        #endregion 

        private void magnifier_Load(object sender, EventArgs e)
        {
            //Don't let the magnifier grow very large------------------------------------
            Rectangle r = Screen.PrimaryScreen.Bounds;
            MaximumSize = new Size((int)(r.Width * 0.75), (int)(r.Height * 0.75));
            //Store the default style because it might be needed later to reset the settings
            defaultStyle = GetWindowLong(Handle, -20);
            //Apply settings when they change -------------------------------------------
            Properties.Settings.Default.PropertyChanged += new PropertyChangedEventHandler(settingsChanged);
            //Apply settings now for the first time -------------------------------------
            applySettings();
            //---------------------------------------------------------------------------

            if (/*Properties.Settings.Default.*/isLens)//If the magnifier is a lens make click-throughable
            {
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                SetWindowLong(this.Handle, -20, defaultStyle | 0x80000 | 0x20);//Layered and transparent
                SetLayeredWindowAttributes(Handle, 0, 255, LayeredWindowAttributeFlags.LWA_ALPHA);
            }
            else
            {  //Otherwise, render it as toolwindow that is not click-throughable
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
                SetWindowLong(this.Handle, -20, defaultStyle | 0x80000 /*| 0x20*/);//Layered but not transparent
                //SetWindowLong(Handle, -20, defaultStyle);
                SetLayeredWindowAttributes(Handle, 0, 255, LayeredWindowAttributeFlags.LWA_ALPHA);
            }
        }
/*
        private void GlobalMouseHandler_MouseMovedEvent(object sender, MouseEventArgs e)
        {
            try
            {

                int x = e.X - this.Bounds.Width / 2;
                int y = e.Y - this.Bounds.Height / 2;

                Debug.WriteLine("move");
                //making the magnifier move to fallow mouse cursor
                 SetWindowPos(this.Handle, -1, x, y, this.Bounds.Width, this.Bounds.Height, 0x0004 | 0x0040);
            }
            catch { }
        }
*/
        private void settingsChanged(Object sender, PropertyChangedEventArgs args)
        {
            applySettings();
        }

        private void applySettings()
        {    
            //magnifier defualt size of width, height = 500,500
            Size = new Size(Properties.Settings.Default.maxMagnifierWidth, Properties.Settings.Default.maxMagnifierHeight);
            TopMost = Properties.Settings.Default.alwaysOnTop;
        }

        private void magnifier_FormClosing(object sender, FormClosingEventArgs e)
        {
            MagShowSystemCursor(true);
            Properties.Settings.Default.PropertyChanged -= new PropertyChangedEventHandler(settingsChanged);
            MainWindow.magnifierWindowLens = null;
            MainWindow.magnifierWindowDocked = null;
        }

        // make if possible to click on the magnifier
       /* private void magnifier_MouseClick(object sender, MouseEventArgs e)
        {
            
            initialStyle = GetWindowLong(this.Handle, -20);
            SetWindowLong(this.Handle, -20, initialStyle | 0x80000 | 0x20);
            SetLayeredWindowAttributes(this.Handle, 0, 255, LayeredWindowAttributeFlags.LWA_ALPHA);

            switch(MainWindow.mouseEvent_var_forMagnification)
            {
                case (int)mouseEvent.LCLICKED:
                    mouse_event(LEFTDOWN, 0, 0, 0, 0); // 마우스 왼쪽 클릭
                    mouse_event(LEFTUP, 0, 0, 0, 0);
                    break;

                case (int)mouseEvent.RCLICKED:
                    mouse_event(RIGHTDOWN, 0, 0, 0, 0); // 마우스 오른쪽 클릭
                    mouse_event(RIGHTUP, 0, 0, 0, 0);
                    break;


                case (int)mouseEvent.DOUBLECLICKED:
                    mouse_event(LEFTDOWN, 0, 0, 0, 0); // 마우스 더블 클릭 
                    mouse_event(LEFTUP, 0, 0, 0, 0);
                    mouse_event(LEFTDOWN, 0, 0, 0, 0);
                    mouse_event(LEFTUP, 0, 0, 0, 0);
                    mouseEvent_var = (int)mouseEvent.LCLICKED;
                    break;

                case (int)mouseEvent.DRAGCLICKED:
                    mouse_event(LEFTDOWN, 0, 0, 0, 0); // 마우스 드래그 
                    mouseEvent_var = (int)mouseEvent.LCLICKED;
                    break;
            }

            if (MainWindow.mouseEvent_var != (int)mouseEvent.DRAGCLICKED)
            {
                MagShowSystemCursor(true);
                this.Close();
            }
            
        }
      */
        private void magnifier_MouseHover(object sender, EventArgs e)
        {
            //MagShowSystemCursor(false);
            ShowCursor(false);
            MessageBox.Show("hover");
        }

        private void magnifier_MouseLeave(object sender, EventArgs e)
        {
            MagShowSystemCursor(true);
        }

        private void magnifier_MouseMove(object sender, MouseEventArgs e)
        {
            MagShowSystemCursor(false);
        }
        
    }
    /*
    //global mouse handler function.
    public class GlobalMouseHandler : IMessageFilter
    {
        private static bool first = true;
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_NCLBUTTONDBLCLK = 0x00A3; //double click
        private const int WM_NCRBUTTONDOWN = 0x00A4; //right mouse button down 
        private const int WM_NCLBUTTONDOWN = 0x00A1;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_LBUTTONDBLCLK = 0x0203;
        private System.Drawing.Point previousMousePosition = new System.Drawing.Point();
       
         public static event EventHandler<MouseEventArgs> MouseMovedEvent = delegate { };
        public static int MouseEvent = 0;

      

        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public bool PreFilterMessage(ref Message m)
        {
            Debug.WriteLine("=======================message 발생");
            if (m.Msg == WM_MOUSEMOVE)
            {
                MessageBox.Show("ooㄱ");
                Debug.WriteLine("move");
                System.Drawing.Point currentMousePoint = Control.MousePosition;
                if (previousMousePosition != currentMousePoint)
                {
                    previousMousePosition = currentMousePoint;
                    MouseMovedEvent(this, new MouseEventArgs(MouseButtons.None, 0, currentMousePoint.X, currentMousePoint.Y, 0));
                }
                return false;
            }
            if((m.Msg == WM_LBUTTONDOWN || m.Msg == WM_LBUTTONDBLCLK || m.Msg == WM_RBUTTONDOWN) &&(first))
            {
                MessageBox.Show("ㄱ!!");
                first = false;
                MouseEvent = m.Msg;
                return true;
            }
            else if(m.Msg == WM_LBUTTONDOWN || m.Msg == WM_LBUTTONDBLCLK || m.Msg == WM_RBUTTONDOWN)
            {
                MessageBox.Show("ㄱㄱ");
                mouse_event(LEFTDOWN, 0, 0, 0, 0); // 마우스 왼쪽 클릭
                mouse_event(LEFTUP, 0, 0, 0, 0);

                switch (MouseEvent)
                {
                    case WM_LBUTTONDOWN:
                        mouse_event(LEFTDOWN, 0, 0, 0, 0); // 마우스 왼쪽 클릭
                        mouse_event(LEFTUP, 0, 0, 0, 0);
                        break;
                    case WM_LBUTTONDBLCLK:
                        mouse_event(LEFTDOWN, 0, 0, 0, 0); // 마우스 더블 클릭 
                        mouse_event(LEFTUP, 0, 0, 0, 0);
                        mouse_event(LEFTDOWN, 0, 0, 0, 0);
                        mouse_event(LEFTUP, 0, 0, 0, 0);
                        break;
                    case WM_RBUTTONDOWN:
                        mouse_event(RIGHTDOWN, 0, 0, 0, 0); // 마우스 오른쪽 클릭
                        mouse_event(RIGHTUP, 0, 0, 0, 0);
                        break;
                }

                if (MainWindow.magnifierWindowLens != null)
                    MainWindow.magnifierWindowLens.Close();
                if (MainWindow.magnifierWindowDocked != null)
                    MainWindow.magnifierWindowDocked.Close();
                return true;
            }

            // Always allow message to continue to the next filter control
            return false;
        }
    
    }
*/
}
