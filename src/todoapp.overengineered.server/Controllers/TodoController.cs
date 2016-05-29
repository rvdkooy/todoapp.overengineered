using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using todoapp.overengineered.server.Projections;

namespace todoapp.overengineered.server.Controllers
{
    public class TodoController : ApiController
    {
        private readonly InMemoryProjectionStore _projectionStore;

        public TodoController(InMemoryProjectionStore projectionStore)
        {
            _projectionStore = projectionStore;
        }

        [Route("todos")]
        public IEnumerable<TodoDto> GetTodos()
        {
            return _projectionStore.GetAll().Select(x => new TodoDto()
            {
                Id = x.Id,
                Text = x.Text

            }).ToList();
        }
    }

    public class TodoDto
    {
        public string Id { get; set; }
        public string Text { get; set; }
    }
}
