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
    public partial class depo : Form
    {
        public Admin ad1;
        connection cn2 = new connection();
        public depo(Admin adminForm)
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            ad1 = adminForm;
            cn2 = new connection();
            
           
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
        private void depo_Load(object sender, EventArgs e)
        {

        }

        private void gunaLabel7_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void gunaLabel5_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void gunaAdvenceButton1_Click(object sender, EventArgs e)
        {
           
            ad1.Show();

            this.Hide();
        }

        private void gunaButton1_Click(object sender, EventArgs e)
        {
          
            
            RealizarDeposito();
            
            
            /*  if (float.TryParse(gunaTextBox2.Text, out float valorDeposito))
            {
                ad1.AtualizarBanca(valorDeposito);
            }*/
        }

        private void RealizarDeposito()
        {
            try
            {
                using (MySqlConnection conn = cn2.GetConnection())
                {
                    cn2.Open();

                    if (conn.State != ConnectionState.Open)
                    {
                        MessageBox.Show("Não foi possível abrir a conexão com o banco de dados.");
                        return;
                    }

                    // Verificar se o nome do usuário está registrado na tabela user
                    string nomeUsuario = gunaTextBox1.Text;

                    int idADM = ObterIdADM(nomeUsuario, conn);

                    if (idADM == -1)
                    {
                        MessageBox.Show("O nome de usuário fornecido não está registrado. Depósito não permitido.");
                        return;
                    }

                    // Obter o saldo atual da banca
                    decimal saldoAtual = ObterSaldoAtual(idADM, conn);

                    // Inserir dados na tabela banca
                    string sql = "INSERT INTO banca (idADM, data, valor) VALUES (@IdADM, @Data, @Valor)";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@IdADM", idADM);
                        cmd.Parameters.AddWithValue("@Data", DateTime.Now);
                        if (decimal.TryParse(gunaTextBox2.Text, out decimal valorDeposito))
                        {
                            // Acumular o valor do depósito ao saldo atual
                            decimal novoSaldo = saldoAtual + valorDeposito;
                            cmd.Parameters.AddWithValue("@Valor", novoSaldo);
                        }
                        else
                        {
                            MessageBox.Show("Por favor, insira um valor válido para o depósito.");
                            return;
                        }

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Depósito registrado com sucesso!");
                            // Realizar outras ações necessárias após o depósito

                            ad1.AtualizarBanca(); // Certifique-se de implementar esse método em Admin

                        }
                        else
                        {
                            MessageBox.Show("Erro ao registrar o depósito. Verifique e tente novamente.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao registrar o depósito. Detalhes: " + ex.Message);
            }
        }

        private decimal ObterSaldoAtual(int idADM, MySqlConnection conn)
        {
            string sql = "SELECT valor FROM banca WHERE idADM = @IdADM ORDER BY data DESC LIMIT 1";

            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@IdADM", idADM);

                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    return Convert.ToDecimal(result);
                }

                return 0m; // Se não houver registros, retorna 0
            }
        }

        private int ObterIdADM(string nomeUsuario, MySqlConnection conn)
        {
            string sql = "SELECT idADM FROM admin WHERE name = @Nome";

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



        private void gunaTextBox1_Click(object sender, EventArgs e)
        {

        }

        private void gunaTextBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
