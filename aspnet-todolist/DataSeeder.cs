using aspnet_todolist.Models;
using Bogus;

namespace aspnet_todolist
{
    public class DataSeeder
    {
        Faker<Category> categoryModelFake;
        Faker<Todo> todoModelFake;
        private readonly TodoDb _db;

        public DataSeeder(TodoDb db)
        {
            _db = db;
            Randomizer.Seed = new Random();
            Random random = new();

            categoryModelFake = new Faker<Category>()
                .RuleFor(u => u.Name, f => f.Lorem.Word())
                .RuleFor(u => u.Color, f => String.Format("#{0:X6}", random.Next(0x1000000)));

            todoModelFake = new Faker<Todo>()
                .RuleFor(u => u.Name, f => f.Lorem.Word())
                .RuleFor(u => u.IsComplete, f => f.Random.Bool())
                .RuleFor(u => u.CategoryId, f =>
                {
                    var categoryIds = _db.Categories.Select(c => c.Id).ToList();
                    return categoryIds.Count > 0 ? f.PickRandom(categoryIds) : (int?)null;
                });
        }

        public Category GenerateCategory()
        {
            return categoryModelFake.Generate();
        }

        public Todo GenerateTodo()
        {
            return todoModelFake.Generate();
        }
    }
}
