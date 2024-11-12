using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab7_Advanced_Command
{
    public partial class Form1 : Form
    {
        private DataTable foodTable;
        public Form1()
        {
            InitializeComponent();
        }
        private void LoadCategory()
        {
            string connectionstring = "Data Source=PC833;Initial Catalog=\"Nha Hang\";Integrated Security=True;";
            SqlConnection conn = new SqlConnection(connectionstring);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ID, NAME From Category";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            conn.Open();
            adapter.Fill(dt);
            conn.Close();
            conn.Dispose();

            cbbCategory.DataSource = dt;
            cbbCategory.DisplayMember = "Name";
            cbbCategory.ValueMember = "ID";
        }
        private void Form1_Load(object sender, EventArgs e)
        {
           
            LoadCategory();
        }

        private void cbbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbCategory.SelectedIndex == -1) return;
            string connectionstring = "Data Source=PC833;Initial Catalog=\"Nha Hang\";Integrated Security=True;";
            SqlConnection conn = new SqlConnection(connectionstring);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * from Food Where FoodCategoryID = @categoryID" ;

            cmd.Parameters.Add("@categoryId", SqlDbType.Int);

            if( cbbCategory.SelectedValue is DataRowView)
            {
                DataRowView rowView = cbbCategory.SelectedValue as DataRowView;
                cmd.Parameters["@categoryId"].Value = rowView["ID"];
            }    
            else
            {
                cmd.Parameters["@categoryID"].Value = cbbCategory.SelectedValue;
            }    
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
             foodTable = new DataTable();
            conn.Open();
            adapter.Fill(foodTable);
            conn.Close();
            conn.Dispose();
            
            dgvFoodList .DataSource = foodTable;
            lblCatName.Text = cbbCategory.Text;
            lblQuantity.Text = foodTable.Rows.Count.ToString();
        }

        private void tsmCalculateQuantity_Click(object sender, EventArgs e)
        {
            string connectionstring = "Data Source=PC833;Initial Catalog=\"Nha Hang\";Integrated Security=True;";
            SqlConnection conn = new SqlConnection(connectionstring);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT @numSaleFood = sum(Quantity) from BillDetails where FoodID= @foodId ";
            
            if ( dgvFoodList.SelectedRows.Count > 0 )
            {
                DataGridViewRow selectedRow = dgvFoodList.SelectedRows[0];
                DataRowView rowView = selectedRow.DataBoundItem as DataRowView;

                cmd.Parameters.Add("@foodId", SqlDbType.Int);
                cmd.Parameters["@foodId"].Value = rowView["ID"];

                cmd.Parameters.Add("@numSaleFood",SqlDbType.Int);
                cmd.Parameters["@numSalefood"].Direction = ParameterDirection.Output;
                conn.Open();
                cmd.ExecuteNonQuery();
                string Result = cmd.Parameters["@numSalefood"].Value.ToString();
                if (Result == "") Result = "0";
                MessageBox.Show("Tổng số lượng món " + rowView["Name"] + " đã bán là " + Result + " " + rowView["Unit"]);
                conn.Close();
            }
            cmd.Dispose();
            conn.Dispose(); 
        }

        private void tsmAddFood_Click(object sender, EventArgs e)
        {
            FoodInfoForm foodForm = new FoodInfoForm();
            foodForm.FormClosed += new FormClosedEventHandler(foodForm_FormClosed);
            foodForm.Show(this);
        }
        private void foodForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            int index = cbbCategory.SelectedIndex;
            cbbCategory.SelectedIndex = -1;
            cbbCategory.SelectedIndex = index;
        }
        private void tsmUpdateFood_Click(object sender, EventArgs e)
        {
            if ( dgvFoodList.SelectedRows.Count > 0 )
            {
                DataGridViewRow selectedRow = dgvFoodList.Rows[0];
                DataRowView rowView = selectedRow.DataBoundItem as DataRowView;
                FoodInfoForm foodForm = new FoodInfoForm();
                foodForm.FormClosed += new FormClosedEventHandler(foodForm_FormClosed);

                foodForm.Show(this);
                foodForm.DisplayFoodInfo(rowView);
            }
        }

        private void txtSearchByName_TextChanged(object sender, EventArgs e)
        {
            if (foodTable == null) return;
            string filterE = "Name like '%" + txtSearchByName.Text + "%'";
            string sortE = "Price DESC";
            DataViewRowState rowStateFilter = DataViewRowState.OriginalRows;

            DataView foodView = new DataView(foodTable, filterE, sortE, rowStateFilter);
            dgvFoodList.DataSource = foodView;
        }
    }
}
