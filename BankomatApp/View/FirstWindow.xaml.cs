using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace BankomatApp.View
{
    public partial class FirstWindow : Window
    {
        public FirstWindow()
        {
            InitializeComponent();
        }
        private void CardNumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {           
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c))
                {                    
                    e.Handled = true;
                    return;
                }
            }
            TextBox textBox = sender as TextBox;
            string text = textBox.Text;            
            if (text.Length == 4 || text.Length == 9 || text.Length == 14)
            {
                if (e.Text != "-")
                {
                    textBox.Text += "-";
                    textBox.CaretIndex = textBox.Text.Length;
                }
            }           
            if (text.Length >= 19)
            {
                e.Handled = true;
                return;
            }
        }
        private void ClearTextBox_Click(object sender, RoutedEventArgs e)
        {
            CardNumberTextBox.Text = "";
        }
}
}
