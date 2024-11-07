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
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;

namespace CREDPLUS
{
    public partial class Admin : Form
    {
        connection cn3 = new connection();
        private System.Windows.Forms.Timer paymentCheckTimer;
        private DateTime ultimaExibicaoAlerta = DateTime.MinValue;
        
        private bool notificacaoExibida = false;
        public float Banca { get; private set; }
        string nomeUsuario;
        private Image fotoAdministrador;


        public Admin()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            label4.Text = Class1.uname;
            AtualizarBanca();
            PreencherDataGridView(nomeUsuario);
            PreencherChart();
            AtualizarProgressBar();
            dataGridView1.CellEndEdit += new DataGridViewCellEventHandler(DataGridView1_CellEndEdit);
            paymentCheckTimer = new System.Windows.Forms.Timer();
            paymentCheckTimer.Interval = 86400;
            paymentCheckTimer.Tick += new EventHandler(CheckPaymentDate);
            paymentCheckTimer.Start();
            CarregarFotoAdministrador();
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

        public void AtualizarValorBanca(float novoValor)
        {
            try
            {
                using (MySqlConnection conn = cn3.GetConnection())
                {
                    cn3.Open();

                    if (conn.State != ConnectionState.Open)
                    {
                        MessageBox.Show("Não foi possível abrir a conexão com o banco de dados.");
                        return;
                    }

                    string sql = "UPDATE banca SET valor = @NovoValor WHERE idDepo = (SELECT MAX(idDepo) FROM banca)";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@NovoValor", novoValor);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Valor na banca atualizado com sucesso!");
                            AtualizarBanca(); // Atualiza a exibição do valor na interface
                        }
                        else
                        {
                            MessageBox.Show("Erro ao atualizar o valor na banca. Verifique e tente novamente.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar o valor na banca. Detalhes: " + ex.Message);
            }
        }

