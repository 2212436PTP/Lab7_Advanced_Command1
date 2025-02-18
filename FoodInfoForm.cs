﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab7_Advanced_Command
{
    public partial class FoodInfoForm : Form
    {
        public FoodInfoForm()
        {
            InitializeComponent();
        }

        private void FoodInfoForm_Load(object sender, EventArgs e)
        {
            this.InitValues();
        }
        private void InitValues()
        {
            string connectionstring = "Data Source=PC831;Initial Catalog=LAB7;Integrated Security=True;";
            SqlConnection conn = new SqlConnection(connectionstring);
            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = "Select ID , Name from Category";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            conn.Open();
            adapter.Fill(ds, "Category");
            cbbCategoryName.DataSource = ds.Tables["Category"];
           cbbCategoryName.DisplayMember = "Name";
            cbbCategoryName.ValueMember = "ID";
            conn.Close();
            conn.Dispose();

        }
        private void ResetText()
        {
            txtFoodID.ResetText();
            txtName.ResetText();    
            txtNotes.ResetText();
            nudPrice.ResetText();
            txtUnit.ResetText();
            nudPrice.ResetText();
        }

        private void btnAddFood_Click(object sender, EventArgs e)
        {
            try
            {
                string connectionstring = "Data Source=PC831;Initial Catalog=LAB7;Integrated Security=True;";
                SqlConnection conn = new SqlConnection(connectionstring);
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "Execute InsertFood @id OUTPUT, @name, @unit, @foodCategoryID,@price,@notes";

                cmd.Parameters.Add("@id", SqlDbType.Int);
                cmd.Parameters.Add("@name", SqlDbType.NVarChar, 1000);
                cmd.Parameters.Add("@unit", SqlDbType.NVarChar, 100);
                cmd.Parameters.Add("@foodCategoryID", SqlDbType.Int);
                cmd.Parameters.Add("@price", SqlDbType.Int);
                cmd.Parameters.Add("@notes", SqlDbType.NVarChar, 3000);

                cmd.Parameters["@id"].Direction = ParameterDirection.Output;

                cmd.Parameters["@name"].Value = txtName.Text;
                cmd.Parameters["@unit"].Value = txtUnit.Text;
                cmd.Parameters["@foodCategoryId"].Value = cbbCategoryName.SelectedValue;
                cmd.Parameters["@price"].Value = nudPrice.Value;
                cmd.Parameters["@notes"].Value = txtNotes.Text;

                conn.Open();
                int numRowAffected = cmd.ExecuteNonQuery();
                if (numRowAffected > 0)
                {
                    string foodID = cmd.Parameters["@id"].Value.ToString();
                    MessageBox.Show("Successfully adding newfood. Food ID = " + foodID, "Message");
                    this.ResetText();
                }
                else
                {
                    MessageBox.Show("Adding food failed");
                }
                conn.Close();
                conn.Dispose();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "SQL Erros");
            }

           catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erros");
            }
        }
        public void DisplayFoodInfo(DataRowView rowView)
        {
            try
            {
                txtFoodID.Text = rowView["ID"].ToString();
                txtName.Text = rowView["Name"].ToString();  
                txtUnit.Text = rowView["Unit"].ToString();
                txtNotes.Text = rowView["Notes"].ToString();
                nudPrice.Text = rowView["Price"].ToString();
                cbbCategoryName.SelectedIndex = -1;

                for( int index = 0; index < cbbCategoryName.Items.Count; index++ )
                {
                    DataRowView cat = cbbCategoryName.Items[index] as DataRowView;
                    if( cat["ID"].ToString() == rowView["FoodCategoryID"].ToString() ) 
                    {
                        cbbCategoryName.SelectedIndex = index;
                        break;  
                    }
                }

            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message, "Erros");
                this.Close();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                string connectionstring = "Data Source=PC831;Initial Catalog=LAB7;Integrated Security=True;";
                SqlConnection conn = new SqlConnection(connectionstring);
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "Execute UpdateFood @id, @name,@unit,@foodCategoryID,@price,@notes";

                cmd.Parameters.Add("@id", SqlDbType.Int);
                cmd.Parameters.Add("@name", SqlDbType.NVarChar, 1000);
                cmd.Parameters.Add("@unit", SqlDbType.NVarChar, 100);
                cmd.Parameters.Add("@foodCategoryID", SqlDbType.Int);
                cmd.Parameters.Add("@price", SqlDbType.Int);
                cmd.Parameters.Add("@notes", SqlDbType.NVarChar, 3000);

                cmd.Parameters["@id"].Value = int.Parse(txtFoodID.Text);
                cmd.Parameters["@name"].Value = txtName.Text;
                cmd.Parameters["@unit"].Value = txtUnit.Text;
                cmd.Parameters["@foodCategoryId"].Value = cbbCategoryName.SelectedValue;
                cmd.Parameters["@price"].Value = nudPrice.Value;
                cmd.Parameters["@notes"].Value = txtNotes.Text;

                conn.Open();

                int numRowAffected = cmd.ExecuteNonQuery();
                if (numRowAffected > 0)
                {
                    MessageBox.Show("Successfully updating food", "Message");
                    this.ResetText();

                }
                else
                {
                    MessageBox.Show("Updating food failed");

                }
                conn.Close();
                conn.Dispose();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "SQL Erros");
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erros");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

}
