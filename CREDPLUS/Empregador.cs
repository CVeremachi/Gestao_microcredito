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
    public partial class Empregador : Form
    {

        connection cn = new connection();
        private string nomeUsuario;

        public Empregador()
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
        private void gunaLabel1_Click(object sender, EventArgs e)
        {

        }

        private void Empregador_Load(object sender, EventArgs e)
        {

        }

        private void SalvarInformacoesEmpregador()
        {
            try
            {
                using (MySqlConnection conn = cn.GetConnection()) // Certifique-se de substituir 'connection' pelo nome da sua classe de conexão
                {
                    cn.Open(); // Abra a conexão com o banco de dados

                    string sql = "INSERT INTO empregador (idUser, Empregador, endereço, Contacto, Cargo, Chefe, Departamento, Contrato, InTrabalho, DataPay) " +
                                 "VALUES (@idUser, @Empregador, @endereço, @Contacto, @Cargo, @Chefe, @Departamento, @Contrato, @InTrabalho, @DataPay)";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        // Substitua os parâmetros pelos valores reais dos seus controles
                        cmd.Parameters.AddWithValue("@idUser", ObterIdUsuario(nomeUsuario, conn)); // Substitua isso pela lógica para obter o id do usuário logado
                        cmd.Parameters.AddWithValue("@Empregador", gunaTextBox1.Text);
                        cmd.Parameters.AddWithValue("@endereço", gunaTextBox2.Text);
                        cmd.Parameters.AddWithValue("@Contacto", gunaTextBox4.Text);
                        cmd.Parameters.AddWithValue("@Cargo", gunaTextBox9.Text);
                        cmd.Parameters.AddWithValue("@Chefe", gunaTextBox3.Text);
                        cmd.Parameters.AddWithValue("@Departamento", gunaTextBox10.Text);
                        cmd.Parameters.AddWithValue("@Contrato", gunaTextBox5.Text);
                        cmd.Parameters.AddWithValue("@InTrabalho", dateTimePicker2.Value);
                        cmd.Parameters.AddWithValue("@DataPay", dateTimePicker1.Value);

                        cmd.ExecuteNonQuery(); // Execute o comando SQL para inserir os dados

                        MessageBox.Show("Informações do empregador salvas com sucesso!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar informações do empregador: " + ex.Message);
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

        private void gunaLabel7_Click(object sender, EventArgs e)
        {

        }

        private void gunaTextBox6_Click(object sender, EventArgs e)
        {

        }

     

        private void label2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void gunaButton2_Click(object sender, EventArgs e)
        {
            SalvarInformacoesEmpregador();
        }

        private void gunaAdvenceButton2_Click(object sender, EventArgs e)
        {
            Bank novoFormulario = new Bank();
            novoFormulario.Show();

            // Fecha o Form1
            this.Hide();
        }

        private void gunaAdvenceButton1_Click(object sender, EventArgs e)
        {
            Registo novoFormulario = new Registo();
            novoFormulario.Show();

            // Fecha o Form1
            this.Hide();
        }

        private void gunaTextBox1_Click(object sender, EventArgs e)
        {

        }

        private void gunaTextBox2_Click(object sender, EventArgs e)
        {

        }

        private void gunaTextBox4_Click(object sender, EventArgs e)
        {

        }

        private void gunaTextBox9_Click(object sender, EventArgs e)
        {

        }

        private void gunaTextBox3_Click(object sender, EventArgs e)
        {

        }

        private void gunaTextBox10_Click(object sender, EventArgs e)
        {

        }

        private void gunaTextBox5_Click(object sender, EventArgs e)
        {

        }

        private void gunaTextBox6_Click_1(object sender, EventArgs e)
        {

        }

        private void gunaTextBox8_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
