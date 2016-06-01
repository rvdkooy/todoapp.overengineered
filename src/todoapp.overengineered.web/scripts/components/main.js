import R from 'reren';
import todoItem from './todoItem';
import footer from './footer';
import uuid from 'uuid';
import { execute as cedarExecute } from '../utils/cedar';
import { query } from '../utils/httpClient';

var Main = R.component({
    controller: function() {
        this.model.todos = [];
        this.model.newTodoText = "";
        this.model.activeFilter = "all";

        query('api/todos').then(result => {
            this.model.todos = result.map(x => {
                return {
                    id: x.Id,
                    text: x.Text
                };
            });
            this.update();
        });

        this.model.onNewTaskKeyUp = (e) => {
            if (e.keyCode == 13 && e.target.value) {
                cedarExecute({
                    commandName: 'newtodo',
                    commandId: uuid.v4(),
                    text: e.target.value
                });


                this.model.todos.push({ id: this.model.todos.length + 1, text: e.target.value });
                this.model.newTodoText = "";
                e.target.value = "";
                this.update();
            }
        };

        this.model.onRemoveTodo = (todo) => {
            cedarExecute({
                commandName: 'deletetodo',
                commandId: uuid.v4(),
                id: todo.id
            });


            this.model.todos = this.model.todos.filter(x => x.id !== todo.id);
            this.update();
        };

        this.model.onTodoChecked = (todo) => {
            todo.completed = !todo.completed;
            this.update();
        };

        this.model.onFilterSelected = (filter) => {
            this.model.activeFilter = filter;
            this.update();
        };
    },
    view: (model) => {
        var filteredItems = model.todos;

        if (model.activeFilter === "active") {
            filteredItems = model.todos.filter(x => !x.completed);
        } else if (model.activeFilter === "completed") {
            filteredItems = model.todos.filter(x => x.completed);
        }

        var todoItems = filteredItems.map(todo => {
            return R.element(todoItem, {
                todo: todo,
                onRemoveTodo: model.onRemoveTodo,
                onTodoChecked: model.onTodoChecked
            });
        });

        return R.section({ classes: "todoapp" }, [
                    R.header({ classes: "header" }, [
                        R.h1(null, "Todo's"),
                        R.input({ classes: "new-todo",
                                    type: "text",
                                    placeholder: "What needs to be done?",
                                    autoFocus: "true",
                                    onKeyDown: model.onNewTaskKeyUp,
                                    value: model.newTodoText
                                })
                ]),
                R.section({ classes: "main" }, R.ul({ classes: "todo-list" }, todoItems)),
                R.element(footer, { todoCount: model.todos.filter(x => !x.completed).length, 
                                    onFilterSelected: model.onFilterSelected,
                                    activeFilter: model.activeFilter
                                })
            ]);
    }
});

export default Main;
