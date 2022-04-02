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
    public partial class RecipeDetail : System.Web.UI.Page
    {
        RecipeData recipe = new RecipeData();
        protected void Page_Load(object sender, EventArgs e)
        {
            
            try
            {
                ShowNotificationIfExists();

                if (!string.IsNullOrEmpty(Request.QueryString["RecipeID"]))
                {
                    int RecipeID = Convert.ToInt32(Request.QueryString["RecipeID"]);
                    LoadRecipeProperties(RecipeID);
                    LoadIngredientTable();
                }
                else
                {
                    throw new Exception("Please Provide RecipeID");
                }
            }
            catch (Exception ex)
            {
                notifIngredient.Show($"ERROR LOAD TABLE: {ex.Message}", NotificationType.Danger);
            }


        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                IngredientData ingredient = GetFormData();               
                int rowAffected = new IngredientSystem().InsertUpdateIngredient(ingredient);
                if (rowAffected <= 0)
                    throw new Exception("No Data Recorded");
                Session["save-success"] = 1;
                Response.Redirect("RecipeDetail.aspx?RecipeID="+ recipe.RecipeID);
            }
            catch (Exception ex)
            {
                notifIngredient.Show($"ERROR SAVE DATA: {ex.Message}", NotificationType.Danger);
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            ResetForm();
            litFormType.Text = "TAMBAH";
            pnlFormIngredient.Visible = true;
            txtIngredientName.Focus();
        }
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string strDeletedIDs = hdfDeletedIngredients.Value;
                if (string.IsNullOrEmpty(strDeletedIDs))
                    throw new Exception("No Data Selected");
                IEnumerable<int> deletedIDs = strDeletedIDs.Split(',').Select(Int32.Parse);
                int rowAffected = new IngredientSystem().DeleteIngredients(deletedIDs);
                if (rowAffected <= 0)
                    throw new Exception("No Data Deleted");
                Session["delete-success"] = 1;
                Response.Redirect("RecipeDetail.aspx?recipeid="+recipe.RecipeID);
            }
            catch (Exception ex)
            {
                notifIngredient.Show($"ERROR DELETE DATA: {ex.Message}", NotificationType.Danger);
            }
        }
        protected void btnEditDesc_Click(object sender, EventArgs e)
        {
            btnSaveDesc.Enabled = true;
            btnEditDesc.Enabled = false;
            txtRecipeDesc.Enabled = true;
            txtRecipeDesc.Focus();
            
        }
        protected void btnSaveDesc_Click(object sender, EventArgs e)
        {
            try
            {
                btnEditDesc.Enabled = true;
                btnSaveDesc.Enabled = false;
                txtRecipeDesc.Enabled = false;
                
                string test = txtRecipeDesc.Text;
                if (recipe.RecipeDesc == test)
                {
                    throw new Exception(test);
                }
                recipe.RecipeDesc = test;
                int rowAffected = new RecipeSystem().InsertUpdateRecipe(recipe);
                if (rowAffected <= 0)
                    throw new Exception("No Data Recorded");
                Session["save-success"] = 1;
                Response.Redirect("RecipeDetail.aspx?RecipeID=" + recipe.RecipeID);
            }
            catch (Exception ex)
            {
                notifIngredient.Show($"ERROR SAVE DATA: {ex.Message}", NotificationType.Danger);
            }
        }
        private void ResetForm()
        {
            hdfIngredientId.Value = String.Empty;
            txtIngredientName.Text = String.Empty;
            txtIngredientQty.Text = String.Empty;
            txtIngredientUnit.Text = String.Empty;
        }

        private void FillForm(IngredientData ingredient)
        {
            hdfIngredientId.Value = ingredient.IngredientID.ToString();
            txtIngredientName.Text = ingredient.IngredientName;
            txtIngredientQty.Text = ingredient.Qty.ToString();
            txtIngredientUnit.Text = ingredient.Unit;
        }
        private IngredientData GetFormData()
        {
            IngredientData ingredient = new IngredientData();
            ingredient.IngredientID = String.IsNullOrEmpty(hdfIngredientId.Value) ? 0 : Convert.ToInt32(hdfIngredientId.Value);
            ingredient.IngredientName = txtIngredientName.Text;
            ingredient.Qty = Convert.ToInt32(txtIngredientQty.Text);
            ingredient.Unit = txtIngredientUnit.Text;
            ingredient.RecipeID = recipe.RecipeID;
            return ingredient;
        }
        private void LoadIngredientTable()
        {
            try
            {
                List<IngredientData> ListIngredient = new IngredientSystem().GetIngredientByRecipeID(recipe.RecipeID);
                rptIngredient.DataSource = ListIngredient;
                rptIngredient.DataBind();
            }
            catch (Exception ex)
            {
                notifIngredient.Show($"ERROR LOAD TABLE: {ex.Message}", NotificationType.Danger);
            }
        }

        private void LoadRecipeProperties(int RecipeID)
        {
            try
            {
                recipe = new RecipeSystem().GetRecipeByID(RecipeID);
                if (recipe == null)
                    throw new Exception("Invalid Recipe ID");
                labTitle.Text = recipe.RecipeName;
                if (!Page.IsPostBack)
                    txtRecipeDesc.Text = recipe.RecipeDesc;

                txtRecipeDesc.Enabled = false;
                btnSaveDesc.Enabled = false;
            }
            catch (Exception ex)
            {
                notifIngredient.Show($"ERROR LOAD TABLE: {ex.Message}", NotificationType.Danger);
            }

        }
        

        protected void rptIngredient_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                IngredientData ingredient = (IngredientData)e.Item.DataItem;
                LinkButton lbIngredientName = (LinkButton)e.Item.FindControl("lbIngredientName");
                Literal litIngredientQty = (Literal)e.Item.FindControl("litIngredientQty");
                Literal litIngredientUnit = (Literal)e.Item.FindControl("litIngredientUnit");
                
                lbIngredientName.Text = ingredient.IngredientName;
                lbIngredientName.CommandArgument = ingredient.IngredientID.ToString();
                litIngredientQty.Text = ingredient.Qty.ToString();
                litIngredientUnit.Text = ingredient.Unit;

                CheckBox chkChoose = (CheckBox)e.Item.FindControl("chkChoose");
                chkChoose.Attributes.Add("data-value", ingredient.IngredientID.ToString());
            }
        }
        protected void rptIngredient_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "EDIT")
            {
                LinkButton lbIngredientName = (LinkButton)e.Item.FindControl("lbIngredientName");
                
                int IngredientID = Convert.ToInt32(e.CommandArgument.ToString());
                IngredientData Ingredient = new IngredientSystem().GetIngredientByID(IngredientID);
                FillForm(new IngredientData
                {
                    IngredientID = Ingredient.IngredientID,
                    IngredientName = Ingredient.IngredientName,
                    Qty = Ingredient.Qty,
                    Unit = Ingredient.Unit
                });
                litFormType.Text = $"UBAH: {lbIngredientName.Text}";
                pnlFormIngredient.Visible = true;
                txtIngredientName.Focus();
            }
            

        }



        private void ShowNotificationIfExists()
        {
            if (Session["save-success"] != null)
            {
                notifIngredient.Show("Data sukses disimpan", NotificationType.Success);
                Session.Remove("save-success");
            }
            if (Session["delete-success"] != null)
            {
                notifIngredient.Show("Data sukses dihapus", NotificationType.Success);
                Session.Remove("delete-success");
            }
        }

    }
}