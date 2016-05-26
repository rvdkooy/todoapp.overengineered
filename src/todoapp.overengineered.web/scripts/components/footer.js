import R from 'reren';

var Footer = R.component({
    controller: function(parentModel) {
        this.model = parentModel;

        this.onUpdate = (updatedParentModel) => {
            this.model.todoCount = updatedParentModel.todoCount;
            this.model.activeFilter = updatedParentModel.activeFilter;
        };
    },
    view: (model) => {
        var allClass = (model.activeFilter === "all") ? "selected" : '';
        var activeClass = (model.activeFilter === "active") ? "selected" : '';
        var completedClass = (model.activeFilter === "completed") ? "selected" : '';

        return R.footer({ classes: "footer" }, [
                    R.span({ classes: "todo-count" }, [
                        R.strong(null, model.todoCount.toString()),
                        R.span(null, " item(s) left")
                    ]),
                    R.ul({ classes: "filters" }, [
                        R.li(null, R.element("a", { href: '#', classes: allClass, onClick: model.onFilterSelected.bind(null, "all") }, "All")),
                        R.li(null, R.element("a", { href: '#', classes: activeClass, onClick: model.onFilterSelected.bind(null, "active") }, "Active")),
                        R.li(null, R.element("a", { href: '#', classes: completedClass, onClick: model.onFilterSelected.bind(null, "completed") }, "Completed"))
                    ])
        ]);
    }
});

export default Footer;