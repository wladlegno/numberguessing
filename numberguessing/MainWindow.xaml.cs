using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace numberguessing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static readonly Regex Regex = new Regex("[^0-9]+");
        private int _lives = 10;
        private bool _isInputForced;
        private bool _canUserType = true;

        public int Lives
        {
            get => _lives;
            set
            {
                _lives = value;
                OnLivesChange();
            }
        }

        private int _pickedNumber;

        public MainWindow()
        {
            InitializeComponent();
            PickNumber();
            OnLivesChange();
            TxbInput.Focus();
        }

        private void PickNumber()
        {
            Random random = new Random();
            _pickedNumber = random.Next(0, 100);
        }

        private void OnLivesChange()
        {
            LblLives.Content = $"Remaining lives: {Lives}";
        }

        private void GuessChecker(int number)
        {
            _isInputForced = true;
            if (number == _pickedNumber)
            {
                TxbInput.Background = new SolidColorBrush(Colors.PaleGreen);
                TxbInput.Text = "Congrats!";
                _canUserType = false;
            }
            else
            {
                TxbInput.Background = new SolidColorBrush(Colors.PaleVioletRed);
                if (Lives > 0)
                {
                    TxbInput.Text = "Wrong.";
                    Lives -= 1;
                }
                else
                {
                    TxbInput.Text = "You lost!";
                    _canUserType = false;
                }
            }
        }

        private bool InputValidator(string str)
        {
            return !Regex.IsMatch(str);
        }

        private bool InputValidator(Key key)
        {
            char num = key.ToString().ToCharArray()[key.ToString().Length - 1];
            bool isValid = InputValidator(num.ToString());

            if (key == Key.Enter || key == Key.Back || key == Key.Escape || isValid)
            {
                return true;
            }

            return false;
        }

        private void TxbInput_OnGotFocus(object sender, RoutedEventArgs e)
        {
            TxbInput.Foreground = new SolidColorBrush(Colors.Black);
            TxbInput.Text = "";
        }

        private void TxbInput_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_canUserType == false)
            {
                e.Handled = true;
                return;
            }

            switch (e.Key)
            {
                case Key.Space:
                    e.Handled = true;
                    break;
                case Key.Enter:
                    if (InputValidator(TxbInput.Text) && TxbInput.Text != "")
                    {
                        // TxbInput.Background = new SolidColorBrush(Colors.Red);
                        GuessChecker(Int32.Parse(TxbInput.Text));
                    }
                    else
                    {
                        _isInputForced = false;
                        TxbInput.Background = new SolidColorBrush(Colors.White);
                        TxbInput.Text = "";
                    }

                    break;
                case Key.Escape:
                    Close();
                    break;
                default:
                    if (_isInputForced)
                    {
                        _isInputForced = false;
                        TxbInput.Background = new SolidColorBrush(Colors.White);
                        TxbInput.Text = "";
                    }

                    e.Handled = !InputValidator(e.Key);
                    break;
            }
        }
    }
}