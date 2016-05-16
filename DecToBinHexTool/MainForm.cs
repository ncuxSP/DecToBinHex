using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace DecToBinHexTool
{
    public partial class MainForm : Form, IView
    {
        public MainForm()
        {
            InitializeComponent();
            addButton.Click += addButton_Click;
            inputTextBox.KeyPress += inputTextBox_KeyPress;
            inputTextBox.KeyDown += inputTextBox_KeyDown;
        }

        public event ComputeNewNumberEvH ComputeNewNumber;

        public void BindDataSource(BindingList<string> container)
        {
            resultList.DataSource = container;
        }

        private void inputTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar))
            {
                if (e.KeyChar == '-' && ((sender as TextBox).SelectionStart == 0))
                {
                    //Check '-' at beginning of number
                }
                else if (e.KeyChar == '\b')
                {
                    //Backspace is ok too
                }
                else
                {
                    e.Handled = true;
                }
            }
        }

        private void inputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                TryParseInt();
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            TryParseInt();
        }

        private void TryParseInt()
        {
            int n = 0;
            try
            {
                n = Convert.ToInt32(inputTextBox.Text);
            }
            catch (Exception ex) when (ex is FormatException || ex is OverflowException)
            {
                MessageBox.Show("Try to enter integer number between " + int.MinValue + " and " + int.MaxValue);
                return;
            }

            ComputeNewNumber?.Invoke(n);
        }

    }
}
