using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservableImmutableDemo.Classes
	{
	public class BO : INotifyPropertyChanged
		{
		private string _caption;
		public string Caption
			{
			get
				{
				return _caption;
				}
			set
				{
				_caption = value;
				RaisePropertyChanged("Caption");
				}
			}

		public BO(string caption)
			{
			_caption = caption;
			}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		public void RaisePropertyChanged(string propertyName)
			{
			var propertyChangedEventHandler = PropertyChanged;

			if (propertyChangedEventHandler != null)
				{
				propertyChangedEventHandler(this, new PropertyChangedEventArgs(propertyName));
				}
			}

		#endregion INotifyPropertyChanged
		}
	}
