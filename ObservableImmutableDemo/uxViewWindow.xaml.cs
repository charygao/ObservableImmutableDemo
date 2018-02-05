using System;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Threading;
using ObservableImmutable;
using ObservableImmutableDemo.Classes;

namespace ObservableImmutableDemo
	{
	/// <summary>
	/// Interaction logic for uxViewWindow.xaml
	/// </summary>
	public partial class uxViewWindow : Window, INotifyPropertyChanged
		{
		private ObservableImmutableList<BO> items;
		System.Timers.Timer timer;

		public string UIThreadID
			{
			get
				{
				return string.Format("UI ThreadId:{0}", System.Threading.Thread.CurrentThread.GetHashCode());
				}
			}

		private ICollectionView view;
		public ICollectionView View
			{
			get
				{
				return view;
				}
			private set
				{
				view = value;
				RaisePropertyChangedEvent("View");
				}
			}

		public uxViewWindow(ObservableImmutableList<BO> items)
			{
			this.items = items;
			View = new CollectionView(this.items);
			rnd = new Random();
			
			InitializeComponent();
			}

		private BO lastItem = null;
		private void UxViewWindow_OnLoaded(object sender, RoutedEventArgs e)
			{

			timer = new System.Timers.Timer(100);
			timer.Elapsed +=
				(timerSender, timerArgs) =>
					{
					var random = rnd.Next(3);

					switch (rnd.Next(2))
						{
						case 0:
							DoOperation(random);
							break;
						case 1:
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
					//items.DoOperation(currentItems => currentItems.Add(lastItem = new BO(string.Format("ThreadId:{0} Time:{1}", System.Threading.Thread.CurrentThread.GetHashCode(), DateTime.Now.ToLongTimeString()))));
					items.DoAdd(currentItems => lastItem = new BO(string.Format("ThreadId:{0} Time:{1}", System.Threading.Thread.CurrentThread.GetHashCode(), DateTime.Now.ToLongTimeString())));
					break;
				case 1:
					items.DoOperation(currentItems => currentItems.Count > 50 ? currentItems.Clear() : currentItems.Count > 0 ? currentItems.RemoveAt(0) : null);
					break;
				case 2:
					if (lastItem != null)
						{
						lastItem.Caption = "###################";
						}
					break;
				}
			}

		private Random rnd;

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
	}
