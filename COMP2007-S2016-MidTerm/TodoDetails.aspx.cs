using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

// Extra using statements
using COMP2007_S2016_MidTerm.Models;
using System.Web.ModelBinding;
using System.Linq.Dynamic; 

namespace COMP2007_S2016_MidTerm
{
    public partial class TodoDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Get the TodoID
                if ((!IsPostBack) && (Request.QueryString.Count > 0))
                {
                    this.GetTodo();
                }
            }
        }

        protected void GetTodo()
        {
            // Populate the fields with the correct information
            int TodoID = Convert.ToInt32(Request.QueryString["TodoID"]);

            // connect to the EF DB
            using (TodoConnection db = new TodoConnection())
            {
                // populate the todo object instance with cooresponding todoID
                Todo updatedTodo = (from todo in db.Todos
                                    where todo.TodoID == TodoID
                                    select todo).FirstOrDefault();

                // map the ToDo properties to the form controls
                if (updatedTodo != null)
                {
                    TodoNameTextBox.Text = updatedTodo.TodoName;
                    TodoNotesTextBox.Text = updatedTodo.TodoNotes;
                    CompletedCheckBox.Checked = updatedTodo.Completed.Value;
                }
            }
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            //Connect to EF
            using (TodoConnection db = new TodoConnection())
            {
                Todo newTodo = new Todo();
                
                int TodoID = 0;

                if (Request.QueryString.Count > 0)
                {
                    // get the id from the URL
                    TodoID = Convert.ToInt32(Request.QueryString["TodoID"]);

                    newTodo = (from todo
                               in db.Todos
                               where todo.TodoID == TodoID
                               select todo).FirstOrDefault();
                }
                // Add all new info to the New Todo
                newTodo.TodoName = TodoNameTextBox.Text;
                newTodo.TodoNotes = TodoNotesTextBox.Text;
                newTodo.Completed = CompletedCheckBox.Checked;

                //If the ID is still 0, ad is as new todo
                if (TodoID == 0) 
                {
                    db.Todos.Add(newTodo);
                }
                //Save All Changes 
                db.SaveChanges();
                //Redirect to the List Page
                Response.Redirect("~/TodoList.aspx");
            }
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            //Simple Redirect to the List Page
            Response.Redirect("~/TodoList.aspx");
        }
    }
}