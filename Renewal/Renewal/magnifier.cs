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

namespace Renewal
{
    public partial class magnifier : Form
    {
        int defaultStyle;
        bool isLens;
        private int initialStyle;

        public magnifier(bool isLens)
        {
            //GlobalMouseHandler.MouseMovedEvent += GlobalMouseHandler_MouseMovedEvent;
            //Application.AddMessageFilter(new GlobalMouseHandler());
            InitializeComponent();
            this.isLens = isLens;

            // Setting a position of Magnifier where is the point of cursor
            this.StartPosition = FormStartPosition.Manual;

            int x = Control.MousePosition.X - this.Bounds.Width / 2; ;
            int y = Control.MousePosition.Y - this.Bounds.Height / 2;
            this.Location = new Point(x, y);

        }

        #region Native Methods
        //Native functions needed---------------------------------------------------------------



        [DllImport("Magnification.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "MagShowSystemCursor")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool MagShowSystemCursor([MarshalAs(UnmanagedType.Bool)]bool fShowCursor);


        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        static extern int GetWindowLong(IntPtr hWnd, int index);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        static extern int SetWindowLong(IntPtr hWnd, int index, int val);

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
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                SetWindowLong(this.Handle, -20, defaultStyle | 0x80000 | 0x20);//Layered and transparent
                SetLayeredWindowAttributes(Handle, 0, 255, LayeredWindowAttributeFlags.LWA_ALPHA);
          
        }

        private void GlobalMouseHandler_MouseMovedEvent(object sender, MouseEventArgs e)
        {
            try
            {

                int x = e.X - this.Bounds.Width / 2;
                int y = e.Y - this.Bounds.Height / 2;

                //making the magnifier move to fallow mouse cursor
               // SetWindowPos(this.Handle, -1, x, y, this.Bounds.Width, this.Bounds.Height, 0x0004 | 0x0040);
            }
            catch { }
        }

        private void settingsChanged(Object sender, PropertyChangedEventArgs args)
        {
            applySettings();
        }

        private void applySettings()
        {    
            //the height and width of magnifier = 500,500 --> can adjust this value in Settings.Designer.cs
            Size = new Size(Properties.Settings.Default.maxMagnifierWidth, Properties.Settings.Default.maxMagnifierHeight);
            TopMost = Properties.Settings.Default.alwaysOnTop;
        }

        private void magnifier_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.PropertyChanged -= new PropertyChangedEventHandler(settingsChanged);
            // show cursor
            MagShowSystemCursor(true);
        }

        // make if possible to click on the magnifier
        private void magnifier_MouseClick(object sender, MouseEventArgs e)
        {
            initialStyle = GetWindowLong(this.Handle, -20);
            SetWindowLong(this.Handle, -20, initialStyle | 0x80000 | 0x20);
            SetLayeredWindowAttributes(this.Handle, 0, 255, LayeredWindowAttributeFlags.LWA_ALPHA);

        }
        /*
        private void magnifier_MouseHover(object sender, EventArgs e)
        {
            MagShowSystemCursor(false);
        }

        private void magnifier_MouseLeave(object sender, EventArgs e)
        {
            MagShowSystemCursor(true);
        }

        private void magnifier_MouseMove(object sender, MouseEventArgs e)
        {

            MagShowSystemCursor(false);
        }
        */



    }
    /*
    //global mouse handler function.
    public class GlobalMouseHandler : IMessageFilter
    {
        private const int WM_MOUSEMOVE = 0x0200;
        private System.Drawing.Point previousMousePosition = new System.Drawing.Point();
        public static event EventHandler<MouseEventArgs> MouseMovedEvent = delegate { };


        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_MOUSEMOVE)
            {
                System.Drawing.Point currentMousePoint = Control.MousePosition;
                if (previousMousePosition != currentMousePoint)
                {
                    previousMousePosition = currentMousePoint;
                    MouseMovedEvent(this, new MouseEventArgs(MouseButtons.None, 0, currentMousePoint.X, currentMousePoint.Y, 0));
                }
            }
            // Always allow message to continue to the next filter control
            return false;
        }
    
    }
    */
}