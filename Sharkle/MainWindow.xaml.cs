using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using Hardcodet.Wpf.TaskbarNotification;

namespace Sharkle {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged {

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOMOVE = 0x0002;
        const uint SWP_NOZORDER = 0x0004;
        const uint SWP_NOACTIVATE = 0x0010;
        const uint WM_WINDOWPOSCHANGING = 0x0046;

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPOS {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public uint flags;
        }

		public string HeySource {
			get {
				var rndNum = this.rnd.Next(8);
				Debug.WriteLine($"Rolled a {rndNum}");
				return $"pack://application:,,,/Sounds/hey_{rndNum}.wav";
			}
        }

        private Random rnd;
        private HwndSource source;

        public MainWindow() {
            this.InitializeComponent();

            this.rnd = new Random();
			this.DataContext = this;
        }

        // These Storyboards have to be controlled in code because the WPF team has no clue what it's fucking doing
        private Storyboard SharkleIdleAnim, SharkleWavingAnim, SharkleShowBubblesAnim, SharkleBubbleFlickerAnim;

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
            if (msg == WM_WINDOWPOSCHANGING) {
                //var windowPos = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
                SetWindowPos(this.source.Handle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_NOZORDER);
            }

            return IntPtr.Zero;
        }

        private TaskbarIcon SharkleIcon;

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            // needs to happen after the window has been loaded because the handle is zero otherwise
            this.source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            this.source.AddHook(new HwndSourceHook(this.WndProc));

            var workingArea = SystemParameters.WorkArea;
			var padding = 20;
			this.Left = workingArea.Right - this.Width - padding;
			this.Top = workingArea.Bottom - this.Height - padding;

			this.SharkleIdleAnim = (Storyboard)this.FindResource("SharkleIdleAnim");
			this.SharkleBubbleFlickerAnim = (Storyboard)this.FindResource("SharkleBubbleFlickerAnim");
			this.SharkleWavingAnim = (Storyboard)this.FindResource("SharkleWavingAnim");
            this.SharkleShowBubblesAnim = (Storyboard)this.FindResource("SharkleShowBubblesAnim");

            this.SharkleIcon = (TaskbarIcon)this.FindResource("SharkleIcon");

            // I'm starting these two Storyboards manually here because otherwise, the .Seek()s below won't work
            this.SharkleIdleAnim.Begin();
            this.SharkleBubbleFlickerAnim.Begin();
		}

		bool isPlaying = false;
		private async void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            this.DragMove();

			if (!this.isPlaying) {
                // Manually rewind that storyboard to the beginning because I am sick of WPF's bullshit
                this.SharkleBubbleFlickerAnim.Seek(TimeSpan.Zero);

                // Start waving
                this.SharkleWavingAnim.Begin();
                this.SharkleShowBubblesAnim.Begin();

                // Play a sound and wait for it to end
                // When isPlaying == true, clicking on Sharkle will do nothing
                this.isPlaying = true;
				await Task.Run(() => {
					var sndResource = App.GetResourceStream(new Uri(this.HeySource));
					var sndPlayer = new SoundPlayer(sndResource.Stream);
					sndPlayer.PlaySync();
				});
                this.isPlaying = false;

                // Fetch a new sound bite
                this.OnPropertyChanged("HeySource");
			}
		}

		private void SharkleShowBubblesAnim_Completed(object sender, EventArgs e) {
            // The waving animation has finished and the idle animation is about to be displayed again
            // Rewind idle animation to the beginning to avoid the transition looking weird
            this.SharkleIdleAnim.Seek(TimeSpan.Zero);
		}

		private void MenuItem_About_Click(object sender, RoutedEventArgs e) {
			foreach (var window in App.Current.Windows) {
				if (window is AboutWindow) {
					((AboutWindow)window).Activate();
					return;
				}
			}

			var aboutWindow = new AboutWindow();
			aboutWindow.Show();
		}

		private void MenuItem_Exit_Click(object sender, RoutedEventArgs e) {
			App.Current.Shutdown();
		}
		
		//
		// INotifyPropertyChanged stuff
		//

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string name) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
