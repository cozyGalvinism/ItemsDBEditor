using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using LiteDB;

namespace ItemDBEditor.Data {

    public class ItemService
    {
        private string _dbPath = "items.ldb";
        private LiteDatabase _ldb;
        private ILiteCollection<LDBItem> _itemCollection;
        private ILiteCollection<LDBRecipe> _recipeCollection;

        public ItemService() {
            _ldb = new LiteDatabase(_dbPath);

            _itemCollection = _ldb.GetCollection<LDBItem>();
            _itemCollection.EnsureIndex(k => k.Tags);
            _itemCollection.EnsureIndex(k => k.Grist);
            _itemCollection.EnsureIndex(k => k.Strifekind);

            _recipeCollection = _ldb.GetCollection<LDBRecipe>();
            _recipeCollection.EnsureIndex(k => k.Method);
            _recipeCollection.EnsureIndex(k => k.ItemA);
            _recipeCollection.EnsureIndex(k => k.ItemB);
        }

        public Task<bool> DeleteItem(string code) {
            var item = _itemCollection.FindById(code);
            if(item == null) return Task.FromResult(false);
            _itemCollection.Delete(code);
            return Task.FromResult(true);
        }

        public Task<bool> CreateItem(LDBItem item) {
            var existingItem = _itemCollection.FindById(item.Code);
            if(existingItem != null) return Task.FromResult(false);
            _itemCollection.Insert(item);
            return Task.FromResult(true);
        }

        public Task<bool> UpdateItem(LDBItem item) {
            var existingItem = _itemCollection.FindById(item.Code);
            if(existingItem == null) return Task.FromResult(false);
            _itemCollection.Update(item);
            return Task.FromResult(true);
        }

        public Task<LDBItem> GetItem(string code) {
            var item = _itemCollection.FindById(code);
            return Task.FromResult(item);
        }

        public Task<List<LDBItem>> GetItems() {
            var items = new List<LDBItem>();
            items = _itemCollection.FindAll().ToList();
            return Task.FromResult(items);
        }

        public Task<bool> DeleteRecipe(string itemA, string itemB, LDBRecipe.Methods method) {
            return Task.FromResult(_recipeCollection.DeleteMany(r => r.ItemA == itemA && r.ItemB == itemB && r.Method == method) > 0);
        }

        public Task<bool> CreateRecipe(LDBRecipe recipe) {
            var existingRecipe = _recipeCollection.FindOne(r => r.ItemA == recipe.ItemA && r.ItemB == recipe.ItemB && r.Method == recipe.Method);
            if(existingRecipe != null) return Task.FromResult(false);
            _recipeCollection.Insert(recipe);
            return Task.FromResult(true);
        }

        public Task<bool> UpdateRecipe(LDBRecipe recipe) {
            var existingRecipe = _recipeCollection.FindOne(r => r.ItemA == recipe.ItemA && r.ItemB == recipe.ItemB && r.Method == recipe.Method);
            if(existingRecipe != null) return Task.FromResult(false);
            _recipeCollection.Update(recipe);
            return Task.FromResult(true);
        }

        public Task<LDBRecipe> GetRecipe(string itemA, string itemB, LDBRecipe.Methods method) {
            var recipe = _recipeCollection.FindOne(r => r.ItemA == itemA && r.ItemB == itemB && r.Method == method);
            return Task.FromResult(recipe);
        }

        public Task<List<LDBRecipe>> GetRecipes() {
            var recipes = new List<LDBRecipe>();
            recipes = _recipeCollection.FindAll().ToList();
            return Task.FromResult(recipes);
        }
    }
}