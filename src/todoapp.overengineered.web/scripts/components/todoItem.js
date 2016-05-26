import R from 'reren';

var TodoItem = R.component({
    controller: function(parentModel) {
        this.model = parentModel;

        this.onUpdate = (updatedParentModel) => {
            this.model.todo = updatedParentModel.todo;
        };
    },
    view: (model) => {
        var classes = (model.todo.completed) ? "completed" : "";

        return R.li({ classes: classes },
                R.div({ classes: "view" }, [
                    R.input({ 
                        classes: "toggle",
                        type: "checkbox",
                        onClick: model.onTodoChecked.bind(null, model.todo),
                        checked: (model.todo.completed) ? 'checked' : null
                    }),
                    R.label(null, model.todo.text),
                    R.button({ classes: "destroy", onClick: model.onRemoveTodo.bind(null, model.todo) })
                ]));
    }
});

export default TodoItem;