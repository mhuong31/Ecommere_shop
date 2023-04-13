using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services.Protocols;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace EcommerceShop
{
    public partial class Admin : System.Web.UI.Page
    {
        SqlConnection conn = new SqlConnection("Data Source=LAPTOP-0FV8221L\\SQLEXPRESS; Integrated Security=true;Initial Catalog=db_ECommerceShop; uid=sa; pwd=1; ");


        protected void Page_Load(object sender, EventArgs e)
        {
           

            if (!IsPostBack)
            {
                BindGridView(); // Hiển thị trang đầu tiên khi trang được tải lần đầu tiên
            }
        }
        [HttpMethod]

        protected void gvProducts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = GridView1.Rows[index];
                int productId = Convert.ToInt32(row.Cells[0].Text);
                Response.Redirect("Admin.aspx?id=" + productId);
            }
        }
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string productId = btn.CommandArgument;
            Console.WriteLine(productId);
            Response.Redirect("admin.aspx?id=" + productId);
        }
        protected void LinkDelete_Click(object sender, EventArgs e)
        {
            int rowIndex = ((GridViewRow)(sender as Control).NamingContainer).RowIndex;
            int productid = Convert.ToInt32(GridView1.Rows[rowIndex].Cells[0].Text);


            Database db = DatabaseFactory.CreateDatabase("strConnet");

            string procName = "spDelete_Product";
            int count = db.ExecuteNonQuery(procName, productid);


            if (count > 0)
            {
                BindGridView();
            }


        }
        protected void LinkUpdate_Click(object sender, EventArgs e)
        {
            int rowIndex = ((GridViewRow)(sender as Control).NamingContainer).RowIndex;
            int productid = Convert.ToInt32(GridView1.Rows[rowIndex].Cells[0].Text);


            Response.Redirect("Create_Update_Products?productID=" + productid);


        }

        //protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        //{
        //    if (e.CommandName == "Edit")
        //    {
        //        // Xử lý lệnh "Cập nhật"
        //        int index = Convert.ToInt32(e.CommandArgument);
        //        GridViewRow row = GridView1.Rows[index];
        //        string productId = row.Cells[0].Text; // Lấy giá trị ProductID từ cột đầu tiên của hàng
        //                                              // Thực hiện các thao tác cập nhật dữ liệu cho sản phẩm có ProductID tương ứng
        //    }
        //    else if (e.CommandName == "Delete")
        //    {
        //        // Xử lý lệnh "Xóa"
        //        int index = Convert.ToInt32(e.CommandArgument);
        //        GridViewRow row = GridView1.Rows[index];
        //        string productId = row.Cells[0].Text; // Lấy giá trị ProductID từ cột đầu tiên của hàng
        //                                              // Thực hiện các thao tác xóa sản phẩm có ProductID tương ứng
        //    }
        //}

        protected void BindGridView()
        {
            // Điều kiện phân trang
            //int pageSize = 10;
            //int startIndex = (pageNumber - 1) * pageSize;

            // Kết nối đến cơ sở dữ liệu và truy vấn dữ liệu
            //string connectionString = ConfigurationManager.ConnectionStrings["Data Source=LAPTOP-0FV8221L\\SQLEXPRESS; Integrated Security=true;Initial Catalog=db_ECommerceShop; uid=sa; pwd=1; "].ConnectionString;
            conn.Open();
            SqlCommand command = new SqlCommand("SELECT ProductID, Name, Description, Price,imgUrl FROM Products ", conn);
            //command.Parameters.AddWithValue("@startIndex", startIndex);
            //command.Parameters.AddWithValue("@pageSize", pageSize);
            SqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("ProductID");
            dataTable.Columns.Add("Name");
            dataTable.Columns.Add("imgUrl");
            dataTable.Columns.Add("Description");
            dataTable.Columns.Add("Price", typeof(decimal));
            while (reader.Read())
            {
                DataRow row = dataTable.NewRow();
                row["ProductID"] = reader["ProductID"].ToString();
                row["Name"] = reader["Name"].ToString();
                row["imgUrl"] = "img/"+reader["imgUrl"].ToString();
                row["Description"] = reader["Description"].ToString();
                row["Price"] = Convert.ToDecimal(reader["Price"]);
                dataTable.Rows.Add(row);
            }
            // Gán dữ liệu vào Gridview
            //GridView1.DataSource = reader;
            //GridView1.DataBind();

            reader.Close();
            GridView1.DataSource = dataTable;
            GridView1.DataBind();

        }
        //protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        //{
        //    GridView1.PageIndex = e.NewPageIndex;
        //    BindGridView(e.NewPageIndex + 1); // Hiển thị trang mới khi chuyển sang trang khác
        //}

        private int GetTotalProducts()
        {
            String sql = "Select count(*) from products";
            int count = 0;
            SqlCommand command = new SqlCommand(sql, conn);
            conn.Open();
            count = (int)command.ExecuteScalar();
            conn.Close();
            return count;

        }
        private void BindPaginationButtons()
        {
            // Số sản phẩm trên mỗi trang
            int pageSize = 10;

            // Tổng số sản phẩm
            int totalProducts = GetTotalProducts();

            // Tổng số trang
            int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            // Tạo danh sách trang
            List<PaginationButton> buttons = new List<PaginationButton>();
            for (int i = 1; i <= totalPages; i++)
            {
                buttons.Add(new PaginationButton() { PageIndex = i });
            }

            // Đưa danh sách trang vào Repeater
            repeaterPagination.DataSource = buttons;
            repeaterPagination.DataBind();
        }
        public class PaginationButton
        {
            public int PageIndex { get; set; }
        }
        protected void repeaterPagination_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Page")
            {
                int pageIndex = int.Parse(e.CommandArgument.ToString());
                BindGridView();
            }
        }



    }
}
