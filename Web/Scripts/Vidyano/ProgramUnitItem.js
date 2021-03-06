﻿function ProgramUnitItem(item, programUnit) {
    this.item = item;
    this.programUnit = programUnit;
    this.id = item.id;
    this.name = item.name;
    this.title = item.title || "";
    this.persistentObjectType = item.persistentObjectType;
    this.filters = !isNullOrEmpty(item.filters) ? item.filters.split(";") : null;
    this.iconId = item.icon;
    this.element = null;

    var queryData = item.query;
    if (queryData != null) {
        this.query = { id: queryData };
        this.queryName = item.queryName;
    }
    else {
        this.query = null;
        this.queryName = null;
    }

    var poData = item.persistentObject;
    if (poData != null) {
        this.persistentObject = { id: poData, objectId: item.objectId };
        this.objectId = item.objectId;
    }
    else {
        this.persistentObject = null;
        this.objectId = null;
    }
}

ProgramUnitItem.prototype.createElement = function (container) {
    /// <summary>Renders the Program Unit Item within the specified container. </summary>
    /// <param name="container" type="jQuery">The container to render the Program Unit in.</param>

    this.container = container;
    var li = $.createElement("li", { item: this, filter: null });
    var div = $.createElement("div");
    this.element = li;

    if (this.filters == null) {
        this._generateImage(li);

        var a = $.createElement("a");
        var href = "#!/";
        if (this.query == null)
            href += app.getUrlForPersistentObject(this.persistentObject, null, this.programUnit);
        else
            href += app.getUrlForQuery(this.query, null, this.programUnit);

        if (href == hasher.getHash())
            li.addClass("programUnitItemSelected");

        a.attr({ href: href, onclick: "return false;" }).text(this.title);
        li.append(a).on("click", ProgramUnitItem._onProgramUnitItemClick).append(div);

        container.append(li);
    }
    else {
        if (this.item.group == null)
            this._generateImage(li);

        var titleLink = $.createElement("a").text(this.title);
        li.append(titleLink).addClass("programUnitItemsGroupHeader").append(div);

        var ul = this._generateFilterItems();
        if ($.mobile) {
            ul.hide();
            li.append(ul);
            li.on("click", function (e) {
                if (ul.is(':visible'))
                    ul.hide();
                else
                    ul.show();

                e.stopPropagation();
                e.preventDefault();
            });
        }
        else
            li.subMenu(ul);

        container.append(li);
    }

    div.css("left", (li.outerWidth() / 2) - 6)
           .css("top", li.outerHeight() - 6);
};

ProgramUnitItem.prototype.open = function (filterName, replace) {
    /// <summary>Opens the Program Unit Item.</summary>
    /// <param name="filterName" type="String">The optional name of the filter to open a Query.</param>
    /// <param name="replace" type="Boolean">Optionally replace the current hash.</param>

    app.openProgramUnitItem(this, filterName, this.programUnit, replace);
};

ProgramUnitItem.prototype._generateFilterItems = function () {
    var ul = $.createElement("ul").addClass("programUnitItemsGroup");

    var self = this;
    this.filters.forEach(function (filter) {
        var filterItem = $.createElement("li", { item: self, filter: filter });

        var a = $.createElement("a");
        a.attr({ href: "#!/" + app.getUrlForQuery(self.query, filter, self.programUnit), onclick: "return false;" }).text(filter);
        filterItem.append(a).on("click", ProgramUnitItem._onProgramUnitItemClick);
        
        ul.append(filterItem);
    });

    return ul;
};

ProgramUnitItem.prototype._generateImage = function (li) {
    var iconId = this.item.icon;
    if (iconId != null) {
        var icon = app.icons[iconId];
        if (icon != null && icon.data != null) {
            var img = $.createElement("img").addClass("programUnitItemIcon");
            img.attr({ src: icon.data.asDataUri(), alt: "Icon", title: this.title });
            li.append(img);
        }
    }
};

ProgramUnitItem._onProgramUnitItemClick = function (e) {
    var dataContext = $(this).dataContext();
    dataContext.item.open(dataContext.filter);
    
    if ($.mobile) {
        e.stopPropagation();
        e.preventDefault();
    }
};