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

			Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = 12 });
			this._rnd = new Random();
			this.DataContext = this;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {

		}

		bool isPlaying = false;
		private async void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			DragMove();

			if (!isPlaying) {
				isPlaying = true;
				await Task.Run(() => {
					var sndResource = App.GetResourceStream(new Uri(this.HeySource));
					var sndPlayer = new SoundPlayer(sndResource.Stream);
					sndPlayer.PlaySync();
				});
				isPlaying = false;
				OnPropertyChanged("HeySource");
			}
		}

		private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e) {
			App.Current.Shutdown();
		}
		
		//
		// INotifyPropertyChanged
		//

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string name) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
