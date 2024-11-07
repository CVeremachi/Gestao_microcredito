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
    public partial class Registo3 : Form
    {
        connection connecting = new connection();
        private string nomeUsuario;

        public Registo3(string nomeUsuario)
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.nomeUsuario = nomeUsuario;
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

        private void label9_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label10_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void gunaAdvenceButton1_Click(object sender, EventArgs e)
        {
            Registo r2 = new Registo();
            r2.Show();
            this.Hide();
        }

        private void RegistarProximo(string nomeDoUsuario)
        {
            try
            {
                using (MySqlConnection conn = connecting.GetConnection())
                {
                    connecting.Open(); // Abra a conexão explicitamente

                    // Verifique se a conexão está aberta antes de prosseguir
                    if (conn.State != ConnectionState.Open)
                    {
                        MessageBox.Show("Não foi possível abrir a conexão com o banco de dados.");
                        return;
                    }

                    // Obter o ID do usuário associado
                    int idUser = ObterIdUsuario(nomeDoUsuario, conn); // Use o nome real do usuário

                    if (idUser == -1)
                    {
                        MessageBox.Show("Erro ao obter o ID do usuário.");
                        return;
                    }

                    string sql = "INSERT INTO proximidades (nome, parentesco, endereço, idUser, locTrabalho, ContactoTrabal, ContactoPess) " +
                                 "VALUES (@Nome, @Parentesco, @Endereco, @IdUser, @LocTrabalho, @ContactoTrabal, @ContactoPess)";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        // Adicionar parâmetros
                        cmd.Parameters.AddWithValue("@Nome", gunaTextBox1.Text);
                        cmd.Parameters.AddWithValue("@Parentesco", gunaTextBox2.Text);
                        cmd.Parameters.AddWithValue("@Endereco", gunaTextBox3.Text);
                        cmd.Parameters.AddWithValue("@IdUser", idUser); // Usar o ID do usuário associado
                        cmd.Parameters.AddWithValue("@LocTrabalho", gunaTextBox4.Text);
                        cmd.Parameters.AddWithValue("@ContactoTrabal", gunaTextBox5.Text);
                        cmd.Parameters.AddWithValue("@ContactoPess", gunaTextBox6.Text);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Registro de 'Alguém Próximo' bem-sucedido!");
                            // Limpar campos ou realizar outras ações necessárias após o registro
                        }
                        else
                        {
                            MessageBox.Show("Erro ao registrar 'Alguém Próximo'. Verifique e tente novamente.");
                        }
                    } // A instrução 'using' fechará automaticamente a conexão aqui
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("Erro ao registrar 'Alguém Próximo'. Detalhes: " + ex.Message);
            }
        }


        private int ObterIdUsuario(string nomeUsuario, MySqlConnection conn)
        {
            string sql = "SELECT idUser FROM user WHERE Nome = @Nome";

            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Nome", nomeUsuario);

                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    return Convert.ToInt32(result);
                }

                return -1;
            }
        }

        private void Registo3_Load(object sender, EventArgs e)
        {
            // Carregue dados ou execute lógica de inicialização, se necessário
        }

        private void gunaTextBox1_Click(object sender, EventArgs e)
        {
            // Lógica para lidar com o clique no textbox1, se necessário
        }

        private void gunaTextBox2_Click(object sender, EventArgs e)
        {
            // Lógica para lidar com o clique no textbox2, se necessário
        }

        private void gunaTextBox3_Click(object sender, EventArgs e)
        {
            // Lógica para lidar com o clique no textbox3, se necessário
        }

        private void gunaTextBox4_Click(object sender, EventArgs e)
        {
            // Lógica para lidar com o clique no textbox4, se necessário
        }

        private void gunaTextBox5_Click(object sender, EventArgs e)
        {
            // Lógica para lidar com o clique no textbox5, se necessário
        }

        private void gunaTextBox6_Click(object sender, EventArgs e)
        {
            // Lógica para lidar com o clique no textbox6, se necessário
        }

        private void gunaButton3_Click(object sender, EventArgs e)
        {
            // Lógica para lidar com o clique no botão, se necessário
            RegistarProximo(nomeUsuario);
        }
    }
}
