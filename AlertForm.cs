using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CheckFlexService
{
    public partial class AlertForm : Form
    {
        public AlertForm(string text)
        {
            InitializeComponent();

            // 아라아닐이
            lbText.Text = text;
        }
    }
}
