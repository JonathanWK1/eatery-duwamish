using BusinessRule;
using Common.Data;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessFacade
{
    public class IngredientSystem
    {
        public List<IngredientData> GetIngredientByRecipeID(int RecipeID)
        {
            try
            {
                return new IngredientDB().GetIngredientByRecipeID(RecipeID);
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
                return new IngredientDB().GetIngredientByID(IngredientID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int InsertUpdateIngredient(IngredientData Ingredient)
        {
            try
            {
                return new IngredientRule().InsertUpdateIngredient(Ingredient);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int DeleteIngredients(IEnumerable<int> IngredientIDs)
        {
            try
            {
                return new IngredientRule().DeleteIngredients(IngredientIDs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
    }
}
