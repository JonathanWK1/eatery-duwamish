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
    public class RecipeSystem
    {
        public List<RecipeData> GetRecipeByDishID(int DishID)
        {
            try
            {
                return new RecipeDB().GetRecipeByDishID(DishID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public RecipeData GetRecipeByID(int recipeID)
        {
            try
            {
                return new RecipeDB().GetRecipeByID(recipeID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int InsertUpdateRecipe(RecipeData recipe)
        {
            try
            {
                return new RecipeRule().InsertUpdateRecipe(recipe);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int DeleteRecipes(IEnumerable<int> recipeIDs)
        {
            try
            {
                return new RecipeRule().DeleteRecipes(recipeIDs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
    }
}
