using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;
using ObservableImmutable;
using ObservableImmutableDemo.Classes;

namespace ObservableImmutableDemo
	{
	public partial class MainWindow : Window, INotifyPropertyChanged
		{
		System.Timers.Timer timer;

		private ObservableImmutableList<BO> _items;
		public ObservableImmutableList<BO> Items
			{
			get
				{
				return _items;
				}
			set
				{
				_items = value;
				RaisePropertyChangedEvent("Items");
				}
			}

		public string UIThreadID
			{
			get
				{
				return string.Format("UI ThreadId:{0}", System.Threading.Thread.CurrentThread.GetHashCode());
				}
			}

		public MainWindow()
			{
			rnd = new Random();
			InitializeComponent();
			}

		private Random rnd;
		private BO lastItem = null;

		private void uxMainWindow_Loaded(object sender, RoutedEventArgs e)
			{
			Items = new ObservableImmutableList<BO>();

			timer = new System.Timers.Timer(100);
			timer.Elapsed +=
				(timerSender, timerArgs) =>
					{
					var random = rnd.Next(3);

					switch (rnd.Next(2))
						{
						case 0 :
							DoOperation(random);
							break;
						case 1 :
							Dispatcher.BeginInvoke
								(
								DispatcherPriority.Background,
								(Action)
									(
									 () =>
										{
										var capturedRandom = random;
										DoOperation(capturedRandom);
										}
									)
								);
							break;
						}
					};

			timer.Start();
			}

		private void DoOperation(int value)
			{
			switch (value)
				{
				case 0:
					//_items.DoOperation(currentItems => currentItems.Add(lastItem = new BO(string.Format("ThreadId:{0} Time:{1}", System.Threading.Thread.CurrentThread.GetHashCode(), DateTime.Now.ToLongTimeString()))));
					_items.DoAdd(currentItems => lastItem = new BO(string.Format("ThreadId:{0} Time:{1}", System.Threading.Thread.CurrentThread.GetHashCode(), DateTime.Now.ToLongTimeString())));
					break;
				case 1:
					_items.DoOperation(currentItems => currentItems.Count > 50 ? currentItems.Clear() : currentItems.Count > 0 ? currentItems.RemoveAt(0) : null);
					break;
				case 2:
					if (lastItem != null)
						{
						lastItem.Caption = "###################";
						}
					break;
				}
			}

		private void Button_Click(object sender, RoutedEventArgs e)
			{
			Thread newWindowThread = new Thread
				(
				new ThreadStart
					(() =>
						{
						uxViewWindow win = new uxViewWindow(Items);
						win.Show();
						System.Windows.Threading.Dispatcher.Run();
						}
					)
				);

			newWindowThread.SetApartmentState(ApartmentState.STA);
			newWindowThread.IsBackground = true;
			newWindowThread.Start();
			}

		private void Button_Click_1(object sender, RoutedEventArgs e)
			{
			timer.Stop();
			}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		public void RaisePropertyChangedEvent(string propertyName)
			{
			if (PropertyChanged != null)
				{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
				}
			}

		#endregion
		}

	public class Test : Selector
		{
		}
	}
