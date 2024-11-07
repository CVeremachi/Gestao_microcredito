using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;


namespace CREDPLUS
{
    public partial class Form1 : Form
    {
        connection con = new connection();
        string id, username, password;

        public Form1()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
          

        }

        private const int cGrip = 16;
        private const int cCaption = 32;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x84)
            {
                Point pos = new Point(m.LParam.ToInt32());
                pos = this.PointToClient(pos);
                if (pos.Y < cCaption)
                {
                    m.Result = (IntPtr)2;
                    return;

                }
                if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
                {
                    m.Result = (IntPtr)17;
                    return;
                }
            }
            base.WndProc(ref m);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        public void gunaAdvenceButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtusername.Text != "" && txtpassword.Text != "")
                {
                    Class1.uname = txtusername.Text;

                    using (MySqlConnection conn = con.GetConnection())
                    {
                        con.Open();
                        string query = "SELECT idADM, name, password FROM Admin WHERE name = @Name";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@Name", txtusername.Text);

                            using (MySqlDataReader row = cmd.ExecuteReader())
                            {
                                if (row.HasRows)
                                {
                                    while (row.Read())
                                    {
                                        id = row["idADM"].ToString();
                                        username = row["name"].ToString();
                                        password = row["password"].ToString();

                                        if (password == txtpassword.Text)
                                        {
                                            MessageBox.Show("Bem-vindo, Sr(a) " + username);
                                            var novoFormulario = new Admin();
                                            novoFormulario.Show();
                                            // Fecha o Form1
                                            this.Hide();
                                        }
                                        else
                                        {
                                            MessageBox.Show("Senha incorreta", "Information");
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Nome de usuário incorreto", "Information");
                                }
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Username or Password is empty", "Information");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao realizar o login: " + ex.Message, "Information");
            }
   
    }




        private void gunaTextBox2_Click(object sender, EventArgs e)
        {

        }

        private void gunaTextBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void gunaLabel1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void gunaCirclePictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void gunaLabel2_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void gunaCirclePictureBox2_Click(object sender, EventArgs e)
        {

        }

       
    }
}
