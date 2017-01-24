﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IMS_System.Class.GUI;
using IMS_System.Class.Forms;
using IMS_System.Class.Database;
using System.Data.SqlClient;
using IMS_System.Class;
using IMS_System.Class.Codes;

namespace IMS_System.Forms
{
    public partial class frmStudents : Form
    {
        frmMainPage MainScreen;
       // private String Query = "";
        private List<int> batch_id;
        private string DgvQuery;
        private int MaximumDisplayValue = 30;

        public frmStudents(frmMainPage MainPage)
        {
            InitializeComponent();
            MainScreen = MainPage;
            batch_id = new List<int>();

            Add_Batches_to_Combobox("select BatchId, BatchName from tblBatch where BatchSatus='True'");
            // DgvQuery= "select StudentId,StudentFirstName,StudentLastName,StudentAddress,CONVERT(date, StudentJoinedDate),StudentStatus from tblStudentDetails order by StudentStatus desc";
            setDgvQuery();
            Add_Details_to_Datagridview(DgvQuery);
        }

        private void frmBatches_Load(object sender, EventArgs e)
        {

        }

        private void label6_Paint(object sender, PaintEventArgs e)
        {
            clsLines.Draw_Horizontal_lines(e, Color.Black, label6);
        }


        private void Add_Batches_to_Combobox(String Query)
        {
            try
            {
                batch_id.Clear();
                comboBox1.Items.Clear();
                batch_id.Add(0);
                comboBox1.Items.Add("All Batches");
                clsDatabase_Connection.Get_Table(Query);
                if (clsDatabase_Connection.objDataSet.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < clsDatabase_Connection.objDataSet.Tables[0].Rows.Count; i++)
                    {
                        batch_id.Add((int)clsDatabase_Connection.objDataSet.Tables[0].Rows[i][0]);
                        comboBox1.Items.Add("Batch " +clsDatabase_Connection.objDataSet.Tables[0].Rows[i][1].ToString());
                    }
                    comboBox1.SelectedIndex = 0;
                }
                else
                { MessageBox.Show("please add batches"); }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); }
        }
        public void Add_Details_to_Datagridview(String Query)
        {
            try
            {
                dgvCart.Rows.Clear();
                clsDatabase_Connection.Get_Table(Query);
                if (clsDatabase_Connection.objDataSet.Tables[0].Rows.Count > 0)
                {
                    dgvCart.Visible = true;
                    for (int i = 0; i < clsDatabase_Connection.objDataSet.Tables[0].Rows.Count; i++)
                    {
                        dgvCart.Rows.Add(clsDatabase_Connection.objDataSet.Tables[0].Rows[i][0].ToString(),
                            clsDatabase_Connection.objDataSet.Tables[0].Rows[i][1].ToString(),
                            clsDatabase_Connection.objDataSet.Tables[0].Rows[i][2].ToString(),
                            clsDatabase_Connection.objDataSet.Tables[0].Rows[i][3].ToString(),
                            clsDatabase_Connection.objDataSet.Tables[0].Rows[i][4].ToString(),
                            clsDatabase_Connection.objDataSet.Tables[0].Rows[i][5].ToString(),
                            clsDatabase_Connection.objDataSet.Tables[0].Rows[i][6].ToString()
                            );
                        if (clsDatabase_Connection.objDataSet.Tables[0].Rows[i][5].ToString().Equals("True"))
                        { dgvCart.Rows[i].DefaultCellStyle.ForeColor = Color.Black; }
                        else { dgvCart.Rows[i].DefaultCellStyle.ForeColor = Color.Red; }

                        dgvCart.ClearSelection();
                      
                    }
                }
                else
                {
                    //dgvCart.Visible = false;
                    // label2.Text = "There are no Details";
                    //  label2.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); }
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_DockChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                setDgvQuery();
                Add_Details_to_Datagridview(DgvQuery);
                //if (comboBox1.SelectedIndex==0)
                //{
                //    Add_Details_to_Datagridview(DgvQuery);
                //}
                //else
                //{
                //    DgvQuery= DgvQuery+ " and A.BatchId=" + batch_id[comboBox1.SelectedIndex];
                //    Add_Details_to_Datagridview(DgvQuery); 
                //}
              }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); }
        }

        
        private void setDgvQuery()
        {
            if (comboBox1.SelectedIndex > 0)
            {
                DgvQuery = "select top " + MaximumDisplayValue + " (A.StudentId),max(A.StudentFirstName),max(A.StudentLastName),max(A.StudentAddress),'Batch ' + max(C.BatchName),CAST(MAX(CAST(A.StudentStatus as INT)) AS BIT),max(B.StudentBatchId) from tblStudentDetails A inner join tblStudentBatchDetails B on A.StudentId=B.studentid inner join tblBatch C on C.BatchId=B.batchid group by A.StudentId,A.StudentStatus  ";
                dgvCart.Columns[4].HeaderText = "Joined";
            }
            else
            {
                DgvQuery = "select top " + MaximumDisplayValue + "(A.StudentId),max(A.StudentFirstName),max(A.StudentLastName),max(A.StudentAddress),'Batch ' + max(C.BatchName),CAST(MAX(CAST(A.StudentStatus as INT)) AS BIT),max(B.StudentBatchId) from tblStudentDetails A inner join tblStudentBatchDetails B on A.StudentId=B.studentid inner join tblBatch C on C.BatchId=B.batchid group by A.StudentId,A.StudentStatus  ";
                dgvCart.Columns[4].HeaderText = "Batch";
            }
            if (!txtValue.Text.Trim().Equals(""))
            { DgvQuery = DgvQuery + " having max(A.StudentFirstName) like '" + txtValue.Text + "%'"; }
            else
            { DgvQuery = DgvQuery + " having max(A.StudentFirstName) like '%'"; }

            if (checkBox2.Checked == true)
            { DgvQuery = DgvQuery + " and CAST(MAX(CAST(A.StudentStatus as INT)) AS BIT)='True' "; }

            if (comboBox1.SelectedIndex > 0)
            { DgvQuery = DgvQuery + " and MAX(B.BatchId)=" + batch_id[comboBox1.SelectedIndex]; }
            DgvQuery = DgvQuery + " order by A.StudentStatus desc,A.StudentId desc";
        }
        private void button2_Click(object sender, EventArgs e)
        {
      
        }

        private void button4_Click(object sender, EventArgs e)
        {
             
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (clsClose_Other_Forms.IMS_IsFormOpen("frmStudentsUpdates") == false)
            {
                frmStudentsUpdates studentUpdate = new frmStudentsUpdates(MainScreen, -1, false, "",true);

                studentUpdate.ShowDialog();
                studentUpdate.BringToFront();
                setDgvQuery();
                Add_Details_to_Datagridview(DgvQuery);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (dgvCart.SelectedRows.Count >= 1  )
            {
                viewSelectedStudent(int.Parse(dgvCart.Rows[dgvCart.CurrentRow.Index].Cells[0].Value.ToString()));
            }
            else
            {
                new frmMessageBox("error", "Empty", "Please Select Student Details!", false, MainScreen).ShowDialog();
            }
           
        }

        private String getWeeDay_Number(string day)
        {
            if(day.Equals("Monday")) { return "0"; }
            else if (day.Equals("Tuesday")) { return "1"; }
            else if (day.Equals("Wednesday")) { return "2"; }
            else if (day.Equals("Thursday")) { return "3"; }
            else if (day.Equals("Friday")) { return "4"; }
            else if (day.Equals("Saturday")) { return "5"; }
            else { return "6"; }

        }

        private void dgvCart_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (btnEdit.Enabled == true)
            {
                viewSelectedStudent(int.Parse(dgvCart.Rows[dgvCart.CurrentRow.Index].Cells[0].Value.ToString()));
            }
        }

        private void viewSelectedStudent(int id)
        {
            if (clsClose_Other_Forms.IMS_IsFormOpen("frmStudentView") == false)
            {
                frmStudentView ViewStudent = new frmStudentView(MainScreen,id);
                ViewStudent.ShowDialog();
                ViewStudent.BringToFront();
                setDgvQuery();
                Add_Details_to_Datagridview(DgvQuery);
            }
        }
        private void dgvCart_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                button1.Enabled = true;
               // selectedIndex = dgvCart.Rows[dgvCart.CurrentRow.Index].Cells[0].Value.ToString();
                if (dgvCart.Rows[dgvCart.CurrentRow.Index].Cells[5].Value.Equals("True"))
                {
                    button1.Text = "Deactivate";
                    btnEdit.Enabled = true;
                }
                else
                {
                    button1.Text = "Activate";
                    btnEdit.Enabled = false;
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvCart.SelectedRows.Count >= 1)
                {
                   // selectedIndex = dgvCart.Rows[dgvCart.CurrentRow.Index].Cells[0].Value.ToString();
                    if (button1.Text.Equals("Activate"))
                    { UpdateActivation("True", dgvCart.Rows[dgvCart.CurrentRow.Index].Cells[0].Value.ToString()); }
                    else
                    { UpdateActivation("False", dgvCart.Rows[dgvCart.CurrentRow.Index].Cells[0].Value.ToString()); }
                }
                else
                {
                    new frmMessageBox("error", "Empty", "Please Select Student Details!", false, MainScreen).ShowDialog();
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); }
        }

        private void UpdateActivation(String Value,string selectedIndex)
        {
            try
            {
                ClsStudents.UpdateActivation(Value, selectedIndex,MainScreen);
                setDgvQuery();
                Add_Details_to_Datagridview(DgvQuery);
                button1.Enabled = false;
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); }
        }

        private void checkBox1_CheckStateChanged(object sender, EventArgs e)
        {
            setDgvQuery();
            Add_Details_to_Datagridview(DgvQuery);
        }

        private void checkBox2_CheckStateChanged(object sender, EventArgs e)
        {
            setDgvQuery();
            Add_Details_to_Datagridview(DgvQuery);
        }

        private void txtValue_TextChanged(object sender, EventArgs e)
        {
            setDgvQuery();
            Add_Details_to_Datagridview(DgvQuery);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void dgvCart_Scroll(object sender, ScrollEventArgs e)
        {
            //if((dgvCart.Rows.Count- dgvCart.FirstDisplayedScrollingRowIndex)==8)
            //{
            //    topvalue += 5;
            //    setDgvQuery();
            //    Add_Details_to_Datagridview(DgvQuery);
            //}
          //  MessageBox.Show(dgvCart.FirstDisplayedScrollingRowIndex.ToString());
        }

        private void txtValue_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    clsDatabase_Connection.Get_Table("select top 1 StudentId from tblStudentBarcode where BarcodeValue='" + txtValue.Text.Trim() + "' and BarcodeStatus='True'");
                    if (clsDatabase_Connection.objDataSet.Tables[0].Rows.Count > 0)
                    {
                        viewSelectedStudent(int.Parse(clsDatabase_Connection.objDataSet.Tables[0].Rows[0][0].ToString()));
                    }
                    txtValue.SelectAll();
                    txtValue.Focus();
                } 
            }
            catch { }
        }
    }
}
