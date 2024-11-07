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
    public partial class Registo : Form
    {
        connection connecting = new connection();
        public Registo()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            connecting = new connection();

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

        private void Registo_Load(object sender, EventArgs e)
        {

        }

        private void gunaTextBox1_Click(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void gunaCirclePictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void gunaTextBox2_Click(object sender, EventArgs e)
        {

        }

        private void gunaTextBox4_Click(object sender, EventArgs e)
        {

        }

        private void gunaLabel1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void gunaLabel2_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }



        private void gunaAdvenceButton1_Click(object sender, EventArgs e)
        {
            Admin novoFormulario = new Admin();
            novoFormulario.Show();
             this.Hide();
        }

        private void gunaAdvenceButton2_Click(object sender, EventArgs e)
        {
            Empregador novoFormulario = new Empregador();
            novoFormulario.Show();

            // Fecha o Form1
            this.Hide();

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
        public string Registar(string nomeUsuario)
        {
            try
            {
                using (MySqlConnection conn = connecting.GetConnection())
                {
                    if (conn != null)
                    {
                        if (conn.State == ConnectionState.Closed)
                        {
                            conn.Open();
                        }

                        if (conn.State == ConnectionState.Open)
                        {
                            string sql = "INSERT INTO user (Nome, Gender, Bi, DataNas, Contacto, Endereço, Andar, Bairro, EstadoCiv, TelFixo, Proximo) " +
                                         "VALUES (@Nome, @Genero, @Bi, @DataNascimento, @Contacto, @Endereco, @Andar, @Bairro, @EstadoCivil, @TelFixo, @Proximo)";

                            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                            {
                                if (gunaTextBox1 != null)
                                {
                                    cmd.Parameters.AddWithValue("@Nome", gunaTextBox1.Text);
                                }
                                else
                                {
                                    MessageBox.Show("O controle gunaTextBox1 é nulo.");
                                    return null;
                                }

                                cmd.Parameters.AddWithValue("@Genero", ObterGeneroSelecionado());
                                cmd.Parameters.AddWithValue("@DataNascimento", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                                cmd.Parameters.AddWithValue("@Bi", gunaTextBox3.Text);
                                cmd.Parameters.AddWithValue("@Contacto", gunaTextBox4.Text);
                                cmd.Parameters.AddWithValue("@Endereco", gunaTextBox5.Text);
                                cmd.Parameters.AddWithValue("@Andar", gunaTextBox6.Text);
                                cmd.Parameters.AddWithValue("@Bairro", gunaTextBox7.Text);
                                cmd.Parameters.AddWithValue("@EstadoCivil", ObterEstadoCivilSelecionado());
                                cmd.Parameters.AddWithValue("@TelFixo", gunaTextBox8.Text);
                                cmd.Parameters.AddWithValue("@Proximo", AlguemProximo());

                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Registro bem-sucedido!");
                                    return nomeUsuario;
                                }
                                else
                                {
                                    MessageBox.Show("Erro ao registrar os dados. Verifique e tente novamente.");
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("A conexão não foi aberta com sucesso.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("A instância de conexão é nula.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao registrar os dados. Detalhes: " + ex.Message);
            }

            // Retorna null em caso de erro
            return null;
        }



        private bool ValidarCampos()
        {
            // Verificar se o campo Nome está preenchido
            if (string.IsNullOrWhiteSpace(gunaTextBox1.Text))
            {
                MessageBox.Show("Por favor, preencha o campo Nome.", "Campo Obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Verificar se um gênero foi selecionado
            if (!radioButton1.Checked && !radioButton2.Checked)
            {
                MessageBox.Show("Por favor, selecione o gênero.", "Campo Obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Verificar se o campo BI está preenchido
            if (string.IsNullOrWhiteSpace(gunaTextBox3.Text))
            {
                MessageBox.Show("Por favor, preencha o campo BI.", "Campo Obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Verificar se o campo DataNas está preenchido
            if (dateTimePicker1.Value == DateTime.MinValue)
            {
                MessageBox.Show("Por favor, selecione a data de nascimento.", "Campo Obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Verificar se o campo Contacto está preenchido e contém apenas números
            if (string.IsNullOrWhiteSpace(gunaTextBox4.Text) || !gunaTextBox4.Text.All(char.IsDigit))
            {
                MessageBox.Show("Por favor, preencha um número de contacto válido.", "Campo Obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Verificar se o campo Endereço está preenchido
            if (string.IsNullOrWhiteSpace(gunaTextBox5.Text))
            {
                MessageBox.Show("Por favor, preencha o campo Endereço.", "Campo Obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Verificar se o campo Andar está preenchido
            if (string.IsNullOrWhiteSpace(gunaTextBox6.Text))
            {
                MessageBox.Show("Por favor, preencha o campo Andar.", "Campo Obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Verificar se o campo Bairro está preenchido
            if (string.IsNullOrWhiteSpace(gunaTextBox7.Text))
            {
                MessageBox.Show("Por favor, preencha o campo Bairro.", "Campo Obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Verificar se um estado civil foi selecionado
            if (!gunaRadioButton1.Checked && !gunaRadioButton2.Checked) 
            {
                MessageBox.Show("Por favor, selecione o estado civil.", "Campo Obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Verificar se o campo TelFixo está preenchido e contém apenas números
            if (string.IsNullOrWhiteSpace(gunaTextBox8.Text) || !gunaTextBox8.Text.All(char.IsDigit))
            {
                MessageBox.Show("Por favor, preencha um número de telefone fixo válido.", "Campo Obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            
            return true;
        }


        private string ObterGeneroSelecionado()
        {
            if (radioButton1.Checked)
            {
                return "Masculino";
            }
            else if (radioButton2.Checked)
            {
                return "Feminino";
            }
            else
            {
                return "Outro";
            }
        }

        private string ObterEstadoCivilSelecionado()
        {
            if (gunaRadioButton1.Checked)
            {
                return "Solteiro(a)";
            }
            else if (gunaRadioButton2.Checked)
            {
                return "Casado(a)";
            }
            else
            {
                return "Outro";
            }
        }

        private string AlguemProximo()
        {
            if (gunaMediumRadioButton1.Checked)
            {
                

                return "Sim";
            }
            else if (gunaMediumRadioButton2.Checked)
            {
                return "Nao";

            }
            else {
                return "Nao declarado";
            }
        }

       
         private void LimparCampos()
          {
              // Limpar os campos do formulário após o registro bem-sucedido
              gunaTextBox1.Text = string.Empty;
              gunaTextBox8.Text = string.Empty;
              gunaTextBox3.Text = string.Empty;
              gunaTextBox4.Text = string.Empty;
              gunaTextBox5.Text = string.Empty;
              gunaTextBox6.Text = string.Empty;
              gunaTextBox7.Text = string.Empty;
              radioButton1.Checked = false;
              radioButton2.Checked = false;
              gunaRadioButton1.Checked = false;
              gunaRadioButton2.Checked = false;
              gunaMediumRadioButton1.Checked = false;
              gunaMediumRadioButton2.Checked = false;
             
          }

       

        private void gunaButton1_Click(object sender, EventArgs e)
        {
          
            if (ValidarCampos())
            {
                string nomeUsuario = gunaTextBox1.Text;
                string nomeUsuarioRegistrado = Registar(nomeUsuario);

                if (!string.IsNullOrEmpty(nomeUsuarioRegistrado))
                {
                    if (gunaMediumRadioButton1.Checked)
                    {
                        Registo3 formularioProximo = new Registo3(nomeUsuarioRegistrado);
                        formularioProximo.Show();
                        this.Hide();
                    }
                    else
                    {
                        LimparCampos();
                    }
                }
            }
        }

        private void gunaRadioButton3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void gunaLabel3_Click(object sender, EventArgs e)
        {

        }

        private void gunaMediumRadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            
         
        }

      

        private void gunaMediumRadioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void gunaLabel8_Click(object sender, EventArgs e)
        {

        }
    }
}

