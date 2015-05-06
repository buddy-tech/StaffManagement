using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StaffManagement
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            showInfo();
        }
        //记录DataGridView控件中指定单元格信息的字段
        public static string str = "";
        //数据库连接字符串
        public static string strConn = @"Data Source=(localdb)\ProjectsV12;Initial Catalog=db_Staff;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"; 
        /// <summary>
        /// 在DataGridView控件上显示记录
        /// </summary>
        private void showInfo()
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                DataTable dt = new DataTable();
                SqlDataAdapter sda = new SqlDataAdapter("SELECT No AS 员工编号,Name AS 员工姓名,Salary as 基本工资,Evaluation AS 工作评价 FROM tb_Staff ORDER BY No", con);
                sda.Fill(dt);
                this.dgvInfo.DataSource = dt.DefaultView;
            }
        }
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("您确定要删除本条信息吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (str != "")
                {
                    using (SqlConnection con = new SqlConnection(strConn))
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM tb_Staff WHERE No='" + str + "'", con);
                        cmd.Connection = con;
                        cmd.ExecuteNonQuery();
                        con.Close();
                        showInfo();
                        MessageBox.Show("删除成功！");
                    }
                }
            }
        }
        /// <summary>
        /// 添加纪录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (this.txtNo.Text == "" | this.txtName.Text == "" | this.txtSalary.Text == "" | this.txtEvaluation.Text == "")
            {
                MessageBox.Show("信息不完整！");
                return;
            }
            if (IsNumeric(this.txtSalary.Text.Trim()) == false)
            {
                MessageBox.Show("薪水信息不完整！");
                return;
            }
            //判断是否有相同记录
            if (IsSameRecord() == true)
                return;
            using (SqlConnection con = new SqlConnection(strConn))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();
                try
                {
                    StringBuilder strSQL = new StringBuilder();
                    strSQL.Append("INSERT INTO tb_Staff(No,Name,Salary,Evaluation)");
                    strSQL.Append(" VALUES('" + this.txtNo.Text.Trim().ToString() + "','" + this.txtName.Text.Trim().ToString() + "','" + Convert.ToSingle(this.txtSalary.Text.Trim().ToString()) + "','" + this.txtEvaluation.Text.Trim().ToString() + "')");
                    using (SqlCommand cmd = new SqlCommand(strSQL.ToString(), con))
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("信息增加成功！");
                    }
                    strSQL.Remove(0, strSQL.Length);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("错误：" + ex.Message, "错误提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
                showInfo();
            }
        }
        public bool IsNumeric(string strCode)
        {
            if (strCode == null || strCode.Length == 0)
                return false;
            ASCIIEncoding ascii = new ASCIIEncoding();
            byte[] byteStr = ascii.GetBytes(strCode);
            foreach (byte code in byteStr)
            {
                if (code < 48 || code > 57)
                    return false;
            }
            return true;
        }
        private bool IsSameRecord()
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();
                string Str_condition = "";
                string Str_cmdtxt = "";
                Str_condition = this.txtNo.Text.Trim();
                Str_cmdtxt = "SELECT * FROM tb_Staff";
                Str_cmdtxt += " WHERE No='" + Str_condition + "'";

                using (SqlCommand cmd = new SqlCommand(Str_cmdtxt, con))
                {
                    SqlDataAdapter sda = new SqlDataAdapter();
                    sda.SelectCommand = cmd;
                    DataSet ds = new DataSet();
                    sda.Fill(ds, "Info");
                    if (ds.Tables["Info"].Rows.Count < 0)
                    {
                        MessageBox.Show("已存在相同的员工信息！");
                        return true;
                    }
                    else
                    {

                    }
                        return false;
                        con.Close();
                        con.Dispose();
                }
                
            }
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                if (this.txtNo.Text.ToString() != "")
                {
                    string Str_condition = "";
                    string Str_cmdtxt = "";
                    Str_condition = this.dgvInfo[0, this.dgvInfo.CurrentCell.RowIndex].Value.ToString();
                    Str_cmdtxt = "UPDATE tb_Staff SET Name='" + this.txtName.Text.Trim() + "',Salary=" + Convert.ToSingle(this.txtSalary.Text.Trim()) + ",Evaluation='" + this.txtEvaluation.Text.Trim() + "' WHERE No='" + Str_condition + "'";
                    try
                    {
                        if (con.State == ConnectionState.Closed)
                        {
                            con.Open();
                            using (SqlCommand cmd = new SqlCommand(Str_cmdtxt, con))
                            {
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("数据修改成功！");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("错误：" + ex.Message, "错误提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                            con.Dispose();
                        }
                    }
                    showInfo();
                }
                else
                {
                    MessageBox.Show("请选择员工编号！", "提示对话框", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void FillControls()
        {
            try
            {
                this.txtNo.Text = this.dgvInfo[0, this.dgvInfo.CurrentCell.RowIndex].Value.ToString();
                this.txtName.Text = this.dgvInfo[1, this.dgvInfo.CurrentCell.RowIndex].Value.ToString();
                this.txtSalary.Text = this.dgvInfo[2, this.dgvInfo.CurrentCell.RowIndex].Value.ToString();
                this.txtEvaluation.Text = this.dgvInfo[3, this.dgvInfo.CurrentCell.RowIndex].Value.ToString();
            }
            catch { }
        }

        private void dgvInfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            str = this.dgvInfo.SelectedCells[0].Value.ToString();
            FillControls();
        }
    }
}