        private void CarregarFotoAdministrador()
        {
            try
            {
                using (MySqlConnection connection = cn3.GetConnection())
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }

                    string query = "SELECT foto FROM admin WHERE name = @Nome";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Nome", Class1.uname);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader["foto"] != DBNull.Value)
                            {
                                byte[] imageBytes = (byte[])reader["foto"];

                                // Redimensionar a imagem para 41x39 pixels
                                Image fotoOriginal = ByteArrayToImage(imageBytes);
                                Image fotoRedimensionada = ResizeImage(fotoOriginal, 41, 39);

                                // Exibir a imagem redimensionada no gunaCirclePictureBox1
                                gunaCirclePictureBox1.Image = fotoRedimensionada;

                                // Adicione uma mensagem de depuração
                                Console.WriteLine("Foto carregada com sucesso!");
                            }
                            else
                            {
                                // Adicione uma mensagem de depuração se não houver foto
                                Console.WriteLine("Nenhuma foto encontrada para o administrador.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Adicione uma mensagem de depuração em caso de erro
                Console.WriteLine("Erro ao carregar a foto do administrador. Detalhes: " + ex.Message);
            }
        }



        private Image ResizeImage(Image image, int width, int height)
        {
            int newWidth, newHeight;
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            // Calcular as novas dimensões mantendo a proporção
            if (originalWidth > originalHeight)
            {
                newWidth = width;
                newHeight = (int)((float)originalHeight / originalWidth * width);
            }
            else
            {
                newWidth = (int)((float)originalWidth / originalHeight * height);
                newHeight = height;
            }

            // Criar um novo bitmap com as novas dimensões
            Bitmap result = new Bitmap(newWidth, newHeight);

            using (Graphics g = Graphics.FromImage(result))
            {
                // Configurar o modo de interpolação para obter uma melhor qualidade
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                // Desenhar a imagem original no novo bitmap
                g.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return result;
        }



        private Image ByteArrayToImage(byte[] byteArrayIn)
        {
            // Converter bytes em uma imagem
            using (MemoryStream ms = new MemoryStream(byteArrayIn))
            {
                Image returnImage = Image.FromStream(ms);
                return returnImage;
            }
        }



        public float ObterUltimoValorBanca()
        {
            try
            {
                using (MySqlConnection conn = cn3.GetConnection())
                {
                    if (conn != null && conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    if (conn != null && conn.State == ConnectionState.Open)
                    {
                        string sql = "SELECT valor FROM banca ORDER BY idDepo DESC LIMIT 1";

                        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                        {
                            object result = cmd.ExecuteScalar();

                            if (result != null)
                            {
                                return Convert.ToSingle(result); // Converta para float
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao obter o último valor da banca: " + ex.Message);
            }

            // Se ocorrer algum erro, retorne 0 ou outro valor padrão conforme necessário
            return 0;
        }



        public void AtualizarBanca()
        {
            // Obtenha o último valor da banca
            float ultimoValor = ObterUltimoValorBanca();

            // Exiba o último valor no gunaLabel1
            gunaLabel1.Text = $"{ultimoValor:C2}Mzn";
        }


        private void PreencherDataGridView(string nomeUsuario)
        {
            try
            {
                using (MySqlConnection conn = cn3.GetConnection())
                {
                    cn3.Open();

                    if (conn.State != ConnectionState.Open)
                    {
                        MessageBox.Show("Não foi possível abrir a conexão com o banco de dados.");
                        return;
                    }

                    string sql = "SELECT user.idUser, user.Nome, user.Contacto, user.Endereço, " +
                                 "dividas.Divida, dividas.Parcela, dividas.Data, dividas.DataPg " +
                                 ", CASE " +
                                 "   WHEN dividas.Divida IS NOT NULL THEN 'Não pago' " +
                                 "   ELSE '-' " +
                                 "END AS StatusPagamento " +
                                 "FROM user " +
                                 "LEFT JOIN dividas ON user.idUser = dividas.idUser " +
                                 "LEFT JOIN pagamentos ON user.idUser = pagamentos.idUser " +
                                 $"WHERE user.Nome LIKE '%{nomeUsuario}%'";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Verifica se existem linhas antes de preencher o DataGridView
                            if (reader.HasRows)
                            {
                                // Configura o DataGridView para aceitar a adição de linhas programaticamente
                                dataGridView1.Rows.Clear();

                                // Itera sobre as linhas do resultado da consulta
                                while (reader.Read())
                                {
                                    // Adiciona uma nova linha ao DataGridView e preenche os dados
                                    //string statusPagamento = reader["StatusPagamento"].ToString();
                                    //string dataPagamento = (statusPagamento == "Não pago") ? "Pendente" : reader["data"].ToString();

                                    dataGridView1.Rows.Add(
                                        reader["idUser"],
                                        reader["Nome"],
                                        reader["Contacto"],
                                        reader["Endereço"],
                                        reader["Divida"],
                                        reader["Parcela"],
                                        reader["Data"],
                                        reader["DataPg"]
                                    //dataPagamento
                                    );
                                }
                            }
                            else
                            {
                                MessageBox.Show("Não foram encontrados dados para exibir.");
                            }
                            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao preencher o DataGridView. Detalhes: " + ex.Message);
            }
        }



        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void PreencherChart()
        {
            try
            {
                using (MySqlConnection conn = cn3.GetConnection())
                {
                    cn3.Open();

                    if (conn.State != ConnectionState.Open)
                    {
                        MessageBox.Show("Não foi possível abrir a conexão com o banco de dados.");
                        return;
                    }

                    string sql = "SELECT user.Nome, COALESCE(dividas.Divida, 0) AS Divida, COALESCE(pagamentos.Pay, 0) AS Pagamentos, COALESCE(banca.valor, 0) AS Banca " +
                                 "FROM user " +
                                 "LEFT JOIN dividas ON user.idUser = dividas.idUser " +
                                 "LEFT JOIN pagamentos ON user.idUser = pagamentos.idUser " +
                                 "LEFT JOIN banca ON banca.idDepo = (SELECT MAX(idDepo) FROM banca) " +
                                 "ORDER BY Divida ASC"; // Adiciona a ordenação por dívida (ASC para crescente)

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Limpe as séries existentes no gráfico
                            chart1.Series.Clear();

                            // Adicione a série ao gráfico para dívidas
                            Series seriesDivida = chart1.Series.Add("Divida");

                            // Defina o tipo de gráfico para barras agrupadas
                            chart1.Series[0].ChartType = SeriesChartType.SplineRange;

                            // Itera sobre as linhas do resultado da consulta
                            while (reader.Read())
                            {
                                // Adicione pontos de dados às séries do gráfico
                                seriesDivida.Points.AddXY(reader["Nome"], Convert.ToDouble(reader["Divida"]));

                                // Adiciona rótulo de dados
                                seriesDivida.Points.Last().Label = $"{reader["Divida"]}";
                            }
                        }
                    }

                    // Inverta a direção do eixo Y
                    chart1.ChartAreas[0].AxisY.IsReversed = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao preencher o gráfico. Detalhes: " + ex.Message);
            }
        }


        private void AtualizarProgressBar()
        {
            // Obtenha o valor atual da banca
            float valorBanca = ObterUltimoValorBanca();

            // Se o valor da banca for maior que 100, defina o valor da barra de progresso para 100
            // Caso contrário, defina o valor da barra de progresso para o valor da banca
            gunaCircleProgressBar1.Value = (int)Math.Min(valorBanca, 100);

            // Atualize o rótulo da barra de progresso com o valor formatado
            gunaCircleProgressBar1.Text = $"{valorBanca:C2}Mzn";
        }


        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void gunaAdvenceButton7_Click(object sender, EventArgs e)
        {
            Form1 novoFormulario = new Form1();
            novoFormulario.Show();
            this.Hide();
        }

        

       
         

        private void CheckPaymentDate(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection conn = cn3.GetConnection())
                {
                    if (conn != null && conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    if (conn != null && conn.State == ConnectionState.Open)
                    {
                        // Defina sua consulta SQL aqui
                        string sql = "SELECT user.Nome, dividas.DataPg " +
                                     "FROM user INNER JOIN dividas ON user.idUser = dividas.idUser " +
                                     "WHERE dividas.DataPg <= CURDATE()";

                        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                        {
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string nomeUsuario = reader["Nome"].ToString();
                                    string dataPagamento = reader["DataPg"].ToString();

                                    // Verifique se já se passaram 24 horas desde a última exibição do alerta
                                    if ((DateTime.Now - ultimaExibicaoAlerta).TotalHours >= 24)
                                    {
                                        // Verifique se a notificação já foi exibida recentemente
                                        if (!notificacaoExibida)
                                        {
                                            // Exiba a notificação
                                            MessageBox.Show($"Lembrete de pagamento para {nomeUsuario}. Data de pagamento: {dataPagamento}");

                                            // Atualize a variável para a hora atual
                                            ultimaExibicaoAlerta = DateTime.Now;
                                            notificacaoExibida = true;
                                        }
                                    }
                                    else
                                    {
                                        // Se já se passaram menos de 24 horas, redefina a variável de notificação
                                        notificacaoExibida = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao verificar data de pagamento: " + ex.Message);
            }
        }



        public void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int idUsuario = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["idUser"].Value);

                // Verifique a coluna editada e obtenha o novo valor
                if (dataGridView1.Columns[e.ColumnIndex].Name == "Nome" || dataGridView1.Columns[e.ColumnIndex].Name == "Endereço" || dataGridView1.Columns[e.ColumnIndex].Name == "Contacto")
                {
                    string novoValor = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

                    // Atualize o valor no banco de dados
                    AtualizarValorUsuario(idUsuario, dataGridView1.Columns[e.ColumnIndex].Name, novoValor);
                }
            }
        }

        private void AtualizarValorUsuario(int idUsuario, string nomeColuna, string novoValor)
        {
            try
            {
                using (MySqlConnection conn = cn3.GetConnection())
                {
                    // Restante do código para conexão

                    string sql = $"UPDATE user SET {nomeColuna} = @NovoValor WHERE idUser = @IdUsuario";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@NovoValor", novoValor);
                        cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show($"{nomeColuna} atualizado com sucesso!");
                        }
                        else
                        {
                            MessageBox.Show($"Erro ao atualizar {nomeColuna}. Verifique e tente novamente.");
                        }
                    }

                    // Restante do código para manipulação do banco de dados
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar {nomeColuna}: {ex.Message}");
            }
        }


        private void ConfirmarExclusao(int idUsuario)
        {
            DialogResult result = MessageBox.Show("Tem certeza que deseja excluir este usuário?", "Confirmação de Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                ExcluirUsuario(idUsuario);
            }
        }


        private void AtualizarDividaNoBanco(int dividaId, float novaDivida, int novaParcela, DateTime novaData, DateTime novaDataPg)
        {
            try
            {
                using (MySqlConnection conn = cn3.GetConnection())
                {
                    if (conn != null && conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    if (conn != null && conn.State == ConnectionState.Open)
                    {
                        string sql = "UPDATE dividas SET Divida = @NovaDivida, Parcela = @NovaParcela, " +
                                     "Data = @NovaData, DataPg = @NovaDataPg, Compromisso = @NovoCompromisso " +
                                     "WHERE ID = @DividaId";

                        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@NovaDivida", novaDivida);
                            cmd.Parameters.AddWithValue("@NovaParcela", novaParcela);
                            cmd.Parameters.AddWithValue("@NovaData", novaData.ToString("yyyy-MM-dd"));
                            cmd.Parameters.AddWithValue("@NovaDataPg", novaDataPg.ToString("yyyy-MM-dd"));
                         
                            cmd.Parameters.AddWithValue("@DividaId", dividaId);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar dívida no banco de dados: " + ex.Message);
            }
        } 
        private void gunaCircleButton2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label14_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void gunaAdvenceButton2_Click(object sender, EventArgs e)
        {
            Registo novoFormulario = new Registo();
            novoFormulario.Show();

            // Fecha o Form1
            this.Hide();
            
        }

        private void gunaAdvenceButton3_Click(object sender, EventArgs e)
        {
            Registo2 novoFormulario = new Registo2();
            novoFormulario.Show();

            // Fecha o Form1
            this.Hide();
            
        }

        private void gunaAdvenceButton4_Click(object sender, EventArgs e)
        {
            Payment novoFormulario = new Payment();
            novoFormulario.Show();

            // Fecha o Form1
            this.Hide();
            
        }

        private void label8_Click(object sender, EventArgs e)
        {

            depo novoFormulario = new depo(this);
            novoFormulario.Show();

            // Fecha o Form1
            this.Hide();
        }

        private void label7_Click(object sender, EventArgs e)
        {
            view novoFormulario = new view();
            novoFormulario.Show();

            // Fecha o Form1
            this.Hide();
        }

        private void Admin_Load(object sender, EventArgs e)
        {

           
        }

        private void label4_Click(object sender, EventArgs e)
        {
            profile cc = new profile();
            cc.Show();
            this.Hide();
        }

        private void gunaLabel1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void gunaCircleProgressBar1_Click(object sender, EventArgs e)
        {

        }

        private void gunaTextBox1_Click(object sender, EventArgs e)
        {

        }

        private void gunaTextBox1_TextChanged(object sender, EventArgs e)
        {
            string nomeUsuario = gunaTextBox1.Text.Trim();

            if (string.IsNullOrEmpty(nomeUsuario))
            {
                // Se a caixa de texto estiver vazia, chame o método para mostrar todos os nomes
                PreencherDataGridView(nomeUsuario);
            }
            else
            {
                // Se houver um texto na caixa de texto, faça a pesquisa com base no texto
                PreencherDataGridView(nomeUsuario);
            }
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void pesquisarbtn_Click(object sender, EventArgs e)
        {
            nomeUsuario = gunaTextBox1.Text.Trim();

            if (string.IsNullOrEmpty(nomeUsuario))
            {
                // Se a caixa de texto estiver vazia, chame o método para mostrar todos os nomes
                PreencherDataGridView(nomeUsuario);
            }
            else
            {
                // Se houver um texto na caixa de texto, faça a pesquisa com base no texto
                PreencherDataGridView(nomeUsuario);
            }
        }


        private void ExcluirUsuario(int idUsuario)
        {
            try
            {
                using (MySqlConnection conn = cn3.GetConnection())
                {
                    // Restante do código para conexão

                    string sql = "DELETE FROM user WHERE idUser = @IdUsuario";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Usuário excluído com sucesso!");
                            PreencherDataGridView(nomeUsuario);
                        }
                        else
                        {
                            MessageBox.Show("Erro ao excluir o usuário. Verifique e tente novamente.");
                        }
                    }

                    // Restante do código para manipulação do banco de dados
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao excluir o usuário: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Obtém o ID do usuário da linha selecionada
                int idUsuario = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["idUser"].Value);

                // Chama o método para excluir o usuário do banco de dados
                ExcluirUsuario(idUsuario);
            }
            else
            {
                MessageBox.Show("Selecione um usuário para excluir.");
            }
        }

        private void editbtn_Click(object sender, EventArgs e)
        {

        }

        private void gunaCirclePictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
