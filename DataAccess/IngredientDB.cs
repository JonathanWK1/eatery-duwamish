using Common.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemFramework;

namespace DataAccess
{
    public class IngredientDB
    {
        public List<IngredientData> GetIngredientByRecipeID(int RecipeID)
        {
            try
            {
                string SpName = "dbo.Ingredient_GetByRecipeID";
                List<IngredientData> ListIngredient = new List<IngredientData>();
                using(SqlConnection SqlConn = new SqlConnection())
                {
                    SqlConn.ConnectionString = SystemConfigurations.EateryConnectionString;
                    SqlConn.Open();
                    SqlCommand SqlCmd = new SqlCommand(SpName, SqlConn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    SqlCmd.Parameters.Add(new SqlParameter("@RecipeId", RecipeID));
                    using (SqlDataReader Reader = SqlCmd.ExecuteReader())
                    {
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                IngredientData ingredient= new IngredientData();
                                ingredient.IngredientID = Convert.ToInt32(Reader["IngredientID"]);
                                ingredient.RecipeID = Convert.ToInt32(Reader["RecipeID"]);
                                ingredient.IngredientName = Convert.ToString(Reader["IngredientName"]);
                                ingredient.Qty = Convert.ToInt32(Reader["Qty"]);
                                ingredient.Unit = Convert.ToString(Reader["Unit"]); 
                                ListIngredient.Add(ingredient);
                            }
                        }
                    }
                    SqlConn.Close();
                }
                return ListIngredient;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IngredientData GetIngredientByID(int IngredientID)
        {
            try
            {
                string SpName = "dbo.Ingredient_GetByID";
                IngredientData ingredient = null;
                using(SqlConnection SqlConn = new SqlConnection())
                {
                    SqlConn.ConnectionString = SystemConfigurations.EateryConnectionString;
                    SqlConn.Open();
                    SqlCommand SqlCmd = new SqlCommand(SpName, SqlConn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    SqlCmd.Parameters.Add(new SqlParameter("@IngredientId", IngredientID));
                    using(SqlDataReader Reader = SqlCmd.ExecuteReader())
                    {
                        if (Reader.HasRows)
                        {
                            Reader.Read();
                            ingredient = new IngredientData();
                            ingredient.IngredientID = Convert.ToInt32(Reader["IngredientID"]);
                            ingredient.RecipeID = Convert.ToInt32(Reader["RecipeID"]);
                            ingredient.IngredientName = Convert.ToString(Reader["IngredientName"]);
                            ingredient.Qty = Convert.ToInt32(Reader["Qty"]);
                            ingredient.Unit = Convert.ToString(Reader["Unit"]);
                        }
                    }
                    SqlConn.Close();
                }
                return ingredient;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int InsertUpdateIngredient(IngredientData Ingredient, SqlTransaction SqlTran)
        {
            try
            {
                string SpName = "dbo.Ingredient_InsertUpdate";
                SqlCommand SqlCmd = new SqlCommand(SpName, SqlTran.Connection, SqlTran);
                SqlCmd.CommandType = CommandType.StoredProcedure;

                SqlParameter IngredientId = new SqlParameter("@IngredientID", Ingredient.IngredientID);
                IngredientId.Direction = ParameterDirection.InputOutput;
                SqlCmd.Parameters.Add(IngredientId);
                SqlCmd.Parameters.Add(new SqlParameter("@RecipeID", Ingredient.RecipeID));
                SqlCmd.Parameters.Add(new SqlParameter("@IngredientName", Ingredient.IngredientName));
                SqlCmd.Parameters.Add(new SqlParameter("@Qty", Ingredient.Qty));
                SqlCmd.Parameters.Add(new SqlParameter("@Unit", Ingredient.Unit));



                return SqlCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int DeleteIngredients(string IngredientIDs,SqlTransaction SqlTran)
        {
            try
            {
                string SpName = "dbo.Ingredient_Delete";
                SqlCommand SqlCmd = new SqlCommand(SpName, SqlTran.Connection, SqlTran);
                SqlCmd.CommandType = CommandType.StoredProcedure;
                SqlCmd.Parameters.Add(new SqlParameter("@IngredientIDs", IngredientIDs));
                return SqlCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
