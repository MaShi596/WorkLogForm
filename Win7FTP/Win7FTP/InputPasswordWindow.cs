using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Win7FTP
{
    public partial class InputPasswordWindow : Form
    {
        public InputPasswordWindow(string txt)
        {
            this.Text = txt;
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text == "iti@240$")
            {
                //if (MessageBox.Show("确定删除文件吗？", "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                //{
                //    this.DialogResult = DialogResult.OK;
                //}
                //else
                //{
 
                //}
                this.DialogResult = DialogResult.OK;

            }
            else
            {
                MessageBox.Show("密码输入错误，您没有该权限");
                this.Close();
            
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
