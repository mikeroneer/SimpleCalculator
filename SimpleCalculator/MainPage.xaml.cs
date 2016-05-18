using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
		public enum Operation { Addition, Subtraction, Multiply, Divide }

		private double? calculationResult;
		private string calculationPath;

		private double prevOperand = 0;
		private Operation? operation = null;

		public double? CalculationResult
		{
			get
			{
				return calculationResult;
			}
			set
			{
				calculationResult = value;
				OnPropertyChanged("CalculationResult");
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

			CalculationResult = 0;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		bool isResult = false;
		bool getNextOperand = false;

		double? operand1 = null;
		double? operand2 = null;

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button)
			{
				var button = sender as Button;
				string buttonContent = button.Content.ToString();
				double number;

				if (Double.TryParse(button.Content.ToString(), out number))
				{
					if (getNextOperand)
					{
						CalculationResult = 0;
						getNextOperand = false;
					}

					CalculationResult = CalculationResult * 10 + number;
				}
				else
				{
					switch (buttonContent)
					{
						case "+":
							CalculationPath += CalculationResult + " + ";

							PerformOperation();
							operand1 = CalculationResult;
							operation = Operation.Addition;
							break;

						case "-":
							CalculationPath += CalculationResult + " - ";

							PerformOperation();
							operand1 = CalculationResult;
							operation = Operation.Subtraction;
							break;

						case "/":
							CalculationPath += CalculationResult + " / ";

							PerformOperation();
							operand1 = CalculationResult;
							operation = Operation.Divide;
							break;

						case "x":
							CalculationPath += CalculationResult + " x ";

							PerformOperation();
							operand1 = CalculationResult;
							operation = Operation.Multiply;
							break;

						case "=":
							CalculationPath = null;
							PerformOperation();
							break;
					}

					getNextOperand = true;


				}
			}
		}

		private void PerformOperation()
		{
			switch(operation)
			{
				case Operation.Addition:
					CalculationResult += operand1;
					break;

				case Operation.Subtraction:
					CalculationResult = operand1 - CalculationResult;
					break;

				case Operation.Divide:
					CalculationResult = operand1 / CalculationResult;
					break;

				case Operation.Multiply:
					CalculationResult = operand1 * CalculationResult;
					break;
			}

			operation = null;
		}

		private void OnPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
