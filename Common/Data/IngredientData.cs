using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data
{
    public class IngredientData
    {
        private int _ingredientID;
        public int IngredientID
        {
            get { return _ingredientID; }
            set { _ingredientID = value; }
        }
        private int _recipeID;
        public int RecipeID
        {
            get { return _recipeID; }
            set { _recipeID = value; }
        }
        
        private string _ingredientName;
        public string IngredientName
        {
            get { return _ingredientName; }
            set { _ingredientName = value; }
        }

        private int _qty;
        public int Qty
        {
            get { return _qty; }
            set { _qty = value; }
        }
        private string _unit;
        public string Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }
    }
}
