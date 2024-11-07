using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CREDPLUS
{
    public partial class ReciboPay : Form
    {
        DataTable dt = new DataTable();
        public ReciboPay(DataTable dt)
        {
            InitializeComponent();
            this.dt = dt ?? new DataTable();
        }

        private void ReciboPay_Load(object sender, EventArgs e)
        {
            if (dt != null)
            {
                dt.Clear();
            }

            this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet1", dt));
            this.reportViewer1.RefreshReport();
        }
    }
}
