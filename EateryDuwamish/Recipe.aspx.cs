using BusinessFacade;
using Common.Data;
using Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EateryDuwamish
{
    public partial class Recipe : System.Web.UI.Page
    {
        int DishID;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                ShowNotificationIfExists();

                if (!string.IsNullOrEmpty(Request.QueryString["DishID"]))
                {
                    DishID = Convert.ToInt32(Request.QueryString["DishID"]);
                    LoadRecipeTable(DishID);
                }
                else
                    throw new Exception("Please provide DishID");
            }
            catch (Exception ex)
            {
                notifRecipe.Show($"ERROR LOAD TABLE: {ex.Message}", NotificationType.Danger);
            }

        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                RecipeData recipe = GetFormData();
                int rowAffected = new RecipeSystem().InsertUpdateRecipe(recipe);
                if (rowAffected <= 0)
                    throw new Exception("No Data Recorded");
                Session["save-success"] = 1;
                Response.Redirect("Recipe.aspx?DishID="+ DishID);
            }
            catch (Exception ex)
            {
                notifRecipe.Show($"ERROR SAVE DATA: {ex.Message}", NotificationType.Danger);
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            ResetForm();
            litFormType.Text = "TAMBAH";
            pnlFormRecipe.Visible = true;
            txtRecipeName.Focus();
        }
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string strDeletedIDs = hdfDeletedRecipes.Value;
                if (string.IsNullOrEmpty(strDeletedIDs))
                    throw new Exception("No Data Selected");
                IEnumerable<int> deletedIDs = strDeletedIDs.Split(',').Select(Int32.Parse);
                int rowAffected = new RecipeSystem().DeleteRecipes(deletedIDs);
                if (rowAffected <= 0)
                    throw new Exception("No Data Deleted");
                Session["delete-success"] = 1;
                Response.Redirect("Recipe.aspx?dishid="+DishID);
            }
            catch (Exception ex)
            {
                notifRecipe.Show($"ERROR DELETE DATA: {ex.Message}", NotificationType.Danger);
            }
        }
        private void ResetForm()
        {
            hdfRecipeId.Value = String.Empty;
            txtRecipeName.Text = String.Empty;
        }

        private void FillForm(RecipeData recipe)
        {
            hdfRecipeId.Value = recipe.RecipeID.ToString();
            txtRecipeName.Text = recipe.RecipeName;
        }
        private RecipeData GetFormData()
        {
            RecipeData recipe = new RecipeData();
            recipe.RecipeID = String.IsNullOrEmpty(hdfRecipeId.Value) ? 0 : Convert.ToInt32(hdfRecipeId.Value);
            recipe.RecipeName = txtRecipeName.Text;
            recipe.DishID = DishID;
            if (recipe.RecipeID == 0)
                recipe.RecipeDesc = "Please describe ur recipe!";
            else
                recipe.RecipeDesc = new RecipeSystem().GetRecipeByID(recipe.RecipeID).RecipeDesc;
            return recipe;
        }
        private void LoadRecipeTable(int DishID)
        {
            try
            {
                List<RecipeData> ListRecipe = new RecipeSystem().GetRecipeByDishID(DishID);
                rptRecipe.DataSource = ListRecipe;
                rptRecipe.DataBind();
            }
            catch (Exception ex)
            {
                notifRecipe.Show($"ERROR LOAD TABLE: {ex.Message}", NotificationType.Danger);
            }
        }
        protected void rptRecipe_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                RecipeData recipe = (RecipeData)e.Item.DataItem;
                LinkButton lbRecipeName = (LinkButton)e.Item.FindControl("lbRecipeName");
                LinkButton lbRecipeDetail = (LinkButton)e.Item.FindControl("lbRecipeDetail");
                lbRecipeDetail.CommandArgument = recipe.RecipeID.ToString();
                lbRecipeName.Text = recipe.RecipeName;
                lbRecipeName.CommandArgument = recipe.RecipeID.ToString();

                CheckBox chkChoose = (CheckBox)e.Item.FindControl("chkChoose");
                chkChoose.Attributes.Add("data-value", recipe.RecipeID.ToString());
            }
        }
        protected void rptRecipe_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "EDIT")
            {
                LinkButton lbRecipeName = (LinkButton)e.Item.FindControl("lbRecipeName");
                
                int recipeID = Convert.ToInt32(e.CommandArgument.ToString());
                RecipeData recipe = new RecipeSystem().GetRecipeByID(recipeID);
                FillForm(new RecipeData
                {
                    RecipeID = recipe.RecipeID,
                    RecipeName = recipe.RecipeName
                });
                litFormType.Text = $"UBAH: {lbRecipeName.Text}";
                pnlFormRecipe.Visible = true;
                txtRecipeName.Focus();
            }

            if (e.CommandName == "DETAIL")
            {

                int recipeID = Convert.ToInt32(e.CommandArgument.ToString());
                Response.Redirect("RecipeDetail.aspx?recipeid=" + recipeID);
            }

        }

        private void ShowNotificationIfExists()
        {
            if (Session["save-success"] != null)
            {
                notifRecipe.Show("Data sukses disimpan", NotificationType.Success);
                Session.Remove("save-success");
            }
            if (Session["delete-success"] != null)
            {
                notifRecipe.Show("Data sukses dihapus", NotificationType.Success);
                Session.Remove("delete-success");
            }
        }

    }
}