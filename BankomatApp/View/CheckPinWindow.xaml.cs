using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BankomatApp.View
{
    /// <summary>
    /// Логика взаимодействия для CheckPinWindow.xaml
    /// </summary>
    public partial class CheckPinWindow : Window
    {
        public CheckPinWindow()
        {
            InitializeComponent();
        }
        private void ClearTextBox_Click(object sender, RoutedEventArgs e)
        {
            PinTextBox.Text = "";
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void PinTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
                return;
            }
            TextBox textBox = sender as TextBox;
            if (textBox.Text.Length >= 4)
            {
                e.Handled = true;
            }
        }

    }
}
