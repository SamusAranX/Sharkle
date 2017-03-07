using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sharkle {
	/// <summary>
	/// Interaction logic for AboutWindow.xaml
	/// </summary>
	public partial class AboutWindow : Window {
		public AboutWindow() {
			InitializeComponent();
			this.DataContext = this;
		}

		public string CurrentVersion {
			get {
				var version = ApplicationDeployment.IsNetworkDeployed
					   ? ApplicationDeployment.CurrentDeployment.CurrentVersion
					   : Assembly.GetExecutingAssembly().GetName().Version;
				
				return version.ToString(3);
			}
		}

		private Array _hyperlinks = new[] {
			new Uri("http://www.nightinthewoods.com/"),
			new Uri("https://www.youtube.com/watch?v=d86WnX_271U&feature=youtu.be&t=1h34m30s"),
			new Uri("https://github.com/SamusAranX/Sharkle/"),
			new Uri("https://github.com/SamusAranX/Sharkle/issues"),
			new Uri("https://twitter.com/SamusAranX"),
			new Uri("https://peterwunder.de"),
		};
		public Array Hyperlinks {
			get {
				return _hyperlinks;
			}
		}

		public Color WindowGlassColor {
			get {
				return SystemParameters.WindowGlassColor;
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {
		}

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
			var link = (Hyperlink)e.OriginalSource;
			Process.Start(link.NavigateUri.AbsoluteUri);
		}
	}
}
