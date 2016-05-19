using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SimpleCalculator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
		private const string COMMA_SIGN = ".";
		private enum Operation { Addition, Subtraction, Multiplication, Division }

		private string displayedNumber;
		private string calculationPath;

		private Operation? operation = null;
		private double prevOperand = 0;
		private double memory = 0;

		StringBuilder numberBuilder = new StringBuilder();

		public string DisplayedNumber
		{
			get
			{
				return displayedNumber;
			}
			set
			{
				displayedNumber = value;
				OnPropertyChanged("DisplayedNumber");
			}
		}

		public string CalculationPath
		{
			get
			{
				return calculationPath;
			}
			set
			{
				calculationPath = value;
				OnPropertyChanged("CalculationPath");
			}
		}

		public MainPage()
        {
            this.InitializeComponent();
		
			ApplicationView.PreferredLaunchViewSize = new Size(480, 800);
			ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

			DataContext = this;
		}		

		/// <summary>
		/// Occurs when a number on keyboard is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnNumberClick(object sender, RoutedEventArgs e)
		{
			if (sender is Button)
			{
				var button = sender as Button;
				string buttonContent = button.Content.ToString();

				// if number already contains a comma, do nothing
				if (buttonContent.Equals(COMMA_SIGN) && numberBuilder.ToString().Contains(COMMA_SIGN)) return;

				// append current digit to number
				numberBuilder.Append(buttonContent);

				// set it in the UI
				DisplayedNumber = numberBuilder.ToString();
			}
		}

		/// <summary>
		/// Occurs when an operator on keyboard is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnOperatorClick(object sender, RoutedEventArgs e)
		{
			if (sender is Button)
			{
				var button = sender as Button;
				string buttonContent = button.Content.ToString();

				double number;

				if (DisplayedNumber != null && Double.TryParse(DisplayedNumber.ToString(), out number))
				{
					// perform the previous operation
					if (operation != null)
					{
						DisplayedNumber = String.Format("{0:0.###}", PerformPreviousOperation(number));
					}
					else
					{
						prevOperand = number;
					}

					switch (buttonContent)
					{
						case "+":
							CalculationPath += number + " + ";
							operation = Operation.Addition;
							break;

						case "-":
							CalculationPath += number + " - ";
							operation = Operation.Subtraction;
							break;

						case "/":
							CalculationPath += number + " / ";
							operation = Operation.Division;
							break;

						case "x":
							CalculationPath += number + " x ";
							operation = Operation.Multiplication;
							break;

						case "=":
							CalculationPath = String.Empty;
							break;
					}

					numberBuilder.Clear();
				}	
			}
		}

		private double PerformPreviousOperation(double number)
		{
			double result = 0;

			switch(operation)
			{
				case Operation.Addition:
					result = prevOperand + number;
					break;

				case Operation.Subtraction:
					result = prevOperand - number;
					break;

				case Operation.Division:
					result = prevOperand / number;
					break;

				case Operation.Multiplication:
					result = prevOperand * number;
					break;
			}

			prevOperand = result;
			operation = null;
			return result;
		}

		/// <summary>
		/// Occurs when clear button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnClearClick(object sender, RoutedEventArgs e)
		{
			// clear calculation path
			CalculationPath = String.Empty;
			numberBuilder.Clear();
			DisplayedNumber = String.Empty;
			prevOperand = 0;
		}

		/// <summary>
		/// Occurs when memory set is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMemorySetClick(object sender, RoutedEventArgs e)
		{
			Double.TryParse(DisplayedNumber.ToString(), out memory);

			BtnMemoryRestore.IsEnabled = true;
			BtnMemoryClear.IsEnabled = true;
		}

		/// <summary>
		/// Occurs when memory restore button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMemoryRestoreClick(object sender, RoutedEventArgs e)
		{
			DisplayedNumber = memory.ToString();
		}

		/// <summary>
		/// Occurs when memory clear button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMemoryClearClick(object sender, RoutedEventArgs e)
		{
			memory = 0;

			BtnMemoryRestore.IsEnabled = false;
			BtnMemoryClear.IsEnabled = false;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
