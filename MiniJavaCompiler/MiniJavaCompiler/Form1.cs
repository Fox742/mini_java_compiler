using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            richTextBox2.Clear();
            treeView1.Nodes.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Compiler MiniJava = new Compiler();

            MiniJava.Compile(richTextBox1.Lines,dataGridView1, 
                richTextBox2,treeView1,textBox1);


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Grammar.InitGrammar();
        }
    }
}
