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
    public partial class Payment : Form
    {

        connection cn1 = new connection();
        DataTable dt = new DataTable();

        public Payment()
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

        private void RealizarPagamento()
        {
            try
            {
                string nomeUsuario = gunaTextBox3.Text;
                decimal valorPagamento;

                if (decimal.TryParse(gunaTextBox1.Text, out valorPagamento))
                {
                    using (MySqlConnection conn = cn1.GetConnection())
                    {
                        cn1.Open();

                        if (conn.State != ConnectionState.Open)
                        {
                            MessageBox.Show("Não foi possível abrir a conexão com o banco de dados.");
                            return;
                        }

                        int idUsuario = ObterIdUsuario(nomeUsuario, conn);

                        if (idUsuario != -1)
                        {
                            // Obter a dívida atual do usuário
                            decimal dividaAtual = ObterDividaAtual(idUsuario, conn);

                            if (dividaAtual == 0m)
                            {
                                MessageBox.Show("Este usuário não possui dívidas.");
                            }
                            else
                            {
                                // Verificar se o valor do pagamento é suficiente para quitar a dívida
                                if (valorPagamento >= dividaAtual)
                                {
                                    // Pagamento na totalidade
                                    AtualizarDivida(idUsuario, 0m, conn); // Atualiza a dívida para zero
                                    MessageBox.Show($"Pagamento de {dividaAtual:C2} realizado com sucesso. Dívida quitada.");
                                    AdicionarValorBanca(valorPagamento, conn);

                                    // Salvar dados de pagamento
                                    SalvarDadosPagamento(nomeUsuario, valorPagamento, DateTime.Now, conn);
                                }
                                else
                                {
                                    // Pagamento parcial
                                    decimal novaDivida = dividaAtual - valorPagamento;
                                    AtualizarDivida(idUsuario, novaDivida, conn);
                                    MessageBox.Show($"Pagamento de {valorPagamento:C2} realizado. Nova dívida: {novaDivida:C2}.");
                                    AdicionarValorBanca(valorPagamento, conn);

                                    // Salvar dados de pagamento
                                    SalvarDadosPagamento(nomeUsuario, valorPagamento, DateTime.Now, conn);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Usuário não encontrado.");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Digite um valor válido para o pagamento.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao realizar o pagamento: " + ex.Message);
            }
        }

        private void SalvarDadosPagamento(string nomeUsuario, decimal valorPagamento, DateTime dataPagamento, MySqlConnection conn)
        {
            try
            {
                string sql = "INSERT INTO pagamentos (name, pay, data) VALUES (@Nome, @Pagamento, @Data)";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Nome", nomeUsuario);
                    cmd.Parameters.AddWithValue("@Pagamento", valorPagamento);
                    cmd.Parameters.AddWithValue("@Data", dataPagamento);

                    cmd.ExecuteNonQuery();

                    // Adicione os dados ao DataTable
                    DataRow newRow = dt.NewRow();
                    newRow["Nome do mutuário"] = nomeUsuario;
                    newRow["Valor"] = valorPagamento;
                    newRow["Data de Pagamento"] = dataPagamento;
                    dt.Rows.Add(newRow);

                    MessageBox.Show("Dados de pagamento salvos com sucesso.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar os dados de pagamento: " + ex.Message);
            }
        }


        private void AdicionarValorBanca(decimal valor, MySqlConnection conn)
        {
            try
            {
                // Obtenha o último valor da banca
                decimal ultimoValor = ObterUltimoValorBanca(conn);

                // Calcule o novo valor somando o valor atual com o valor a ser pago
                decimal novoValor = ultimoValor + valor;

                // Atualize o valor na tabela banca
                string sql = "UPDATE banca SET valor = @NovoValor WHERE idDepo = (SELECT MAX(idDepo) FROM banca)";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@NovoValor", novoValor);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show($"Valor {valor:C2} adicionado à banca com sucesso. Novo saldo: {novoValor:C2}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao adicionar valor à banca. Detalhes: " + ex.Message);
            }
        }

        private decimal ObterUltimoValorBanca(MySqlConnection conn)
        {
            try
            {
                string sql = "SELECT valor FROM banca ORDER BY idDepo DESC LIMIT 1";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        return Convert.ToDecimal(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao obter o último valor da banca: " + ex.Message);
            }

            // Se ocorrer algum erro, retorne 0 ou outro valor padrão conforme necessário
            return 0m;
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

        private decimal ObterDividaAtual(int idUsuario, MySqlConnection conn)
        {
            string sql = "SELECT Divida FROM dividas WHERE idUser = @IdUsuario ORDER BY Data DESC LIMIT 1";

            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    return Convert.ToDecimal(result);
                }

                return 0m; // Se não houver registros, retorna 0
            }
        }

        private void AtualizarDivida(int idUsuario, decimal novaDivida, MySqlConnection conn)
        {
            try
            {
                string sql = "UPDATE dividas SET Divida = @NovaDivida, Parcela = 0, Data = @Data, Compromisso = @Compromisso WHERE idUser = @IdUsuario";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    // Adicionar parâmetros
                    cmd.Parameters.AddWithValue("@NovaDivida", novaDivida);
                    cmd.Parameters.AddWithValue("@Data", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Compromisso", "-");
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                    // Executar o comando
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Dívida atualizada com sucesso.");

                    // Verificar se a nova dívida é zero (dívida quitada)
                    if (novaDivida == 0m)
                    {
                        // Se a dívida foi quitada, apagar o registro
                        ApagarRegistroDivida(idUsuario, conn);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar a dívida: " + ex.Message);
            }
        }

        private void ApagarRegistroDivida(int idUsuario, MySqlConnection conn)
        {
            try
            {
                string deleteSql = "DELETE FROM dividas WHERE idUser = @IdUsuario";

                using (MySqlCommand deleteCmd = new MySqlCommand(deleteSql, conn))
                {
                    deleteCmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    deleteCmd.ExecuteNonQuery();

                    MessageBox.Show("Dívida quitada. Registro apagado.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao apagar o registro da dívida: " + ex.Message);
            }
        }



        private void label2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void Payment_Load(object sender, EventArgs e)
        {

        }

        private void gunaAdvenceButton1_Click(object sender, EventArgs e)
        {
            Admin novoFormulario = new Admin();
            novoFormulario.Show();
            this.Hide();
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void gunaTextBox3_Click(object sender, EventArgs e)
        {

        }

        private void gunaButton1_Click(object sender, EventArgs e)
        {
            RealizarPagamento();

            DataTable dadosParaRelatorio = ObterDadosPagamento();

            // Criar instância de ReciboPay e passar os dados
            ReciboPay n1 = new ReciboPay(dadosParaRelatorio);


            n1.ShowDialog();
        }


        private DataTable ObterDadosPagamento()
        {
            DataTable dt = new DataTable();

            using (MySqlConnection conn = cn1.GetConnection())
            {
                cn1.Open();

                if (conn.State != ConnectionState.Open)
                {
                    MessageBox.Show("Não foi possível abrir a conexão com o banco de dados.");
                    return null;
                }

                string sql = "SELECT name AS 'Nome do Mutuário', pay AS 'Valor', data AS 'Data de Pagamento' FROM pagamentos";

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn))
                {
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }




    }
}

        
    