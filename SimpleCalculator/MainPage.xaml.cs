using System;
using System.ComponentModel;
using System.Text;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

		private Operation? currentOperation = null;
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
		
			// set preferred window size
			ApplicationView.PreferredLaunchViewSize = new Size(480, 800);
			ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

			// set DataContext for property-binding to this class
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
					// if there's already an operation left
					if (currentOperation != null)
					{
						// perform previous operation
						double? result = PerformOperation(currentOperation, prevOperand, number);

						if (result != null)
						{
							prevOperand = (double)result;
							currentOperation = null;

							// display result in UI with optional three decimal places
							DisplayedNumber = String.Format("{0:0.###}", result);
						}
						else
						{
							DisplayedNumber = "Invalid Operation";
						}
					}
					else
					{
						// if there isn't any operation to perform, just save the number as previous operand
						prevOperand = number;
					}

					switch (buttonContent)
					{
						case "+":
							CalculationPath += number + " + ";
							currentOperation = Operation.Addition;
							break;

						case "-":
							CalculationPath += number + " - ";
							currentOperation = Operation.Subtraction;
							break;

						case "/":
							CalculationPath += number + " / ";
							currentOperation = Operation.Division;
							break;

						case "x":
							CalculationPath += number + " x ";
							currentOperation = Operation.Multiplication;
							break;

						case "=":
							CalculationPath = String.Empty;
							break;
					}

					numberBuilder.Clear();
				}	
			}
		}

		/// <summary>
		/// Runs an operation to the given operands.
		/// </summary>
		/// <param name="operation">Operation to run.</param>
		/// <param name="leftOperand">The operand to the left of the operator.</param>
		/// <param name="rightOperand">The operand to the right of the operator.</param>
		/// <returns>The result of the operation. Null if operation is invalid.</returns>
		private double? PerformOperation(Operation? operation, double leftOperand, double rightOperand)
		{
			double? result = null;

			switch(operation)
			{
				case Operation.Addition:
					result = leftOperand + rightOperand;
					break;

				case Operation.Subtraction:
					result = leftOperand - rightOperand;
					break;

				case Operation.Division:
					if (rightOperand == 0)
					{
						result = null;
					}
					else
					{
						result = leftOperand / rightOperand;
					}
					break;

				case Operation.Multiplication:
					result = leftOperand * rightOperand;
					break;
			}

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
			if (!String.IsNullOrEmpty(DisplayedNumber))
			{
				Double.TryParse(DisplayedNumber, out memory);

				BtnMemoryRestore.IsEnabled = true;
				BtnMemoryClear.IsEnabled = true;
			}
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
