using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
	public partial class MainWindow : Window {
		public MainWindow() {
			InitializeComponent();

			Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = 12 });

			//this.Resources["SharkleBGBrush"] = new SolidColorBrush(Color.FromRgb(255, 127, 0));
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {

		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			DragMove();
		}
	}
}
