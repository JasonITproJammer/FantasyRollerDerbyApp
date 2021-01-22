using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace FantasyRollerDerby.Models
{
    public class User
    {
        //Variables
        public int UserID { get; set; }
        public string AspNetUserID { get; set; }

        [StringLength(100, ErrorMessage = "The {0} cannot be more than 100 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [StringLength(100, ErrorMessage = "The {0} cannot be more than 100 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [StringLength(200, ErrorMessage = "The {0} cannot be more than 200 characters.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [StringLength(100, ErrorMessage = "The {0} cannot be more than 100 characters.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public int CreateByUserID { get; set; }
        public DateTime CreateDate { get; set; }
        public int RetireByUserID { get; set; }
        public DateTime RetireDate { get; set; }

        //Methods
        public Return AddUser(SqlConnection dbconAPP, User model)
        {
            Return cR = new Return();
            cR.ReturnFlag = false;

            //if the model is blank create a new model
            if (model == null)
            {
                model = new User();
            }

            //If the CreateByUserID is blank the stored procedure will use the system ID (5)
            if(model.CreateByUserID == default(int))
            {
                model.CreateByUserID = 5;
            }

            //If the CreateDate is blank the stored procedure will use GETDATE() [PST]
            if(model.CreateDate == default(DateTime))
            {
                model.CreateDate = DateTime.Now;
            }

            using (SqlCommand cmd = new SqlCommand("wboiUser", dbconAPP))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AspNetUserID", model.AspNetUserID);
                cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                cmd.Parameters.AddWithValue("@LastName", model.LastName);
                cmd.Parameters.AddWithValue("@Email", model.Email);
                //cmd.Parameters.AddWithValue("@CreateByUserID", model.CreateByUserID);
                //cmd.Parameters.AddWithValue("@CreateDate", model.CreateDate);
                cmd.ExecuteScalar();
            }

            cR.ReturnFlag = true;
            return cR;
        }

        public int GetUserID(SqlConnection dbconAPP, string aspNetUserID)
        {
            string strsql = "SELECT UserID FROM tblUser WHERE AspNetUserID = '" + aspNetUserID + "'";
            var cmd = new SqlCommand(strsql, dbconAPP);
            SqlDataReader myReader;
            myReader = cmd.ExecuteReader();
            int count = 0;
            int userID = 0;
            while (myReader.Read())
            {
                userID = Convert.ToInt32(myReader["UserID"].ToString());
                count++;
            }
            myReader.Close();
            if(count > 1)
            {
                throw new System.ArgumentException("Multiple users returned with matching AspNetUserIDs.");
            }
            return userID;
        }

        public string GetUserName(SqlConnection dbconAPP, int UserID)
        {
            string strsql = "SELECT FirstName + ' ' + LastName as UserName FROM tblUser WHERE UserID = " + UserID;
            var cmd = new SqlCommand(strsql, dbconAPP);
            SqlDataReader myReader;
            myReader = cmd.ExecuteReader();
            int count = 0;
            string userName = "";
            while (myReader.Read())
            {
                userName = myReader["UserName"].ToString();
                count++;
            }
            myReader.Close();
            if (count > 1)
            {
                throw new System.ArgumentException("Multiple users returned with matching UserIDs.");
            }
            return userName;
        }
    }
}