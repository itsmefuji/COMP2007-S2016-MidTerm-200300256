using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

//Other using statements
using COMP2007_S2016_MidTerm.Models;
using System.Web.ModelBinding;
using System.Linq.Dynamic;

namespace COMP2007_S2016_MidTerm
{
    public partial class TodoList : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["SortColumn"] = "TodoID"; // default sort column
                Session["SortDirection"] = "ASC"; // Ascending Order

                this.GetTodos();
            }
        }

        protected void GetTodos()
        {
            // connect to Entity Framework
            using (TodoConnection db = new TodoConnection())
            {
                string SortString = Session["SortColumn"].ToString() + " " + Session["SortDirection"].ToString();

                var Todo = (from allTodos in db.Todos
                            select allTodos);

                // bind the result to the GridView
                TodoGridView.DataSource = Todo.AsQueryable().OrderBy(SortString).ToList();
                TodoGridView.DataBind();
            }
        }

        protected void TodoGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int selectedRow = e.RowIndex; 

            // get the selected StudentID 
            int TodoID = Convert.ToInt32(TodoGridView.DataKeys[selectedRow].Values["TodoID"]); 

            // connect to EF and find the selected todo Item in the DB and remove it
            using (TodoConnection db = new TodoConnection())
            {
                // Create the object of the ToDo class so it can later be killed
                Todo deletedTodo = (from todoRecords in db.Todos
                                    where todoRecords.TodoID == TodoID
                                    select todoRecords).FirstOrDefault();

                // Physical Removing Action of the Item
                db.Todos.Remove(deletedTodo);

                // Save the Change
                db.SaveChanges();

                // Make the change Visible
                this.GetTodos();
            }
        }

        protected void TodoGridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            // Sorting the Column
            Session["SortColumn"] = e.SortExpression;

            // Make changes visible
            this.GetTodos();

            // Toggling of the Direction 
            Session["SortDirection"] = Session["SortDirection"].ToString() == "ASC" ? "DESC" : "ASC";
        }

        protected void TodoGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            // Set the new page number
            TodoGridView.PageIndex = e.NewPageIndex;

            // Make Changes visible
            this.GetTodos();
        }

        protected void TodoGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (IsPostBack)
            {
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    LinkButton linkbutton = new LinkButton();
                    for (int index = 0; index < TodoGridView.Columns.Count - 1; index++)
                    {
                        if (Session["SortDirection"].ToString() == "ASC")
                        {
                            linkbutton.Text = " <i class='fa fa-caret-up fa-lg'></i>";
                        }
                        else
                        {
                            linkbutton.Text = " <i class='fa fa-caret-down fa-lg'></i>";
                        }

                        e.Row.Cells[index].Controls.Add(linkbutton);
                    }
                }
            }
        }
    }
}