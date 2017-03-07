using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Hardcodet.Wpf.TaskbarNotification;

namespace Sharkle {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged {

		private Random _rnd;
		public string HeySource {
			get {
				var rndNum = _rnd.Next(8);
				Debug.WriteLine($"Rolled a {rndNum}");
				return $"pack://application:,,,/Sounds/hey_{rndNum}.wav";
			}
		}

		public MainWindow() {
			InitializeComponent();
			
			this._rnd = new Random();
			this.DataContext = this;
		}

		// These Storyboards have to be controlled in code because the WPF team has no clue what it's fucking doing
		private Storyboard SharkleIdleAnim, SharkleWavingAnim, SharkleShowBubblesAnim, SharkleBubbleFlickerAnim;

		private TaskbarIcon SharkleIcon;
		private void Window_Loaded(object sender, RoutedEventArgs e) {
			var workingArea = SystemParameters.WorkArea;
			var padding = 20;
			this.Left = workingArea.Right - this.Width - padding;
			this.Top = workingArea.Bottom - this.Height - padding;

			SharkleIdleAnim = (Storyboard)FindResource("SharkleIdleAnim");
			SharkleBubbleFlickerAnim = (Storyboard)FindResource("SharkleBubbleFlickerAnim");
			SharkleWavingAnim = (Storyboard)FindResource("SharkleWavingAnim");
			SharkleShowBubblesAnim = (Storyboard)FindResource("SharkleShowBubblesAnim");

			SharkleIcon = (TaskbarIcon)FindResource("SharkleIcon");

			// I'm starting these two Storyboards manually here because otherwise, the .Seek()s below won't work
			SharkleIdleAnim.Begin();
			SharkleBubbleFlickerAnim.Begin();
		}

		bool isPlaying = false;
		private async void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			DragMove();

			if (!isPlaying) {
				// Manually rewind that storyboard to the beginning because I am sick of WPF's bullshit
				SharkleBubbleFlickerAnim.Seek(TimeSpan.Zero);

				// Start waving
				SharkleWavingAnim.Begin();
				SharkleShowBubblesAnim.Begin();

				// Play a sound and wait for it to end
				// When isPlaying == true, clicking on Sharkle will do nothing
				isPlaying = true;
				await Task.Run(() => {
					var sndResource = App.GetResourceStream(new Uri(this.HeySource));
					var sndPlayer = new SoundPlayer(sndResource.Stream);
					sndPlayer.PlaySync();
				});
				isPlaying = false;

				// Fetch a new sound bite
				OnPropertyChanged("HeySource");
			}
		}

		private void SharkleShowBubblesAnim_Completed(object sender, EventArgs e) {
			// The waving animation has finished and the idle animation is about to be displayed again
			// Rewind idle animation to the beginning to avoid the transition looking weird
			SharkleIdleAnim.Seek(TimeSpan.Zero);
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

		private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e) {
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
