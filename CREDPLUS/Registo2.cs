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

   
    public partial class Registo2 : Form
    {
        const float mensal = 0.30f;
        const float diario = 0.01f;
        float Divida = 0f;
        float parcelas=0f;
        public int valorParcelas { get; set; }

        connection cn1 = new connection();
        Admin admin = new Admin();

       
        public Registo2()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            cn1 = new connection();
            admin = new Admin();

            

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

      
        private void Registo2_Load(object sender, EventArgs e)
        {

        }

        private void gunaPictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        //calculo mensal do emprestimo
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {


            if (mes.Checked)
            {
                textBoxParcelas.Text = "1";
                if (float.TryParse(tako.Text, out float valorTako) && int.TryParse(textBoxParcelas.Text, out int valorParcelas))
                {

                    Divida = (valorTako * mensal * valorParcelas) + valorTako;

                   


                    textBoxParcelas.Enabled = false;

                }
                else
                {
                    MessageBox.Show("Digite um valor válido.");
                }
            }
            else
            {

                textBoxParcelas.Enabled = true;
            }


            
        }


        //calculo diario do emprestimo
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            
            
                if (day.Checked)
                {
                    if (float.TryParse(tako.Text, out float valorTako) && int.TryParse(textBoxParcelas.Text, out int valorParcelas))
                    {
                        //Divida = valorTako * (float)Math.Pow(0 + diario, valorParcelas);
                        Divida = (valorTako * diario * valorParcelas)+valorTako;
                          
                }
                    else
                    {
                        MessageBox.Show("Digite um valor válido.");
                    }

            }
        }

        
        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private bool VerificarUsuarioExistente(string nomeUsuario)
        {
            try
            {
                using (MySqlConnection conn = cn1.GetConnection())
                {
                    if (conn != null && conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    if (conn != null && conn.State == ConnectionState.Open)
                    {
                        string sql = "SELECT COUNT(*) FROM user WHERE Nome = @Nome";

                        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@Nome", nomeUsuario);

                            int count = Convert.ToInt32(cmd.ExecuteScalar());

                            return count > 0; // Se o count for maior que 0, o usuário existe
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao verificar usuário: " + ex.Message);
                return false;
            }

            return false;
        }



        private void RegistarDivida()
        {
            float valorTako = float.Parse(tako.Text);
            try
            {
                if (ValidarCamposDivida())
                {
                    string nomeUsuario = gunaTextBox3.Text;

                    if (VerificarUsuarioExistente(nomeUsuario))
                    {
                        using (MySqlConnection conn = cn1.GetConnection())
                        {
                            if (conn != null && conn.State == ConnectionState.Closed)
                            {
                                conn.Open();
                            }

                            if (conn != null && conn.State == ConnectionState.Open)
                            {
                                // Obter o ID do usuário a partir do nome
                                int idUsuario = ObterIdUsuario(nomeUsuario, conn);

                                if (idUsuario != -1)
                                {
                                    string sql = "INSERT INTO dividas (Divida, Parcela, Data, DataPg, Compromisso, idUser) " +
                                                 "VALUES (@Divida, @Parcela, @Data, @DataPg, @Compromisso, @IdUsuario)";

                                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                                    {
                                        // Adicionar parâmetros
                                        cmd.Parameters.AddWithValue("@Divida", Divida);
                                        cmd.Parameters.AddWithValue("@Parcela", textBoxParcelas.Text); // Usar o valor calculado
                                        cmd.Parameters.AddWithValue("@Data", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                                        cmd.Parameters.AddWithValue("DataPg", dateTimePicker2.Value.ToString("yyyy-MM-dd"));
                                        cmd.Parameters.AddWithValue("@Compromisso", gunaTextBox1.Text);
                                        cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                                        // Executar o comando
                                        cmd.ExecuteNonQuery();

                                      

                                        MessageBox.Show("Dívida registrada com sucesso.");
                                        LimparCampos();
                                    }

                                    float ultimoValorBanca = admin.ObterUltimoValorBanca();

                                    // Subtrair o valor do empréstimo
                                    float novoValorBanca = ultimoValorBanca - valorTako;

                                    // Atualizar o valor na banca
                                    admin.AtualizarValorBanca(novoValorBanca);
                                }
                                else
                                {
                                    MessageBox.Show("Erro ao obter o ID do usuário.");
                                    LimparCampos();
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Usuário não encontrado. Cadastre o usuário antes de fazer o empréstimo.");
                        LimparCampos();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao registrar dívida: " + ex.Message);
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

        private bool ValidarCamposDivida()
        {
            if (Divida < 0)
            {
                MessageBox.Show("O valor da dívida deve ser maior que zero.");
                return false;
            }

            if (parcelas < 0)
            {
                MessageBox.Show("O número de parcelas deve ser maior que zero.");
                return false;
            }

            // Adicione outras validações conforme necessário

            return true;
        }

        private void LimparCampos()
        {
            // Limpar campos relacionados ao empréstimo
            tako.Text = string.Empty;
            textBoxParcelas.Text = string.Empty;
            dateTimePicker1.Value = DateTime.Now;
            gunaTextBox1.Text = string.Empty;

            // Limpar campos relacionados ao usuário
            gunaTextBox3.Text = string.Empty;

            // Desmarcar seleção nos radio buttons
            mes.Checked = false;
            day.Checked = false;

            // Habilitar o TextBox de parcelas
            textBoxParcelas.Enabled = true;
        }


        private void gunaAdvenceButton1_Click(object sender, EventArgs e)
        {
            Admin novoFormulario = new Admin();
            novoFormulario.Show();
            this.Hide();
        }

        private void parcel_Click(object sender, EventArgs e)
        {
            if (float.TryParse(textBoxParcelas.Text, out float valorParcelas))
            {
              
                // Calculo das parcelas com base no valor inserido no TextBox de parcelas
                parcelas = Divida / valorParcelas;
                
               // MessageBox.Show($"Número de parcelas calculado: {parcelas}");
            }
            else
            {
             //MessageBox.Show("Digite um valor válido no campo de parcelas.");
            }
        }


        private void gunaButton1_Click(object sender, EventArgs e)
        {
            RegistarDivida();

        }

       

        private void gunaTextBox3_Click(object sender, EventArgs e)
        {

        }

        public void tako_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void gunaButton2_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
